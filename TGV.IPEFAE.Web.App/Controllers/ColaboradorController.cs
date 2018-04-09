using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Controllers
{
    public class ColaboradorController : BaseController
    {
        protected const string NomeSessionUsuarioColaborador = "SessionUsuarioColaborador";

        public ActionResult Index()
        {
            Session[NomeSessionUsuarioColaborador] = null;
            return View("Index");
        }

        public ActionResult Cadastro(int? id)
        {
            int idt = id.HasValue && id.Value > 0 ? id.Value : 0;

            if (UsuarioLogado == null || UsuarioLogado.Id != idt)
                idt = 0;

            ColaboradorModel cM = ColaboradorBusiness.Obter(idt);

            return View("Cadastro", cM);
        }

        public ActionResult EnviarSenhaPorEmail(string cpf)
        {
            // Busca cpf nos usuários cadastrados
            ColaboradorModel cM = ColaboradorBusiness.ObterPorCPF(cpf);

            // Caso não encontre, retorna falso
            if (cM == null)
                return Json(false, JsonRequestBehavior.AllowGet);

            // Caso encontre, envia um e-mail com a senha
            bool sucesso = EnviarSenha(cM.nome, cM.email, cM.senhaDescriptografada);

            return Json(sucesso, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RealizarLogin(string email, string senha)
        {
            ColaboradorModel cM = ColaboradorBusiness.RealizarLogin(email, senha);
            bool sucesso = false;

            if (cM != null)
            {
                sucesso = true;
                UsuarioLogado = new UsuarioModel(cM);
            }

            return Json(new { Sucesso = sucesso, IdColaborador = UsuarioLogado.Id, Url = UsuarioLogado.UrlAcessoInicial }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Salvar(ColaboradorModel cM)
        {
            ColaboradorModel colaborador = ColaboradorBusiness.Salvar(cM);

            // Atualiza a sessão do usuário
            RealizarLogin(cM.email, cM.senha);

            return Json(new { Colaborador = colaborador, Sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        private bool EnviarSenha(string nome, string email, string senha)
        {
            if (String.IsNullOrEmpty(nome) || String.IsNullOrEmpty(email) || String.IsNullOrEmpty(senha))
                return false;

            // Envia e-mail
            string tituloEmail = String.Format(TGV.IPEFAE.Web.Resources.Email._HtmlEnvioSenha.TituloEmail, nome);
            string corpoEmail = MontarCorpoEmailEnvioSenha(nome, email, senha, tituloEmail);
            EmailBusiness.EnviarEmail(email, BaseBusiness.EmailNaoRespondaIPEFAE, BaseBusiness.NomeNaoRespondaIPEFAE, tituloEmail, corpoEmail, false, false, new List<string>());

            return true;
        }

        private string MontarCorpoEmailEnvioSenha(string nome, string email, string senha, string assunto)
        {
            EmailModel eModel = new EmailModel();
            eModel.Nome = nome;
            eModel.Email = email;
            eModel.Assunto = assunto;
            eModel.Senha = senha;

            return System.Web.HttpUtility.HtmlDecode(this.PartialViewToString("~/Views/Email/_HtmlEnvioSenha.cshtml", eModel));
        }
	}
}