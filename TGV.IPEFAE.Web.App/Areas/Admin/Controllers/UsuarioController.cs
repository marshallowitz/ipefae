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
    public class UsuarioController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastro(int? id)
        {
            UsuarioModel usuario = new UsuarioModel();

            if (id.HasValue && id.Value > 0)
            {
                // Verifica se este usuário logado tem permissão para ver a ser editado
                if (!ValidarAcesso(id.Value))
                {
                    System.Web.Routing.RouteValueDictionary redirectTargetDictionary = new System.Web.Routing.RouteValueDictionary();
                    redirectTargetDictionary.Add("action", "UnauthorizedAccess");
                    redirectTargetDictionary.Add("controller", "Error");
                    redirectTargetDictionary.Add("area", "");
                    redirectTargetDictionary.Add("id", 401);
                    return new RedirectToRouteResult(redirectTargetDictionary);
                }

                usuario = new UsuarioModel(UsuarioBusiness.Obter(id.Value));
            }

            return View(usuario);
        }

        public ActionResult EditarAtivacao(int id)
        {
            tb_usu_usuario usuario = UsuarioBusiness.EditarAtivacao(id);
            return Json(new { Sucesso = usuario != null, Ativo = usuario.usu_bit_ativo }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarUsuarios()
        {
            int per_idt_permissao = UsuarioLogado.IdPermissao;
            List<UsuarioModel> usuarios = UsuarioBusiness.Listar(per_idt_permissao).ConvertAll(usu => new UsuarioModel(usu));
            return PartialView("_ListaUsuarios", usuarios);
        }

        public ActionResult Salvar(int id, string nome, string email, long telefone, string senha, int permissao, bool ativo)
        {
            tb_usu_usuario usuario = new tb_usu_usuario()
            {
                per_idt_permissao = permissao,
                usu_bit_ativo = ativo,
                usu_des_email = email,
                usu_des_senha = senha,
                usu_idt_usuario = id,
                usu_nom_usuario = nome,
                usu_num_telefone = telefone
            };

            bool sucesso = UsuarioBusiness.Salvar(usuario) != null;
            string mensagem = sucesso ? Resources.Admin.Usuario.Cadastro.SalvarSucesso : Resources.Admin.Usuario.Cadastro.SalvarErro;

            return Json(new { Sucesso = sucesso, Mensagem = mensagem }, JsonRequestBehavior.AllowGet);
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