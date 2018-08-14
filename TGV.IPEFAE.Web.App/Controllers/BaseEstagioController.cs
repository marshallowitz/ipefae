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
    public class BaseEstagioController : BaseController
    {
        protected const string NomeSessionUsuarioEstagio = "SessionUsuarioEstagio";
        protected const string NomeSessionTotalExperiencias = "SessionTotalExperiencias";
        protected const string NomeSessionFotoUpload = "SessionFotoUpload";

        protected ActionResult BaseIndex()
        {
            Session[NomeSessionUsuarioEstagio] = null;
            Session[NomeSessionTotalExperiencias] = null;
            Session[NomeSessionFotoUpload] = null;
            return View("Index");
        }

        public ActionResult BuscarCursosCapacitacoes(int? idUsuarioEstagio)
        {
            UsuarioEstagioModel ueModel = new UsuarioEstagioModel();
            List<UsuarioEstagioModel.CursosCapacitacoesModel> ccs = new List<UsuarioEstagioModel.CursosCapacitacoesModel>();

            if (idUsuarioEstagio.HasValue && idUsuarioEstagio.Value > 0)
            {
                if (Session[NomeSessionUsuarioEstagio] == null)
                    ueModel = DefinirSessaoUsuarioEstagio(idUsuarioEstagio.Value);
                else
                    ueModel = (UsuarioEstagioModel)Session[NomeSessionUsuarioEstagio];

                ccs = ueModel.CursosCapacitacoes;
            }

            for (int intCtr = ccs.Count; intCtr < 5; intCtr++)
            {
                UsuarioEstagioModel.CursosCapacitacoesModel cc = new UsuarioEstagioModel.CursosCapacitacoesModel();
                cc.Posicao = intCtr + 1;
                cc.Visivel = cc.Posicao == 1; // O primeiro sempre deverá ser visível
                cc.EhCurso = true;
                ccs.Add(cc);
            }

            try
            {
                string view = BaseController.RenderViewToString(ControllerContext, "~/Views/Estagio/_CursosCapacitacoes.cshtml", ccs, true);
                return Json(new { View = view }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult BuscarTotalRelatorios()
        {
            int total = PDFBusiness.ListarTotalGeradosMesCorrente();

            return Json(new { Total = total }, JsonRequestBehavior.AllowGet);
        }

        public List<UsuarioEstagioModel.ExperienciaProfissionalModel> MontarListaExperienciaProfissional(int? idUsuarioEstagio, List<UsuarioEstagioModel.ExperienciaProfissionalModel> eps)
        {
            UsuarioEstagioModel ueModel = new UsuarioEstagioModel();

            if (eps == null)
            {
                eps = new List<UsuarioEstagioModel.ExperienciaProfissionalModel>();

                if (idUsuarioEstagio.HasValue && idUsuarioEstagio.Value > 0)
                {
                    if (Session[NomeSessionUsuarioEstagio] == null)
                        ueModel = DefinirSessaoUsuarioEstagio(idUsuarioEstagio.Value);
                    else
                        ueModel = (UsuarioEstagioModel)Session[NomeSessionUsuarioEstagio];

                    eps = ueModel.ExperienciasProfissionais;
                }
            }

            for (int intCtr = eps.Count; intCtr < 3; intCtr++)
            {
                UsuarioEstagioModel.ExperienciaProfissionalModel ep = new UsuarioEstagioModel.ExperienciaProfissionalModel();
                ep.Posicao = intCtr + 1;
                ep.Visivel = ep.Ativo || ep.Posicao == 1; // O primeiro sempre deverá ser visível
                eps.Add(ep);
            }

            return eps;
        }

        public ActionResult BuscarExperienciaProfissional(int? idUsuarioEstagio)
        {
           List<UsuarioEstagioModel.ExperienciaProfissionalModel> eps = MontarListaExperienciaProfissional(idUsuarioEstagio, null);

            try
            {
                string view = BaseController.RenderViewToString(ControllerContext, "~/Views/Estagio/_ExperienciasProfissionais.cshtml", eps, true);
                return Json(new { View = view }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult BuscarOutrosConhecimentos(int? idUsuarioEstagio)
        {
            UsuarioEstagioModel ueModel = new UsuarioEstagioModel();
            List<UsuarioEstagioModel.CursosCapacitacoesModel> ocs = new List<UsuarioEstagioModel.CursosCapacitacoesModel>();

            if (idUsuarioEstagio.HasValue && idUsuarioEstagio.Value > 0)
            {
                if (Session[NomeSessionUsuarioEstagio] == null)
                    ueModel = DefinirSessaoUsuarioEstagio(idUsuarioEstagio.Value);
                else
                    ueModel = (UsuarioEstagioModel)Session[NomeSessionUsuarioEstagio];

                ocs = ueModel.OutrosConhecimentos;
            }

            for (int intCtr = ocs.Count; intCtr < 5; intCtr++)
            {
                UsuarioEstagioModel.CursosCapacitacoesModel oc = new UsuarioEstagioModel.CursosCapacitacoesModel();
                oc.Posicao = intCtr + 1;
                oc.Visivel = oc.Posicao == 1; // O primeiro sempre deverá ser visível
                oc.EhCurso = false;
                ocs.Add(oc);
            }

            try
            {
                string view = BaseController.RenderViewToString(ControllerContext, "~/Views/Estagio/_OutrosConhecimentos.cshtml", ocs, true);
                return Json(new { View = view }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult BaseCadastro(int? id)
        {
            UsuarioEstagioModel ueModel = ObterEstagiario(id);
            return View("Cadastro", ueModel);
        }

        public ActionResult GerarPDF(int id)
        {
            int idt = id > 0 ? id : 0;

            if (UsuarioLogado == null || (UsuarioLogado.Id != idt && !UsuarioLogado.IsEstagio))
                idt = 0;

            UsuarioEstagioModel ueModel = new UsuarioEstagioModel();

            if (idt == 0)
                return RedirecionarPagina("UnauthorizedAccess", "Error", "", 401);

            ueModel = DefinirSessaoUsuarioEstagio(idt);

            // Atualiza a 5 chave
            PDFBusiness.Atualizar5Chave();

            return GerarPDFCurriculo(ueModel);
        }

        public static ViewAsPdf GerarPDFCurriculo(UsuarioEstagioModel ueModel)
        {
            try
            {
                ViewAsPdf vpdf = new ViewAsPdf(ueModel);

                return vpdf;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected UsuarioEstagioModel ObterEstagiario(int? id)
        {
            Session[NomeSessionUsuarioEstagio] = null;
            Session[NomeSessionTotalExperiencias] = null;
            Session[NomeSessionFotoUpload] = null;
            UsuarioEstagioModel ueModel = new UsuarioEstagioModel();

            if (id.HasValue)
                ueModel = DefinirSessaoUsuarioEstagio(id.Value);
            else
            {
                List<UsuarioEstagioModel.ExperienciaProfissionalModel> eps = MontarListaExperienciaProfissional(null, null);
                ueModel.ExperienciasProfissionais = eps;
            }

            return ueModel;
        }

        public ActionResult PreviewCurriculo(int id)
        {
            int idt = id > 0 ? id : 0;

            if (UsuarioLogado == null || (UsuarioLogado.Id != idt && !UsuarioLogado.IsEstagio))
                idt = 0;

            UsuarioEstagioModel ueModel = new UsuarioEstagioModel();

            if (idt == 0)
                return RedirecionarPagina("UnauthorizedAccess", "Error", "", 401);

            ueModel = DefinirSessaoUsuarioEstagio(idt);

            return View(ueModel);

            //try
            //{
            //    string view = BaseController.RenderViewToString(ControllerContext, "~/Views/Estagio/PreviewCurriculo.cshtml", ueModel, true);
            //    return Json(new { View = view }, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        public ActionResult ValidarEmailExistente(int id, string email)
        {
            tb_ues_usuario_estagio ues = UsuarioEstagioBusiness.ObterPorEmail(email);

            bool retorno = (ues != null && ues.ues_idt_usuario_estagio != id);

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        protected ActionResult Salvar(int id, string nome, string email, string senha, string cpf, string rg, DateTime dataExpedicao, string nroCarteira,
            string serieCarteira, int ufCarteira, string estadoCivil, DateTime dataNasc, string telefone01, string telefone02, string endereco, int nroEndereco,
            string complemento, string bairro, string cep, int idCidade, int qtdadeFilhos, bool possuiDef, string defQual, string objetivos, int idDadosEscolares, string tipo_ensino,
            string nomeEscola, string de01, string de02, string periodo, string nomeCursoEscola, string dataInicioDE, string dataTerminoDE, bool possuiExp,
            string listaExp, string listaCursos, string listaOutros, bool ehMasc, bool temFoto, bool ehAdmin, bool considerar_busca, bool estagiando, bool ativo,
            string motivoDesativacao, string observacoesAdmin)
        {
            DateTime dataCriacaoCV = BaseBusiness.DataAgora;
            string password = senha;
            int anoInicio = 0;
            int anoTermino = 0;
            int mesInicio = 0;
            int mesTermino = 0;
            int ano_semestre = 0;
            string tipo_profissionalizante = null;
            bool ead = false;

            switch (tipo_ensino)
            {
                case "M":
                    ano_semestre = Convert.ToInt32(de01);
                    break;
                case "S":
                    ano_semestre = Convert.ToInt32(de01);
                    ead = String.Equals(de02, "e", StringComparison.InvariantCultureIgnoreCase);
                    break;
                case "T":
                case "E":
                    tipo_profissionalizante = de01;
                    ano_semestre = Convert.ToInt32(de02);
                    break;
                default:
                    break;
            }

            anoInicio = Convert.ToInt32(dataInicioDE.Substring(3, 4));
            mesInicio = Convert.ToInt32(dataInicioDE.Substring(0, 2));
            anoTermino = Convert.ToInt32(dataTerminoDE.Substring(3, 4));
            mesTermino = Convert.ToInt32(dataTerminoDE.Substring(0, 2));

            DateTime dataAtualizacao = BaseBusiness.DataAgora;

            if (id <= 0)
                id = 0;
            else
            {
                UsuarioEstagioModel ueModel = DefinirSessaoUsuarioEstagio(id);
                dataCriacaoCV = ueModel.DataCadastro;
                password = ueModel.SenhaDescriptografada;

                // Se for o administrativo que está alterando, mantem a data do usuário
                if (ehAdmin)
                    dataAtualizacao = ueModel.DataUltimaAtualizacao;
            }

            tb_ued_usuario_estagio_dados_escolares ued = new tb_ued_usuario_estagio_dados_escolares()
            {
                ued_bit_ativo = true,
                ued_bit_ead = ead,
                ued_des_nome_curso = nomeCursoEscola,
                ued_des_nome_escola = nomeEscola,
                ued_flg_periodo = periodo,
                ued_flg_tipo_dados_escolares = tipo_ensino,
                ued_flg_tipo_profissionalizante = tipo_profissionalizante,
                ued_idt_usuario_estagio_dados_escolares = idDadosEscolares,
                ued_num_ano_inicio = anoInicio,
                ued_num_mes_inicio = mesInicio,
                ued_num_ano_termino = anoTermino,
                ued_num_mes_termino = mesTermino,
                ued_num_ano_semestre = ano_semestre
            };

            tb_ues_usuario_estagio ues = new tb_ues_usuario_estagio()
            {
                cid_idt_cidade_endereco = idCidade,
                est_idt_estado_carteira_trabalho = ufCarteira,
                ues_bit_ativo = ativo,
                ues_bit_considerar_busca = considerar_busca,
                ues_bit_estagiando = estagiando,
                ues_bit_masculino = ehMasc,
                ues_bit_possui_deficiencia = possuiDef,
                ues_bit_possui_experiencia_profissional = possuiExp,
                ues_bit_tem_foto = temFoto,
                ues_dat_criacao_cv = dataCriacaoCV,
                ues_dat_nascimento = dataNasc,
                ues_dat_rg_expedicao = dataExpedicao,
                ues_dat_ultima_atualizacao = dataAtualizacao,
                ues_des_bairro = bairro,
                ues_des_carteira_trabalho_numero = nroCarteira,
                ues_des_carteira_trabalho_serie = serieCarteira,
                ues_des_celular = telefone02,
                ues_des_cep = cep,
                ues_des_complemento = complemento,
                ues_des_cpf = cpf,
                ues_des_email = email,
                ues_des_endereco = endereco,
                ues_des_numero_endereco = nroEndereco,
                ues_des_objetivos = objetivos,
                ues_des_observacoes_admin = observacoesAdmin,
                ues_des_rg = rg,
                ues_des_senha = password,
                ues_des_telefone = telefone01,
                ues_flg_deficiencia = defQual,
                ues_flg_estado_civil = estadoCivil,
                ues_flg_motivo_desativacao = motivoDesativacao,
                ues_idt_usuario_estagio = id,
                ues_nom_usuario_estagio = nome,
                ues_num_quantidade_filhos = qtdadeFilhos
            };

            List<tb_uee_usuario_estagio_experiencia> exps = MontarExperienciaProfissional(listaExp);
            List<tb_ueo_usuario_estagio_outros> ccs = MontaCursosComplementaresEOutros(listaCursos, true);
            List<tb_ueo_usuario_estagio_outros> ocs = MontaCursosComplementaresEOutros(listaOutros, false);

            ues = UsuarioEstagioBusiness.Salvar(ues, ued, exps, ccs, ocs, ehAdmin);

            if (ues == null)
                return Json(new { Status = false }, JsonRequestBehavior.AllowGet);

            // Verifica se tem foto para salvar (ou apagar)
            if (ues.ues_bit_tem_foto)
            {
                // Verifica se a session tem informação
                if (Session[NomeSessionFotoUpload] != null)
                {
                    dynamic foto = Session[NomeSessionFotoUpload];
                    string pathFolder = Server.MapPath("~/Anexos/Estagio/");

                    using (MemoryStream ms = foto.Stream)
                    {
                        // Verifica se o diretório existe
                        if (!Directory.Exists(pathFolder))
                            Directory.CreateDirectory(pathFolder);

                        using (FileStream fs = new FileStream(String.Format("{1}{0}.jpg", ues.ues_idt_usuario_estagio, pathFolder), FileMode.Create, FileAccess.Write))
                        {
                            ms.WriteTo(fs);
                        }
                    }
                }
            }
            else
            {
                // Apaga o arquivo
                FileInfo fi = new FileInfo(Server.MapPath(String.Format("~/Anexos/Estagio/{0}.jpg", ues.ues_idt_usuario_estagio)));

                if (fi.Exists)
                    fi.Delete();
            }

            // Limpa a sessão de foto
            Session[NomeSessionFotoUpload] = null;

            return Json(new { Status = true, Id = ues.ues_idt_usuario_estagio, IdDadosEscolares = ues.ued_idt_usuario_estagio_dados_escolares }, JsonRequestBehavior.AllowGet);
        }


        protected UsuarioEstagioModel DefinirSessaoUsuarioEstagio(int idUsuarioEstagio)
        {
            UsuarioEstagioModel ueModel = new UsuarioEstagioModel();
            ueModel = new UsuarioEstagioModel(UsuarioEstagioBusiness.Obter(idUsuarioEstagio));
            List<UsuarioEstagioModel.ExperienciaProfissionalModel> eps = MontarListaExperienciaProfissional(idUsuarioEstagio, ueModel.ExperienciasProfissionais);
            ueModel.ExperienciasProfissionais = eps;

            Session[NomeSessionUsuarioEstagio] = ueModel;

            return ueModel;
        }

        protected List<tb_uee_usuario_estagio_experiencia> MontarExperienciaProfissional(string listaExp)
        {
            List<tb_uee_usuario_estagio_experiencia> exps = new List<tb_uee_usuario_estagio_experiencia>();

            string[] stringSeparators = new string[] {"||"};
            string[] result = listaExp.Split(stringSeparators, StringSplitOptions.None);
            int id = 0;
            int mes = 0;
            int ano = 0;

            foreach (string item in result)
            {
                string[] campos = item.Split(',');

                if (campos.Length == 1)
                    continue;

                tb_uee_usuario_estagio_experiencia exp = new tb_uee_usuario_estagio_experiencia();

                foreach (string campo in campos)
                {
                    string[] keyvaluepair = campo.Split(':');

                    if (keyvaluepair.Length <= 1 || (keyvaluepair[0].Equals("nomeEmpresa", StringComparison.InvariantCultureIgnoreCase) && String.IsNullOrEmpty(keyvaluepair[1])))
                        break;

                    switch (keyvaluepair[0].ToLower())
                    {
                        case "id":
                            Int32.TryParse(keyvaluepair[1], out id);
                            exp.uee_idt_usuario_estagio_experiencia = id;
                            break;
                        case "nomeempresa":
                            exp.uee_des_nome_empresa = keyvaluepair[1];
                            break;
                        case "cargo":
                            exp.uee_des_cargo = keyvaluepair[1];
                            break;
                        case "atividadesdesenvolvidas":
                            exp.uee_des_atividades_desenvolvidas = Server.UrlDecode(keyvaluepair[1]);
                            break;
                        case "datainicio":
                            string[] data = keyvaluepair[1].Split('/');
                            Int32.TryParse(data[0], out mes);
                            Int32.TryParse(data[1], out ano);
                            exp.uee_num_mes_inicio = mes;
                            exp.uee_num_ano_inicio = ano;
                            break;
                        case "datatermino":
                            if (!String.IsNullOrEmpty(keyvaluepair[1]))
                            {
                                string[] dataT = keyvaluepair[1].Split('/');
                                Int32.TryParse(dataT[0], out mes);
                                Int32.TryParse(dataT[1], out ano);
                                exp.uee_num_mes_termino = mes;
                                exp.uee_num_ano_termino = ano;
                            }
                            break;
                        default:
                            break;
                    }
                }

                exp.uee_bit_ativo = (!String.IsNullOrEmpty(exp.uee_des_nome_empresa) && !exp.uee_des_nome_empresa.Equals("undefined"));
                exps.Add(exp);
            }

            return exps;
        }

        protected List<tb_ueo_usuario_estagio_outros> MontaCursosComplementaresEOutros(string listaCursos, bool ehCurso)
        {
            List<tb_ueo_usuario_estagio_outros> ueos = new List<tb_ueo_usuario_estagio_outros>();

            string[] stringSeparators = new string[] { "||" };
            string[] result = listaCursos.Split(stringSeparators, StringSplitOptions.None);
            int id = 0;

            foreach (string item in result)
            {
                string[] campos = item.Split(',');

                if (campos.Length == 1)
                    continue;

                tb_ueo_usuario_estagio_outros ueo = new tb_ueo_usuario_estagio_outros();
                ueo.ueo_bit_curso = ehCurso;

                foreach (string campo in campos)
                {
                    string[] keyvaluepair = campo.Split(':');

                    if (keyvaluepair.Length <= 1 || (keyvaluepair[0].Equals("nomeCurso", StringComparison.InvariantCultureIgnoreCase) && String.IsNullOrEmpty(keyvaluepair[1])))
                        break;

                    switch (keyvaluepair[0].ToLower())
                    {
                        case "id":
                            Int32.TryParse(keyvaluepair[1], out id);
                            ueo.ueo_idt_usuario_estagio_outros = id;
                            break;
                        case "nomecurso":
                            ueo.ueo_nom_usuario_estagio_outros = keyvaluepair[1];
                            break;
                        case "duracao":
                            ueo.ueo_des_duracao = keyvaluepair[1];
                            break;
                        default:
                            break;
                    }
                }

                ueo.ueo_bit_ativo = (!String.IsNullOrEmpty(ueo.ueo_nom_usuario_estagio_outros) && !ueo.ueo_nom_usuario_estagio_outros.Equals("undefined"));
                ueos.Add(ueo);
            }

            return ueos;
        }

        [HttpPost]
        public async Task<JsonResult> UploadFotoCurriculo()
        {
            string pathFoto = String.Empty;

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

                        if (fileContent.ContentLength > 2224000)
                            return Json(new { Sucesso = false, PathFoto = "" }, JsonRequestBehavior.AllowGet);

                        Session[NomeSessionFotoUpload] = new { Stream = ms, FileName = fileName };

                        pathFoto = Convert.ToBase64String(ms.ReadToEnd());
                    }
                }
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Sucesso = false, PathFoto = "" }, JsonRequestBehavior.AllowGet);
            }

            JsonResult result = Json(new { Sucesso = true, PathFoto = String.Format("data:image/jpg;base64,{0}", pathFoto) }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
	}
}