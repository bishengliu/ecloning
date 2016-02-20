using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ecloning.Startup))]
namespace ecloning
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
