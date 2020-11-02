namespace SeaBattleASP.Helpers
{
    using Microsoft.EntityFrameworkCore;
    using SeaBattleASP.Models;
    using SeaBattleASP.Models.Enums;
    using System;
    using System.Collections.Generic;

    public static class DbManager
    {
        public static ApplicationContext db;

        public static void SaveGameToDB(Game CurrantGame)
        {
            DbManager.db.Games.Add(CurrantGame);
            DbManager.db.SaveChanges();
        }

        public static void DeleteGameFromDb(Game CurrantGame)
        {
            DbManager.db.Games.Remove(CurrantGame);

            var fields = DbManager.db.PlayingField.ToListAsync<PlayingField>().Result;
            var decks = DbManager.db.Decks.ToListAsync<Deck>().Result;
            var cells = DbManager.db.Cells.ToListAsync<Cell>().Result;
            var cellDecks = DbManager.db.DeckCells.ToListAsync<DeckCell>().Result;

            foreach (var cell in CurrantGame.PlayingField.ShipsDeckCells)
            {
                DbManager.db.Decks.Remove(cell.Deck);
                DbManager.db.Cells.Remove(cell.Cell);
                DbManager.db.DeckCells.Remove(cell);
            }

            var currentField = fields.Find(f => f == CurrantGame.PlayingField);
            if (currentField != null)
            {
                DbManager.db.PlayingField.Remove(currentField);
            }

            DbManager.db.SaveChanges();
        }

        public static void UpdateGameInDb(Game CurrantGame)
        {
            DbManager.db.Update(CurrantGame);
            DbManager.db.SaveChanges();
        }

        public static void SaveShipToDB(Ship ship)
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
        }

        public static void SaveDeckCellAndPlayingFieldToDB(List<DeckCell> shipCoordinates, PlayingField PlayingField)
        {
            foreach (DeckCell deckCell in shipCoordinates)
            {
                db.Cells.Add(deckCell.Cell);
                db.Decks.Add(deckCell.Deck);
                db.DeckCells.Add(deckCell);
                db.SaveChanges();
            }

            db.PlayingField.Add(PlayingField);
            db.SaveChanges();
        }
    }
}
