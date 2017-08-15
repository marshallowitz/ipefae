using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TGV.IPEFAE.Web.App
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Concurso_Inscricao",
                url: "Concurso/{idConcurso}/Inscricao/{tela}",
                defaults: new { controller = "Concurso", action = "Inscricao", tela = UrlParameter.Optional },
                namespaces: new string[] { "TGV.IPEFAE.Web.App.Controllers" },
                constraints: new { idConcurso = @"\d+" }
            );

            routes.MapRoute(
                name: "Concurso_Dados_old",
                url: "Concurso/Dados/{id}",
                defaults: new { controller = "Concurso", action = "RedirectDados" },
                namespaces: new string[] { "TGV.IPEFAE.Web.App.Controllers" }
            );

            routes.MapRoute(
                name: "Concurso_Dados",
                url: "Concurso/{idConcurso}",
                defaults: new { controller = "Concurso", action = "Dados" },
                namespaces: new string[] { "TGV.IPEFAE.Web.App.Controllers" },
                constraints: new { idConcurso = @"\d+" }
            );

            routes.MapRoute(
                name: "Default_Concurso",
                url: "Concurso/{action}",
                defaults: new { controller = "Concurso", action = "Index" },
                namespaces: new string[] { "TGV.IPEFAE.Web.App.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                namespaces: new string[] { "TGV.IPEFAE.Web.App.Controllers" }
            );
        }
    }
}
