namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Constants;
    using System.Collections.Generic;

    public class PlayingField
    {
        public PlayingField()
        {
            ShipsDeckCells = new List<DeckCell>();
        }

        public int Id { get; set; }

        public List<DeckCell> ShipsDeckCells { get; set; }

        public int Width { get; set; }

        public int Heigth {get; set;}

        public PlayingField CreateField()
        {
            this.Width = Rules.FieldWidth;
            this.Heigth = Rules.FieldHeight;
            return this;
        }
    }
}
