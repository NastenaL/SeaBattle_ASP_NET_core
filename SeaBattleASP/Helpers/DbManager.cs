namespace SeaBattleASP.Helpers
{
    using Microsoft.EntityFrameworkCore;
    using SeaBattleASP.Models;
    using SeaBattleASP.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DbManager
    {
        public static ApplicationContext db;

        public static void SaveGameToDB(Game game)
        {
            db.Games.Add(game);
            db.SaveChanges();
        }

        public static void SavePlayerToDB(Player player)
        {
            db.Players.Add(player);
            db.SaveChanges();
        }

        private static void DeleteDecksAndCells(Game game)
        {
            foreach (var cell in game.PlayingField.Ships)
            {
                foreach (DeckCell deckCell in cell.DeckCells)
                {
                    db.Decks.Remove(deckCell.Deck);
                    db.Cells.Remove(deckCell.Cell);
                    db.DeckCells.Remove(deckCell);
                }
            }
        }

        private static void DeletePlayingField(Game game)
        {
            var fields = db.PlayingFields.ToListAsync<PlayingField>().Result;
            var currentField = fields.Find(f => f == game.PlayingField);
            if (currentField != null)
            {
                db.PlayingFields.Remove(currentField);
            }
        }

        public static void UpdateShip(List<DeckCell> hurtedShipDecks)
        {
            for (int i = 0; i < hurtedShipDecks.Count; i++)//Save to DB
            {
                db.Decks.Update(hurtedShipDecks[i].Deck);
                db.Cells.Update(hurtedShipDecks[i].Cell);
                db.DeckCells.Update(hurtedShipDecks[i]);
                db.SaveChanges();
            }
        }

        public static void DeleteGameFromDb(Game game)
        {
            db.Games.Remove(game);
            DeleteDecksAndCells(game);
            DeletePlayingField(game);
            db.SaveChanges();
        }

        public static void UpdateGameInDb(Game game)
        {
            db.Update(game);
            db.SaveChanges();
        }

        private static void SaveDeckCellToDb(Ship ship)
        {
            foreach (DeckCell deckCell in ship.DeckCells.ToList())
            {
                SaveCell(deckCell.Cell);
                SaveDeck(deckCell.Deck);
                SaveDeckCell(deckCell);
            }
        }

        private static void SaveCell(Cell cell)
        {
            db.Cells.Add(cell);
            db.SaveChanges();
        }

        private static void SaveDeck(Deck deck)
        {
            db.Decks.Add(deck);
            db.SaveChanges();
        }

        private static void SaveDeckCell(DeckCell deckCell)
        {
            db.DeckCells.Add(deckCell);
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

        private static void SaveShip(Ship ship)
        {
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

        public static void SavePlayingFieldToDB(PlayingField PlayingField)
        {
            db.PlayingFields.Add(PlayingField);
            db.SaveChanges();
        }

        public static void SaveShipToDB(Ship ship)
        {
            SaveDeckCellToDb(ship);
            SaveShip(ship);
        }
    }
}
