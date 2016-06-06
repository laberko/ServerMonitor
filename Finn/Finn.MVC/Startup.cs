using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Finn.MVC.Startup))]
namespace Finn.MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
