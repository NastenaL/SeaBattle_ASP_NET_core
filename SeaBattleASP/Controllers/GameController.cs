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
        private NotificationsMessageHandler NotificationsMessageHandler { get; set; }

        //public GameController(NotificationsMessageHandler notificationsMessageHandler)
        //{
        //    this.NotificationsMessageHandler = notificationsMessageHandler;
        //}

        [HttpPost]
        public async void MakeStep([FromQueryAttribute]string message)
        {
            if (NotificationsMessageHandler != null)
            {
                await this.NotificationsMessageHandler.SendMessageToAllAsync(message);
            }
           
        }

        public GameController(ApplicationContext context)
        {
            DbManager.db = context;
            CurrantGame = new Game();
            Model = new MapModel
            {
                Ships = Rules.CreateShips()
            };

            PlayingField = new PlayingField();
            PlayingField.CreateField();
        }
        PlayingField PlayingField { get; set; }

        readonly Array shipDirections = Enum.GetValues(typeof(ShipDirection));
        Game CurrantGame { get; set; }
        MapModel Model { get; set; }

        [HttpPost]
        public IActionResult AddShipToField(int id)
        {
            var ship = Model.Ships.Find(i => i.Id == id);
            
            List<DeckCell> shipCoordinates = new List<DeckCell>();
            if(ship != null)
            { 
                ship.IsSelectedShip = true;
                shipCoordinates = GetCoordinatesForShip(ship);

                DbManager.SaveShipToDB(ship);
                DbManager.SaveDeckCellAndPlayingFieldToDB(shipCoordinates, PlayingField);
            }
            FillMapModel(shipCoordinates);
            return Json(Model);
        }
        
        private void FillMapModel(List<DeckCell> shipCoordinates)
        {
            foreach (var shipDeckCell in shipCoordinates)
            {
                Model.Coord.Add(shipDeckCell.Cell);
            }
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