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
          //  var allShips = db.Ships.ToListAsync<Ship>().Result;
            model = new MapModel
            {
                Ships = Rules.CreateShips()//allShips)
            };

            PlayingField = new PlayingField();
            PlayingField.CreateField();
        }
        PlayingField PlayingField { get; set; }

        readonly Array shipDirections = Enum.GetValues(typeof(ShipDirection));
        List<Game> Games { get; set; }

        MapModel model { get; set; }

        [HttpPost]
        public IActionResult AddShipToField(int id)
        {
            var ship = model.Ships.Find(i => i.Id == id);
            List<DeckCell> shipCoordinates = new List<DeckCell>();
            if(ship != null)
            { 
                ship.IsSelectedShip = true;
                shipCoordinates = GetCoordinatesForShip(ship);

                foreach(DeckCell deckCell in shipCoordinates)
                {
                    db.Cells.Add(deckCell.Cell);
                    db.Decks.Add(deckCell.Deck);
                    db.DeckCells.Add(deckCell);
                    db.SaveChanges();
                }
            }   
            
           foreach(var keyValuePair in shipCoordinates)
            {
                model.Coord.Add(keyValuePair.Cell);
            }

            return Json(model);
        }
        
        [HttpPost]
        public IActionResult SelectShip(int x, int y)
        {
            var ship = PlayingField.ShipsDeckCells.Find(s => s.Cell.X == x && s.Cell.Y == y);
            if(ship == null)
            {
                return Json("Ship not found");
            }
            return Json(ship);
        }

        private List<DeckCell> GetCoordinatesForShip(Ship ship)
        {
            List<DeckCell> result = new List<DeckCell>();
            Random random = new Random();

            Point point = new Point
            {
                X = random.Next(0, Rules.FieldWidth-1),
                Y = random.Next(0, Rules.FieldHeight-1)
            };

            var direction = (ShipDirection)shipDirections.GetValue(random.Next(shipDirections.Length));

            List<DeckCell> deckCell = new List<DeckCell>(); 

            foreach (Deck deck in ship.Decks)
            {
                point.X = direction == ShipDirection.horizontal ? point.X + 1 : point.X;
                point.Y = direction == ShipDirection.vertical ? point.Y + 1 : point.Y;
                DeckCell res = new DeckCell
                {
                    Cell = new Cell { Color = CellColor.White, X = point.X, Y = point.Y, State = CellState.ShipDeck },
                    Deck = deck
                };
                result.Add(res);
                Cell cell = new Cell { Color = CellColor.White, X = point.X, Y = point.Y, State = CellState.ShipDeck };

                if (PlayingField.ShipsDeckCells.Count > 1)
                {
                    foreach (var po in PlayingField.ShipsDeckCells.ToList())
                    {
                        if (point.X == po.Cell.X && point.Y == po.Cell.Y) //Check coincidence cells
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
                deckCell.Add(new DeckCell() { Deck = deck, Cell = cell});
            }
            PlayingField.ShipsDeckCells.AddRange(deckCell);

            //db.PlayingField.Add(PlayingField);
           // db.SaveChanges();
            return result;
        }

        [HttpPost]
        public IActionResult Index(Player Player2)
        {
            ViewBag.Message = TempData["PlayerName"];

            Game game = new Game
            {
                
                Player1 = model.Players.Find(p => p.Name == ViewBag.Message),
                Player2 = Player2,
                PlayingField = PlayingField
            };

            db.Games.Add(game);
            db.SaveChanges();
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