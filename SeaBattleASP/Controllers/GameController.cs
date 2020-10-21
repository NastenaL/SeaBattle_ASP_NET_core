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

    public class GameController : Controller
    {
        private readonly ApplicationContext db;

        public GameController(ApplicationContext context)
        {
            db = context;
            Players = new List<Player>();
            Games = new List<Game>();
            DefaultShips = new List<Ship>();
            DefaultShips = Rules.CreateShips();
        }

        List<Player> Players { get; set; }
        List<Game> Games { get; set; }
        List<Ship> DefaultShips { get; set; }

      

        [HttpPost]
        public IActionResult AddShipToField(int id)
        {
            var ship = DefaultShips.Find(i => i.Id == id);
            if(ship != null)
            {
                PlayingField playingField = new PlayingField();
                var dd = GetCoordinatesForShip(ship);

            }
            return RedirectToAction("StartGame");
        }

        private Dictionary<Cell, Deck> GetCoordinatesForShip(Ship ship)
        {
            Dictionary<Cell, Deck> result = new Dictionary<Cell, Deck>();
            Random random = new Random();

            Point point = new Point
            {
                X = random.Next(1, Rules.FieldWidth),
                Y = random.Next(1, Rules.FieldHeight)
            };

            Array shipDirections = Enum.GetValues(typeof(ShipDirection));
            var direction = (ShipDirection)shipDirections.GetValue(random.Next(shipDirections.Length));

            foreach (Deck deck in ship.Decks)
            {
                point.X = direction == ShipDirection.horizontal ? point.X + 1 : point.X;
                point.Y = direction == ShipDirection.vertical ? point.Y + 1 : point.Y;
                result.Add(new Cell { Color = CellColor.White, Coordinate = point, State = CellState.ShipDeck }, deck);
            }

            if (point.X > Rules.FieldWidth || point.Y > Rules.FieldHeight)
            {
                result.Clear();
                result = GetCoordinatesForShip(ship);
            }
            return result;
        }

        [HttpPost]
        public IActionResult Index(Player Player2)
        {
            ViewBag.Message = TempData["PlayerName"];

            PlayingField playingField = new PlayingField();
            playingField.CreateField();
            Game game = new Game
            {
                Id = Games.Count + 1,
                Player1 = Players.Find(p => p.Name == ViewBag.Message),
                Player2 = Player2,
                PlayingField = playingField
            };

            db.Games.Add(game);
           // db.SaveChanges();
            return this.RedirectToAction("StartGame", "Game", game.Id);
        }

        [HttpGet]
        public IActionResult StartGame()
        {
            ViewData["Width"] = Rules.FieldWidth;
            ViewData["Height"] = Rules.FieldHeight;
            
            return View(DefaultShips);
        }

        [HttpPost]
        public IActionResult StartGame(string action, Ship selected)
        {
            if (action == "addShips")
            { 
            }
            return View(DefaultShips);
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Message = TempData["PlayerName"];
            Players = db.Players.ToListAsync<Player>().Result;
            return View(Players);
        }
    }
}