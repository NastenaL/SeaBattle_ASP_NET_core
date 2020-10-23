namespace SeaBattleASP.Models
{
    using System.Collections.Generic;

    public class MapModel
    {
        public MapModel()
        {
            Coord = new List<Cell>();
            Players = new List<Player>();
            Ships = new List<Ship>();
        }
        public List<Cell> Coord { get; set; }
        public List<Player> Players { get; set; }
        public List<Ship> Ships { get; set; }
    }
}
