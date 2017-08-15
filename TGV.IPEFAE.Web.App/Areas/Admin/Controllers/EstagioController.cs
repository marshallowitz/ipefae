using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Controllers;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Areas.Admin.Controllers
{
    [IPEFAEAuthorizationAttribute(PermissaoModel.Tipo.Estagio)]
    public class EstagioController : BaseEstagioController
    {
        private const string NomeSessionPDF = "SessionPDFEstagiarios";
        private const string NomeSessionCSV = "SessionCSVEstagiarios";

        public ActionResult Index()
        {
            List<UsuarioEstagioModel> estagiarios = ListarEstagiarios(1, true, null, null, null, null, null, true, null, "N");

            return View(estagiarios);
        }

        public ActionResult BuscarCidadesCadastradas()
        {
            List<AutoCompleteItem> cidades = CidadeBusiness.ListarCidadesComEstagiario().ConvertAll(cid => new AutoCompleteItem(new CidadeModel(cid)));
            return Json(cidades, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Cadastro(int? id)
        {
            UsuarioEstagioModel ueModel = ObterEstagiario(id);
            ueModel.SenhaDescriptografada = BaseBusiness.Descriptografar(ueModel.Senha);
            return View("Cadastro", ueModel);
        }

        public ActionResult GerarCSVPesquisa(int pagina, string nome, string curso, int? semAno, bool? estagiando, string cpf, bool visualizacao, string cidade, string ordem)
        {
            List<UsuarioEstagioModel> todosEstagiarios = ListarEstagiarios(pagina, false, nome, curso, semAno, estagiando, cpf, visualizacao, cidade, ordem);
            Session[NomeSessionCSV] = todosEstagiarios;

            Session["GerouCSV"] = null;
            return Json(todosEstagiarios.Count() > 0, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GerarCSVPesquisaConfirmacao()
        {
            bool? gerou = Session["GerouCSV"] as Nullable<Boolean>;
            bool retorno = gerou.HasValue && gerou.Value;

            if (retorno)
                Session["GerouCSV"] = null;

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GerarPDFPesquisa(int pagina, string nome, string curso, int? semAno, bool? estagiando, string cpf, bool visualizacao, string cidade, string ordem)
        {
            List<UsuarioEstagioModel> todosEstagiarios = ListarEstagiarios(pagina, false, nome, curso, semAno, estagiando, cpf, visualizacao, cidade, ordem);
            Session[NomeSessionPDF] = todosEstagiarios;

            Session["GerouPDF"] = null;
            return Json(todosEstagiarios.Count() > 0, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GerarPDFPesquisaConfirmacao()
        {
            bool? gerou = Session["GerouPDF"] as Nullable<Boolean>;
            bool retorno = gerou.HasValue && gerou.Value;

            if (retorno)
                Session["GerouPDF"] = null;

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Pesquisar(int pagina, string nome, string curso, int? semAno, bool? estagiando, string cpf, bool visualizacao, string cidade, string ordem)
        {
            List<UsuarioEstagioModel> estagiarios = ListarEstagiarios(pagina, true, nome, curso, semAno, estagiando, cpf, visualizacao, cidade, ordem);

            try
            {
                string view = BaseController.RenderViewToString(ControllerContext, "~/Areas/Admin/Views/Estagio/_ListaEstagiarios.cshtml", estagiarios, true);
                return Json(new { View = view, TotalItens = estagiarios.Count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<UsuarioEstagioModel> ListarEstagiarios(int pagina, bool comPaginacao, string nome, string curso, int? semAno, bool? estagiando, string cpf, bool visualizacao, string cidade, string ordem)
        {
            int tamanhoPagina = TamanhoPaginaLista;
            List<UsuarioEstagioModel> estagiarios = UsuarioEstagioBusiness.Listar(pagina, comPaginacao, tamanhoPagina, nome, curso, semAno, estagiando, cpf, visualizacao, cidade, ordem).ConvertAll(ues => new UsuarioEstagioModel(ues));

            return estagiarios;
        }

        public ActionResult Salvar(int id, string nome, string email, string senha, string cpf, string rg, DateTime dataExpedicao, string nroCarteira,
            string serieCarteira, int ufCarteira, string estadoCivil, DateTime dataNasc, string telefone01, string telefone02, string endereco, int nroEndereco,
            string complemento, string bairro, string cep, int idCidade, int qtdadeFilhos, bool possuiDef, string defQual, string objetivos, int idDadosEscolares, string tipo_ensino,
            string nomeEscola, string de01, string de02, string periodo, string nomeCursoEscola, string dataInicioDE, string dataTerminoDE, bool possuiExp,
            string listaExp, string listaCursos, string listaOutros, bool ehMasc, bool temFoto, bool ehAdmin, bool considerar_busca, bool estagiando, bool ativo,
            string motivoDesativacao, string observacoesAdmin)
        {
            return base.Salvar(id, nome, email, senha, cpf, rg, dataExpedicao, nroCarteira, serieCarteira, ufCarteira, estadoCivil, dataNasc, telefone01, telefone02,
                endereco, nroEndereco, complemento, bairro, cep, idCidade, qtdadeFilhos, possuiDef, defQual, objetivos, idDadosEscolares, tipo_ensino, nomeEscola, de01, de02, periodo,
                nomeCursoEscola, dataInicioDE, dataTerminoDE, possuiExp, listaExp, listaCursos, listaOutros, ehMasc, temFoto, ehAdmin, considerar_busca, estagiando, ativo,
                motivoDesativacao, observacoesAdmin);
        }

        public class AutoCompleteItem
        {
            public AutoCompleteItem(CidadeModel cidade)
            {
                this.label = cidade.CidadeEstado;
                this.value = cidade.Id.ToString();
            }

            public string label { get; set; }
            public string value { get; set; }
        }
	}
}