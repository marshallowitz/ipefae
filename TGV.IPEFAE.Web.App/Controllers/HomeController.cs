using System.Web.Mvc;

namespace TGV.IPEFAE.Web.App.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewCarregando()
        {
            return View();
        }
    }
}