using System.Web.Mvc;
using System.Web.Routing;

namespace DIHMT
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Search",
                url: "Search",
                defaults: new { controller = "Search", action = "Search" });

            routes.MapRoute(
                name: "Game",
                url: "Game",
                defaults: new { controller = "Game", action = "Index" });

            routes.MapRoute(
                name: "Thumb",
                url: "Thumb",
                defaults: new { controller = "Thumb", action = "Index" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
