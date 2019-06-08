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

            if (cM == null)
                cM = new ColaboradorModel();

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

        public ActionResult ListarCidades(int estado_id)
        {
            List<tb_cid_cidade> cs = CidadeBusiness.Listar(estado_id);
            List<TGV.IPEFAE.Web.App.Models.CidadeModel> cidades = cs.ConvertAll(c => TGV.IPEFAE.Web.App.Models.CidadeModel.Clone(c));

            return Json(new { Cidades = cidades }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarDadosTela()
        {
            List<BancoModel> bancos = BancoBusiness.Listar();
            List<BL.Data.EstadoModel> estados = EstadoBusiness.ListarModel();
            List<GrauInstrucaoModel> grausInstrucao = GrauInstrucaoBusiness.Listar();
            List<RacaModel> racas = RacaBusiness.Listar();

            return Json(new { Bancos = bancos, Estados = estados, GrausInstrucao = grausInstrucao, Racas = racas }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Obter(int id, bool isAdmin)
        {
            ColaboradorModel cM = ColaboradorBusiness.Obter(id);
            bool sucesso = (cM != null);

            return Json(new { Sucesso = sucesso, Colaborador = cM, SD = isAdmin ? cM?.senhaDescriptografada : null }, JsonRequestBehavior.AllowGet);
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

        public ActionResult Salvar(ColaboradorModel cM, bool novaSenha)
        {
            int id = cM.id;
            ColaboradorModel colaborador = ColaboradorBusiness.Salvar(cM, novaSenha);

            if (colaborador != null)
            {
                if (id <= 0)
                {
                    // Envia a senha por e-mail
                    EnviarSenha(colaborador.nome, colaborador.email, colaborador.senhaDescriptografada, true);
                }

                if (!UsuarioLogado.IsAdministrador)
                {
                    // Atualiza a sessão do usuário
                    RealizarLogin(colaborador.email, colaborador.senhaDescriptografada);
                }
            }

            return Json(new { Colaborador = colaborador, Sucesso = true, SD = UsuarioLogado.IsAdministrador ? cM?.senhaDescriptografada : null }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerificarCPFJaExiste(int id, string cpf)
        {
            ColaboradorModel cM = ColaboradorBusiness.ObterPorCPF(cpf);

            bool retorno = (cM != null && cM.id != id);

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerificarEmailJaExiste(int id, string email)
        {
            ColaboradorModel cM = ColaboradorBusiness.ObterPorEmail(email);

            bool retorno = (cM != null && cM.id != id);

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        private bool EnviarSenha(string nome, string email, string senha, bool novoUsuario = false)
        {
            if (String.IsNullOrEmpty(nome) || String.IsNullOrEmpty(email) || String.IsNullOrEmpty(senha))
                return false;

            // Envia e-mail
            string tituloEmail = novoUsuario ? "[IPEFAE] Cadastro Realizado com Sucesso" : String.Format(TGV.IPEFAE.Web.Resources.Email._HtmlEnvioSenha.TituloEmail, nome);
            string corpoEmail = MontarCorpoEmailEnvioSenha(nome, email, senha, tituloEmail, novoUsuario);
            EmailBusiness.EnviarEmail(email, BaseBusiness.EmailNaoRespondaIPEFAE, BaseBusiness.NomeNaoRespondaIPEFAE, tituloEmail, corpoEmail, false, false, new List<string>());

            return true;
        }

        private string MontarCorpoEmailEnvioSenha(string nome, string email, string senha, string assunto, bool isNovo)
        {
            EmailModel eModel = new EmailModel();
            eModel.Nome = nome;
            eModel.Email = email;
            eModel.Assunto = assunto;
            eModel.Senha = senha;
            eModel.IsNovo = isNovo;

            return System.Web.HttpUtility.HtmlDecode(this.PartialViewToString("~/Views/Email/_HtmlEnvioSenha.cshtml", eModel));
        }
	}
}