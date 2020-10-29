﻿namespace SeaBattleASP.Helpers
{
    using SeaBattleASP.Models;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public static class PointManager
    {
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