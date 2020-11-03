namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Constants;
    using System.Collections.Generic;

    public class MapModel
    {
        public int width;
        public int height;

        public MapModel()
        {
            Coord = new List<Cell>();
            Players = new List<Player>();
            Ships = new Dictionary<int, Ship>();
            width = Rules.FieldWidth;
            height = Rules.FieldHeight;
        }

        public List<Cell> Coord { get; set; }

        public List<Player> Players { get; set; }

        public Dictionary<int, Ship> Ships { get; set; }

        public Ship SelectedShip { get; set; }
    }
}
