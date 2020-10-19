namespace SeaBattleASP.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using SeaBattleASP.Models;
    using System.Collections.Generic;

    public class GameController : Controller
    {
        public GameController()
        {
            Players = new List<Player>();
            Games = new List<Game>();
        }

        List<Player> Players { get; set; }
        List<Game> Games { get; set; }

       
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
            return View();
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