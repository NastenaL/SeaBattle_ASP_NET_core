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
        private readonly ApplicationContext db;

        public GameController(ApplicationContext context)
        {
            db = context;
            Games = new List<Game>();
            model = new MapModel
            {
                Ships = Rules.CreateShips()
            };

            playingField = new PlayingField();
        }
        PlayingField playingField { get; set; }

        readonly Array shipDirections = Enum.GetValues(typeof(ShipDirection));
        List<Game> Games { get; set; }

        MapModel model { get; set; }

        [HttpPost]
        public IActionResult AddShipToField(int id)
        {
            var ship = model.Ships.Find(i => i.Id == id);
            Dictionary<Cell, Deck> shipCoordinates = new Dictionary<Cell, Deck>();
            if(ship != null)
            {
                ship.IsSelectedShip = true;
                shipCoordinates = GetCoordinatesForShip(ship);
             //   playingField.Ships = shipCoordinates;
            }   
            
           foreach(var keyValuePair in shipCoordinates)
            {
                model.Coord.Add(keyValuePair.Key);
            }

            return Json(model);
        }
        
        [HttpPost]
        public IActionResult SelectShip(int x, int y)
        {
            foreach(Cell p in playingField.Ships.Keys)
            {
                if(p.Coordinate.X == x && p.Coordinate.Y == y)
                {
                    // select ship
                }
            }
            
            return Json(x);
        }

        private Dictionary<Cell, Deck> GetCoordinatesForShip(Ship ship)
        {
            Dictionary<Cell, Deck> result = new Dictionary<Cell, Deck>();
            Random random = new Random();

            Point point = new Point
            {
                X = random.Next(0, Rules.FieldWidth-1),
                Y = random.Next(0, Rules.FieldHeight-1)
            };

            var direction = (ShipDirection)shipDirections.GetValue(random.Next(shipDirections.Length));

            foreach (Deck deck in ship.Decks)
            {
                point.X = direction == ShipDirection.horizontal ? point.X + 1 : point.X;
                point.Y = direction == ShipDirection.vertical ? point.Y + 1 : point.Y;
                result.Add(new Cell { Color = CellColor.White, Coordinate = point, State = CellState.ShipDeck }, deck);
                Cell cell = new Cell { Color = CellColor.White, Coordinate = point, State = CellState.ShipDeck };

                if (playingField.Ships.Count > 1)
                {
                    foreach (var po in playingField.Ships.ToList())
                    {
                        if (point.X == po.Key.Coordinate.X && point.Y == po.Key.Coordinate.Y) //Check coincidence cells
                           /* || ((point.X == po.Key.Coordinate.X + 1 && point.Y == po.Key.Coordinate.Y) ||//Check adjacent cells
                                (point.X == po.Key.Coordinate.X && point.Y == po.Key.Coordinate.Y + 1))) */
                        {
                            result.Clear();
                            result = GetCoordinatesForShip(ship);
                        }
                        
                    }
                }

                if (point.X > Rules.FieldWidth-1 || point.Y > Rules.FieldHeight-1)//Check abroad
                {
                    result.Clear();
                    result = GetCoordinatesForShip(ship);
                }
                playingField.Ships.Add(cell, deck);
                db.PlayingFields.Add(playingField);
                //db.SaveChanges();
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
                Player1 = model.Players.Find(p => p.Name == ViewBag.Message),
                Player2 = Player2,
                PlayingField = playingField
            };

            db.Games.Add(game);
           // db.SaveChanges();
            return this.RedirectToAction("StartGame", "Game");
        }

        [HttpGet]
        public IActionResult StartGame()
        {
            ViewData["Width"] = Rules.FieldWidth;
            ViewData["Height"] = Rules.FieldHeight;
            
            return View(model);
        }

        [HttpPost]
        public IActionResult StartGame(string action, Ship selected)
        {
            if (action == "addShips")
            { 
            }
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Message = TempData["PlayerName"];
            model.Players = db.Players.ToListAsync<Player>().Result;
            return View(model);
        }
    }
}