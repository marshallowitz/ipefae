using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Controllers;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Areas.Admin.Controllers
{
    [IPEFAEAuthorizationAttribute()]
    public class ConcursoController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastro(int? id)
        {
            int idt = id.HasValue && id.Value > 0 ? id.Value : 0;

            ConcursoModel cM = ConcursoBusiness.Obter(idt);

            if (cM == null)
                cM = new ConcursoModel();

            return View("Cadastro", cM);
        }

        public ActionResult Funcao_Excluir(int idFuncao)
        {
            ConcursoBusiness.Funcao_Excluir(idFuncao);

            return Json(new { Sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Funcao_Salvar(int idConcurso, ConcursoFuncaoModel cfM)
        {
            ConcursoFuncaoModel funcao = ConcursoBusiness.Funcao_Salvar(idConcurso, cfM);

            return Json(new { Funcao = funcao, Sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarConcursos()
        {
            List<ConcursoModel> concursos = ConcursoBusiness.Listar();
            return PartialView("_ListaConcursos", concursos);
        }

        public ActionResult Obter(int id)
        {
            ConcursoModel cM = ConcursoBusiness.Obter(id);
            bool sucesso = (cM != null);

            return Json(new { Sucesso = sucesso, Concurso = cM }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Salvar(ConcursoModel cM)
        {
            int id = cM.id;
            ConcursoModel concurso = ConcursoBusiness.Salvar(cM);

            return Json(new { Concurso = concurso, Sucesso = true }, JsonRequestBehavior.AllowGet);
        }
	}
}