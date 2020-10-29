namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Enums;

    public class Deck
    {
        public Deck(int id, DeckState state, bool isHead)
        {
            Id = id;
            State = state;
            IsHead = isHead;
        }

        public int Id { get; set; }

        public DeckState State { get; set; }

        public bool IsHead { get; set; }

        public int ShipId { get; set; }
    }
}
