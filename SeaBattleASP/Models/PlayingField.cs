namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Constants;
    using System.Collections.Generic;

    public class PlayingField
    {
        public PlayingField()
        {
            Ships = new List<Ship>();
        }

        public int Id { get; set; }

        public List<Ship> Ships { get; set; }

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
