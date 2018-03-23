using System.Web.Mvc;
using DIHMT.Models;
using DIHMT.Static;

namespace DIHMT.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        [HttpGet]
        public ActionResult Index(int id)
        {
            var displayGame = GameHelpers.RefreshDisplayGame(id, true);

            return View(displayGame);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitRating(RatingInputModel input)
        {
            if (input.Valid && Request.UserHostAddress != null)
            {
                input.SubmitterIp = Request.IsAuthenticated ? string.Empty : CryptHelper.Hash(Request.UserHostAddress);

                GameHelpers.SubmitRating(input, Request.IsAuthenticated);
            }

            return RedirectToAction("Index", new { id = input.Id });
        }
    }
}
