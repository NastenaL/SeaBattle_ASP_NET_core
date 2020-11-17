namespace SeaBattleASP.Helpers
{
    using SeaBattleASP.Models;
    using SeaBattleASP.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DbManager
    {
        public static ApplicationContext db;

        #region Add commands
        private static void AddDeckCellToDb(Ship ship)
        {
            foreach (DeckCell deckCell in ship.DeckCells.ToList())
            {
                AddCell(deckCell.Cell);
                AddDeck(deckCell.Deck);
                AddDeckCell(deckCell);
            }
        }

        #region For deckCell
        private static void AddCell(Cell cell)
        {
            db.Cells.Add(cell);
            db.SaveChanges();
        }

        private static void AddDeck(Deck deck)
        {
            db.Decks.Add(deck);
            db.SaveChanges();
        }

        private static void AddDeckCell(DeckCell deckCell)
        {
            db.DeckCells.Add(deckCell);
            db.SaveChanges();
        }
        #endregion

        public static void AddGameToDB(Game game)
        {
            db.Games.Add(game);
            db.SaveChanges();
        }

        public static void AddPlayer(Player player)
        {
            db.Players.Add(player);
            db.SaveChanges();
        }

        public static void AddPlayingField(PlayingField PlayingField)
        {
            db.PlayingFields.Add(PlayingField);
            db.SaveChanges();
        }

        public static void AddShip(Ship ship)
        {
            AddDeckCellToDb(ship);

            var shipType = ship.GetType();
            var type = Enum.Parse(typeof(ShipType), shipType.Name);

            switch ((ShipType)type)
            {
                case ShipType.AuxiliaryShip:
                    db.AuxiliaryShips.Add((AuxiliaryShip)ship);
                    break;
                case ShipType.MilitaryShip:
                    db.MilitaryShips.Add((MilitaryShip)ship);
                    break;
                case ShipType.MixShip:
                    db.MixShips.Add((MixShip)ship);
                    break;
            };

            db.SaveChanges();
        }
        #endregion

        #region Update
        public static void UpdateShip(List<DeckCell> hurtedShipDecks)
        {
            for (int i = 0; i < hurtedShipDecks.Count; i++)
            {
                db.Decks.Update(hurtedShipDecks[i].Deck);
                db.Cells.Update(hurtedShipDecks[i].Cell);
                db.DeckCells.Update(hurtedShipDecks[i]);
                db.SaveChanges();
            }
        }

        public static void UpdateGame(Game game)
        {
            db.Update(game);
            db.SaveChanges();
        }

        public static void UpdateShip(Ship ship)
        {
            foreach (DeckCell deckCell in ship.DeckCells.ToList())
            {
                db.Cells.Update(deckCell.Cell);
                db.Decks.Update(deckCell.Deck);
                db.DeckCells.Update(deckCell);
                db.SaveChanges();
            }

            var shipType = ship.GetType();
            var shipTypeEnum = Enum.Parse(typeof(ShipType), shipType.Name);

            switch ((ShipType)shipTypeEnum)
            {
                case ShipType.AuxiliaryShip:
                    db.AuxiliaryShips.Update((AuxiliaryShip)ship);
                    break;
                case ShipType.MilitaryShip:
                    db.MilitaryShips.Update((MilitaryShip)ship);
                    break;
                case ShipType.MixShip:
                    db.MixShips.Update((MixShip)ship);
                    break;
            }

            db.SaveChanges();
        }

        #endregion

        #region Delete from DB
        public static void RemoveDecksAndCells(Ship ship)
        {
            foreach (DeckCell deckCell in ship.DeckCells)
            {
                DbManager.db.Cells.Remove(deckCell.Cell);
                DbManager.db.Decks.Remove(deckCell.Deck);
            }
            DbManager.db.DeckCells.RemoveRange(ship.DeckCells);
            DbManager.db.SaveChanges();
        }

        public static void RemovePlayingField(PlayingField playingField)
        {
            db.PlayingFields.Remove(playingField);
            db.SaveChanges();
        }

        public static void RemoveGameFromDb(Game game)
        {
            db.Games.Remove(game);
            db.SaveChanges();
        }
        #endregion
    }
}
