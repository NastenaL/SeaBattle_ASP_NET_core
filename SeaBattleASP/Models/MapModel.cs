namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Constants;
    using System.Collections.Generic;

    public class MapModel
    {
        public MapModel()
        {
            Coord = new List<Cell>();
            Players = new List<Player>();
            Ships = new List<Ship>();
            width = Rules.FieldWidth;
            height = Rules.FieldHeight;
        }

        public int width;
        public int height;
        public List<Cell> Coord { get; set; }
        public List<Player> Players { get; set; }
        public List<Ship> Ships { get; set; }
    }
}
