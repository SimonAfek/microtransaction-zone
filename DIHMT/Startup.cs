using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(DIHMT.Startup))]
namespace DIHMT
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}