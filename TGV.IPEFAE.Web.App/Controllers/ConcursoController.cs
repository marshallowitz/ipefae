using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.BL.Data;
using System.Text;
using RazorEngine;
using RazorEngine.Templating;

namespace TGV.IPEFAE.Web.App.Controllers
{
    public class ConcursoController : BaseController
    {
        #region [ Membros ]

        private const string SessionRecursoAberto = "RecursoAberto";
        private const string SessionArquivoRecursoUpload = "ArquivoRecursoUpload";
        private const string SessionDadosInscrito = "DadosCadastroInscrito";

        private enum TipoBuscaInscrito
        {
            Indefinido = 0,
            Boleto = 1,
            ComprovantePagamento = 2,
            Classificacao = 3,
            StatusPagamento = 4
        }

        #endregion [ FIM - Membros ]

        #region [ Views ]

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dados(int idConcurso)
        {
            Session[SessionDadosInscrito] = null;

            // Busca os dados do concurso a partir do Id
            tb_con_concurso con = ConcursoBusiness.Obter(idConcurso, false, false);
            ConcursoModel concurso = new ConcursoModel(con);

            if (concurso == null || concurso.Id <= 0)
                return RedirecionarPagina("Index", "Concurso", "", 0);

            return View(concurso);
        }

        public ActionResult RedirectDados(int id)
        {
            return RedirectPermanent("/Concurso/" + id);
        }

        #region [ Inscrição ]

        public ActionResult Inscricao(int idConcurso, string tela)
        {
            if (String.IsNullOrEmpty(tela))
            {
                ConcursoModel.InscritoModel inscrito = new ConcursoModel.InscritoModel();

                // Busca os dados do concurso a partir do Id
                ConcursoModel concurso = new ConcursoModel(ConcursoBusiness.Obter(idConcurso, false));

                if (concurso == null || concurso.Id <= 0)
                    return RedirecionarPagina("Index", "Concurso", "", 0);

                // Verifica se o período de inscricoes do concurso está aberto
                if (!concurso.PeriodoInscricoesEstaAberto)
                    return RedirecionarPagina("InscricoesFechadas", "Error", "", 500);

                if (Session[SessionDadosInscrito] != null)
                {
                    dynamic inscritoSession = Session[SessionDadosInscrito];
                    inscrito = (ConcursoModel.InscritoModel)inscritoSession.InscritoModel;
                    inscrito.Id = 0;
                }

                inscrito.IdConcurso = concurso.Id;
                inscrito.NomeConcurso = concurso.Nome;
                inscrito.IdTipoLayoutConcurso = concurso.IdTipoLayoutConcurso;

                return View(inscrito);
            }
            else
                return RedirecionarPagina(String.Format("Inscricao{0}", tela), "Concurso", "", 0);
        }

        public ActionResult InscricaoConfirmacao()
        {
            ConcursoModel.InscritoModel inscrito = new ConcursoModel.InscritoModel();

            if (Session[SessionDadosInscrito] == null)
                return RedirecionarPagina("Index", "Concurso", "", 0);

            dynamic inscritoSession = Session[SessionDadosInscrito];

            inscrito = (ConcursoModel.InscritoModel)inscritoSession.InscritoModel;

            if (inscritoSession.IdTipoLayoutConcurso == 2 && inscritoSession.tb_icv_inscrito_concurso_vestibular != null) // Se for vestibular, vai buscar as outras opções
            {
                int? idOpcao2 = inscritoSession.tb_icv_inscrito_concurso_vestibular.icv_idt_opcao_2;
                int? idOpcao3 = inscritoSession.tb_icv_inscrito_concurso_vestibular.icv_idt_opcao_3;

                List<tb_cco_cargo_concurso> cargos = ConcursoBusiness.ListarCargosPorId(idOpcao2, idOpcao3);
                string nomeCargo = inscrito.NomeCargo;

                if (idOpcao2.HasValue)
                {
                    tb_cco_cargo_concurso cargo = cargos.SingleOrDefault(cco => cco.cco_idt_cargo_concurso == idOpcao2.Value);

                    if (cargo != null)
                        nomeCargo = String.Format("{0} / {1}", nomeCargo, cargo.cco_nom_cargo_concurso);
                }

                if (idOpcao3.HasValue)
                {
                    tb_cco_cargo_concurso cargo = cargos.SingleOrDefault(cco => cco.cco_idt_cargo_concurso == idOpcao3.Value);

                    if (cargo != null)
                        nomeCargo = String.Format("{0} / {1}", nomeCargo, cargo.cco_nom_cargo_concurso);
                }

                inscrito.NomeCargo = nomeCargo;
            }

            return View(inscrito);
        }

