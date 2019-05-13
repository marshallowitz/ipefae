using System.Web.Mvc;
using TGV.IPEFAE.Web.BL.Business;

namespace TGV.IPEFAE.Web.App.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Neotrading()
        {
            return View();
        }

        public ActionResult Neotrading_ObterSenha(string codigo)
        {
            string senha = NeotradingBusiness.ObterSenha(codigo);

            return Json(new { Sucesso = true, Senha = senha }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Neotrading_Senha()
        {
            return View();
        }

        public ActionResult Neotrading_Senha_Gerar(string email, string nome, string codigo)
        {
            string senha = NeotradingBusiness.GerarSenha(email, nome, codigo);

            return Json(new { Sucesso = true, Senha = senha }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewCarregando()
        {
            return View();
        }
    }
}