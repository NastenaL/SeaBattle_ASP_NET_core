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
            CurrantGame = new Game();
            model = new MapModel
            {
                Ships = Rules.CreateShips()
            };

            PlayingField = new PlayingField();
            PlayingField.CreateField();
        }
        PlayingField PlayingField { get; set; }

        readonly Array shipDirections = Enum.GetValues(typeof(ShipDirection));
        Game CurrantGame { get; set; }
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

                SaveShipToDB(ship);
                SaveDeckCellAndPlayingFieldToDB(shipCoordinates);
            }
            FillMapModel(shipCoordinates);
            return Json(model);
        }
        
        private void FillMapModel(List<DeckCell> shipCoordinates)
        {
            foreach (var shipDeckCell in shipCoordinates)
            {
                model.Coord.Add(shipDeckCell.Cell);
            }
        }

        private void SaveShipToDB(Ship ship)
        {
            var shipType = ship.GetType();
            var type = Enum.Parse(typeof(ShipType), shipType.Name);
            switch ((ShipType)type)
            {
                case ShipType.AuxiliaryShip:
                    db.AuxiliaryShips.Add((AuxiliaryShip)ship);
                    break;
                case ShipType.MilitaryShip:
                    db.MilitaryShips.Add((MilitaryShip)ship);
                    break;
                case ShipType.MixShip:
                    db.MixShips.Add((MixShip)ship);
                    break;
            };
        }

        private void SaveDeckCellAndPlayingFieldToDB(List<DeckCell> shipCoordinates)
        {
            foreach (DeckCell deckCell in shipCoordinates)
            {
                db.Cells.Add(deckCell.Cell);
                db.Decks.Add(deckCell.Deck);
                db.DeckCells.Add(deckCell);
                db.SaveChanges();
            }

            db.PlayingField.Add(PlayingField);
            db.SaveChanges();
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

           
            return result;
        }

        [HttpPost]
        public IActionResult Index(Player Player2)
        {
            var name = ViewData["PlayerName"];

            CurrantGame = new Game
            { 
                Player1 = model.Players.Find(p => p.Name == name.ToString()),
                Player2 = Player2,
                PlayingField = PlayingField
            };
           
            db.Games.Add(CurrantGame);
            db.SaveChanges();
            return this.RedirectToAction("StartGame", "Game");
        }

        [HttpGet]
        public IActionResult StartGame()
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult StartGame(string action)
        {
            if(CurrantGame != null)
            {
                CurrantGame.StartGame();
                db.Update(CurrantGame);
                db.SaveChanges();
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

        [HttpPost]
        public IActionResult EndGame()
        {
            if(CurrantGame != null)
            {
                CurrantGame.EndGame();
                db.Games.Remove(CurrantGame);

                var fields = db.PlayingField.ToListAsync<PlayingField>().Result;
                var decks = db.Decks.ToListAsync<Deck>().Result;
                var cells = db.Cells.ToListAsync<Cell>().Result;
                var cellDecks = db.DeckCells.ToListAsync<DeckCell>().Result;

                foreach (var cell in CurrantGame.PlayingField.ShipsDeckCells)
                {
                    db.Decks.Remove(cell.Deck);
                    db.Cells.Remove(cell.Cell);
                    db.DeckCells.Remove(cell);
                }
                
                var currentField = fields.Find(f => f == CurrantGame.PlayingField);
                if(currentField != null)
                {
                    db.PlayingField.Remove(currentField);
                }
       
                db.SaveChanges();
            }
            return View();
        }
    }
}