        public ActionResult InscricaoConfirmada()
        {
            if (Session[SessionDadosInscrito] == null)
                return RedirecionarPagina("Index", "Concurso", "", 0);

            dynamic inscritoSession = Session[SessionDadosInscrito];
            ConcursoModel.InscritoModel inscrito = (ConcursoModel.InscritoModel)inscritoSession.InscritoModel;
            
            if (String.IsNullOrEmpty(inscrito.NomeCargo))
                inscrito.NomeCargo = inscrito.CargosString;

            if (String.IsNullOrEmpty(inscrito.RGComEstado))
                inscrito.RGComEstado = String.Format("{0} / {1}", inscrito.RG, inscrito.EstadoRG.Sigla);

            ConcursoModel concurso = new ConcursoModel(ConcursoBusiness.Obter(inscrito.IdConcurso, false));

            if (String.IsNullOrEmpty(inscrito.DataEncerramentoInscricaoString))
                inscrito.DataEncerramentoInscricaoString = concurso.DataEncerramentoInscricoesString;

            if (inscrito.DataEncerramentoInscricao.Year < 2000)
                inscrito.DataEncerramentoInscricao = concurso.DataEncerramentoInscricoes;

            if (String.IsNullOrEmpty(inscrito.DataVencimentoBoletoString))
                inscrito.DataVencimentoBoletoString = concurso.DataBoletoString;

            inscrito.BancoCss = concurso.BancoCss;

            tb_ico_inscrito_concurso ico = (tb_ico_inscrito_concurso)inscritoSession.tb_ico_inscrito_concurso;

            Session[SessionDadosInscrito] = null;
            return View(inscrito);
        }

        #endregion [ FIM - Inscrição ]

        #region [ Partial Views ]

