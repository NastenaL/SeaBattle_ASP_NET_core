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

        #region GetEntities from DB
        public static List<Cell> GetCells()
        {
            return db.Cells.ToListAsync<Cell>().Result;
        }

        public static List<Deck> GetDecks()
        {
            return db.Decks.ToListAsync<Deck>().Result;
        }

        public static List<DeckCell> GetDeckCells()
        {
            return db.DeckCells.ToListAsync<DeckCell>().Result;
        }

        public static List<Game> GetGames()
        {
            return db.Games.ToListAsync<Game>().Result;
        }

        public static List<Player> GetPlayers()
        {
            return db.Players.ToListAsync<Player>().Result;
        }

        public static List<PlayingField> GetPlayingFields()
        {
            return db.PlayingFields.ToListAsync<PlayingField>().Result;
        }

        public static List<Ship> GetAllShips()
        {
            db.Cells.ToListAsync<Cell>();
            db.Decks.ToListAsync<Deck>();
            db.DeckCells.ToListAsync<DeckCell>();

            var auxiliaryShips = db.AuxiliaryShips.ToListAsync<AuxiliaryShip>().Result;
            var militaryShip = db.MilitaryShips.ToListAsync<MilitaryShip>().Result;
            var mixShip = db.MixShips.ToListAsync<MixShip>().Result;
            List<Ship> allShips = new List<Ship>();
            allShips.AddRange(auxiliaryShips);
            allShips.AddRange(militaryShip);
            allShips.AddRange(mixShip);
            return allShips;
        }
        #endregion

        #region Add commands
        private static void AddDeckCell(Ship ship)
        {
            foreach (DeckCell deckCell in ship.DeckCells.ToList())
            {
                AddCell(deckCell.Cell);
                AddDeck(deckCell.Deck);
                AddDeckCellOnly(deckCell);
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

        private static void AddDeckCellOnly(DeckCell deckCell)
        {
            db.DeckCells.Add(deckCell);
            db.SaveChanges();
        }
        #endregion

        public static void AddGame(Game game)
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
            AddDeckCell(ship);

            var type = Enum.Parse(typeof(ShipType), ship.GetShipType());

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

        #region Update commands
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

            var shipTypeEnum = Enum.Parse(typeof(ShipType), ship.GetShipType());

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

        #region Delete commands
        public static void RemoveDecksAndCells(Ship ship)
        {
            foreach (DeckCell deckCell in ship.DeckCells)
            {
                db.Cells.Remove(deckCell.Cell);
                db.Decks.Remove(deckCell.Deck);
            }
            db.DeckCells.RemoveRange(ship.DeckCells);
            db.SaveChanges();
        }

        public static void RemovePlayingField(PlayingField playingField)
        {
            db.PlayingFields.Remove(playingField);
            db.SaveChanges();
        }

        public static void RemoveGame(Game game)
        {
            db.Games.Remove(game);
            db.SaveChanges();
        }
        #endregion
    }
}