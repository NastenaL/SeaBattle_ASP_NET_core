namespace SeaBattleASP.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Models;
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Enums;

    public class GameController : Controller
    {
        private readonly Random random;
        private readonly Array shipDirections = Enum.GetValues(typeof(ShipDirection));

        public GameController(ApplicationContext context)
        {
            DbManager.Db = context;
            this.random = new Random();

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
        public IActionResult MakeFireStep(int shipId)
        {
            if (this.Model.CurrentGame != null)
            {
                var ship = Ship.GetShipByIdFromDB(shipId);

                List<DeckCell> enemyDeckCells = ShipManager.GetEnemyShipsDeckCells(this.Model.CurrentGame);

                if (enemyDeckCells.Count > 0)
                {
                    var hurtedShipDecks = ship.Fire(enemyDeckCells);

                    if (hurtedShipDecks.Count > 0)
                    {
                        this.Model.HurtedShips = hurtedShipDecks;
                        DbManager.UpdateShip(hurtedShipDecks);
                    }
                }
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

            return this.Json(this.Model);
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
        public IActionResult AddShipToField(int shipId, int playerId, int gameId)
        {
            var ship = Ship.GetShipByIdFromMapModel(shipId, this.Model);
            if (ship != null)
            {
                this.Model.Players = Player.GetPlayers();
                var player = this.Model.Players.Find(i => i.Id == playerId);
                ship.Player = player;
                var shipDeckCells = this.GetCoordinatesForShip(ship);
                ship.DeckCells = shipDeckCells;

                var playerFields = PlayingField.GetAllPlayingFields();
                var games = Game.GetAll();
                var game = games.Find(g => g.Id == gameId);
                game.PlayingField.Ships.Add(ship);

               // DbManager.db.PlayingFields.Update(game.PlayingField);
                DbManager.UpdateGame(game);
                this.Model.SelectedShip = ship;
                this.Model.CurrentGame = game;
                MapModel.FillMapModelWithCoordinates(shipDeckCells, this.Model);
            }

            return this.Json(this.Model);
        }
       
        [HttpPost]
        public IActionResult ShiftShip(int shipId, string direction)
        {
            Player.GetPlayers();
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
                            deckCell.Cell.Y -= 1;
                        }

                        break;
                    case "right":
                        foreach (DeckCell deckCell in ship.DeckCells)
                        {
                            deckCell.Cell.Y += 1;
                        }

                        break;
                    case "up":
                        foreach (DeckCell deckCell in ship.DeckCells)
                        {
                            deckCell.Cell.X -= 1;
                        }

                        break;
                    case "down":
                        foreach (DeckCell deckCell in ship.DeckCells)
                        {
                            deckCell.Cell.X += 1;
                        }

                        break;
                }

                DbManager.UpdateShip(ship);
            }
           
            return this.Json(ship);
        }

        [HttpPost]
        public IActionResult SelectShip(int x, int y)
        {
            DeckCell deckCell = new DeckCell();
            foreach (Ship ship in this.Model.CurrentGame.PlayingField.Ships)
            {
                deckCell = ship.DeckCells.Find(s => s.Cell.X == x && s.Cell.Y == y);
            }

            var result = deckCell == null ? Json("Ship not found") : Json(deckCell);
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
            this.LoadRelatedEntities();
            var allPlayingF = PlayingField.GetAllPlayingFields();
            var games = Game.GetAll();

            var game = games.Find(g => g.Id == gameId);
            var allShipsInCurrentGame = allPlayingF.Find(g => g.Id == game.PlayingField.Id);
            if (game != null)
            {
                bool isAllPlayersInGame = game.Player1 != null && game.Player2 != null;
                bool isEnoughShips = allShipsInCurrentGame.Ships.Count == this.Model.Ships.Count * 2;

                if (!isEnoughShips)
                {
                    this.Model.Message = "Not enough ships. You or second player select not all ships";
                }
                else if (!isAllPlayersInGame)
                {
                    this.Model.Message = "Wait for second player";
                }
                else
                {
                    this.Model.Message = "The game is start. First step is ";
                    var pl1Turn = game.StartGame();

                    if (pl1Turn)
                    {
                        this.Model.Message += game.Player1.Name;
                    }
                    else
                    {
                        this.Model.Message += game.Player2.Name;
                    }

                    DbManager.UpdateGame(game);
                    this.Model.CurrentGame = game;
                }
            }

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
            var allPLayers = Player.GetPlayers();
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
        public IActionResult JoinToGame(int gameId, int playerId)
        {
            var allGames = Game.GetAll();
            
            var game = allGames.Find(g => g.Id == gameId);
            if (game != null)
            {
                var allPlayers = Player.GetPlayers();
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
        public IActionResult GameOver(int gameId, int playerId)
        {
            this.LoadRelatedEntities();
            var games = Game.GetAll();
            var game = games.Find(g => g.Id == gameId);
            if (game != null)
            {
                var ii = game.PlayingField.Ships.First().DeckCells.First().Cell.Id;
                foreach (Ship ship in game.PlayingField.Ships)
                {
                    DbManager.RemoveDecksAndCells(ship.DeckCells);
                }
                
                DbManager.RemovePlayingField(game.PlayingField);
                DbManager.RemoveGame(game);
            }

            return this.Json(new { redirectToUrl = Url.Action("Index", "Game", new {playerId }) });
        }

        private List<DeckCell> CheckCoordinates(Point initalPoint, List<DeckCell> shipDeckCells, Ship ship)
        {
            var decks = Deck.GetAll();
            var cells = Cell.GetAll();
            var deckCells = DeckCell.GetAll();
            var allShips = Ship.GetAll();
            var allPlayerShips = allShips.Where(i => i.Player == ship.Player).ToList();
            if (allPlayerShips.Count > 0)
            {
                var allPlayerDeckCells = new List<DeckCell>();
                foreach (Ship s in allPlayerShips)
                {
                    allPlayerDeckCells.AddRange(s.DeckCells);
                }

                var isShip = ShipManager.CheckShipWithOtherShips(allPlayerDeckCells, shipDeckCells);
                var isShipOutOfAbroad = ShipManager.CheckPointAbroad(initalPoint);

                if (isShip || isShipOutOfAbroad)
                {
                    shipDeckCells.Clear();
                    shipDeckCells = this.GetCoordinatesForShip(ship);
                }
            }

            return shipDeckCells;
        }

        private List<DeckCell> GetCoordinatesForShip(Ship ship)
        {
            List<DeckCell> shipDeckCells = new List<DeckCell>();

            var initalPoint = ShipManager.GetRandomPoint(this.random);
            var direction = (ShipDirection)this.shipDirections.GetValue(this.random.Next(this.shipDirections.Length));

            foreach (DeckCell deck in ship.DeckCells)
            {
                initalPoint = ShipManager.ShiftPoint(initalPoint, direction);

                var currentDeckCell = DeckCell.Create(initalPoint, deck.Deck);
                shipDeckCells.Add(currentDeckCell);
                shipDeckCells = this.CheckCoordinates(initalPoint, shipDeckCells, ship);
            }

            return shipDeckCells;
        }

        private void LoadRelatedEntities()
        {
            Player.GetPlayers();
            PlayingField.GetAllPlayingFields();
            DeckCell.GetAll();
            Cell.GetAll();
            Deck.GetAll();
            Ship.GetAll();
        }
        #endregion
    }
}