        /// <summary>
        /// Busca os dados do inscrito
        /// </summary>
        /// <param name="idTipo">
        /// 1: Boleto
        /// 2: Comprovante Pagamento
        /// 3: Classificação
        /// 4: Status Pagamento
        /// </param>
        /// <returns></returns>
        public ActionResult AbrirModalBuscaInscrito(int idTipo)
        {
            try
            {
                string viewName = String.Empty;

                switch ((TipoBuscaInscrito)idTipo)
                {
                    case TipoBuscaInscrito.Boleto:
                        viewName = "~/Views/Concurso/_ModalBuscaInscrito.cshtml";
                        break;
                    case TipoBuscaInscrito.ComprovantePagamento:
                        viewName = "~/Views/Concurso/_ModalBuscaInscritoComprovante.cshtml";
                        break;
                    case TipoBuscaInscrito.Classificacao:
                        viewName = "~/Views/Concurso/_ModalBuscaInscritoClassificacao.cshtml";
                        break;
                    case TipoBuscaInscrito.StatusPagamento:
                        viewName = "~/Views/Concurso/_ModalBuscaInscritoStatusPagamento.cshtml";
                        break;
                    default:
                        return Json(new { View = "" }, JsonRequestBehavior.AllowGet);
                }

                string view = BaseController.RenderViewToString(ControllerContext, viewName, null, true);
                return Json(new { View = view }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ExibirModalListaCargos(int idTipo, int idConcurso, string cpf)
        {
            // Busca para ver se tem alguem cadastrado para este concurso com este CPF
            List<tb_ico_inscrito_concurso> icos = InscritoConcursoBusiness.Listar(idConcurso, cpf);
            List<ConcursoModel.InscritoModel> inscritos = icos.ConvertAll(ico => new ConcursoModel.InscritoModel(ico));

            ConcursoModel concurso = new ConcursoModel(ConcursoBusiness.Obter(idConcurso, false, false));

            // Se as inscricoes ja acabaram, remove todo mundo que nao é isento e nao pagou
            if (concurso.DataEncerramentoInscricoes < BaseBusiness.DataAgora)
                if (idTipo != 4 && (idTipo != 1 || concurso.DataBoleto < BaseBusiness.DataAgora))
                    inscritos.RemoveAll(ico => !ico.Isento && !ico.ValorPago.HasValue); 

            try
            {
                string view = BaseController.RenderViewToString(ControllerContext, "~/Views/Concurso/_ListaCargosInscrito.cshtml", inscritos, true);
                return Json(new { View = view, IdConcurso = idConcurso }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion [ FIM - Partial Views ]

        #endregion [ FIM - Views ]

        #region [ Métodos ]

        public ActionResult BuscarInscritoPorCPF(int idConcurso, string cpf, int idMatricula, int idTipo)
        {
            // Busca para ver se tem alguem cadastrado para este concurso com este CPF
            ConcursoModel concurso = new ConcursoModel(ConcursoBusiness.Obter(idConcurso, false, false));

            tb_ico_inscrito_concurso ico = new tb_ico_inscrito_concurso();

            if (idMatricula > 0)
                ico = InscritoConcursoBusiness.Obter(idMatricula);
            else
            {
                bool dentroPeriodoInscricoes = 
                    (idTipo == 1) || (idTipo == 4) ||
                    (concurso.Data <= BaseBusiness.DataAgora && concurso.DataEncerramentoInscricoes >= BaseBusiness.DataAgora) ||
                    (idTipo == 4 && concurso.DataBoleto >= BaseBusiness.DataAgora);

                // Verifica se existe mais de um cargo
                List<tb_ico_inscrito_concurso> icos = InscritoConcursoBusiness.ListarConsiderandoPeriodoInscricoes(idConcurso, cpf, dentroPeriodoInscricoes);

                if (icos.Count > 0)
                {
                    if (icos.Count > 1) // Se tiver mais do que 1, retorna
                        return Json(new { Sucesso = true, MaisUmCargo = true }, JsonRequestBehavior.AllowGet);

                    ico = icos[0];
                }
            }

            if (ico == null || ico.ico_idt_inscrito_concurso <= 0)
                return Json(new { Sucesso = false }, JsonRequestBehavior.AllowGet);

            tb_cci_concurso_cargo_inscrito cci = idMatricula == 0 ? ico.tb_cci_concurso_cargo_inscrito.FirstOrDefault() : ico.tb_cci_concurso_cargo_inscrito.SingleOrDefault(ci => ci.ico_idt_inscrito_concurso == idMatricula);

            List<tb_cci_concurso_cargo_inscrito> ccis = new List<tb_cci_concurso_cargo_inscrito>()
            {
                new tb_cci_concurso_cargo_inscrito()
                {
                    tb_cco_cargo_concurso = cci != null ? cci.tb_cco_cargo_concurso : ico.tb_con_concurso.tb_cco_cargo_concurso.FirstOrDefault()
                }
            };

            ConcursoModel.InscritoModel inscrito = new ConcursoModel.InscritoModel(ico, ccis);
            inscrito.NomeConcurso = concurso.Nome;
            inscrito.NomeCargo = String.IsNullOrEmpty(inscrito.NomeCargo) ? inscrito.CargosString : inscrito.NomeCargo;
            inscrito.RGComEstado = String.IsNullOrEmpty(inscrito.RGComEstado) ? String.Format("{0} / {1}", inscrito.RG, inscrito.EstadoRG.Sigla) : inscrito.RGComEstado;

            string dadosStatusPagamento = String.Format("#{0} - {1} ({2})", inscrito.NroMatricula, inscrito.Nome, inscrito.NomeCargo);

            //Session[SessionDadosInscrito] = new { InscritoModel = inscrito, tb_ico_inscrito_concurso = ico };
            return Json(new { Sucesso = true, IdInscrito = inscrito.Id, MaisUmCargo = false, StatusPagamento = inscrito.StatusPagamento, DadosStatusPagamento = dadosStatusPagamento, NomeConcurso = concurso.Nome }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ConfirmarInscricao()
        {
            if (Session[SessionDadosInscrito] == null)
                return RedirecionarPagina("Index", "Concurso", "", 0);

            dynamic inscritoSession = Session[SessionDadosInscrito];
            ConcursoModel.InscritoModel inscrito = (ConcursoModel.InscritoModel)inscritoSession.InscritoModel;
            tb_ico_inscrito_concurso ico = (tb_ico_inscrito_concurso)inscritoSession.tb_ico_inscrito_concurso;
            tb_icv_inscrito_concurso_vestibular icv = inscritoSession.tb_icv_inscrito_concurso_vestibular as tb_icv_inscrito_concurso_vestibular;
            List<tb_cci_concurso_cargo_inscrito> ccis = new List<tb_cci_concurso_cargo_inscrito>() { new tb_cci_concurso_cargo_inscrito() { con_idt_concurso = inscrito.IdConcurso, cco_idt_cargo_concurso = inscrito.IdCargo } };

            ico = InscritoConcursoBusiness.Salvar(ico, ccis);

            // Se for vestibular
            if (inscritoSession.IdTipoLayoutConcurso == 2)
            {
                #region [ Verifica se está tudo ok ]
                // Verifica se o id do inscrito foi gerado corretamente
                if (ico.ico_idt_inscrito_concurso <= 0)
                {
                    // Se não foi gerado corretamente, vai buscar os 5 ultimos cadastros
                    List<tb_ico_inscrito_concurso> ultimosInscritos = InscritoConcursoBusiness.ListarUltimosInscritos(ico.con_idt_concurso);

                    foreach (tb_ico_inscrito_concurso inscr in ultimosInscritos)
                    {
                        if (inscr.ico_des_cpf == ico.ico_des_cpf && inscr.ico_dat_inscricao == ico.ico_dat_inscricao)
                        {
                            ico.ico_idt_inscrito_concurso = inscr.ico_idt_inscrito_concurso;
                            break;
                        }
                    }
                }

                #endregion [ FIM - Verifica se está tudo ok ]

                try
                {
                    if (ico.ico_idt_inscrito_concurso <= 0)
                        throw new Exception("Id do inscrito não foi gerado");

                    if (icv.icv_bit_ativo)
                    {
                        icv.ico_idt_inscrito_concurso = ico.ico_idt_inscrito_concurso;
                        icv = InscritoConcursoBusiness.Salvar(icv);
                    }
                }
                catch (Exception ex)
                {
                    GravarLogErroInscricaoVestibular(ico, icv, ex);
                }
            }

            inscrito.Id = ico.ico_idt_inscrito_concurso;
            Session[SessionDadosInscrito] = new { InscritoModel = inscrito, tb_ico_inscrito_concurso = ico, tb_icv_inscrito_concurso_vestibular = icv };
            
            return Json(new { Sucesso = true, IdInscrito = ico.ico_idt_inscrito_concurso, Mensagem = Resources.Shared._DadosInscrito.MensagemSucessoSalvar }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GerarBoletoBancario(int idConcurso, string idInscrito)
        {
            ConcursoModel.InscritoModel iModel = MontarInscrito(idInscrito);

            if (iModel == null)
                return RedirecionarPagina("Index", "Concurso", "", 0);

            BoletoModel bm = new BoletoModel();
            string htmlBoleto = bm.GerarBoleto(idConcurso, iModel);

            if (String.IsNullOrEmpty(htmlBoleto))
                htmlBoleto = System.Web.HttpUtility.HtmlDecode(this.PartialViewToString("~/Views/Error/BoletoEnviado.cshtml", null));

            return Json(new { Boleto = htmlBoleto }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GerarClassificacao(string nomeConcurso, string idInscrito)
        {
            ConcursoModel.InscritoModel inscrito = MontarInscrito(idInscrito);

            if (inscrito == null)
                return RedirecionarPagina("Index", "Concurso", "", 0);

            inscrito.NomeCargo = inscrito.CargosString;
            inscrito.RGComEstado = String.Format("{0} / {1}", inscrito.RG, inscrito.EstadoRG.Sigla);
            inscrito.NomeConcurso = nomeConcurso;

            try
            {
                // Carrega os dados da classificacao, se houver
                inscrito.Classificacao = new ConcursoModel.InscritoModel.ClassificacaoModel(ConcursoBusiness.ObterClassificacao(inscrito.Id));

                string view = BaseController.RenderViewToString(ControllerContext, "~/Views/Concurso/ImprimirClassificacao.cshtml", inscrito, true);
                return Json(new { Comprovante = view }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GerarComprovanteInscricao(string nomeConcurso, string idInscrito)
        {
            ConcursoModel.InscritoModel inscrito = MontarInscrito(idInscrito);

            if (inscrito == null)
                return RedirecionarPagina("Index", "Concurso", "", 0);

            inscrito.NomeCargo = inscrito.CargosString;
            inscrito.RGComEstado = String.Format("{0} / {1}", inscrito.RG, inscrito.EstadoRG.Sigla);
            inscrito.NomeConcurso = nomeConcurso;

            try
            {
                // Carrega os dados da prova, se houver
                inscrito.Prova = new ConcursoModel.InscritoModel.ProvaModel(ConcursoBusiness.ObterDadosProva(inscrito.Id));

                string view = BaseController.RenderViewToString(ControllerContext, "~/Views/Concurso/ImprimirComprovanteInscricao.cshtml", inscrito, true);
                return Json(new { Comprovante = view }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GerarStatusPagamento(string nomeConcurso, string idInscrito)
        {
            ConcursoModel.InscritoModel iModel = MontarInscrito(idInscrito);

            if (iModel == null)
                return RedirecionarPagina("Index", "Concurso", "", 0);

            iModel.NomeCargo = iModel.CargosString;
            iModel.RGComEstado = String.Format("{0} / {1}", iModel.RG, iModel.EstadoRG.Sigla);
            iModel.NomeConcurso = nomeConcurso;
            iModel.IsStatusPagamento = true;

            try
            {
                var html = System.Web.HttpUtility.HtmlDecode(this.PartialViewToString("InscricaoConfirmada", iModel));

                return Json(new { Panel = html }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult LiberarOpcaoRecurso(int idConcurso, string cpf, int idMatricula)
        {
            ConcursoModel concurso = new ConcursoModel(ConcursoBusiness.Obter(idConcurso, false, false));
            ConcursoModel.InscritoModel inscrito = null;

            bool dentroPeriodoInscricoes = concurso.Data <= BaseBusiness.DataAgora && concurso.DataEncerramentoInscricoes >= BaseBusiness.DataAgora;

            // Se tiver matricula, busca o valor diretamente
            if (idMatricula > 0)
                inscrito = new ConcursoModel.InscritoModel(InscritoConcursoBusiness.Obter(idMatricula));
            else
            {
                // Verifica se existe mais de um cargo
                List<tb_ico_inscrito_concurso> icos = InscritoConcursoBusiness.Listar(idConcurso, cpf).Where(ico => ico.ico_des_cpf == cpf && ico.ico_bit_ativo && (dentroPeriodoInscricoes || ico.ico_bit_pago || ico.ico_bit_isento)).ToList();
                List<ConcursoModel.InscritoModel> inscritos = icos.ConvertAll(ico => new ConcursoModel.InscritoModel(ico));

                if (inscritos.Count > 0)
                {
                    if (inscritos.Count > 1) // Se tiver mais do que 1, retorna
                        return Json(new { Sucesso = true, MaisUmCargo = true }, JsonRequestBehavior.AllowGet);

                    inscrito = inscritos[0];
                }
            }

            bool sucesso = (inscrito != null && (dentroPeriodoInscricoes || inscrito.Pagou || inscrito.Isento));
            string nome = sucesso ? inscrito.Nome : String.Empty;
            int idInscrito = sucesso ? inscrito.Id : 0;
            string inscricaoRequerente = sucesso ? inscrito.Id.ToString().PadLeft(10, '0') : String.Empty;
            string cargoRequerente = sucesso ? inscrito.CargosString : String.Empty;

            return Json(new { Sucesso = sucesso, InscricaoRequerente = inscricaoRequerente, NomeRequerente = nome, CargoRequerente = cargoRequerente, IdInscrito = idInscrito, MaisUmCargo = false }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarDadosConcurso(int? idConcurso)
        {
            if (!idConcurso.HasValue)
                return RedirecionarPagina("Index", "Concurso", "", 0);

            int concursoId = idConcurso.Value;
            ConcursoModel concurso = new ConcursoModel(ConcursoBusiness.Obter(concursoId, false, false));

            try
            {
                string publicacoes = BaseController.RenderViewToString(ControllerContext, "~/Views/Concurso/_ListaPublicacoes.cshtml", concurso.Anexos.Where(ane => ane.IdTipo == 1).ToList(), true);
                string provasGabaritos = BaseController.RenderViewToString(ControllerContext, "~/Views/Concurso/_ListaProvasGabaritos.cshtml", concurso.Anexos.Where(ane => ane.IdTipo != 1).ToList(), true);

                return Json(new { Publicacoes = publicacoes, ProvasGabaritos = provasGabaritos }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListarConcursos()
        {
            List<ConcursoModel> concursos = ConcursoBusiness.ListarSomenteConcursos().ConvertAll(con => new ConcursoModel(con));
            ConcursosExtension ce = concursos.ObterConcursosExtension();

            try
            {
                string abertas = BaseController.RenderViewToString(ControllerContext, "~/Views/Concurso/_ListaConcursos.cshtml", ce.Abertas, true);
                string emAndamento = BaseController.RenderViewToString(ControllerContext, "~/Views/Concurso/_ListaConcursos.cshtml", ce.EmAndamento, true);
                string encerradas = BaseController.RenderViewToString(ControllerContext, "~/Views/Concurso/_ListaConcursos.cshtml", ce.Encerradas, true);

                return Json(new { Abertas = abertas, TotalAbertas = ce.Abertas.Count, EmAndamento = emAndamento, TotalEmAndamento = ce.EmAndamento.Count, Encerradas = encerradas, TotalEncerradas = ce.Encerradas.Count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        #region [ Privados ]

        private ConcursoModel.InscritoModel MontarInscrito(string idInscrito)
        {
            int idt_inscrito = 0;

            if (!Int32.TryParse(idInscrito, out idt_inscrito))
                return null;

            tb_ico_inscrito_concurso ico = InscritoConcursoBusiness.Obter(idt_inscrito);

            if (ico == null)
                return null;

            List<tb_cci_concurso_cargo_inscrito> ccis = new List<tb_cci_concurso_cargo_inscrito>();

            if (ico.tb_cci_concurso_cargo_inscrito.Count > 0)
                ccis = ico.tb_cci_concurso_cargo_inscrito.ToList();
            else
                ccis = new List<tb_cci_concurso_cargo_inscrito>() { new tb_cci_concurso_cargo_inscrito() { tb_cco_cargo_concurso = ico.tb_con_concurso.tb_cco_cargo_concurso.FirstOrDefault() } };

            return new ConcursoModel.InscritoModel(ico, ccis);
        }

        private void GravarLogErroInscricaoVestibular(tb_ico_inscrito_concurso ico, tb_icv_inscrito_concurso_vestibular icv, Exception ex)
        {
            StringBuilder sbInscrito = new StringBuilder();
            sbInscrito.AppendFormat("IdConcurso: {0}", ico.con_idt_concurso);
            sbInscrito.AppendLine();

            if (ico.ico_idt_inscrito_concurso > 0)
            {
                sbInscrito.AppendFormat("Id Inscrito: {0}", ico.ico_idt_inscrito_concurso);
                sbInscrito.AppendLine();
            }

            sbInscrito.AppendFormat("CPF: {0}", ico.ico_des_cpf);
            sbInscrito.AppendLine();
            sbInscrito.AppendFormat("Data Inscrição: {0}", ico.ico_dat_inscricao.ToString("dd/MM/yyyy HH:mm"));
            sbInscrito.AppendLine();
            sbInscrito.AppendFormat("Exception: {0}", ex.Message);
            sbInscrito.AppendLine();

            if (ex.InnerException != null)
            {
                sbInscrito.AppendFormat("InnerException: {0}", ex.InnerException.Message);
                sbInscrito.AppendLine();
            }

            if (icv != null)
            {
                sbInscrito.AppendFormat("Opção 2: {0}", icv.icv_idt_opcao_2.HasValue ? icv.icv_idt_opcao_2.Value : 0);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("Opção 3: {0}", icv.icv_idt_opcao_3.HasValue ? icv.icv_idt_opcao_3.Value : 0);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("Masc: {0}", icv.icv_bit_eh_masculino ? "Sim" : "Não");
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_dat_aceito_termos: {0}", icv.icv_dat_aceito_termos.ToString("dd/MM/yyyy HH:mm"));
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_des_conhecimento_unifae_outros: {0}", String.IsNullOrEmpty(icv.icv_des_conhecimento_unifae_outros) ? "Vazio" : icv.icv_des_conhecimento_unifae_outros);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_des_curso_indicado_por: {0}", String.IsNullOrEmpty(icv.icv_des_curso_indicado_por) ? "Vazio" : icv.icv_des_curso_indicado_por);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_des_nome_indicado_por: {0}", String.IsNullOrEmpty(icv.icv_des_nome_indicado_por) ? "Vazio" : icv.icv_des_nome_indicado_por);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_des_semestre_curso_indicado_por: {0}", String.IsNullOrEmpty(icv.icv_des_semestre_curso_indicado_por) ? "Vazio" : icv.icv_des_semestre_curso_indicado_por);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_num_data_prova: {0}", icv.icv_num_data_prova);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_num_atividade_remunerada: {0}", icv.icv_num_atividade_remunerada);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_num_conhecimento_unifae: {0}", icv.icv_num_conhecimento_unifae);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_num_escolaridade_mae: {0}", icv.icv_num_escolaridade_mae);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_num_escolaridade_pai: {0}", icv.icv_num_escolaridade_pai);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_num_local_prova: {0}", icv.icv_num_local_prova);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_num_optar_curso: {0}", icv.icv_num_optar_curso);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_num_optar_unifae: {0}", icv.icv_num_optar_unifae);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_num_renda_mensal: {0}", icv.icv_num_renda_mensal);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_num_tipo_concluiu_ensino_fundamental: {0}", icv.icv_num_tipo_concluiu_ensino_fundamental);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_num_tipo_concluiu_ensino_medio: {0}", icv.icv_num_tipo_concluiu_ensino_medio);
                sbInscrito.AppendLine();
                sbInscrito.AppendFormat("icv_num_tipo_ensino_medio: {0}", icv.icv_num_tipo_ensino_medio);
                sbInscrito.AppendLine();
            }

            Int64 idErro = Convert.ToInt64(BaseBusiness.DataAgora.ToString("yyyyMMddHHmmssfff"));
            tb_ler_log_erro ler = new tb_ler_log_erro();
            ler.ler_dat_erro = BaseBusiness.DataAgora;
            ler.ler_des_inner_exception = null;
            ler.ler_des_mensagem = sbInscrito.ToString();
            ler.ler_des_nome_metodo = "ConcursoController/ConfirmarInscricao";
            ler.ler_des_source = "ConcursoController";
            ler.ler_des_stack_trace = String.Empty;
            ler.ler_des_tipo_exception = String.Empty;
            ler.ler_des_url = String.Empty;
            ler.ler_idt_log_erro = idErro;
            ler.ler_num_codigo_erro = null;
            ler.usu_idt_usuario = null;

            LogErroBusiness.Salvar(ler);
        }

        #endregion [ FIM - Privados ]

        #region [ Upload ]

        [HttpPost]
        public async Task<JsonResult> UploadAnexosConcurso(int idConcurso)
        {
            try
            {
                string diretorio = Server.MapPath(String.Format("~/Anexos/Concurso/{0}", idConcurso));

                if (!Directory.Exists(diretorio))
                    Directory.CreateDirectory(diretorio);

                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var stream = fileContent.InputStream;
                        var fileName = fileContent.FileName;

                        if ((Path.GetExtension(fileContent.FileName).ToLower() != ".pdf" && Path.GetExtension(fileContent.FileName).ToLower() != ".doc" && Path.GetExtension(fileContent.FileName).ToLower() != ".docx") ||
                            fileContent.ContentLength > 4194304) // 4MB
                            return Json(new { Sucesso = false }, JsonRequestBehavior.AllowGet);

                        var path = Path.Combine(diretorio, fileName);
                        using (var fileStream = System.IO.File.Create(path))
                            stream.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Sucesso = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> UploadAnexoRecurso()
        {
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        MemoryStream ms = new MemoryStream();
                        Stream stream = fileContent.InputStream;
                        stream.CopyTo(ms);
                        ms.Position = 0;

                        var fileName = fileContent.FileName;

                        if ((Path.GetExtension(fileContent.FileName).ToLower() != ".pdf" && Path.GetExtension(fileContent.FileName).ToLower() != ".doc" && Path.GetExtension(fileContent.FileName).ToLower() != ".docx") ||
                            fileContent.ContentLength > 2224000)
                            return Json(new { Sucesso = false }, JsonRequestBehavior.AllowGet);

                        Session[SessionArquivoRecursoUpload] = new { Stream = ms, FileName = fileName };
                    }
                }
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Sucesso = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion [ FIM - Upload ]

        #endregion [ FIM - Métodos ]

        #region [ Chamada Por Outros Controllers ]

        /// <summary> Inscrito.js </summary>
        public ActionResult CarregarCargos(int idConcurso)
        {
            List<tb_cco_cargo_concurso> cargos = ConcursoBusiness.ListarCargos(idConcurso);
            List<SelectListItem> cargosLI = cargos.ConvertAll(cco => new SelectListItem() { Text = cco.cco_nom_cargo_concurso, Value = cco.cco_idt_cargo_concurso.ToString() });

            return Json(cargosLI, JsonRequestBehavior.AllowGet);
        }

        #region [ Recursos ]

        public ActionResult EnviarRecurso(int idConcurso, int idInscrito, string mensagem)
        {
            string msg = System.Uri.UnescapeDataString(mensagem).FixStringUnicode();
            msg = msg.Replace("<br>", @"
");

            tb_rec_recurso recurso = new tb_rec_recurso()
            {
                con_idt_concurso = idConcurso,
                ico_idt_inscrito_concurso = idInscrito,
                rec_bit_ativo = true,
                rec_dat_abertura = BaseBusiness.DataAgora,
                rec_des_mensagem = msg,
                sre_idt_status_recurso = (int)RecursoModel.StatusRecurso.Novo,
            };

            recurso = RecursoBusiness.Salvar(recurso);
            int idRecurso = recurso.rec_idt_recurso;

            dynamic anexo = Session[SessionArquivoRecursoUpload];

            if (anexo != null)
            {
                string diretorio = Server.MapPath(String.Format("~/Anexos/Recurso/{0}/{1}", idRecurso, "Requerente"));
                var fileName = anexo.FileName;
                var stream = anexo.Stream;

                SalvarArquivo(diretorio, fileName, stream);
            }

            Session[SessionRecursoAberto] = recurso;

            return Json(idRecurso, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EnviarRecursoEmail()
        {
            bool sucesso = false;
            string protocolo = "";
            if (Session[SessionRecursoAberto] != null)
            {
                tb_rec_recurso recurso = (tb_rec_recurso)Session[SessionRecursoAberto];
                RecursoModel r = new RecursoModel(recurso);
                r.Concurso = new ConcursoModel(recurso.tb_con_concurso);
                r.Inscrito = new ConcursoModel.InscritoModel(recurso.tb_ico_inscrito_concurso);
                protocolo = r.Protocolo;

                try
                {
                    EnviarEmailRecurso(r, "_HtmlAberturaRecurso", TGV.IPEFAE.Web.Resources.Email._HtmlAberturaRecurso.TituloEmail, false);
                }
                catch (Exception ex)
                {
                    int? idUsuario = r != null && r.Inscrito != null && r.Inscrito.Id > 0 ? r.Inscrito.Id : (int?)null;
                    LogErroBusiness.Salvar(ex, idUsuario);
                }

                sucesso = true;
                Session[SessionRecursoAberto] = null;
                Session[SessionArquivoRecursoUpload] = null;
            }

            return Json(new { Sucesso = sucesso, Protocolo = protocolo }, JsonRequestBehavior.AllowGet);
        }

        #endregion [ FIM - Recursos ]

        #endregion [ FIM - Chamada Por Outros Controllers ]

        /*
        public ActionResult ListarAnexos()
        {
            if (Session[SessionConcursosUsuario] == null)
                DefinirSessionConcursosUsuario();

            ConcursosExtension ce = (ConcursosExtension)Session[SessionConcursosUsuario];

            try
            {
                string abertas = BaseController.RenderViewToString(ControllerContext, "~/Views/Concurso/_ListaConcursos.cshtml", ce.Abertas, true);
                string emAndamento = BaseController.RenderViewToString(ControllerContext, "~/Views/Concurso/_ListaConcursos.cshtml", ce.EmAndamento, true);
                string encerradas = BaseController.RenderViewToString(ControllerContext, "~/Views/Concurso/_ListaConcursos.cshtml", ce.Encerradas, true);

                return Json(new { Abertas = abertas, EmAndamento = emAndamento, Encerradas = encerradas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        */
    }
}