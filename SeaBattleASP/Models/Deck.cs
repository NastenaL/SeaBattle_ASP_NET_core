namespace SeaBattleASP.Models
{
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Models.Enums;
    using System.Collections.Generic;

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

        public static List<Deck> GetAll()
        {
            return DbManager.GetDecks();
        }
    }
}
