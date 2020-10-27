namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Constants;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public class PlayingField
    {
        public PlayingField()
        {
            Ships = new List<DeckCell>();
        }
        public int Id { get; set; }

        [NotMapped]
        public List<DeckCell> Ships { get; set; }

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
