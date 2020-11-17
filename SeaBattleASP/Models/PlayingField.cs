namespace SeaBattleASP.Models
{
    using System.Collections.Generic;
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Models.Constants;

    public class PlayingField
    {
        public PlayingField()
        {
            this.Ships = new List<Ship>();
        }

        public int Id { get; set; }

        public List<Ship> Ships { get; set; }

        public int Width { get; set; }

        public int Heigth { get; set; }

        public static List<PlayingField> GetAllPlayingFields()
        {
            return DbManager.GetPlayingFields();
        }

        public PlayingField CreateField()
        {
            this.Width = Rules.FieldWidth;
            this.Heigth = Rules.FieldHeight;
            return this;
        }
    }
}
