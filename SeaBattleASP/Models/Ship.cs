namespace SeaBattleASP.Models
{
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Enums;
    using SeaBattleASP.Models.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Drawing;

    public abstract class Ship : IRepairable, IFireable
    {
        public Ship()
        {
            DeckCells = new List<DeckCell>();
        }

        public int Id { get; set; }

        public int Range { get; set; }

        [NotMapped]
        public Point Direction { get; set; }

        public Player Player { get; set; }

        [NotMapped]
        public ShipType ShipType { get; set; }

        [NotMapped]
        public List<DeckCell> DeckCells { get; set; }

        public bool IsSelectedShip { get; set; }

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

        public List<DeckCell> Move(List<DeckCell> shipDecks)
        {
            List<DeckCell> result = new List<DeckCell>();
            var head = shipDecks.Find(s =>s.Deck.IsHead);
            if(this.Direction.X != 0)
            {
               foreach(DeckCell shipDeck in shipDecks)
                {
                    DeckCell cell = new DeckCell
                    {
                        Cell = shipDeck.Cell,
                        Deck = shipDeck.Deck
                    };
                    Point p = new Point
                    {
                        X = cell.Cell.X,
                        Y = cell.Cell.Y
                    };

                    p.X = this.Direction.X != 0 ? p.X + this.Range : p.X;
                    p.Y = this.Direction.Y != 0 ? p.Y + this.Range : p.Y;

                    CheckNewCoordinate(p, result, shipDecks);

                    cell.Cell.X = p.X;
                    cell.Cell.Y = p.Y;

                    result.Add(cell);
                }
            }
            return result;
        }

        private void CheckNewCoordinate(Point p, List<DeckCell> result, List<DeckCell> shipDecks)
        {
            if (p.X > Rules.FieldWidth-1 || p.Y > Rules.FieldHeight-1)
            {
                var newDirection = p.X > Rules.FieldWidth? new Point() { X = 0, Y = 1 }: new Point() { X = 1, Y = 0 };
                this.Direction = newDirection;

                result.Clear();
                Move(shipDecks);
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
