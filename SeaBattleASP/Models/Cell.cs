namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Enums;
    using System.Drawing;

    public class Cell
    {
        public Point Coordinate { get; set; }

        public CellColor Color { get; set; }

        public CellState State { get; set; }
    }
}
