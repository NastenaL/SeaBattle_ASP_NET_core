﻿namespace SeaBattleASP.Controllers
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using SeaBattleASP.Helpers;
    using SeaBattleASP.Models;
    using SeaBattleASP.Models.Constants;

    public class HomeController : Controller
    {

        public HomeController(ApplicationContext context)
        {
            DbManager.db = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string userName)
        {
            ViewData["PlayerName"] = userName;
            if(!string.IsNullOrEmpty(userName))
            {
                Player player = new Player
                {
                    Name = userName
                };
              DbManager.SavePlayerToDB(player);
            }

            return this.RedirectToAction("Index", "Game");
        }

        [HttpGet]
        public IActionResult Setting()
        {
            ViewData["Width"] = Rules.FieldWidth;
            ViewData["Height"] = Rules.FieldHeight;

           
            var defaultShips = Rules.CreateShips();
            return View(defaultShips);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
