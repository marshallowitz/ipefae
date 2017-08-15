using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TGV.IPEFAE.Web.App.Startup))]
namespace TGV.IPEFAE.Web.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            ConfigureAuth(app);
        }
    }
}
