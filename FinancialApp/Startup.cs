using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FinancialApp.Startup))]
namespace FinancialApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
