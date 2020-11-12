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
        private readonly Random random;
        readonly Array shipDirections = Enum.GetValues(typeof(ShipDirection));

        public GameController(ApplicationContext context)
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
        }

        #region Properties 
        PlayingField PlayingField { get; set; }
       
        private Game CurrantGame { get; set; }

        private MapModel Model { get; set; }
        #endregion

        private List<Ship> GetAllShips()
        {
            DbManager.db.Cells.ToListAsync<Cell>();
            DbManager.db.Decks.ToListAsync<Deck>();
            DbManager.db.DeckCells.ToListAsync<DeckCell>();
         
            var auxiliaryShips = DbManager.db.AuxiliaryShips.ToListAsync<AuxiliaryShip>().Result;
            var militaryShip = DbManager.db.MilitaryShips.ToListAsync<MilitaryShip>().Result;
            var mixShip = DbManager.db.MixShips.ToListAsync<MixShip>().Result;
            List<Ship> allShips = new List<Ship>();
            allShips.AddRange(auxiliaryShips);
            allShips.AddRange(militaryShip);
            allShips.AddRange(mixShip);
            return allShips;
        }

        [HttpPost]
        public Ship MakeStep(int shipId, int Type)
        {
            MovementType type = (MovementType)Type;
            List<Ship> allShips = GetAllShips();
            var ship = allShips.Find(i => i.Id == shipId);
            if (ship != null)
            {
                switch (type)
                {
                    case MovementType.Fire:
                        //ship.Fire(ship.DeckCells);
                        break;
                    case MovementType.Move:
                        var shipDeckCells = ship.Move(ship);
                        if(shipDeckCells.Count > 0)
                        {
                            ship.DeckCells = shipDeckCells;
                            DbManager.UpdateShip(ship);
                        }
                       
                        break;
                    case MovementType.Repair:
                        ship.Repair(ship, allShips);
                        break;
                }
            }
            return ship;
        }

        [HttpPost]
        public IActionResult AddShipToField(int id)
        {
            var ship = Ship.GetShipById(id, Model);
            if (ship != null)
            {
                ship.Player = Model.CurrantPlayer;
         
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
            var isShip = ShipManager.CheckShipWithOtherShips(initalPoint, PlayingField);
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
            if(player2Id != player1Id)
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