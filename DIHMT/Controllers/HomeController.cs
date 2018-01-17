using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using DIHMT.Static;

namespace DIHMT.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Game", new { id = 47551 });

            // return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public async Task<ActionResult> Game(int id)
        {
            var displayGame = await GameHelpers.RefreshDisplayGame(id);

            return View(displayGame);
        }
    }
}