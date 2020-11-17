namespace SeaBattleASP.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using SeaBattleASP.Models;
    using SeaBattleASP.Models.Enums;

    public static class DbManager
    {
        public static ApplicationContext Db { get; set; }

        #region GetEntities from DB
        public static List<Cell> GetCells()
        {
            return Db.Cells.ToListAsync<Cell>().Result;
        }

        public static List<Deck> GetDecks()
        {
            return Db.Decks.ToListAsync<Deck>().Result;
        }

        public static List<DeckCell> GetDeckCells()
        {
            return Db.DeckCells.ToListAsync<DeckCell>().Result;
        }

        public static List<Game> GetGames()
        {
            return Db.Games.ToListAsync<Game>().Result;
        }

        public static List<Player> GetPlayers()
        {
            return Db.Players.ToListAsync<Player>().Result;
        }

        public static List<PlayingField> GetPlayingFields()
        {
            return Db.PlayingFields.ToListAsync<PlayingField>().Result;
        }

        public static List<Ship> GetAllShips()
        {
            Db.Cells.ToListAsync<Cell>();
            Db.Decks.ToListAsync<Deck>();
            Db.DeckCells.ToListAsync<DeckCell>();

            var auxiliaryShips = Db.AuxiliaryShips.ToListAsync<AuxiliaryShip>().Result;
            var militaryShip = Db.MilitaryShips.ToListAsync<MilitaryShip>().Result;
            var mixShip = Db.MixShips.ToListAsync<MixShip>().Result;
            List<Ship> allShips = new List<Ship>();
            allShips.AddRange(auxiliaryShips);
            allShips.AddRange(militaryShip);
            allShips.AddRange(mixShip);
            return allShips;
        }
        #endregion

        #region Add commands
        public static void AddGame(Game game)
        {
            Db.Games.Add(game);
            Db.SaveChanges();
        }

        public static void AddPlayer(Player player)
        {
            Db.Players.Add(player);
            Db.SaveChanges();
        }

        public static void AddPlayingField(PlayingField playingField)
        {
            Db.PlayingFields.Add(playingField);
            Db.SaveChanges();
        }

        public static void AddShip(Ship ship)
        {
            AddDeckCell(ship);

            var type = Enum.Parse(typeof(ShipType), ship.GetShipType());

            switch ((ShipType)type)
            {
                case ShipType.AuxiliaryShip:
                    Db.AuxiliaryShips.Add((AuxiliaryShip)ship);
                    break;
                case ShipType.MilitaryShip:
                    Db.MilitaryShips.Add((MilitaryShip)ship);
                    break;
                case ShipType.MixShip:
                    Db.MixShips.Add((MixShip)ship);
                    break;
            }

            Db.SaveChanges();
        }
        #endregion

        #region Update commands
        public static void UpdateShip(List<DeckCell> hurtedShipDecks)
        {
            for (int i = 0; i < hurtedShipDecks.Count; i++)
            {
                Db.Decks.Update(hurtedShipDecks[i].Deck);
                Db.Cells.Update(hurtedShipDecks[i].Cell);
                Db.DeckCells.Update(hurtedShipDecks[i]);
                Db.SaveChanges();
            }
        }

        public static void UpdateGame(Game game)
        {
            Db.Update(game);
            Db.SaveChanges();
        }

        public static void UpdateShip(Ship ship)
        {
            var shipTypeEnum = Enum.Parse(typeof(ShipType), ship.GetShipType());

            switch ((ShipType)shipTypeEnum)
            {
                case ShipType.AuxiliaryShip:
                    Db.AuxiliaryShips.Update((AuxiliaryShip)ship);
                    break;
                case ShipType.MilitaryShip:
                    Db.MilitaryShips.Update((MilitaryShip)ship);
                    break;
                case ShipType.MixShip:
                    Db.MixShips.Update((MixShip)ship);
                    break;
            }

            Db.SaveChanges();
        }

        #endregion

        #region Delete commands
        public static void RemoveDecksAndCells(List<DeckCell> deckCells)
        {
            foreach (DeckCell deckCell in deckCells)
            {
                Db.Cells.Remove(deckCell.Cell);
                Db.Decks.Remove(deckCell.Deck);
            }

            Db.DeckCells.RemoveRange(deckCells);
            Db.SaveChanges();
        }

        public static void RemovePlayingField(PlayingField playingField)
        {
            Db.PlayingFields.Remove(playingField);
            Db.SaveChanges();
        }

        public static void RemoveGame(Game game)
        {
            Db.Games.Remove(game);
            Db.SaveChanges();
        }
        #endregion

        #region For deckCell
        private static void AddCell(Cell cell)
        {
            Db.Cells.Add(cell);
            Db.SaveChanges();
        }

        private static void AddDeck(Deck deck)
        {
            Db.Decks.Add(deck);
            Db.SaveChanges();
        }

        private static void AddDeckCellOnly(DeckCell deckCell)
        {
            Db.DeckCells.Add(deckCell);
            Db.SaveChanges();
        }

        private static void AddDeckCell(Ship ship)
        {
            foreach (DeckCell deckCell in ship.DeckCells.ToList())
            {
                AddCell(deckCell.Cell);
                AddDeck(deckCell.Deck);
                AddDeckCellOnly(deckCell);
            }
        }
        #endregion
    }
}