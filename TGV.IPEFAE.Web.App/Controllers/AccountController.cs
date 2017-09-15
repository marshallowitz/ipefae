using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Controllers
{
    public class AccountController : BaseController
    {
        public ActionResult AbrirModalLogin()
        {
            string view = BaseController.RenderViewToString(ControllerContext, "~/Views/Shared/_ModalLogin.cshtml", null, true);

            return Json(new { View = view }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOff()
        {
            HttpCookie userCookie = new HttpCookie(BaseController._nomeCookieUsuarioLogado);
            userCookie.Expires = DateTime.Today.AddDays(-1);
            userCookie["email"] = "expired";
            Response.Cookies.Add(userCookie);
            BaseController.EhGestor = false;
            Session.Clear();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RealizarLogin(string email, string senha)
        {
            tb_usu_usuario usuario = UsuarioBusiness.RealizarLogin(email, senha);
            string status = "ok";
            string mensagem = "sucesso";

            if (usuario == null)
            {
                status = "senhaerrada";
                mensagem = Resources.Shared._ModalLogin.SenhaErrada;
            }
            else
            {
                UsuarioLogado = new UsuarioModel(usuario);

                HttpCookie userCookie = new HttpCookie(BaseController._nomeCookieUsuarioLogado);
                userCookie.Expires = BaseBusiness.DataAgora.AddDays(1);
                userCookie["email"] = usuario.usu_des_email;
                Response.Cookies.Add(userCookie);
                BaseController.EhGestor = usuario.per_idt_permissao == (int)PermissaoModel.Tipo.Administrador;
            }

            return Json(new { status = status.ToString(), Mensagem = mensagem, Admin = UsuarioLogado.IsAdministrador, Id = UsuarioLogado.Id, Url = UsuarioLogado.UrlAcessoInicial }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerificarUsuarioLogado()
        {
            UsuarioModel usuarioLogado = UsuarioLogado;
            bool logado = usuarioLogado != null && usuarioLogado.Id > 0;
            string nomeUsuario = String.Format("{0} {1}", Resources.Shared._LoginPartial.LabelOla, usuarioLogado.PrimeiroNome);
            return Json(new { Logado = logado, Ola = nomeUsuario, Admin = usuarioLogado.IsAdministrador }, JsonRequestBehavior.AllowGet);
        }
    }
}