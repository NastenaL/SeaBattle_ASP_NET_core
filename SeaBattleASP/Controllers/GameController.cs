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
           
        }

        List<Player> Players { get; set; }


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