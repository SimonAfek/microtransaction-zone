using System.Collections.Generic;
using System.Linq;
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
            if (ModelState.Keys.Contains("ReCaptcha") && ModelState["ReCaptcha"].Errors.Any())
            {
                Response.StatusCode = 400;
                return Json("Captcha error - please complete the captcha and try again.");
            }

            if (input.Valid && Request.UserHostAddress != null)
            {
                if (!input.Flags.Contains((int)EnumTag.Spotless) && (string.IsNullOrEmpty(input.Basically) || string.IsNullOrEmpty(input.RatingExplanation)))
                {
                    Response.StatusCode = 400;
                    return Json(System.Text.RegularExpressions.Regex.Unescape("When submitting a game without the 'Spotless'-tag, we require that you fill out the 'Basically' and 'Rating Explanation'-fields explaining the game's purchases in some detail. Please fill out those fields and submit again."));
                }

                input.SubmitterIp = string.Empty;

                if (!Request.IsAuthenticated)
                {
                    input.SubmitterIp = CryptHelper.Hash(Request.UserHostAddress);

                    var blockStatus = BlockHelpers.GetBlockStatus(input.SubmitterIp);

                    if (blockStatus == BlockType.ExplicitBlocked)
                    {
                        Response.StatusCode = 400;
                        return Json("We have temporarily stopped your ability to send in submissions - come back tomorrow and you can submit ratings again.");
                    }

                    if (blockStatus == BlockType.HiddenBlocked)
                    {
                        return Json("Your submission has been accepted. It will go live as soon as it's been verified. Thank you for helping out!");
                    }
                }

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
