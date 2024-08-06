using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Undisclosed_Shop.Startup))]
namespace Undisclosed_Shop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
