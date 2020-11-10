namespace SeaBattleASP.Models
{
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Drawing;

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

        public static Ship GetShipById(int id, MapModel model)
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

        public virtual void Fire(List<DeckCell> enemyShips, List<DeckCell> selectedShip)
        {
            var neighborsPoints = DeckCell.GetNeighboringPoints(selectedShip, this.Range);
            var firedShipDecks = ShipManager.CheckEnemyShips(enemyShips, neighborsPoints);
            if (firedShipDecks.Count > 0)
            {
                foreach (DeckCell firedDeck in firedShipDecks)
                {
                    firedDeck.Deck.State = Enums.DeckState.Normal;
                    //Update to DB
                }
            }

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

        public virtual void Repair(List<DeckCell> shipDecks)
        {
            var neighborsPoints = DeckCell.GetNeighboringPoints(shipDecks, this.Range);
            var hurtedDecks = ShipManager.GetHurtedShip(neighborsPoints, shipDecks);
            if (hurtedDecks.Count > 0)
            {
                foreach (DeckCell hurtedDeck in hurtedDecks)
                {
                    hurtedDeck.Deck.State = Enums.DeckState.Normal;
                    //Update to DB
                }
            }
        }
    }
}
