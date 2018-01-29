using System.Threading.Tasks;
using System.Web.Mvc;
using DIHMT.Static;

namespace DIHMT.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        public async Task<ActionResult> Index(int id)
        {
            var displayGame = await GameHelpers.RefreshDisplayGame(id, true);

            return View(displayGame);
        }
    }
}