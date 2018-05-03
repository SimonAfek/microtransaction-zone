using System.Linq;
using System.Web.Mvc;
using DIHMT.Static;

namespace DIHMT.Controllers
{
    public class ThumbController : Controller
    {
        [HttpGet]
        public ActionResult Index(int id)
        {
            if (id <= 0)
            {
                return Redirect("/Images/MTZ_profile_pic.png");
            }

            var thumb = ThumbHelpers.GetThumbByGameId(id);

            if (thumb == null || !new[] { "image/png", "image/jpeg" }.Contains(thumb.Item2))
            {
                return Redirect("/Images/MTZ_profile_pic.png");
            }

            return File(thumb.Item1, thumb.Item2);
        }
    }
}
