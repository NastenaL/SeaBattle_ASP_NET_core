namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Enums;

    public class Cell
    {
        public int Id { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public CellColor Color { get; set; }

        public CellState State { get; set; }
    }
}
