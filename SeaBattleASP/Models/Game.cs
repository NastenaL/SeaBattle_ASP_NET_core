namespace SeaBattleASP.Models
{
    using System;
    using System.Collections.Generic;
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Models.Enums;

    public class Game
    {
        #region Properties
        public int Id { get; set; }

        public PlayingField PlayingField { get; set; }

        public GameState State { get; set; }

        public Player Player1 { get; set; }

        public Player Player2 { get; set; }

        public bool IsPl1Turn { get; set; }
        #endregion

        #region Method
        public static List<Game> GetAll()
        {
            return DbManager.GetGames();
        }

        public bool StartGame()
        {
            this.State = GameState.Started;

            Random gen = new Random();
            this.IsPl1Turn = gen.Next(100) < 50;
            return this.IsPl1Turn;
        }

        public void EndGame()
        {
            this.State = GameState.Finished;
        }

        public static Game GetGameById(int gameId)
        {
            LoadRelatedEntities();
            var games = Game.GetAll();
            var game = games.Find(g => g.Id == gameId);
            return game;
        }

        private static void LoadRelatedEntities()
        {
            Player.GetPlayers();
            PlayingField.GetAllPlayingFields();
            DeckCell.GetAll();
            Cell.GetAll();
            Deck.GetAll();
            Ship.GetAll();
        }

        #endregion
    }
}
