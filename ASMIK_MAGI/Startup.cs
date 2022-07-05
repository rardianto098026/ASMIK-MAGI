using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ASMIK_MAGI.Startup))]
namespace ASMIK_MAGI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
