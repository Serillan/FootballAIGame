using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FootballAIGameWeb.Startup))]
namespace FootballAIGameWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
