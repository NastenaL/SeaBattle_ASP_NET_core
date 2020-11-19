namespace SeaBattleASP.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Models.Constants;
    using SeaBattleASP.Models.Enums;

    public class Game
    {
        #region Properties
        public int Id { get; set; }

        public PlayingField PlayingField { get; set; }

        public GameState State { get; set; }

        public Player Player1 { get; set; }

        public Player Player2 { get; set; }

        public bool IsPl1Turn { get; set; }
        #endregion

        #region Method
        public static List<Game> GetAll()
        {
            return DbManager.GetGames();
        }

        public static MapModel CheckGame(Game game)
        {
            MapModel result = new MapModel
            {
                Ships = Rules.CreateShips()
            };
            var allPlayingF = PlayingField.GetAllPlayingFields();
            var allShipsInCurrentGame = allPlayingF.Find(g => g.Id == game.PlayingField.Id);

            bool isAllPlayersInGame = game.Player1 != null 
                                      && game.Player2 != null;
            bool isEnoughShips = allShipsInCurrentGame.Ships.Count == result.Ships.Count * 2;

            if (!isEnoughShips)
            {
                result.Message = "Not enough ships. You or second player select not all ships";
            }
            else if (!isAllPlayersInGame)
            {
                result.Message = "Wait for second player";
            }
            else
            {
                result.Message = "The game is start. First step is ";
                var pl1Turn = game.StartGame();
                result.Message += pl1Turn ? game.Player1.Name 
                                          : game.Player2.Name;

                DbManager.UpdateGame(game);
                result.CurrentGame = game;
            }

            return result;
        }

        public static MapModel CheckWinner(Game game)
        {
            MapModel mapModel = new MapModel();
            if (game != null)
            {
                var player1Ships = game.PlayingField.Ships.Where(i => i.Player == game.Player1).ToList();
                var player2Ships = game.PlayingField.Ships.Where(i => i.Player == game.Player2).ToList();

                var isPlayer1Lose = CheckAllShipsDrowned(player1Ships);
                var isPlayer2Lose = CheckAllShipsDrowned(player2Ships);

                if (isPlayer1Lose || isPlayer2Lose)
                {
                    mapModel.Message = "Player2 win. Con";
                    mapModel.Message += isPlayer1Lose ? game.Player2.Name 
                                                      : game.Player1.Name;
                }
            }
            return mapModel;
        }

        public void EndGame()
        {
            this.State = GameState.Finished;
        }

        public static Game GetGameById(int gameId)
        {
            LoadRelatedEntities();
            var games = Game.GetAll();
            var game = games.Find(g => g.Id == gameId);
            return game;
        }

        private static void LoadRelatedEntities()
        {
            Player.GetAll();
            PlayingField.GetAllPlayingFields();
            DeckCell.GetAll();
            Cell.GetAll();
            Deck.GetAll();
            Ship.GetAll();
        }

        private static Ship CheckDrownedShip(Ship ship)
        {
            Ship result = null;
            var drownedDeckCells = ship.DeckCells.Where(i => i.Deck.State == DeckState.Drowned).ToList();
            if (drownedDeckCells.Count == ship.DeckCells.Count)
            {
                result = ship;
            }
            return result;
        }

        private static bool CheckAllShipsDrowned(List<Ship> ships)
        {
            List<Ship> drownedShips = new List<Ship>();
            foreach (Ship ship in ships)
            {
                var drownedShip = CheckDrownedShip(ship);
                if (drownedShip != null)
                {
                    drownedShips.Add(drownedShip);
                }
            }

            return drownedShips.Count == ships.Count;
        }

        public bool StartGame()
        {
            this.State = GameState.Started;

            Random gen = new Random();
            this.IsPl1Turn = gen.Next(100) < 50;
            return this.IsPl1Turn;
        }
        #endregion
    }
}
