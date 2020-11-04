namespace SeaBattleASP.Controllers
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
            Player player = new Player();
            if(!string.IsNullOrEmpty(userName))
            {
                player.Name = userName;
              DbManager.SavePlayerToDB(player);
            }

            return this.RedirectToAction("Index", "Game", new {id = player.Id});
        }

        [HttpGet]
        public IActionResult Setting()
        {
            MapModel model = new MapModel
            {
                Ships = Rules.CreateShips(),
                width = Rules.FieldWidth,
                height = Rules.FieldHeight
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
