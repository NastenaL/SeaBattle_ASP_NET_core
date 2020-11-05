﻿namespace SeaBattleASP.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Helpers.WebSocket;
    using SeaBattleASP.Models;
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Enums;
    using System;
    using System.Collections.Generic;

    public class GameController : Controller
    {
        private readonly Random random;
        readonly Array shipDirections = Enum.GetValues(typeof(ShipDirection));

        public GameController(ApplicationContext context, NotificationsMessageHandler notificationsMessageHandler)
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

            this.NotificationsMessageHandler = notificationsMessageHandler;
        }

        #region Properties 
        PlayingField PlayingField { get; set; }

        private NotificationsMessageHandler NotificationsMessageHandler { get; set; }
       
        private Game CurrantGame { get; set; }

        private MapModel Model { get; set; }
        #endregion

        [HttpPost]
        public async void MakeStep(string message)
        {
            if (NotificationsMessageHandler != null)
            {
                await this.NotificationsMessageHandler.SendMessageToAllAsync(message);
            }

        }

        [HttpPost]
        public IActionResult AddShipToField(int id)
        {
            foreach(KeyValuePair<int, Ship> k in Model.Ships)
            {
                if(k.Key == id)
                {
                    var ship = k.Value;
                    if (ship != null)
                    {
                        ship.Player = Model.CurrantPlayer;

                        var shipType = ship.GetType();
                        var type = Enum.Parse(typeof(ShipType), shipType.Name);
                        PlayingShip playingShip = new PlayingShip
                        {
                            Ship = ship,
                            ShipType = (ShipType)type
                        };

                        var shipCoordinates = GetCoordinatesForShip(playingShip);

                        ship.DeckCells = shipCoordinates;
                        DbManager.SaveShipToDB(ship, playingShip);
                        DbManager.SaveDeckCellAndPlayingFieldToDB(shipCoordinates, PlayingField);

                        Model.SelectedShip = ship;
                        MapModel.FillMapModelWithCoordinates(shipCoordinates, Model);
                    }
                }
            }

            return Json(Model);
        }
       

        [HttpPost]
        public IActionResult SelectShip(int x, int y)
        {
            DeckCell test = new DeckCell();
            foreach(PlayingShip ship in PlayingField.PlayingShips)
            {
                test = ship.Ship.DeckCells.Find(s => s.Cell.X == x && s.Cell.Y == y);
            }
           
            if(test == null)
            {
                return Json("Ship not found");
            }
            return Json(test);
        }

        private List<DeckCell> GetCoordinatesForShip(PlayingShip ship)
        {
            List<DeckCell> ShipDeckCells = new List<DeckCell>();

            var initalPoint = ShipManager.GetRandomPoint(random);
            var direction = (ShipDirection)shipDirections.GetValue(random.Next(shipDirections.Length));

            foreach (DeckCell deck in ship.Ship.DeckCells)
            {
                initalPoint  = ShipManager.ShiftPoint(initalPoint, direction);

                var currentDeckCell = DeckCell.Create(initalPoint, deck.Deck);
                ShipDeckCells.Add(currentDeckCell);

                var isShip = ShipManager.CheckShipWithOtherShips(initalPoint, PlayingField);
                var isShipOutOfAbroad = ShipManager.CheckPointAbroad(initalPoint);

                if(isShip || isShipOutOfAbroad)
                {
                    ShipDeckCells.Clear();
                    ShipDeckCells = GetCoordinatesForShip(ship);
                }
            }
            PlayingField.PlayingShips.Add(ship);
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