namespace SeaBattleASP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class AuxiliaryShip : Ship
    {
        public override void Fire()
        {
            throw new NotImplementedException();
        }

        public override void Repair(List<DeckCell> shipDecks)
        {
            var neighborsPoints = GetNeighboringPoints(shipDecks);
            var hurtedDecks = GetHurtesShip(neighborsPoints, shipDecks);
            if(hurtedDecks.Count > 0)
            {
                foreach(DeckCell hurtedDeck in hurtedDecks)
                {
                    hurtedDeck.Deck.State = Enums.DeckState.Normal;
                    //Update to DB
                }
            }
        }

        private List<DeckCell> GetHurtesShip(List<Point> repairedPoints, List<DeckCell> allShipsDecks)
        {
            List<DeckCell> hurtedShips = new List<DeckCell>();
            foreach(Point point in repairedPoints)
            {
                var hurtedDeck = allShipsDecks.Find(s => s.Deck.State == Enums.DeckState.Hurted && s.Cell.Coordinate == point);
                if(hurtedDeck != null)
                {
                    hurtedShips.Add(hurtedDeck);
                }
                
            }
            return hurtedShips;
        }

        private List<Point> GetNeighboringPoints(List<DeckCell> shipDecks)
        {
            List<Point> NeighboringCoordinates = new List<Point>();
            foreach(DeckCell deckCell in shipDecks)
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
