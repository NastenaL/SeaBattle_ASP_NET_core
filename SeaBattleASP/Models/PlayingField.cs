namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Constants;
    using System.Collections.Generic;

    public class PlayingField
    {
        public int Id { get; set; }
        Dictionary<Cell, Ship> Ships { get; set; }
        public int Width { get; set; }
        public int Heigth {get; set;}

        public PlayingField CreateField()
        {
            PlayingField playingField = new PlayingField
            {
                Width = Rules.FieldWidth,
                Heigth = Rules.FieldHeight
            };

            return playingField;
        }
    }
}
