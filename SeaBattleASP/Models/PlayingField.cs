namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Constants;
    using System.Collections.Generic;

    public class PlayingField
    {
        public PlayingField()
        {
            PlayingShips = new List<PlayingShip>();
        }

        public int Id { get; set; }

        public List<PlayingShip> PlayingShips { get; set; }

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
