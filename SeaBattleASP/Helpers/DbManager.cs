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

        public static void SaveGameToDB(Game CurrantGame)
        {
            db.Games.Add(CurrantGame);
            db.SaveChanges();
        }

        public static void SavePlayerToDB(Player player)
        {
            db.Players.Add(player);
            db.SaveChanges();
        }

        private static void DeleteDecksAndCells(Game CurrantGame)
        {
            foreach (var cell in CurrantGame.PlayingField.Ships)
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
                DbManager.db.Decks.Update(hurtedShipDecks[i].Deck);
                DbManager.db.Cells.Update(hurtedShipDecks[i].Cell);
                DbManager.db.DeckCells.Update(hurtedShipDecks[i]);
                DbManager.db.SaveChanges();
            }
        }

        public static void DeleteGameFromDb(Game game)
        {
            db.Games.Remove(game);
            DeleteDecksAndCells(game);
            DeletePlayingField(game);
            db.SaveChanges();
        }

        public static void UpdateGameInDb(Game CurrantGame)
        {
            db.Update(CurrantGame);
            db.SaveChanges();
        }

        private static void SaveDeckCell(Ship ship)
        {
            foreach (DeckCell deckCell in ship.DeckCells)
            {
                db.Cells.Add(deckCell.Cell);
                db.Decks.Add(deckCell.Deck);
                db.DeckCells.Add(deckCell);
            }
            db.SaveChanges();
        }

        public static void UpdateShip(Ship ship)
        {
            foreach (DeckCell deckCell in ship.DeckCells.ToList())
            {
                DbManager.db.Cells.Update(deckCell.Cell);
                DbManager.db.Decks.Update(deckCell.Deck);
                DbManager.db.DeckCells.Update(deckCell);
                DbManager.db.SaveChanges();
            }

            var shipType = ship.GetType();
            var shipTypeEnum = Enum.Parse(typeof(ShipType), shipType.Name);

            switch ((ShipType)shipTypeEnum)
            {
                case ShipType.AuxiliaryShip:
                    DbManager.db.AuxiliaryShips.Update((AuxiliaryShip)ship);
                    break;
                case ShipType.MilitaryShip:
                    DbManager.db.MilitaryShips.Update((MilitaryShip)ship);
                    break;
                case ShipType.MixShip:
                    DbManager.db.MixShips.Update((MixShip)ship);
                    break;
            }

            DbManager.db.SaveChanges();
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
            SaveDeckCell(ship);
            SaveShip(ship);
        }
    }
}
