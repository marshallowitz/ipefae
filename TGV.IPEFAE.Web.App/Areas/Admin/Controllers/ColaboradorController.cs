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
    public class ColaboradorController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastro(int? id)
        {
            int idt = id.HasValue && id.Value > 0 ? id.Value : 0;

            ColaboradorModel cM = ColaboradorBusiness.Obter(idt);

            if (cM == null)
                cM = new ColaboradorModel();

            return View("Cadastro", cM);
        }

        public ActionResult ListarColaboradores()
        {
            List<ColaboradorModel> colaboradores = ColaboradorBusiness.Listar();
            return PartialView("_ListaColaboradores", colaboradores);
        }

        public ActionResult Salvar(ColaboradorModel cM)
        {
            int id = cM.id;
            ColaboradorModel colaborador = ColaboradorBusiness.Salvar(cM);

            return Json(new { Colaborador = colaborador, Sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        private bool ValidarAcesso(int usu_idt_usuario)
        {
            if (UsuarioLogado.IsAdministrador)
                return true;

            tb_usu_usuario usuario = UsuarioBusiness.Obter(usu_idt_usuario);

            if (usuario != null)
            {
                UsuarioModel u = new UsuarioModel(usuario);

                return (UsuarioLogado.IsConcurso && u.IsConcurso)
                    || (UsuarioLogado.IsEstagio && u.IsEstagio)
                    || (UsuarioLogado.IsVestibular && u.IsVestibular);
            }

            return true;
        }
	}
}