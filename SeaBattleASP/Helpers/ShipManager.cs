﻿namespace SeaBattleASP.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using SeaBattleASP.Models;
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Enums;

    public static class ShipManager
    {
        public static List<DeckCell> GetEnemyShipsDeckCells(Game game, Player player)
        {
            var enemyPlayer = game.Player1 == player ? game.Player2 : game.Player1;
            List<DeckCell> enemyDeckCells = new List<DeckCell>();
      
            if (enemyPlayer != null)
            {
                var enemyShips = game.PlayingField.Ships.Where(s => s.Player == enemyPlayer).ToList();

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

        public static Ship ShiftShipDeckCell(string direction, Ship ship)
        {
            switch (direction)
            {
                case "left":
                    foreach (DeckCell deckCell in ship.DeckCells)
                    {
                        deckCell.Cell.X -= 1;
                    }

                    break;
                case "right":
                    foreach (DeckCell deckCell in ship.DeckCells)
                    {
                        deckCell.Cell.X += 1;
                    }

                    break;
                case "up":
                    foreach (DeckCell deckCell in ship.DeckCells)
                    {
                        deckCell.Cell.Y -= 1;
                    }

                    break;
                case "down":
                    foreach (DeckCell deckCell in ship.DeckCells)
                    {
                        deckCell.Cell.Y += 1;
                    }

                    break;
            }
            return ship;
        }

        public static List<Ship> GetAllPlayerShips(Game game,
                                                   Player player)
        {
            var allDeckCell = DeckCell.GetAll();
            var allShips = Ship.GetAll();
            var games = Game.GetAll();

            var allPlayersShips = game.PlayingField.Ships.Where(s => s.Player == player).ToList();
            var lastShip = allPlayersShips.Last();
            allPlayersShips.Remove(lastShip);

            return allPlayersShips;
        }

        public static List<DeckCell> GetDeckCellsForShip(List<DeckCell> deckCells,
                                                         Player player,
                                                        Game game)
        {
            Random random = new Random();
            Array shipDirections = Enum.GetValues(typeof(ShipDirection));
            List<DeckCell> resultDeckCells = new List<DeckCell>();

            var initalPoint = ShipManager.GetRandomPoint(random);
            var direction = (ShipDirection)shipDirections.GetValue(random.Next(shipDirections.Length));

            foreach (DeckCell deck in deckCells)
            {
                initalPoint = ShipManager.ShiftPoint(initalPoint,
                                                     direction);

                var currentDeckCell = DeckCell.Create(initalPoint,
                                                      deck.Deck);
                resultDeckCells.Add(currentDeckCell);
            }

            var isWrong = CheckNewDeckCells(resultDeckCells,
                              player,
                              game);
            if(isWrong)
            {
                resultDeckCells.Clear();
                resultDeckCells = GetDeckCellsForShip(deckCells,
                                    player,
                                    game);
            }

            return resultDeckCells;
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

        public static Point ShiftCell(Point p,
                                      bool IsXDirection,
                                      int Range)
        {
            if (IsXDirection)
            {
                p.X = p.X + Range > Rules.FieldWidth - 1 ? p.X - Range 
                                                         : p.X + Range; 
            }
            else
            {
                p.Y = p.Y + Range > Rules.FieldHeight - 1 ? p.Y - Range 
                                                          : p.Y + Range;
            }

            return p;
        }

        public static List<DeckCell> GetHurtedShip(List<Point> repairedPoints, 
                                                   List<DeckCell> allShipsDecks)
        {
            List<DeckCell> hurtedShips = new List<DeckCell>();
            foreach (Point point in repairedPoints)
            {
                var hurtedDeck = allShipsDecks.Find(s => s.Deck.State == Models.Enums.DeckState.Hurted 
                                                    && s.Cell.X == point.X 
                                                    && s.Cell.Y == point.Y);
                if (hurtedDeck != null)
                {
                    hurtedShips.Add(hurtedDeck);
                }
            }

            return hurtedShips;
        }

        public static List<DeckCell> CheckEnemyShips(List<DeckCell> enemyShips, 
                                                     List<Point> neighborsPoints)
        {
            List<DeckCell> result = new List<DeckCell>();

            foreach (Point point in neighborsPoints)
            {
                var firedDeck = enemyShips.Find(s => s.Cell.X == point.X 
                                                && s.Cell.Y == point.Y);
                if (firedDeck != null)
                {
                    result.Add(firedDeck);
                }
            }

            return result;
        }

        public static Point ShiftPoint(Point initalPoint, 
                                       ShipDirection direction)
        {
            initalPoint.X = direction == ShipDirection.horizontal ? initalPoint.X + 1 
                                                                  : initalPoint.X;
            initalPoint.Y = direction == ShipDirection.vertical ? initalPoint.Y + 1 
                                                                : initalPoint.Y;
            return initalPoint;
        }

        private static bool CheckNewDeckCells(List<DeckCell> deckCells,
                                            Player player,
                                            Game game)
        {
            var isOutOfBorder = DeckCell.CheckDeckCellOutOfBorder(deckCells);
            var isCrossOtherShip = DeckCell.CheckDeckCellOtherShips(deckCells,
                                                          game,
                                                          player);
            if (isOutOfBorder || isCrossOtherShip)
            {
                return true;
            }
            return false;
        }
    }
}