namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Drawing;

    public abstract class Ship : IRepairable, IFireable
    {
        public int Id { get; set; }

        public int Range { get; set; }

        public Point Direction { get; set; }

        [NotMapped]
        public Player Player { get; set; }

        public Guid PlayerId { get; set; }

        [NotMapped]
        public List<Deck> Decks { get; set; }

        public bool IsSelectedShip { get; set; }

        public abstract void Fire();

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
                        X = cell.Cell.Coordinate.X,
                        Y = cell.Cell.Coordinate.Y
                    };

                    if (this.Direction.X != 0)
                    {
                        p.X += this.Range;
                    }
                    else
                    {
                        p.Y += this.Range;
                    }

                    CheckNewCoordinate(p, result, shipDecks);

                    cell.Cell.Coordinate = p;
                    result.Add(cell);
                }
            }
            return result;
        }

        private void CheckNewCoordinate(Point p, List<DeckCell> result, List<DeckCell> shipDecks)
        {
            if (p.X > Rules.FieldWidth)
            {
                var newDirection = new Point() { X = 0, Y = 1 };
                this.Direction = newDirection;

                result.Clear();
                Move(shipDecks);
            }
            else if (p.Y > Rules.FieldHeight)
            {
                var newDirection = new Point() { X = 1, Y = 0 };
                this.Direction = newDirection;

                result.Clear();
                Move(shipDecks);
            }
        }
        public abstract void Repair();
    }
}
