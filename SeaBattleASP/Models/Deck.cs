using SeaBattleASP.Models.Enums;

namespace SeaBattleASP.Models
{
    public class Deck
    {
        public int Id { get; set; }
        public DeckState State { get; set; }
        public bool IsHead { get; set; }
    }
}
