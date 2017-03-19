using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FootballAIGame.Web.Startup))]
namespace FootballAIGame.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
