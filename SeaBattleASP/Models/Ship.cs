namespace SeaBattleASP.Models
{
    using SeaBattleASP.Helpers;
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

        public virtual void Fire(List<DeckCell> enemyShips, List<DeckCell> selectedShip)
        {
            var neighborsPoints = PointManager.GetNeighboringPoints(selectedShip, this.Range);
            var firedShips = PointManager.CheckEnemyShips(enemyShips, neighborsPoints);
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
                        X = cell.Cell.Coordinate.X,
                        Y = cell.Cell.Coordinate.Y
                    };

                    p.X = this.Direction.X != 0 ? p.X + this.Range : p.X;
                    p.Y = this.Direction.Y != 0 ? p.Y + this.Range : p.Y;

                    CheckNewCoordinate(p, result, shipDecks);

                    cell.Cell.Coordinate = p;
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
            var neighborsPoints = PointManager.GetNeighboringPoints(shipDecks, this.Range);
            var hurtedDecks = PointManager.GetHurtesShip(neighborsPoints, shipDecks);
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
