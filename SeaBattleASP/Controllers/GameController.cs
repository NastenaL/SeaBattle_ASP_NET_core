namespace SeaBattleASP.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using SeaBattleASP.Models;
    using SeaBattleASP.Models.Constants;
    using System.Collections.Generic;

    public class GameController : Controller
    {
        public GameController()
        {
            Players = new List<Player>();
            Games = new List<Game>();
            Ships = new List<Ship>();
        }

        List<Player> Players { get; set; }
        List<Game> Games { get; set; }
        List<Ship> Ships { get; set; }
       
        [HttpPost]
        public IActionResult Index(Player Player2)
        {
            ViewBag.Message = TempData["PlayerName"];

            PlayingField playingField = new PlayingField();
            playingField.CreateField();
            Game game = new Game
            {
                Id = Games.Count + 1,
                Player1 = Players.Find(p => p.Name == ViewBag.Message),
                Player2 = Player2,
                PlayingField = playingField
            };
           
            return this.RedirectToAction("StartGame", "Game", game.Id);
        }

        [HttpGet]
        public IActionResult StartGame()
        {
            ViewData["Width"] = Rules.FieldWidth;
            ViewData["Height"] = Rules.FieldHeight;
            
            return View();
        }

        [HttpPost]
        public IActionResult StartGame(string action)
        {
            if (action == "addShips")
            {
                Ships = Rules.CreateShips();
            }
            return View(Ships);
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Message = TempData["PlayerName"];
            if(!string.IsNullOrEmpty(ViewBag.Message))
            {
                Player player = new Player
                {
                    Id = Players.Count + 1,
                    Name = ViewBag.Message
                };
                Players.Add(player);
            }    
            return View(Players);
        }
    }
}