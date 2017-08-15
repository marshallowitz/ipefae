using System.Web.Mvc;

namespace TGV.IPEFAE.Web.App.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                new string[] { "TGV.IPEFAE.Web.App.Areas.Admin.Controllers" }
            );

            context.MapRoute(
                "Admin_Inscrito_default",
                "Admin/{controller}/Inscrito/{action}/{id}",
                new { action = "Index_Inscrito", id = UrlParameter.Optional }, // Parameter defaults
                new string[] { "TGV.IPEFAE.Web.App.Areas.Admin.Controllers" }
            );
        }
    }
}