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
        public IActionResult MakeFireStep(int shipId)
        {
            if(CurrantGame != null)
            {
                var ship = Ship.GetShipByIdFromDB(shipId);

                List<DeckCell> enemyDeckCells = ShipManager.GetEnemyShipsDeckCells(CurrantGame);

                if (enemyDeckCells.Count > 0)
                {
                    var hurtedShipDecks = ship.Fire(enemyDeckCells);

                    if(hurtedShipDecks.Count > 0)
                    {
                        Model.HurtedShips = hurtedShipDecks;
                        DbManager.UpdateShip(hurtedShipDecks);
                    }
                }
            }
            return Json(Model);
        }

        [HttpPost]
        public IActionResult MakeRepairStep(int shipId)
        {
            var ship = Ship.GetShipByIdFromDB(shipId);

            var allShips = Ship.GetAllShips();
            var allPlayerShips = allShips.Where(i => i.Player == ship.Player).ToList();

            Model.RepairedShips = ship.Repair(allPlayerShips);

            return Json(Model);
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
                initalPoint = ShipManager.ShiftPoint(initalPoint, direction);

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
            foreach (Ship s in allPlayerShips)
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

        [HttpGet]
        public IActionResult StartGame()
        {
            return View(Model);
        }

        [HttpPost]
        public IActionResult StartGame(string action)
        {
            if (CurrantGame != null)
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
            var games =DbManager.db.Games.ToListAsync<Game>().Result;
            Model.Games = games.Where(g => g.Player2 == null).ToList();
            return View(Model);
        }
       
        [HttpPost]
        public IActionResult CreateGame(int playerId)
        {
            Game game = new Game();
            var allPLayers = DbManager.db.Players.ToListAsync<Player>().Result;
            var firstPlayer = allPLayers.Find(g => g.Id == playerId);
            if(firstPlayer != null)
            {
                game = new Game
                {
                    Player1 = firstPlayer,
                    State = GameState.Initialized
                };

                DbManager.SaveGameToDB(game);
            }

            return Json(new { redirectToUrl = Url.Action("StartGame", "Game", new { gameId = game.Id, playerId }) });
        }

        [HttpPost]
        public IActionResult JoinToGame(int gameId, int playerId)
        {
            var allGames = DbManager.db.Games.ToListAsync<Game>().Result;
            
            var game = allGames.Find(g => g.Id == gameId);
            if(game != null)
            {
                var allPlayers = DbManager.db.Players.ToListAsync<Player>().Result;
                var secondPlayer = allPlayers.Find(p => p.Id == playerId);
                if(secondPlayer != null)
                {
                    var playingField = new PlayingField();
                    playingField.CreateField();

                    DbManager.SavePlayingFieldToDB(playingField);

                    game.PlayingField = playingField;
                    game.Player2 = secondPlayer;
                    DbManager.UpdateGameInDb(game);
                }
       
            }

            return Json(new { redirectToUrl = Url.Action("StartGame", "Game", new { gameId = game.Id, playerId }) });
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