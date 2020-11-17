namespace SeaBattleASP.Models
{
    using System.Collections.Generic;
    using SeaBattleASP.Models.Constants;

    public class MapModel
    {
        #region Fields
        public int width;
        public int height;
        #endregion

        public MapModel()
        {
            this.Coord = new List<Cell>();
            this.Players = new List<Player>();
            this.Ships = new Dictionary<int, Ship>();
            this.HurtedShips = new List<DeckCell>();
            this.RepairedShips = new List<DeckCell>();
            this.Games = new List<Game>();
            this.width = Rules.FieldWidth;
            this.height = Rules.FieldHeight;
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
