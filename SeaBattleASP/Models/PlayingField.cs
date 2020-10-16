namespace SeaBattleASP.Models
{
    using System.Collections.Generic;

    public class PlayingField
    {
        public int Id { get; set; }
        Dictionary<Cell, Ship> Ships { get; set; }
        public int Width { get; set; }
        public int Heigth {get; set;}
    }
}
