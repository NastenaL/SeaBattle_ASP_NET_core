namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Constants;
    using System.Collections.Generic;

    public class MapModel
    {
        #region Fields
        public int width;
        public int height;
        #endregion

        public MapModel()
        {
            Coord = new List<Cell>();
            Players = new List<Player>();
            Ships = new Dictionary<int, Ship>();
            width = Rules.FieldWidth;
            height = Rules.FieldHeight;
        }

        #region Property
        public List<Cell> Coord { get; set; }

        public List<Player> Players { get; set; }

        public Dictionary<int, Ship> Ships { get; set; }

        public Ship SelectedShip { get; set; }

        public Game CurrentGame { get; set; }
        #endregion

        public static void FillMapModelWithCoordinates(List<DeckCell> shipCoordinates, MapModel Model)
        {
            foreach (var shipDeckCell in shipCoordinates)
            {
                Model.Coord.Add(shipDeckCell.Cell);
            }
        }
    }
}
