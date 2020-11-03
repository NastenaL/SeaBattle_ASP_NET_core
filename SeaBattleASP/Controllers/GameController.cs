namespace SeaBattleASP.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Helpers.WebSocket;
    using SeaBattleASP.Models;
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Enums;
    using System;
    using System.Collections.Generic;

    public class GameController : Controller
    {
        private readonly Random random;
        readonly Array shipDirections = Enum.GetValues(typeof(ShipDirection));

        public GameController(ApplicationContext context, NotificationsMessageHandler notificationsMessageHandler)
        {
            random = new Random();
            DbManager.db = context;
            CurrantGame = new Game();
            Model = new MapModel
            {
                Ships = Rules.CreateShips()
            };

            PlayingField = new PlayingField();
            PlayingField.CreateField();

            this.NotificationsMessageHandler = notificationsMessageHandler;
        }

        #region Properties 
        PlayingField PlayingField { get; set; }

        private NotificationsMessageHandler NotificationsMessageHandler { get; set; }
       
        private Game CurrantGame { get; set; }

        private MapModel Model { get; set; }
        #endregion

        [HttpPost]
        public async void MakeStep(string message)
        {
            if (NotificationsMessageHandler != null)
            {
                await this.NotificationsMessageHandler.SendMessageToAllAsync(message);
            }

        }

        [HttpPost]
        public IActionResult AddShipToField(int id)
        {
            var ship = Model.Ships.Find(i => i.Id == id);
          
            if(ship != null)
            { 
                ship.IsSelectedShip = true;
                var shipCoordinates = GetCoordinatesForShip(ship);

                DbManager.SaveShipToDB(ship);
                DbManager.SaveDeckCellAndPlayingFieldToDB(shipCoordinates, PlayingField);

                ShipManager.FillMapModel(shipCoordinates, Model);
            }
            
            return Json(Model);
        }
       

        [HttpPost]
        public IActionResult SelectShip(int x, int y)
        {
            var shipDeckCell = PlayingField.ShipsDeckCells.Find(s => s.Cell.X == x && s.Cell.Y == y);
            if(shipDeckCell == null)
            {
                return Json("Ship not found");
            }
            return Json(shipDeckCell);
        }

        private List<DeckCell> GetCoordinatesForShip(Ship ship)
        {
            List<DeckCell> ShipDeckCells = new List<DeckCell>();

            var initalPoint = ShipManager.GetRandomPoint(random);
            var direction = (ShipDirection)shipDirections.GetValue(random.Next(shipDirections.Length));

            foreach (Deck deck in ship.Decks)
            {
                initalPoint  = ShipManager.ShiftPoint(initalPoint, direction);

                var currentDeckCell = ShipManager.CreateDeckCell(initalPoint, deck);
                ShipDeckCells.Add(currentDeckCell);

                var isShip = ShipManager.CheckShipWithOtherShips(initalPoint, PlayingField);
                var isShipOutOfAbroad = ShipManager.CheckPointAbroad(initalPoint);

                if(isShip || isShipOutOfAbroad)
                {
                    ShipDeckCells.Clear();
                    ShipDeckCells = GetCoordinatesForShip(ship);
                }
            }
            PlayingField.ShipsDeckCells.AddRange(ShipDeckCells);
            return ShipDeckCells;
        }

        [HttpPost]
        public IActionResult Index(Player Player2)
        {
            var name = ViewData["PlayerName"];

            CurrantGame = new Game
            { 
                Player1 = Model.Players.Find(p => p.Name == name.ToString()),
                Player2 = Player2,
                PlayingField = PlayingField
            };

            DbManager.SaveGameToDB(CurrantGame);
            return this.RedirectToAction("StartGame", "Game");
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
            ViewBag.Message = TempData["PlayerName"];
            Model.Players = DbManager.db.Players.ToListAsync<Player>().Result;
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