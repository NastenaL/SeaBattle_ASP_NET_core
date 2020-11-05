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

        public static void DeleteGameFromDb(Game CurrantGame)
        {
            db.Games.Remove(CurrantGame);

            var fields = db.PlayingField.ToListAsync<PlayingField>().Result;
            var decks = db.Decks.ToListAsync<Deck>().Result;
            var cells = db.Cells.ToListAsync<Cell>().Result;
            var cellDecks = db.DeckCells.ToListAsync<DeckCell>().Result;

            foreach (var cell in CurrantGame.PlayingField.Ships)
            {
                foreach(DeckCell deckCell in cell.DeckCells)
                {
                    db.Decks.Remove(deckCell.Deck);
                    db.Cells.Remove(deckCell.Cell);
                    db.DeckCells.Remove(deckCell);
                }
            }

            var currentField = fields.Find(f => f == CurrantGame.PlayingField);
            if (currentField != null)
            {
                db.PlayingField.Remove(currentField);
            }

            db.SaveChanges();
        }

        public static void UpdateGameInDb(Game CurrantGame)
        {
            db.Update(CurrantGame);
            db.SaveChanges();
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
         
            db.PlayingField.Add(PlayingField);
            db.SaveChanges();
        }
    }
}
