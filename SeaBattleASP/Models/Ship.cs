﻿namespace SeaBattleASP.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Drawing;
    using System.Linq;
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Interfaces;

    public abstract class Ship : IRepairable, IFireable
    {
        public Ship()
        {
            this.DeckCells = new List<DeckCell>();
            this.Type = this.GetShipType();
        }

        #region Properties
        [Key]
        public int Id { get; set; }

        public int Range { get; set; }

        public bool IsXDirection { get; set; }

        public Player Player { get; set; }

        [NotMapped]
        public string Type { get; set; }

        public List<DeckCell> DeckCells { get; set; }

        public bool IsSelectedShip { get; set; }
        #endregion

        #region Methods

        public static Ship ShiftShip(string direction, Ship ship)
        {
            if (ship != null)
            {
                ship = ShipManager.ShiftShipDeckCell(direction, ship);
                DbManager.UpdateShip(ship);
            }

            return ship;
        }

        private static void LoadRelatedDataForShip()
        {
            Player.GetAll();
            Deck.GetAll();
            Cell.GetAll();
            DeckCell.GetAll();
        }

        public static Ship GetShipByIdFromDB(int shipId)
        {
            LoadRelatedDataForShip();
            List<Ship> allShips = GetAll();
            return allShips.Find(i => i.Id == shipId);
        }

        public static List<Ship> GetAll()
        {
            return DbManager.GetAllShips();
        }

        public static Ship SetShipProperties(int gameId,
                                           int playerId,
                                           Ship ship)
        {
            var players = Player.GetAll();
            var game = Game.GetGameById(gameId);
            game.PlayingField.Ships.Add(ship);

            var player = players.Find(i => i.Id == playerId);
            if (player != null)
            {
                ship.Player = player;
            }

            var shipDeckCells = ShipManager.GetDeckCellsForShip(ship.DeckCells,
                                                                player,
                                                                game);
            ship.DeckCells = shipDeckCells;

            return ship;
        }

        public static Ship GetShipByIdFromMapModel(int id, 
                                                   MapModel model)
        {
            Ship ship = null;

            foreach (KeyValuePair<int, Ship> indexedShip in model.Ships)
            {
                if (indexedShip.Key == id)
                {
                    ship = indexedShip.Value;
                }
            }

            return ship;
        }

        public string GetShipType()
        {
            return this.GetType().Name;
        }

        public virtual List<DeckCell> Fire(List<DeckCell> enemyShips)
        {
            var allDeckCells = DeckCell.GetAll();
            List<DeckCell> selectedShip = new List<DeckCell>();
            foreach (DeckCell deckCell in this.DeckCells)
            {
                selectedShip.Add(allDeckCells.Find(i => i.Id == deckCell.Id));
            }
            
            var neighborsPoints = DeckCell.GetNeighboringPoints(selectedShip, 
                                                                this.Range);
            var firedShipDecks = ShipManager.CheckEnemyShips(enemyShips, 
                                                             neighborsPoints);
            if (firedShipDecks.Count > 0)
            {
                foreach (DeckCell firedDeck in firedShipDecks)
                {
                    firedDeck.Deck.State = Enums.DeckState.Hurted;
                    firedDeck.Cell.Color = Enums.CellColor.Yellow;
                }
            }

            return firedShipDecks;
        }

        public Ship Move()
        {
            if (this != null)
            {
                var shipDeckCells = this.ShiftShipDeckCells();
                if (shipDeckCells.Count > 0)
                {
                    this.DeckCells = shipDeckCells;
                    DbManager.UpdateShip(this);
                }
            }
            return this;
        }

        public List<DeckCell> ShiftShipDeckCells()
        {
            List<DeckCell> result = new List<DeckCell>();
            result.Clear();
            var head = this.DeckCells.Find(s => s.Deck.IsHead);
            if (this.DeckCells.Count > 0)
            {
                foreach (DeckCell shipDeck in this.DeckCells.ToList())
                {
                    Point p = new Point
                    {
                        X = shipDeck.Cell.X,
                        Y = shipDeck.Cell.Y
                    };

                    p = ShipManager.ShiftCell(p, this.IsXDirection, this.Range);

                    shipDeck.Cell.X = p.X;
                    shipDeck.Cell.Y = p.Y;

                    result.Add(shipDeck);
                }
            }
            
            this.CheckNewCoordinate(result);
            return result;
        }
       
        public List<DeckCell> Repair(List<Ship> myShips)
        {
            List<DeckCell> hurtedDecks = new List<DeckCell>();
            var neighborsPoints = DeckCell.GetNeighboringPoints(this.DeckCells, 
                                                                this.Range);

            foreach(Ship sh in myShips)
            {
                hurtedDecks.AddRange(ShipManager.GetHurtedShip(neighborsPoints,
                                     sh.DeckCells));
                if (hurtedDecks.Count > 0)
                {
                    foreach (DeckCell hurtedDeck in hurtedDecks)
                    {
                        hurtedDeck.Deck.State = Enums.DeckState.Normal;

                        DbManager.Db.DeckCells.Update(hurtedDeck);
                        DbManager.Db.SaveChanges();
                    }
                }
            }
            
            return hurtedDecks;
        }

        private void CheckNewCoordinate(List<DeckCell> deckCells)
        {
            if (deckCells.Count > 0)
            {
                foreach (DeckCell deckCell in deckCells)
                {
                    bool isAbroad = deckCell.Cell.X > Rules.FieldWidth - 1 
                         || deckCell.Cell.Y > Rules.FieldHeight - 1;
                    if (isAbroad)
                    {
                        this.IsXDirection = !this.IsXDirection;
                        this.ShiftShipDeckCells();
                    }
                }
            }
        }
        #endregion
    }
}