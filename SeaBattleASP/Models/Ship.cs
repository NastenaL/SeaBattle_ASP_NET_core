namespace SeaBattleASP.Models
{
    using Microsoft.EntityFrameworkCore;
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Drawing;
    using System.Linq;

    public abstract class Ship : IRepairable, IFireable
    {
        public Ship()
        {
            DeckCells = new List<DeckCell>();
        }

        [Key]
        public int Id { get; set; }

        public int Range { get; set; }

        public bool IsXDirection { get; set; }

        public Player Player { get; set; }

        public List<DeckCell> DeckCells { get; set; }

        public bool IsSelectedShip { get; set; }

        public static Ship GetShipByIdFromDB(int shipId)
        {
            List<Ship> allShips = GetAllShips();
            return allShips.Find(i => i.Id == shipId);
        }

        public static List<Ship> GetAllShips()
        {
            DbManager.db.Cells.ToListAsync<Cell>();
            DbManager.db.Decks.ToListAsync<Deck>();
            DbManager.db.DeckCells.ToListAsync<DeckCell>();

            var auxiliaryShips = DbManager.db.AuxiliaryShips.ToListAsync<AuxiliaryShip>().Result;
            var militaryShip = DbManager.db.MilitaryShips.ToListAsync<MilitaryShip>().Result;
            var mixShip = DbManager.db.MixShips.ToListAsync<MixShip>().Result;
            List<Ship> allShips = new List<Ship>();
            allShips.AddRange(auxiliaryShips);
            allShips.AddRange(militaryShip);
            allShips.AddRange(mixShip);
            return allShips;
        }

        public static Ship GetShipByIdFromMapModel(int id, MapModel model)
        {
            Ship ship = null;

            foreach (KeyValuePair<int, Ship> k in model.Ships)
            {
                if (k.Key == id)
                {
                    ship = k.Value;
                }
            }
            return ship;
        }

        public virtual List<DeckCell> Fire(List<DeckCell> enemyShips)
        {
            var allDeckCells= DbManager.db.DeckCells.ToListAsync<DeckCell>().Result;
            List<DeckCell> selectedShip = new List<DeckCell>();
            foreach(DeckCell deckCell in this.DeckCells)
            {
                selectedShip.Add(allDeckCells.Find(i => i.Id == deckCell.Id));
            }
            
            var neighborsPoints = DeckCell.GetNeighboringPoints(selectedShip, this.Range);
            var firedShipDecks = ShipManager.CheckEnemyShips(enemyShips, neighborsPoints);
            if (firedShipDecks.Count > 0)
            {
                foreach (DeckCell firedDeck in firedShipDecks)
                {
                    firedDeck.Deck.State = Enums.DeckState.Normal;
                }

            }
            return firedShipDecks;
        }

        public List<DeckCell> Move(Ship ship)
        {
            List<DeckCell> result = new List<DeckCell>();
            var head = ship.DeckCells.Find(s =>s.Deck.IsHead);
            foreach(DeckCell shipDeck in ship.DeckCells)
             {
                
                 Point p = new Point
                 {
                     X = shipDeck.Cell.X,
                     Y = shipDeck.Cell.Y
                 };

                 p.X = ship.IsXDirection? p.X + ship.Range : p.X;
                 p.Y = !ship.IsXDirection? p.Y + ship.Range : p.Y;

                 CheckNewCoordinate(p, result, ship);

                 shipDeck.Cell.X = p.X;
                 shipDeck.Cell.Y = p.Y;

                 result.Add(shipDeck);
             }
            return result;
        }

        private void CheckNewCoordinate(Point p, List<DeckCell> result, Ship ship)
        {
            if (p.X > Rules.FieldWidth-1 || p.Y > Rules.FieldHeight-1)
            {
                this.IsXDirection = p.X > Rules.FieldWidth;

                result.Clear();
                Move(ship);
            }
           
        }

        public virtual void Repair(List<Ship> allShips)
        {
            var neighborsPoints = DeckCell.GetNeighboringPoints(this.DeckCells, this.Range);
            if(this.Player != null)
            {
                List<DeckCell> allPlayerDeckCells = new List<DeckCell>();
                foreach (Ship s in allShips)
                {
                    allPlayerDeckCells.AddRange(s.DeckCells);
                }

                var hurtedDecks = ShipManager.GetHurtedShip(neighborsPoints, allPlayerDeckCells);
                if (hurtedDecks.Count > 0)
                {
                    foreach (DeckCell hurtedDeck in hurtedDecks)
                    {
                        hurtedDeck.Deck.State = Enums.DeckState.Normal;
                    }
                }
            }  
        }
    }
}
