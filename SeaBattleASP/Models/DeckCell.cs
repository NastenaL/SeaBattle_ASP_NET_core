namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Enums;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class DeckCell
    {
        public int Id { get; set; }
        public Deck Deck { get; set; }
        public Cell Cell { get; set; }

        public static DeckCell Create(Point point, Deck deck)
        {
            DeckCell currentDeckCell = new DeckCell
            {
                Cell = new Cell { Color = CellColor.White, X = point.X, Y = point.Y, State = CellState.ShipDeck },
                Deck = deck
            };
            return currentDeckCell;
        }

        private static Point CreateNewPoint(DeckCell deckCell)
        {
            return new Point(x: deckCell.Cell.X, y: deckCell.Cell.Y);
        }

        public static List<Point> GetNeighboringPoints(List<DeckCell> shipDecks, int range)
        {
            List<Point> NeighboringCoordinates = new List<Point>();
            foreach (DeckCell deckCell in shipDecks)
            {
                var leftPoint = CreateNewPoint(deckCell);
                var rightPoint = CreateNewPoint(deckCell);
                var upPoint = CreateNewPoint(deckCell);
                var downPoint = CreateNewPoint(deckCell);

                for (int i = 0; i < range; i++)
                {
                    leftPoint.X  -= 1;
                    rightPoint.X += 1;
                    upPoint.Y    -= 1;
                    downPoint.Y  += 1;
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