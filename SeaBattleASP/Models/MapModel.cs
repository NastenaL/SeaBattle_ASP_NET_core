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
            HurtedShips = new List<DeckCell>();
            RepairedShips = new List<DeckCell>();
            Games = new List<Game>();
            width = Rules.FieldWidth;
            height = Rules.FieldHeight;
        }

        #region Property
        public List<Cell> Coord { get; set; }

        public List<Game> Games { get; set; }

        public List<Player> Players { get; set; }

        public List<DeckCell> RepairedShips { get; set; }

        public List<DeckCell> HurtedShips { get; set; }

        public Dictionary<int, Ship> Ships { get; set; }

        public Ship SelectedShip { get; set; }

        public Game CurrentGame { get; set; }

        public string Message { get; set; }
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
