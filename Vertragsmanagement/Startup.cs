using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Vertragsmanagement.Startup))]

namespace Vertragsmanagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}