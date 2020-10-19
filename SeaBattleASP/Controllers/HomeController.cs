namespace SeaBattleASP.Controllers
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using SeaBattleASP.Models;
    using SeaBattleASP.Models.Constants;

    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string userName)
        {
            TempData["PlayerName"] = userName;

            return this.RedirectToAction("Index", "Game");
        }

        [HttpGet]
        public IActionResult Setting()
        {
           
            ViewData["Width"] = Rules.FieldWidth;
            ViewData["Height"] = Rules.FieldHeight;
            var ships = Rules.CreateShips();
            return View(ships);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
