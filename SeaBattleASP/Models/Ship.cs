namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Drawing;
    using System.Linq;

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
        public virtual void Repair(List<DeckCell> shipDecks)
        {
            var neighborsPoints = GetNeighboringPoints(shipDecks);
            var hurtedDecks = GetHurtesShip(neighborsPoints, shipDecks);
            if (hurtedDecks.Count > 0)
            {
                foreach (DeckCell hurtedDeck in hurtedDecks)
                {
                    hurtedDeck.Deck.State = Enums.DeckState.Normal;
                    //Update to DB
                }
            }
        }

        private List<DeckCell> GetHurtesShip(List<Point> repairedPoints, List<DeckCell> allShipsDecks)
        {
            List<DeckCell> hurtedShips = new List<DeckCell>();
            foreach (Point point in repairedPoints)
            {
                var hurtedDeck = allShipsDecks.Find(s => s.Deck.State == Enums.DeckState.Hurted && s.Cell.Coordinate == point);
                if (hurtedDeck != null)
                {
                    hurtedShips.Add(hurtedDeck);
                }

            }
            return hurtedShips;
        }

        private List<Point> GetNeighboringPoints(List<DeckCell> shipDecks)
        {
            List<Point> NeighboringCoordinates = new List<Point>();
            foreach (DeckCell deckCell in shipDecks)
            {
                var leftPoint = new Point();
                var rightPoint = new Point();
                var upPoint = new Point();
                var downPoint = new Point();

                leftPoint = deckCell.Cell.Coordinate;
                rightPoint = deckCell.Cell.Coordinate;
                upPoint = deckCell.Cell.Coordinate;
                downPoint = deckCell.Cell.Coordinate;

                for (int i = 0; i < this.Range; i++)
                {
                    leftPoint.X -= 1;
                    rightPoint.X += 1;
                    upPoint.Y -= 1;
                    downPoint.Y += 1;
                    NeighboringCoordinates.Add(leftPoint);
                    NeighboringCoordinates.Add(rightPoint);
                    NeighboringCoordinates.Add(upPoint);
                    NeighboringCoordinates.Add(downPoint);
                }

            }
            var wrongPoints = NeighboringCoordinates.Where(w => w.Y < 0 || w.X < 0).ToList();

            if (wrongPoints.Count > 0)
            {
                foreach (Point point in wrongPoints)
                {
                    NeighboringCoordinates.Remove(point);
                }
            }

            return new HashSet<Point>(NeighboringCoordinates).ToList();
        }

    }
}
