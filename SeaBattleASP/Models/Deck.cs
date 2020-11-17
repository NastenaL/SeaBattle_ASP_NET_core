namespace SeaBattleASP.Models
{
    using System.Collections.Generic;
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Models.Enums;

    public class Deck
    {
        public Deck(DeckState state, bool isHead)
        {  
            this.State = state;
            this.IsHead = isHead;
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
