using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.BL.Data;
using Rotativa;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace TGV.IPEFAE.Web.App.Controllers
{
    public class EstagioController : BaseEstagioController
    {
        public ActionResult Index()
        {
            Session[NomeSessionUsuarioEstagio] = null;
            Session[NomeSessionTotalExperiencias] = null;
            Session[NomeSessionFotoUpload] = null;
            return BaseIndex();
        }

        public ActionResult Cadastro(int? id)
        {
            int idt = id.HasValue && id.Value > 0 ? id.Value : 0;

            if (UsuarioLogado == null || UsuarioLogado.Id != idt)
                idt = 0;

            return BaseCadastro(idt);
        }

        public ActionResult CarregarCidades(int est_idt_estado)
        {
            List<tb_cid_cidade> cidades = CidadeBusiness.Listar(est_idt_estado);
            List<SelectListItem> cidadesLI = cidades.ConvertAll(cid => new SelectListItem() { Text = cid.cid_nom_cidade, Value = cid.cid_idt_cidade.ToString() });

            return Json(cidadesLI, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CarregarEstados(bool exibirNome)
        {
            List<tb_est_estado> estados = EstadoBusiness.Listar();
            List<SelectListItem> estadosLI = estados.ConvertAll(est => new SelectListItem() { Text = exibirNome ? est.est_nom_estado : est.est_sig_estado, Value = est.est_idt_estado.ToString() });

            return Json(estadosLI, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EnviarSenhaPorEmail(string cpf)
        {
            // Busca cpf nos usuários cadastrados
            UsuarioEstagioModel ueModel = new UsuarioEstagioModel(UsuarioEstagioBusiness.ObterPorCPF(cpf));

            // Caso não encontre, retorna falso
            if (ueModel == null)
                return Json(false, JsonRequestBehavior.AllowGet);

            // Caso encontre, envia um e-mail com a senha
            bool sucesso = EnviarSenha(ueModel.Nome, ueModel.Email, ueModel.SenhaDescriptografada);

            return Json(sucesso, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RealizarLogin(string email, string senha)
        {
            tb_ues_usuario_estagio usuario = UsuarioEstagioBusiness.RealizarLogin(email, senha);
            bool sucesso = false;

            if (usuario != null)
            {
                sucesso = true;
                UsuarioLogado = new UsuarioModel(usuario);
            }

            return Json(new { Sucesso = sucesso, IdEstagiario = UsuarioLogado.Id, Url = UsuarioLogado.UrlAcessoInicial }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Salvar(int id, string nome, string email, string senha, string cpf, string rg, DateTime dataExpedicao, string nroCarteira,
            string serieCarteira, int ufCarteira, string estadoCivil, DateTime dataNasc, string telefone01, string telefone02, string endereco, int nroEndereco,
            string complemento, string bairro, string cep, int idCidade, int qtdadeFilhos, bool possuiDef, string defQual, string objetivos, int idDadosEscolares, string tipo_ensino,
            string nomeEscola, string de01, string de02, string periodo, string nomeCursoEscola, string dataInicioDE, string dataTerminoDE, bool possuiExp,
            string listaExp, string listaCursos, string listaOutros, bool ehMasc, bool temFoto, bool ehAdmin, bool considerar_busca, bool estagiando, bool ativo,
            string motivoDesativacao, string observacoesAdmin)
        {
            ActionResult ar = base.Salvar(id, nome, email, senha, cpf, rg, dataExpedicao, nroCarteira, serieCarteira, ufCarteira, estadoCivil, dataNasc, telefone01, telefone02,
                endereco, nroEndereco, complemento, bairro, cep, idCidade, qtdadeFilhos, possuiDef, defQual, objetivos, idDadosEscolares, tipo_ensino, nomeEscola, de01, de02, periodo,
                nomeCursoEscola, dataInicioDE, dataTerminoDE, possuiExp, listaExp, listaCursos, listaOutros, ehMasc, temFoto, ehAdmin, considerar_busca, estagiando, ativo,
                motivoDesativacao, observacoesAdmin);

            if (id == 0) // Se for criação, atualiza a sessão do usuário
                RealizarLogin(email, senha);

            return ar;
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