using Microsoft.AspNetCore.Mvc;

namespace SeaBattleASP.Controllers
{
    public class GameController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}