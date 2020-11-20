namespace SeaBattleASP.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Hubs;
    using SeaBattleASP.Models;
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Enums;

    public class GameController : Controller
    {
        private readonly IHubContext<StateGameHub> context;

        public GameController(ApplicationContext context, IHubContext<StateGameHub> contextHub)
        {
            DbManager.Db = context;
            this.context = contextHub;
            this.Model = new MapModel
            {
                Ships = Rules.CreateShips()
            };
        }

        #region Properties 

        private MapModel Model { get; set; }
        #endregion

        #region Methods

        #region Step methods
        [HttpPost]
        public IActionResult MakeFireStep(int shipId, 
                                          int gameId)
        {
             var ship = Ship.GetShipByIdFromDB(shipId);
            var game = Game.GetGameById(gameId);

            if (ship != null)
            {
                List<DeckCell> enemyDeckCells = ShipManager.GetEnemyShipsDeckCells(game, ship.Player);

                if (enemyDeckCells.Count > 0)
                {
                    var hurtedShipDecks = ship.Fire(enemyDeckCells);

                    if (hurtedShipDecks.Count > 0)
                    {
                        this.Model.HurtedShips = hurtedShipDecks;
                        DbManager.UpdateShip(hurtedShipDecks);
                    }
                }

                this.context.Clients.All.SendAsync("makeStepFireSignalR", game);
                this.Model.Message = Game.CheckWinner(this.Model.CurrentGame);
            }

            return this.Json(this.Model);
        }

        [HttpPost]
        public IActionResult MakeRepairStep(int shipId, int gameId, int playerId)
        {
            var game = Game.GetGameById(gameId);
            var ship = Ship.GetShipByIdFromDB(shipId);

            var playerShips = game.PlayingField.Ships.Where(s => s.Player.Id == playerId).ToList();
            if(playerShips.Count > 0)
            {
                var repairedShipDeckCells = ship.Repair(playerShips);
            }

            var updatedGame = Game.GetGameById(gameId);


            this.Model.Message = Game.CheckWinner(updatedGame);

            this.context.Clients.All.SendAsync("makeRepairStepSignalR", updatedGame);

            return this.Json(this.Model);
        }

        [HttpPost]
        public IActionResult MakeMoveStep(int shipId, 
                                          int gameId)
        {
            var ship = Ship.GetShipByIdFromDB(shipId);
            ship = ship.Move();

            var game = Game.GetGameById(gameId);
            this.Model.Message = Game.CheckWinner(Game.GetGameById(gameId));
            this.context.Clients.All.SendAsync("makeStepSignalR", game);
            
            return this.Json(this.Model);
        }
        #endregion

        [HttpPost]
        public IActionResult AddShipToField(int shipId, 
                                            int playerId, 
                                            int gameId)
        {
            var game = Game.GetGameById(gameId);
            var ship = Ship.GetShipByIdFromMapModel(shipId, this.Model);
            if (ship != null)
            {
                ship = Ship.SetShipProperties(gameId, 
                                              playerId, 
                                              ship);

                DbManager.UpdateGame(game);
                this.Model.SelectedShip = ship;
                this.Model.CurrentGame = game;
                MapModel.FillMapModelWithCoordinates(ship.DeckCells, this.Model);
            }

            return this.Json(this.Model);
        }

        [HttpPost]
        public IActionResult ShiftShip(int shipId, 
                             string direction)
        {
            var ship = Ship.GetShipByIdFromDB(shipId);
            ship = Ship.ShiftShip(direction, ship);

            return this.Json(ship);
        }

        [HttpPost]
        public IActionResult SelectShip(int x, 
                                        int y)
        {
            DeckCell deckCell = new DeckCell();
            foreach (Ship ship in this.Model.CurrentGame.PlayingField.Ships)
            {
                deckCell = ship.DeckCells.Find(s => s.Cell.X == x && s.Cell.Y == y);
            }

            var result = deckCell == null ? Json("Ship not found") 
                                          : Json(deckCell);
            return result;
        }

        [HttpGet]
        public IActionResult StartGame()
        {
            return this.View(this.Model);
        }

        [HttpPost]
        public IActionResult StartGame(int gameId)
        {
            var game = Game.GetGameById(gameId);

            if (game != null)
            {
                this.Model = Game.CheckGame(game);
            }

            this.context.Clients.All.SendAsync("startGameSignalR", game);
            return this.Json(this.Model);
        }

        [HttpGet]
        public IActionResult Index()
        {
            this.Model.Players = Player.GetPlayersNotInGame(this.Model);
            var games = Game.GetAll();
            this.Model.Games = games.Where(g => g.Player2 == null).ToList();
            return this.View(this.Model);
        }
       
        [HttpPost]
        public IActionResult CreateGame(int playerId)
        {
            Game game = Game.CreateGame(playerId);
            return this.Json(new { redirectToUrl = Url.Action("StartGame", "Game", new { gameId = game.Id, playerId }) });
        }

        [HttpPost]
        public IActionResult JoinToGame(int gameId, 
                                        int playerId)
        {
            var game = Game.GetGameById(gameId);

            if (game != null)
            {
                Game.AddPlayerToGame(game, playerId);
            }

            return this.Json(new { redirectToUrl = Url.Action("StartGame", "Game", new { gameId = game.Id, playerId }) });
        }

        [HttpPost]
        public IActionResult GameOver(int gameId, 
                                      int playerId)
        {
            var game = Game.GetGameById(gameId);
            if (game != null)
            {
                DbManager.RemoveGameWithRelatedObjects(game);
            }

            this.context.Clients.All.SendAsync("gameOverSignalR");
            return this.View();
        }
        #endregion
    }
}