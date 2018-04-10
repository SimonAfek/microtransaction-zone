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
        [ReCaptcha]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitRating(RatingInputModel input)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return Json("Captcha error - please complete the captcha and try again.");
            }

            if (input.Valid && Request.UserHostAddress != null)
            {
                input.SubmitterIp = Request.IsAuthenticated ? string.Empty : CryptHelper.Hash(Request.UserHostAddress);

                GameHelpers.SubmitRating(input, Request.IsAuthenticated);

                if (Request.IsAuthenticated)
                {
                    return Json("All clear.");
                }

                return Json("Your submission has been accepted. It will go live as soon as it's been verified. Thank you for helping out!");
            }

            Response.StatusCode = 400; // Bad Request
            return Json("Something went wrong with your submission - most likely, you have a malformed URL in the \"Links\"-section. Please try again or contact one of the site administrators.");
        }
    }
}
