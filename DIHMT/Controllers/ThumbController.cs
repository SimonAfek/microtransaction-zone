using System.IO;
using System.Web.Hosting;
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

            var thumb = ThumbHelpers.GetThumbByGameId(id, false);

            if (thumb == null)
            {
                return Redirect("/Images/MTZ_profile_pic.png");
            }

            if (!System.IO.File.Exists($"{HostingEnvironment.ApplicationPhysicalPath}{thumb.Item1.Substring(1).Replace("/", @"\")}"))
            {
                thumb = ThumbHelpers.GetThumbByGameId(id, true);
            }

            return File(thumb.Item1, thumb.Item2);
        }
    }
}
