namespace SeaBattleASP.Models
{
    using Microsoft.EntityFrameworkCore;
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

        public static List<Cell> GetAllCells()
        {
            return DbManager.db.Cells.ToListAsync<Cell>().Result;
        }
    }
}
