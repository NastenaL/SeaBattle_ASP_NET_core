using SeaBattleASP.Models.Enums;

namespace SeaBattleASP.Models
{
    public class Game
    {
        public int Id { get; set; }
        public PlayingField PlayingField { get; set; }
        public GameState State { get; set; }

        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public bool IsPl1Turn { get; set; }

        public void StartGame()
        {

        }

        public void EndGame()
        {

        }

        public void MakeStep()
        {

        }
    }
}
