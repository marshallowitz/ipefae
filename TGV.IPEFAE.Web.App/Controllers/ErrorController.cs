using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.App.Models;

namespace TGV.IPEFAE.Web.App.Controllers
{
    [IPEFAEAuthorizationAttribute(PermissaoModel.Tipo.Publico)]
    public class ErrorController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("ErroIndeterminado", "Error");
        }

        public ViewResult EmConstrucao()
        {
            Response.StatusCode = 404;
            return View("EmConstrucao");
        }

        public ViewResult ErroIndeterminado()
        {
            Response.StatusCode = 500;
            return View("Error");
        }

        public ViewResult InscricoesFechadas()
        {
            Response.StatusCode = 500;
            return View("InscricoesFechadas");
        }

        public ViewResult UnauthorizedAccess()
        {
            Response.StatusCode = 401;
            return View("UnauthorizedAccess");
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }
    }
}