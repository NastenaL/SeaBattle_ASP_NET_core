namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Enums;
    using System;
    using System.Collections.Generic;

    public class Game
    {
        public int Id { get; set; }

        public PlayingField PlayingField { get; set; }

        public GameState State { get; set; }

        public Player Player1 { get; set; }

        public Player Player2 { get; set; }

        public bool IsPl1Turn { get; set; }

        public static Game Create(List<Player> players, int player1Id, int player2Id, PlayingField playingField)
        {
            Game game = new Game
            {
                Player1 = players.Find(p => p.Id == player1Id),
                Player2 = players.Find(p => p.Id == player2Id),
                PlayingField = playingField
            };
            return game;
        }
        public void StartGame()
        {
            this.State = GameState.Started;

            Random gen = new Random();
            this.IsPl1Turn = gen.Next(100) < 50 ? true : false;
        }

        public void EndGame()
        {
            this.State = GameState.Finished;
        }
    }
}
