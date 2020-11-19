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
        public IActionResult MakeFireStep(int shipId, int gameId)
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

                this.context.Clients.All.SendAsync("makeStepFireSignalR", this.Model);
                this.Model = Game.CheckWinner(this.Model.CurrentGame);
            }

            return this.Json(this.Model);
        }

        [HttpPost]
        public IActionResult MakeRepairStep(int shipId)
        {
            var ship = Ship.GetShipByIdFromDB(shipId);

            var allShips = Ship.GetAll();
            var allPlayerShips = allShips.Where(i => i.Player == ship.Player).ToList();

            this.Model.RepairedShips = ship.Repair(allPlayerShips);

            this.Model = Game.CheckWinner(this.Model.CurrentGame);

            this.context.Clients.All.SendAsync("makeStepSignalR");

            return this.Json(this.Model);
        }

        [HttpPost]
        public IActionResult MakeMoveStep(int shipId, 
                                          int gameId)
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

            this.context.Clients.All.SendAsync("makeStepSignalR", ship);
            this.Model = Game.CheckWinner(Game.GetGameById(gameId));
            return this.View();
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
            Player.GetAll();
            Cell.GetAll();
            Deck.GetAll();
            DeckCell.GetAll();
            var ship = Ship.GetShipByIdFromDB(shipId);
            if (ship != null)
            {
                switch (direction)
                {
                    case "left":
                        foreach (DeckCell deckCell in ship.DeckCells)
                        {
                            deckCell.Cell.X -= 1;
                        }

                        break;
                    case "right":
                        foreach (DeckCell deckCell in ship.DeckCells)
                        {
                            deckCell.Cell.X += 1;
                        }

                        break;
                    case "up":
                        foreach (DeckCell deckCell in ship.DeckCells)
                        {
                            deckCell.Cell.Y -= 1;
                        }

                        break;
                    case "down":
                        foreach (DeckCell deckCell in ship.DeckCells)
                        {
                            deckCell.Cell.Y += 1;
                        }

                        break;
                }

                DbManager.UpdateShip(ship);
            }
           
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
                this.Model.CurrentGame = game;
                this.Model = Game.CheckGame(game);
            }

            this.context.Clients.All.SendAsync("startGameSignalR", this.Model);
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
            Game game = new Game();
            var allPLayers = Player.GetAll();
            var firstPlayer = allPLayers.Find(g => g.Id == playerId);
            if (firstPlayer != null)
            {
                var playingField = new PlayingField();
                playingField.CreateField();

                game = new Game
                {
                    Player1 = firstPlayer,
                    State = GameState.Initialized,
                    PlayingField = playingField
                };

                DbManager.AddPlayingField(playingField);
                DbManager.AddGame(game);
            }

            return this.Json(new { redirectToUrl = Url.Action("StartGame", "Game", new { gameId = game.Id, playerId }) });
        }

        [HttpPost]
        public IActionResult JoinToGame(int gameId, 
                                        int playerId)
        {
            var game = Game.GetGameById(gameId);

            if (game != null)
            {
                var allPlayers = Player.GetAll();
                var secondPlayer = allPlayers.Find(p => p.Id == playerId);
                if (secondPlayer != null)
                {
                    game.Player2 = secondPlayer;
                    
                    DbManager.UpdateGame(game);
                    var allGames2 = Game.GetAll();
                }
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
                foreach (Ship ship in game.PlayingField.Ships)
                {
                    DbManager.RemoveDecksAndCells(ship.DeckCells);
                }
                
                DbManager.RemovePlayingField(game.PlayingField);
                DbManager.RemoveGame(game);
            }

            this.context.Clients.All.SendAsync("gameOverSignalR");
            return this.View();
        }
        #endregion
    }
}