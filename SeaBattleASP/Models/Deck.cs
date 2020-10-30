namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Enums;

    public class Deck
    {
        public Deck(DeckState state, bool isHead)
        {  
            State = state;
            IsHead = isHead;
        }

        public int Id { get; set; }

        public DeckState State { get; set; }

        public bool IsHead { get; set; }
    }
}
