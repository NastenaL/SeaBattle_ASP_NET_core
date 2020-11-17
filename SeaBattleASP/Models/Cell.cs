namespace SeaBattleASP.Models
{
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Models.Enums;
    using System.Collections.Generic;

    public class Cell
    {
        public int Id { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public CellColor Color { get; set; }

        public CellState State { get; set; }

        public static List<Cell> GetAll()
        {
            return DbManager.GetCells();
        }
    }
}
