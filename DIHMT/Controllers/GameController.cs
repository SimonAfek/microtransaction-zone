using System.Threading.Tasks;
using System.Web.Mvc;
using DIHMT.Models;
using DIHMT.Static;

namespace DIHMT.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        [HttpGet]
        public async Task<ActionResult> Index(int id)
        {
            var displayGame = await GameHelpers.RefreshDisplayGame(id, true);

            return View(displayGame);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitRating(RatingInputModel input)
        {
            if (ModelState.IsValid)
            {
                GameHelpers.SubmitRating(input);
            }

            return RedirectToAction("Index", new { id = input.Id });
        }
    }
}