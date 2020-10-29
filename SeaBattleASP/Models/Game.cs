namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Enums;
    using System;

    public class Game
    {
        public int Id { get; set; }

        public PlayingField PlayingField { get; set; }

        public GameState State { get; set; }

        public Player Player1 { get; set; }

        public Player Player2 { get; set; }

        public bool IsPl1Turn { get; set; }


        public bool StartGame()
        {
            this.State = GameState.Started;

            Random gen = new Random();
            this.IsPl1Turn = gen.Next(100) < 50 ? true : false;
            return this.IsPl1Turn;
        }

        public void EndGame()
        {
            this.State = GameState.Finished;
        }

        public void MakeStep()
        {
        }
    }
}
