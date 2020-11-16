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
        public static List<DeckCell> GetEnemyShipsDeckCells(Game game)
        {
            List<DeckCell> enemyDeckCells = new List<DeckCell>();
            var allShips = Ship.GetAllShips();
            if (game.Player2 != null)
            {
                var enemyShips = allShips.Where(i => i.Id == game.Player2.Id).ToList();

                if (enemyShips.Count > 0)
                {
                    foreach (Ship s in enemyShips)
                    {
                        enemyDeckCells.AddRange(s.DeckCells);
                    }
                }
            }
            return enemyDeckCells;
        }
       

        public static bool CheckShipWithOtherShips(List<DeckCell> allPlayerDeckCells, List<DeckCell> ShipDeckCells)
        {
            bool error = false;
            if(allPlayerDeckCells.Count > 0)
            {
                List<DeckCell> commonCells = new List<DeckCell>();
                foreach (DeckCell deckCell in ShipDeckCells)
                {
                    if(deckCell.Cell != null)
                    {
                        commonCells.AddRange(allPlayerDeckCells.Where(s => s.Cell.X == deckCell.Cell.X && s.Cell.Y == deckCell.Cell.Y).ToList());
                    }
                  
                }
                if(commonCells.Count > 0)
                {
                    error = true;
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
    }
}
