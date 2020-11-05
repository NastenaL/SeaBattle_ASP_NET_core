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
            foreach (var cell in CurrantGame.PlayingField.PlayingShips)
            {
                foreach (DeckCell deckCell in cell.Ship.DeckCells)
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

        public static void SaveShipToDB(Ship ship, PlayingShip playingShip)
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

            db.PlayingShips.Add(playingShip);
            db.SaveChanges();
        }

        public static void SaveDeckCellAndPlayingFieldToDB(List<DeckCell> shipCoordinates, PlayingField PlayingField)
        {
            foreach (DeckCell deckCell in shipCoordinates)
            {
                db.Cells.Add(deckCell.Cell);
                db.Decks.Add(deckCell.Deck);
                db.DeckCells.Add(deckCell);
            }
         
            db.PlayingFields.Add(PlayingField);
            db.SaveChanges();
        }
    }
}
