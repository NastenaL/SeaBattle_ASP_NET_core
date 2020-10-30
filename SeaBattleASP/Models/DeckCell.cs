namespace SeaBattleASP.Models
{
    public class DeckCell
    {
        public int Id { get; set; }
        public Deck Deck { get; set; }
        public Cell Cell { get; set; }
    }
}