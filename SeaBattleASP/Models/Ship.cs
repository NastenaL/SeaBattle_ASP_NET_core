namespace SeaBattleASP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Drawing;
    using System.Linq;
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Enums;
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
        public static Ship GetShipByIdFromDB(int shipId)
        {
            Player.GetAll();
            Deck.GetAll();
            Cell.GetAll();
            DeckCell.GetAll();
            List<Ship> allShips = GetAll();
            return allShips.Find(i => i.Id == shipId);
        }

        public static List<Ship> GetAll()
        {
            return DbManager.GetAllShips();
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
            var shipDeckCells = GetDeckCellsForShip(ship.DeckCells, 
                                                    player, 
                                                    game);
            ship.DeckCells = shipDeckCells;

            return ship;
        }

        private static List<DeckCell> GetDeckCellsForShip(List<DeckCell> deckCells, 
                                                          Player player, 
                                                          Game game)
        {
            Random random = new Random();
            Array shipDirections = Enum.GetValues(typeof(ShipDirection));
            List<DeckCell> resultDeckCells = new List<DeckCell>();

            var initalPoint = ShipManager.GetRandomPoint(random);
            var direction = (ShipDirection)shipDirections.GetValue(random.Next(shipDirections.Length));

            foreach (DeckCell deck in deckCells)
            {
                initalPoint = ShipManager.ShiftPoint(initalPoint, 
                                                     direction);

                var currentDeckCell = DeckCell.Create(initalPoint, 
                                                      deck.Deck);
                resultDeckCells.Add(currentDeckCell);
            }

            CheckNewDeckCells(resultDeckCells,
                              game,
                              player); 

            return resultDeckCells;
        }

        private static void CheckNewDeckCells(List<DeckCell> deckCells,
                                              Player player,
                                              Game game)
        {
            var isError = DeckCell.CheckDeckCellOutOfBorder(deckCells);
            var isBool = DeckCell.CheckDeckCellOtherShips(deckCells,
                                                          game,
                                                          player);

            if (isError || isBool)
            {
                GetDeckCellsForShip(deckCells,
                                    player,
                                    game);
            }
        }

        public List<DeckCell> Move()
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

                    if (this.IsXDirection)
                    {
                        if (p.X + this.Range > Rules.FieldWidth)
                        {
                            p.X -= this.Range;
                        }
                        else
                        {
                            p.X += this.Range;
                        }
                    }
                    else
                    {
                        if (p.Y + this.Range > Rules.FieldHeight)
                        {
                            p.Y -= this.Range;
                        }
                        else
                        {
                            p.Y += this.Range;
                        }
                    }

                    shipDeck.Cell.X = p.X;
                    shipDeck.Cell.Y = p.Y;

                    result.Add(shipDeck);
                }
            }
            
            this.CheckNewCoordinate(result);
            return result;
        }

        public virtual List<DeckCell> Repair(List<Ship> allShips)
        {
            List<DeckCell> hurtedDecks = new List<DeckCell>();
            var neighborsPoints = DeckCell.GetNeighboringPoints(this.DeckCells, 
                                                                this.Range);
            if (this.Player != null)
            {
                List<DeckCell> allPlayerDeckCells = new List<DeckCell>();
                foreach (Ship s in allShips)
                {
                    allPlayerDeckCells.AddRange(s.DeckCells);
                }

                hurtedDecks = ShipManager.GetHurtedShip(neighborsPoints, 
                                                        allPlayerDeckCells);
                if (hurtedDecks.Count > 0)
                {
                    foreach (DeckCell hurtedDeck in hurtedDecks)
                    {
                        hurtedDeck.Deck.State = Enums.DeckState.Normal;
                    }
                }
            }

            return hurtedDecks;
        }

        private void CheckNewCoordinate(List<DeckCell> result)
        {
            if (result.Count > 0)
            {
                foreach (DeckCell deckCell in result)
                {
                    bool isAbroad = deckCell.Cell.X > Rules.FieldWidth - 1 
                         || deckCell.Cell.Y > Rules.FieldHeight - 1;
                    if (isAbroad)
                    {
                        this.IsXDirection = !this.IsXDirection;
                        this.Move();
                    }
                }
            }
        }

        #endregion
    }
}