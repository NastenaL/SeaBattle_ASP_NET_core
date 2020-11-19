namespace SeaBattleASP.Models
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Enums;

    public class DeckCell
    {
        #region Properties
        public int Id { get; set; }

        public Deck Deck { get; set; }

        public Cell Cell { get; set; }
        #endregion

        #region Methods
        public static DeckCell Create(Point point, 
                                      Deck deck)
        {
            DeckCell currentDeckCell = new DeckCell
            {
                Cell = new Cell { Color = CellColor.White, 
                                  X = point.X, 
                                  Y = point.Y, 
                                  State = CellState.ShipDeck 
                },
                Deck = deck
            };
            return currentDeckCell;
        }

        public static List<DeckCell> GetAll()
        {
            return DbManager.GetDeckCells();
        }

        public static List<Point> GetNeighboringPoints(List<DeckCell> shipDecks, int range)
        {
            List<Point> neighboringCoordinates = new List<Point>();
            foreach (DeckCell deckCell in shipDecks)
            {
                var left = CreateNewPoint(deckCell);
                var right = CreateNewPoint(deckCell);
                var up = CreateNewPoint(deckCell);
                var down = CreateNewPoint(deckCell);

                for (int i = 0; i < range; i++)
                {
                    left.X  -= 1;
                    right.X += 1;
                    up.Y    -= 1;
                    down.Y  += 1;
                    neighboringCoordinates.Add(left);
                    neighboringCoordinates.Add(right);
                    neighboringCoordinates.Add(up);
                    neighboringCoordinates.Add(down);
                }
            }

            var wrongPoints = neighboringCoordinates.Where(w => w.Y < 0 
                                                           || w.X < 0).ToList();

            if (wrongPoints.Count > 0)
            {
                foreach (Point point in wrongPoints)
                {
                    neighboringCoordinates.Remove(point);
                }
            }

            return new HashSet<Point>(neighboringCoordinates).ToList();
        }

        public static bool CheckDeckCellOutOfBorder(List<DeckCell> deckCell)
        {
            List<DeckCell> wrong = new List<DeckCell>();
            foreach (DeckCell dc in deckCell)
            {
                bool isShipOutAbroad = dc.Cell.X > Rules.FieldWidth - 1 
                                       || dc.Cell.Y > Rules.FieldHeight - 1;
                if (isShipOutAbroad)
                {
                    wrong.Add(dc);
                }
            }

            return wrong.Count > 0;
        }

        private static List<DeckCell> FindWrongDeckCells(Ship ship, List<DeckCell> currentShipDeckCells)
        {
            List<DeckCell> wrongDeckCells = new List<DeckCell>();
            foreach (DeckCell deckCell in currentShipDeckCells)
            {
                var dc = ship.DeckCells.Find(i => i.Cell.X == deckCell.Cell.X
                                             && i.Cell.Y == deckCell.Cell.Y);
                if (dc != null)
                {
                    wrongDeckCells.Add(dc);
                }

            }

            return wrongDeckCells;
        }

        public static bool CheckDeckCellOtherShips(List<DeckCell> currentShipDeckCells, 
                                                   Game game, 
                                                   Player player)
        {
            var allPlayersShips = ShipManager.GetAllPlayerShips(game,
                                                                player);

            List<DeckCell> wrongDeckCells = new List<DeckCell>();
            if (allPlayersShips.Count > 0)
            {
                foreach (Ship ship in allPlayersShips)
                {
                    wrongDeckCells = FindWrongDeckCells(ship, currentShipDeckCells);
                }
            }
            return wrongDeckCells.Count > 0;
        }

        private static Point CreateNewPoint(DeckCell deckCell)
        {
            return new Point(x: deckCell.Cell.X, 
                             y: deckCell.Cell.Y);
        }
        #endregion
    }
}