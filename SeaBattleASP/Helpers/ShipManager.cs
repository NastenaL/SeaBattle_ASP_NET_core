namespace SeaBattleASP.Helpers
{
    using SeaBattleASP.Models;
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public static class ShipManager
    {
        public static void FillMapModel(List<DeckCell> shipCoordinates, MapModel Model)
        {
            foreach (var shipDeckCell in shipCoordinates)
            {
                Model.Coord.Add(shipDeckCell.Cell);
            }
        }

        public static bool CheckShipWithOtherShips(Point point, PlayingField PlayingField)
        {
            bool error = false;
            if (PlayingField.ShipsDeckCells.Count > 1)
            {
                foreach (var p in PlayingField.ShipsDeckCells.ToList())
                {
                    if (point.X == p.Cell.X && point.Y == p.Cell.Y) //Check coincidence cells
                                                                    /* || ((point.X == po.Key.Coordinate.X + 1 && point.Y == po.Key.Coordinate.Y) ||//Check adjacent cells
                                                                         (point.X == po.Key.Coordinate.X && point.Y == po.Key.Coordinate.Y + 1))) */
                    {
                        error = true;
                    }

                }
            }
            return error;
        }

        public static bool CheckPointAbroad(Point point)
        {
            bool isShipAbroad = false;
            if (point.X > Rules.FieldWidth - 1 || point.Y > Rules.FieldHeight - 1)//Check abroad
            {
                isShipAbroad = true;
            }

            return isShipAbroad;
        }

        public static Point GetRandomPoint(Random random)
        {
            Point point = new Point
            {
                X = random.Next(0, Rules.FieldWidth - 1),
                Y = random.Next(0, Rules.FieldHeight - 1)
            };
            return point;
        }

        public static DeckCell CreateDeckCell(Point point, Deck deck)
        {
            DeckCell currentDeckCell = new DeckCell
            {
                Cell = new Cell { Color = CellColor.White, X = point.X, Y = point.Y, State = CellState.ShipDeck },
                Deck = deck
            };
            return currentDeckCell;
        }

        public static List<DeckCell> GetHurtedShip(List<Point> repairedPoints, List<DeckCell> allShipsDecks)
        {
            List<DeckCell> hurtedShips = new List<DeckCell>();
            foreach (Point point in repairedPoints)
            {
                var hurtedDeck = allShipsDecks.Find(s => s.Deck.State == Models.Enums.DeckState.Hurted && s.Cell.X == point.X && s.Cell.Y == point.Y);
                if (hurtedDeck != null)
                {
                    hurtedShips.Add(hurtedDeck);
                }

            }
            return hurtedShips;
        }

        public static List<DeckCell> CheckEnemyShips(List<DeckCell> enemyShips, List<Point> neighborsPoints)
        {
            List<DeckCell> result = new List<DeckCell>();

            foreach (Point point in neighborsPoints)
            {
                var firedDeck = enemyShips.Find(s => s.Cell.X == point.X && s.Cell.Y == point.Y);
                if (firedDeck != null)
                {
                    result.Add(firedDeck);
                }

            }
            return result;
        }

        public static Point ShiftPoint(Point initalPoint, ShipDirection direction)
        {
            initalPoint.X = direction == ShipDirection.horizontal ? initalPoint.X + 1 : initalPoint.X;
            initalPoint.Y = direction == ShipDirection.vertical ? initalPoint.Y + 1 : initalPoint.Y;
            return initalPoint;
        }

        public static List<Point> GetNeighboringPoints(List<DeckCell> shipDecks, int range)
        {
            List<Point> NeighboringCoordinates = new List<Point>();
            foreach (DeckCell deckCell in shipDecks)
            {
                var leftPoint = new Point();
                var rightPoint = new Point();
                var upPoint = new Point();
                var downPoint = new Point();

                leftPoint.X = deckCell.Cell.X;
                leftPoint.Y = deckCell.Cell.Y;
                rightPoint.X = deckCell.Cell.X;
                rightPoint.Y = deckCell.Cell.Y;
                upPoint.X = deckCell.Cell.X;
                upPoint.Y = deckCell.Cell.Y;
                downPoint.X = deckCell.Cell.X;
                downPoint.Y = deckCell.Cell.Y;

                for (int i = 0; i < range; i++)
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
