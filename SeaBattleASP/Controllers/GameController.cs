namespace SeaBattleASP.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Models;
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Enums;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public class GameController : Controller
    {
        private readonly Random random;
        readonly Array shipDirections = Enum.GetValues(typeof(ShipDirection));

        public GameController(ApplicationContext context)
        {
            DbManager.db = context;
            random = new Random();
            CurrantGame = new Game();
            Model = new MapModel
            {
                Ships = Rules.CreateShips()
            };

            PlayingField = new PlayingField();
            PlayingField.CreateField();
        }

        #region Properties 
        PlayingField PlayingField { get; set; }
       
        private Game CurrantGame { get; set; }
        private MapModel Model { get; set; }
        #endregion

        #region Step methods
        [HttpPost]
        public void MakeFireStep(int shipId)
        {
            if(CurrantGame != null)
            {
                var ship = Ship.GetShipByIdFromDB(shipId);

                List<DeckCell> enemyDeckCells = GetEnemyShips();

                if (enemyDeckCells.Count > 0)
                {
                    ship.Fire(enemyDeckCells);
                }
            }
        }

        [HttpPost]
        public void MakeRepairStep(int shipId)
        {
            var ship = Ship.GetShipByIdFromDB(shipId);
            var allShips = Ship.GetAllShips();
            var allPlayerShips = allShips.Where(i => i.Player == ship.Player).ToList();
            ship.Repair(allPlayerShips);
        }

        [HttpPost]
        public Ship MakeMoveStep(int shipId)
        {
            var ship = Ship.GetShipByIdFromDB(shipId);
            if (ship != null)
            {
                var shipDeckCells = ship.Move();
                if (shipDeckCells.Count > 0)
                {
                    ship.DeckCells = shipDeckCells;
                    DbManager.UpdateShip(ship);
                }
            }
            return ship;
        }
        #endregion

        private List<DeckCell> GetEnemyShips()
        {
            List<DeckCell> enemyDeckCells = new List<DeckCell>();
            var allShips = Ship.GetAllShips();
            if (CurrantGame.Player2 != null)
            {
                var enemyShips = allShips.Where(i => i.Id == CurrantGame.Player2.Id).ToList();

                if (enemyShips.Count > 0)
                {
                    foreach (Ship s in enemyShips)
                    {
                        enemyDeckCells.AddRange(s.DeckCells);
                    }
                }
            }
            return enemyDeckCells;
        }

        [HttpPost]
        public IActionResult AddShipToField(int id, int playerId)
        {
            var ship = Ship.GetShipByIdFromMapModel(id, Model);
            if (ship != null)
            {
                Model.Players = DbManager.db.Players.ToListAsync<Player>().Result;
                var player = Model.Players.Find(i => i.Id == playerId);
                ship.Player = player;
         
                var shipDeckCells = GetCoordinatesForShip(ship);
                ship.DeckCells = shipDeckCells;

                DbManager.SaveShipToDB(ship);
                DbManager.SavePlayingFieldToDB(PlayingField);
                Model.SelectedShip = ship;
                MapModel.FillMapModelWithCoordinates(shipDeckCells, Model);
            }

            return Json(Model);
        }
       

        [HttpPost]
        public IActionResult SelectShip(int x, int y)
        {
            DeckCell deckCell = new DeckCell();
            foreach(Ship ship in PlayingField.Ships)
            {
                deckCell = ship.DeckCells.Find(s => s.Cell.X == x && s.Cell.Y == y);
            }

            var result = deckCell == null ? Json("Ship not found") : Json(deckCell);
            return result;
        }

        private List<DeckCell> GetCoordinatesForShip(Ship playingShip)
        {
            List<DeckCell> ShipDeckCells = new List<DeckCell>();

            var initalPoint = ShipManager.GetRandomPoint(random);
            var direction = (ShipDirection)shipDirections.GetValue(random.Next(shipDirections.Length));

            foreach (DeckCell deck in playingShip.DeckCells)
            {
                initalPoint  = ShipManager.ShiftPoint(initalPoint, direction);

                var currentDeckCell = DeckCell.Create(initalPoint, deck.Deck);
                ShipDeckCells.Add(currentDeckCell);

                ShipDeckCells = CheckCoordinates(initalPoint, ShipDeckCells, playingShip);
            }
            PlayingField.Ships.Add(playingShip);
            return ShipDeckCells;
        }

        private List<DeckCell> CheckCoordinates(Point initalPoint, List<DeckCell> ShipDeckCells, Ship ship)
        {
            var allShips = Ship.GetAllShips();
            var allPlayerShips = allShips.Where(i => i.Player == ship.Player).ToList();
            var allPlayerDeckCells = new List<DeckCell>();
            foreach(Ship s in allPlayerShips)
            {
                allPlayerDeckCells.AddRange(s.DeckCells);
            }
            var isShip = ShipManager.CheckShipWithOtherShips(allPlayerDeckCells, ship);
            var isShipOutOfAbroad = ShipManager.CheckPointAbroad(initalPoint);

            if (isShip || isShipOutOfAbroad)
            {
                ShipDeckCells.Clear();
                ShipDeckCells = GetCoordinatesForShip(ship);
            }
            return ShipDeckCells;
        }

        [HttpPost]
        public IActionResult Index(int player2Id, int player1Id)
        {
            Model.Players = DbManager.db.Players.ToListAsync<Player>().Result;
            Model.CurrantPlayer = Model.Players.Find(s => s.Id == player1Id);
            if (player2Id != player1Id)
            {
                CurrantGame = Game.Create(Model.Players, player1Id, player2Id, PlayingField);
                Model.CurrentGame = CurrantGame;
                DbManager.SaveGameToDB(CurrantGame);
            }
            return Json(new { redirectToUrl = Url.Action("StartGame", "Game", new { id = CurrantGame.Id }) });
        }

        [HttpGet]
        public IActionResult StartGame()
        {
            return View(Model);
        }

        [HttpPost]
        public IActionResult StartGame(string action)
        {
            if(CurrantGame != null)
            {
                CurrantGame.StartGame();
                DbManager.UpdateGameInDb(CurrantGame);
            }
            
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            Model.Players = Player.GetPlayersNotInGame(Model);
            return View(Model);
        }
       
        [HttpPost]
        public IActionResult EndGame()
        {
            if(CurrantGame != null)
            {
                CurrantGame.EndGame();
                DbManager.DeleteGameFromDb(CurrantGame);
            }
            return View();
        } 
    }
}