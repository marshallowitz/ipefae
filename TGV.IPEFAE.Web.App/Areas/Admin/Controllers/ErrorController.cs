using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Controllers;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.App.Models;

namespace TGV.IPEFAE.Web.App.Areas.Admin.Controllers
{
    [IPEFAEAuthorizationAttribute(PermissaoModel.Tipo.Publico)]
    public class ErrorController : BaseController
    {
        public ActionResult Index()
        {
            return RedirecionarPagina("Index", "Error", "", 0);
        }

        public ViewResult EmConstrucao()
        {
            Response.StatusCode = 404;
            return RedirecionarPagina("EmConstrucao", "Error", "", 404) as ViewResult;
        }

        public ViewResult ErroIndeterminado()
        {
            Response.StatusCode = 500;
            return RedirecionarPagina("Error", "Error", "", 500) as ViewResult;
        }

        public ViewResult UnauthorizedAccess()
        {
            Response.StatusCode = 401;
            return RedirecionarPagina("UnauthorizedAccess", "Error", "", 401) as ViewResult;
        }

        public ViewResult NotFound()
        {
            Response.StatusCode = 404;
            return RedirecionarPagina("NotFound", "Error", "", 404) as ViewResult;
        }
	}
}