namespace SeaBattleASP.Models
{
    public class DeckCell
    {
        public int id { get; set; }
        public Deck Deck { get; set; }
        public Cell Cell { get; set; }
    }
}