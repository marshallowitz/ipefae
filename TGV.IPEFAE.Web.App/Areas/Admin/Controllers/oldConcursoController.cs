using BoletoNet;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Controllers;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Areas.Admin.Controllers
{
    [IPEFAEAuthorizationAttribute(PermissaoModel.Tipo.Concurso)]
    public class oldConcursoController : BaseController
    {
        //private const string SessionArquivoRemessaUpload = "ArquivoRemessaUpload";
        //private const string SessionArquivoConcursoUpload = "ArquivoConcursoUpload";

        //#region [ Concursos ]

        //public ActionResult Index()
        //{
        //    // Verifica se existem concursos antigos e apaga
        //    int diasExclusaoConcursos = 0;

        //    if (Int32.TryParse(ConfigurationManager.AppSettings["DiasExclusaoConcursos"], out diasExclusaoConcursos))
        //    {
        //        string diretorioBase = Server.MapPath("~/Anexos/");
        //        ConcursoBusiness.ApagarConcursosAntigos(diasExclusaoConcursos, diretorioBase);
        //    }

        //    Session[SessionArquivoRemessaUpload] = null;
        //    ConcursoSelecionado = null;
        //    TodosConcursos = null;
        //    return View();
        //}

        //public ActionResult AdicionarAnexo(int? idAnexo, int idConcurso, int idTipo)
        //{
        //    DateTime dataPublicacao = BaseBusiness.DataAgora;
        //    oldConcursoModel.AnexoModel anexo = new oldConcursoModel.AnexoModel(idConcurso, idTipo, dataPublicacao);

        //    if (idAnexo.HasValue && idAnexo.Value != 0)
        //        anexo = ConcursoSelecionado.Anexos.SingleOrDefault(can => can.Id == idAnexo.Value);

        //    try
        //    {
        //        string view = BaseController.RenderViewToString(ControllerContext, "~/Areas/Admin/Views/Concurso/_ModalAnexosConcurso.cshtml", anexo, true);
        //        return Json(new { View = view }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public ActionResult AdicionarCargo(int? idCargo, int idConcurso)
        //{
        //    if (ConcursoSelecionado == null)
        //        ConcursoSelecionado = new oldConcursoModel(ConcursoBusiness.Obter(idConcurso));

        //    oldConcursoModel.CargoModel cargo = new oldConcursoModel.CargoModel();

        //    if (idCargo.HasValue && idCargo.Value != 0)
        //        cargo = ConcursoSelecionado.Cargos.SingleOrDefault(cco => cco.Id == idCargo.Value);

        //    try
        //    {
        //        string view = BaseController.RenderViewToString(ControllerContext, "~/Areas/Admin/Views/Concurso/_ModalCargosConcurso.cshtml", cargo, true);
        //        return Json(new { View = view }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public ActionResult AtivarCargo(int idCargo, int idConcurso)
        //{
        //    if (ConcursoSelecionado == null)
        //        ConcursoSelecionado = new oldConcursoModel(ConcursoBusiness.Obter(idConcurso));

        //    oldConcursoModel.CargoModel cargo = new oldConcursoModel.CargoModel();

        //    if (idCargo != 0)
        //        cargo = ConcursoSelecionado.Cargos.SingleOrDefault(cco => cco.Id == idCargo);

        //    cargo.Ativo = !cargo.Ativo;

        //    return Json(new { Sucesso = true }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Cadastro(int? id)
        //{
        //    oldConcursoModel concurso = new oldConcursoModel();

        //    if (id.HasValue && id.Value > 0)
        //        concurso = new oldConcursoModel(ConcursoBusiness.Obter(id.Value));

        //    ConcursoSelecionado = concurso;

        //    return View(concurso);
        //}

        //public ActionResult CarregarAlertaRecurso(int idConcurso)
        //{
        //    if (ConcursoSelecionado == null)
        //        ConcursoSelecionado = new oldConcursoModel(ConcursoBusiness.Obter(idConcurso));

        //    try
        //    {
        //        string view = BaseController.RenderViewToString(ControllerContext, "~/Areas/Admin/Views/Concurso/_AlertaRecurso.cshtml", ConcursoSelecionado, true);
        //        return Json(new { View = view }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public ActionResult ExcluirAnexo(int idAnexo, int idConcurso, int idTipo)
        //{
        //    if (ConcursoSelecionado == null)
        //        ConcursoSelecionado = new oldConcursoModel(ConcursoBusiness.Obter(idConcurso));

        //    ConcursoSelecionado.Anexos.RemoveAll(can => can.Id == idAnexo);
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult ExcluirCargo(int idCargo, int idConcurso)
        //{
        //    if (ConcursoSelecionado == null)
        //        ConcursoSelecionado = new oldConcursoModel(ConcursoBusiness.Obter(idConcurso));

        //    ConcursoSelecionado.Cargos.RemoveAll(cco => cco.Id == idCargo);
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult ExcluirConcurso(int id)
        //{
        //    string diretorioBase = Server.MapPath("~/Anexos/");
        //    ConcursoBusiness.Excluir(id, diretorioBase);
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult ListarConcursos()
        //{
        //    List<oldConcursoModel> concursos = ConcursoBusiness.Listar().ConvertAll(con => new oldConcursoModel(con));
        //    TodosConcursos = concursos;
        //    return PartialView("_ListaConcursos", concursos);
        //}

        //private bool ProcessarArquivoConcurso(string pathFile)
        //{
        //    int intCtr = 0;
        //    Dictionary<int, string> dicColunas = new Dictionary<int, string>();
        //    List<tb_est_estado> estados = EstadoBusiness.Listar();
        //    List < tb_ico_inscrito_concurso> inscritos = new List<tb_ico_inscrito_concurso>();
        //    List<tb_idt_inscrito_dados_prova> dadosProva = new List<tb_idt_inscrito_dados_prova>();
        //    List<tb_icl_inscrito_classificacao> classificacoes = new List<tb_icl_inscrito_classificacao>();

        //    using (StreamReader sr = new StreamReader(pathFile, Encoding.GetEncoding("iso-8859-15")))
        //    {
        //        while(!sr.EndOfStream)
        //        {
        //            string linha = sr.ReadLine();
                 
        //            var values = linha.Split(';');

        //            if (intCtr == 0) // Monta o cabeçalho
        //            {
        //                int intCtrCol = 0;

        //                foreach (string col in values)
        //                {
        //                    string nomeCampo = this.ObterCampoTabela(col);
        //                    dicColunas.Add(intCtrCol++, nomeCampo);
        //                }

        //                // Verifica se existe a coluna de inscrição. Se não houver, termina o processo
        //                if (!dicColunas.ContainsValue("ico_idt_inscrito_concurso"))
        //                    break;

        //                intCtr++;
        //                continue;
        //            }

        //            // Cria os inscritos, dados da prova e classificacao
        //            inscritos.AdicionarDadosInscrito(estados, values, dicColunas);
        //            dadosProva.AdicionarDadosProva(values, dicColunas);
        //            classificacoes.AdicionarClassificacao(values, dicColunas);
        //        }
        //    }

        //    ConcursoBusiness.Salvar(inscritos, dadosProva, classificacoes);

        //    return true;
        //}

        //private string ObterCampoTabela(string coluna)
        //{
        //    switch (coluna.ToLower())
	       // {
        //        case "inscrição":
        //            return "ico_idt_inscrito_concurso";
        //        case "inscritonome":
        //            return "ico_nom_inscrito_concurso";
        //        case "inscritonasc":
        //            return "ico_dat_nascimento";
        //        case "inscritorg":
        //            return "ico_des_rg";
        //        case "inscritoestadorg":
        //            return "est_idt_estado_rg";
        //        case "provadata":
        //            return "idt_dat_prova";
        //        case "provahorario":
        //            return "idt_dat_prova_hor";
        //        case "provalocal":
        //            return "idt_des_local";
        //        case "provasala":
        //            return "idt_des_sala";
        //        case "provaandar":
        //            return "idt_des_andar";
        //        case "provaendereco":
        //            return "idt_des_endereco";
        //        case "provanumero":
        //            return "idt_des_numero";
        //        case "provabairro":
        //            return "idt_des_bairro";
        //        case "provacidade":
        //            return "idt_des_cidade";
        //        case "provacep":
        //            return "idt_des_cep";
        //        case "classposicao":
        //            return "icl_num_posicao";
        //        case "classnota":
        //            return "icl_num_nota";
        //        case "classcg":
        //            return "icl_num_cg";
        //        case "classce":
        //            return "icl_num_ce";
        //        case "classtit":
        //            return "icl_num_tit";
        //        case "classpp":
        //            return "icl_num_pp";
        //        case "classsituacao":
        //            return "icl_des_situacao";
	       // }

        //    return String.Empty;
        //}

        //#region [ Tratar Arquivo Remessa/Retorno ]

        //private bool ProcessarArquivoRemessa(string pathFile, string originalFileName)
        //{
        //    string inicioDetalhe = "7";
        //    int tamanhoNossoNro = 10;
        //    int linhaNro = 0;
        //    var novosPagos = new List<dynamic>();

        //    foreach (string linha in System.IO.File.ReadLines(pathFile))
        //    {
        //        linhaNro++;

        //        if (linhaNro == 1)
        //        {
        //            if (linha.Substring(0, 3) == "001")
        //                return ProcessarArquivoRemessaBB(pathFile, originalFileName);

        //            inicioDetalhe = (!String.IsNullOrEmpty(linha) && linha.Contains("BRADESCO")) ? "1" : "7";
        //            tamanhoNossoNro = (!String.IsNullOrEmpty(linha) && linha.Contains("BRADESCO")) ? 11 : 10;
        //        }

        //        // Se a linha estiver em branco ou não for do tipo "7" - detalhe: ignora
        //        if (String.IsNullOrEmpty(linha) || linha.Substring(0, 1) != inicioDetalhe || linha.Length < 400)
        //            continue;

        //        string nossoNumeroString = linha.Substring(70, tamanhoNossoNro); // Obtém o nosso número
        //        //string valorPagoString = linha.Substring(153, 12); // Obtém o valor pago
        //        string valorPagoString = linha.Substring(253, 13); // Obtém o valor pago
        //        //string dataPagamentoString = linha.Substring(110, 6); // Obtém a data de pagamento
        //        string dataPagamentoString = linha.Substring(295, 6); // Obtém a data de pagamento DDMMYY
        //        int nossoNumero = 0;
        //        decimal valorPago = 0;
        //        DateTime dataPagamento = DateTime.MinValue;

        //        if (!Int32.TryParse(nossoNumeroString, out nossoNumero))
        //            continue;

        //        if (!Decimal.TryParse(valorPagoString, out valorPago) || valorPago == 0) // Se não achar o valor ou ele for zerado
        //            continue;

        //        if (!DateTime.TryParseExact(dataPagamentoString, "ddMMyy", new System.Globalization.CultureInfo("pt-BR"), System.Globalization.DateTimeStyles.None, out dataPagamento))
        //            continue;

        //        valorPago = valorPago / 100;

        //        novosPagos.Add(new { NossoNumeroString = nossoNumeroString, NossoNumero = nossoNumero, ValorPagoString = valorPagoString, ValorPago = valorPago, DataPagamentoString = dataPagamentoString, DataPagamento = dataPagamento });
        //    }

        //    return GravarBDArquivosRemessa(novosPagos);
        //}

        //private bool ProcessarArquivoRemessaBB(string pathFile, string originalFileName)
        //{
        //    string inicioDetalhe = "06";
        //    int tamanhoNossoNro = 10;
        //    int linhaNro = 0;
        //    var novosPagos = new List<dynamic>();
        //    string dataPagamentoString = Path.GetFileNameWithoutExtension(originalFileName).Replace("-bb", "");

        //    foreach (string linha in System.IO.File.ReadLines(pathFile))
        //    {
        //        linhaNro++;

        //        // Se a linha estiver em branco ou não for do tipo "06" - detalhe: ignora
        //        if (String.IsNullOrEmpty(linha) || linha.Substring(13, 1) != "T" || linha.Substring(15, 2) != inicioDetalhe || linha.Length < 240)
        //            continue;

        //        string nossoNumeroString = linha.Substring(45, tamanhoNossoNro); // Obtém o nosso número
        //        string valorPagoString = linha.Substring(82, 14); // Obtém o valor pago
        //        int nossoNumero = 0;
        //        decimal valorPago = 0;
        //        DateTime dataPagamento = DateTime.MinValue;

        //        if (!Int32.TryParse(nossoNumeroString, out nossoNumero))
        //            continue;

        //        if (!Decimal.TryParse(valorPagoString, out valorPago))
        //            continue;

        //        if (!DateTime.TryParseExact(dataPagamentoString, "yyyy_MM_dd", new System.Globalization.CultureInfo("pt-BR"), System.Globalization.DateTimeStyles.None, out dataPagamento) &&
        //            !DateTime.TryParseExact(dataPagamentoString, "yyyy-MM-dd", new System.Globalization.CultureInfo("pt-BR"), System.Globalization.DateTimeStyles.None, out dataPagamento) &&
        //            !DateTime.TryParseExact(dataPagamentoString, "dd-MM-yyyy", new System.Globalization.CultureInfo("pt-BR"), System.Globalization.DateTimeStyles.None, out dataPagamento))
        //            continue;

        //        valorPago = valorPago / 100;

        //        novosPagos.Add(new { NossoNumeroString = nossoNumeroString, NossoNumero = nossoNumero, ValorPagoString = valorPagoString, ValorPago = valorPago, DataPagamentoString = dataPagamentoString, DataPagamento = dataPagamento });
        //    }

        //    return GravarBDArquivosRemessa(novosPagos);
        //}

        //private bool ProcessarArquivoRemessaCSV(string pathFile, string originalFileName)
        //{
        //    var novosPagos = new List<dynamic>();

        //    using (CsvReader csv = new CsvReader(System.IO.File.OpenText(pathFile)))
        //    {
        //        csv.Configuration.RegisterClassMap<CSVRetornoBradescoMap>();
        //        csv.Configuration.HasHeaderRecord = true;
        //        csv.Configuration.Delimiter = ";";

        //        while (csv.Read())
        //        {
        //            var record = csv.GetRecord<CSVRetornoBradesco>();
        //            int nossoNumero = 0;
        //            decimal valorPago = 0;
        //            DateTime dtPagamento = DateTime.MinValue;

        //            if (!Int32.TryParse(record.NroPedido, out nossoNumero))
        //                continue;

        //            if (!Decimal.TryParse(record.ValorPagamento, out valorPago))
        //                continue;

        //            if (record.ValorPagamento.Contains("."))
        //                valorPago /= 100;

        //            // Se não tiver a data do pagamento, ignora
        //            if (!DateTime.TryParseExact(record.DataPagamento, "dd/MM/yyyy HH:mm:ss", new System.Globalization.CultureInfo("pt-BR"), System.Globalization.DateTimeStyles.None, out dtPagamento) &&
        //                !DateTime.TryParseExact(record.DataPagamento, "dd/MM/yyyy HH:mm", new System.Globalization.CultureInfo("pt-BR"), System.Globalization.DateTimeStyles.None, out dtPagamento) &&
        //                !DateTime.TryParseExact(record.DataPagamento, "dd/MM/yyyy  HH:mm:ss", new System.Globalization.CultureInfo("pt-BR"), System.Globalization.DateTimeStyles.None, out dtPagamento) &&
        //                !DateTime.TryParseExact(record.DataPagamento, "dd/MM/yyyy  HH:mm", new System.Globalization.CultureInfo("pt-BR"), System.Globalization.DateTimeStyles.None, out dtPagamento))
        //                continue;

        //            novosPagos.Add(new { NossoNumeroString = record.NroPedido, NossoNumero = nossoNumero, ValorPagoString = record.ValorPagamento, ValorPago = valorPago, DataPagamentoString = record.DataPagamento, DataPagamento = dtPagamento });
        //        }
        //    }

        //    return GravarBDArquivosRemessa(novosPagos);
        //}

        //private bool GravarBDArquivosRemessa(List<dynamic> novosPagos)
        //{
        //    foreach (var item in novosPagos)
        //    {
        //        // Busca o inscrito a partir do Nosso Numero
        //        tb_ico_inscrito_concurso ico = InscritoConcursoBusiness.Obter(item.NossoNumero);

        //        if (ico == null)
        //            continue;

        //        // Caso ele exista, valida se o valor pago é igual ou maior do que o da inscricao
        //        tb_cco_cargo_concurso cco = ConcursoBusiness.ObterCargoInscritoCandidato(ico.ico_idt_inscrito_concurso);

        //        if (cco == null)
        //            continue;

        //        decimal valorInscricao = cco.cco_num_valor_inscricao.HasValue ? cco.cco_num_valor_inscricao.Value : 0;

        //        if (item.ValorPago < valorInscricao)
        //            continue;

        //        // Atualiza os dados do inscrito
        //        ico.ico_num_valor_pago = item.ValorPago;
        //        ico.ico_dat_pagamento = item.DataPagamento;
        //        ico.ico_bit_pago = true;

        //        InscritoConcursoBusiness.Salvar(ico);
        //    }

        //    return novosPagos.Count > 0;
        //}

        //#endregion [ FIM - Tratar Arquivo Remessa/Retorno ]

        //public ActionResult RealizarUploadArquivoConcurso()
        //{
        //    dynamic concurso = Session[SessionArquivoConcursoUpload];
        //    bool sucesso = false;

        //    if (concurso != null)
        //    {
        //        string diretorio = Server.MapPath("~/Anexos/DadosConcurso/");
        //        var fileName = String.Format("{0}_{1}", BaseBusiness.DataAgora.ToString("yyyyMMddHHmmss"), concurso.FileName);
        //        var stream = concurso.Stream;

        //        // Salvar o arquivo no diretorio
        //        SalvarArquivo(diretorio, fileName, stream);

        //        // Processa o arquivo
        //        sucesso = ProcessarArquivoConcurso(Path.Combine(diretorio, fileName));
        //    }

        //    return Json(new { Sucesso = sucesso }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult RealizarUploadArquivoRemessa()
        //{
        //    dynamic remessa = Session[SessionArquivoRemessaUpload];
        //    bool sucesso = false;

        //    if (remessa != null)
        //    {
        //        string diretorio = Server.MapPath("~/Anexos/Remessa/");
        //        var fileName = String.Format("{0}_{1}", BaseBusiness.DataAgora.ToString("yyyyMMddHHmmss"), Path.GetFileName(remessa.FileName));
        //        var stream = remessa.Stream;

        //        // Salvar o arquivo no diretorio
        //        SalvarArquivo(diretorio, fileName, stream);

        //        string extensao = Path.GetExtension(fileName);

        //        // Processa o arquivo
        //        if (extensao.Equals(".CSV", StringComparison.InvariantCultureIgnoreCase))
        //            sucesso = ProcessarArquivoRemessaCSV(Path.Combine(diretorio, fileName), remessa.FileName);
        //        else
        //            sucesso = ProcessarArquivoRemessa(Path.Combine(diretorio, fileName), remessa.FileName);
        //    }

        //    return Json(new { Sucesso = sucesso }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult RecarregarTabelaAnexo(int idConcurso, int idTipo)
        //{
        //    if (ConcursoSelecionado == null)
        //        ConcursoSelecionado = new oldConcursoModel(ConcursoBusiness.Obter(idConcurso));

        //    int idTipoPublicacao = (int)oldConcursoModel.AnexoModel.TipoEnum.Publicacoes;

        //    List<oldConcursoModel.AnexoModel> anexos = idTipo == idTipoPublicacao ? 
        //                                                            ConcursoSelecionado.Anexos.Where(ane => ane.IdTipo == idTipo).ToList() :
        //                                                            ConcursoSelecionado.Anexos.Where(ane => ane.IdTipo != idTipoPublicacao).ToList();

        //    try
        //    {
        //        string view = BaseController.RenderViewToString(ControllerContext, "~/Areas/Admin/Views/Concurso/_ListaAnexosConcurso.cshtml", anexos, true);

        //        return Json(new { View = view }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public ActionResult RecarregarTabelaCargo(int idConcurso)
        //{
        //    if (ConcursoSelecionado == null)
        //        ConcursoSelecionado = new oldConcursoModel(ConcursoBusiness.Obter(idConcurso));

        //    List<oldConcursoModel.CargoModel> cargos = ConcursoSelecionado.Cargos.ToList();

        //    try
        //    {
        //        string view = BaseController.RenderViewToString(ControllerContext, "~/Areas/Admin/Views/Concurso/_ListaCargosDisponiveis.cshtml", cargos, true);
        //        return Json(new { View = view }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public ActionResult Salvar(int id, int idTipoLayout, int emp_idt_empresa, string nome, string dataInicio, string dataEncerramentoInscricoes, string dataEncerramento, string dataInicioComprovante, string dataEncerramentoComprovante, string dataInicioClassificacao, string dataInicioIsento, string dataEncerramentoIsento, string dataBoleto, bool encerrado, bool inscricaoOnline, bool ativo)
        //{
        //    if (ConcursoSelecionado == null)
        //        ConcursoSelecionado = new oldConcursoModel(ConcursoBusiness.Obter(id));

        //    DateTime dtInicio = DateTime.ParseExact(dataInicio, "dd/MM/yyyy HH:mm", null);
        //    DateTime datEncerramentoInscricoes = DateTime.ParseExact(dataEncerramentoInscricoes, "dd/MM/yyyy HH:mm", null);
        //    DateTime? dtEncerramento = String.IsNullOrEmpty(dataEncerramento) ? (DateTime?)null : DateTime.ParseExact(dataEncerramento, "dd/MM/yyyy HH:mm", null);
        //    DateTime dtInicioComprovante = DateTime.ParseExact(dataInicioComprovante, "dd/MM/yyyy HH:mm", null);
        //    DateTime datEncerramentoComprovante = DateTime.ParseExact(dataEncerramentoComprovante, "dd/MM/yyyy HH:mm", null);
        //    DateTime dtInicioClassificacao = DateTime.ParseExact(dataInicioClassificacao, "dd/MM/yyyy HH:mm", null);
        //    DateTime dtInicioIsento = DateTime.ParseExact(dataInicioIsento, "dd/MM/yyyy HH:mm", null);
        //    DateTime datEncerramentoIsento = DateTime.ParseExact(dataEncerramentoIsento, "dd/MM/yyyy HH:mm", null);
        //    DateTime datBoleto = DateTime.ParseExact(dataBoleto, "dd/MM/yyyy HH:mm", null);

        //    tb_con_concurso concurso = new tb_con_concurso()
        //    {
        //        con_bit_ativo = ativo,
        //        con_bit_encerrado = encerrado,
        //        con_bit_inscricao_online = inscricaoOnline,
        //        con_dat_concurso = dtInicio,
        //        con_dat_encerramento_inscricoes = datEncerramentoInscricoes,
        //        con_dat_encerramento = dtEncerramento,
        //        con_dat_inicio_comprovante = dtInicioComprovante,
        //        con_dat_encerramento_comprovante = datEncerramentoComprovante,
        //        con_dat_inicio_classificacao = dtInicioClassificacao,
        //        con_dat_inicio_isento = dtInicioIsento,
        //        con_dat_encerramento_isento = datEncerramentoIsento,
        //        con_dat_boleto = datBoleto,
        //        con_idt_concurso = id,
        //        tlc_idt_tipo_layout_concurso = idTipoLayout,
        //        con_nom_concurso = nome,
        //        emp_idt_empresa = emp_idt_empresa
        //    };

        //    List<tb_can_concurso_anexo> anexos = new List<tb_can_concurso_anexo>();
        //    ConcursoSelecionado.Anexos.ForEach(ane => anexos.Add(new tb_can_concurso_anexo()
        //        {
        //            can_bit_ativo = true,
        //            can_bit_tem_recurso = ane.TemRecurso,
        //            can_dat_fim_recurso = ane.DataFimRecurso,
        //            can_dat_inicio_recurso = ane.DataInicioRecurso,
        //            can_dat_publicacao = ane.DataPublicacao,
        //            can_nom_concurso_anexo = ane.NomeArquivo,
        //            can_des_path_arquivo = ane.NomeOriginalArquivo,
        //            can_idt_concurso_anexo = ane.Id > 0 ? ane.Id : 0,
        //            con_idt_concurso = ane.IdConcurso,
        //            tca_idt_tipo_concurso_anexo = ane.IdTipo
        //        }));

        //    List<tb_cco_cargo_concurso> cargos = new List<tb_cco_cargo_concurso>();
        //    ConcursoSelecionado.Cargos.ForEach(car => cargos.Add(new tb_cco_cargo_concurso()
        //        {
        //            cco_bit_ativo = car.Ativo,
        //            cco_idt_cargo_concurso = car.Id > 0 ? car.Id : 0,
        //            cco_nom_cargo_concurso = car.Nome,
        //            cco_num_valor_inscricao = car.ValorInscricao,
        //            con_idt_concurso = car.IdConcurso
        //        }));

        //    concurso = ConcursoBusiness.Salvar(concurso, anexos, cargos);

        //    bool sucesso = concurso.con_idt_concurso > 0;

        //    if (sucesso)
        //    {
        //        string pathConcurso = Server.MapPath(String.Format("~/Anexos/Concurso/{0}", concurso.con_idt_concurso));

        //        if (id <= 0 && anexos.Count > 0)
        //        {
        //            string diretorio = Server.MapPath(String.Format("~/Anexos/Concurso/{0}", Int32.MinValue));
        //            DirectoryInfo di = new DirectoryInfo(diretorio);

        //            // Atualiza o nome da pasta que foram salvos os arquivos
        //            if (di.Exists)
        //                di.MoveTo(pathConcurso);
        //        }

        //        // Passa pelos arquivos anexo para verificar se ainda existem
        //        DirectoryInfo diConcurso = new DirectoryInfo(pathConcurso);

        //        if (diConcurso.Exists)
        //        {
        //            foreach (FileInfo fi in diConcurso.GetFiles())
        //            {
        //                if (!anexos.Any(ane => ane.can_des_path_arquivo.ToLower() == Path.GetFileName(fi.FullName).ToLower()))
        //                    fi.Delete();
        //            }
        //        }
        //    }

        //    string mensagem = sucesso ? Resources.Admin.Concurso.Cadastro.SalvarSucesso : Resources.Admin.Concurso.Cadastro.SalvarErro;

        //    return Json(new { Sucesso = sucesso, Mensagem = mensagem }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult SalvarAnexo(int idAnexo, int idConcurso, int idTipo, string nomeArquivo, string dataPublicacao, bool temRecurso, string recInicio, string recFim, string fileName)
        //{
        //    if (ConcursoSelecionado == null)
        //        ConcursoSelecionado = new oldConcursoModel(ConcursoBusiness.Obter(idConcurso));

        //    DateTime dataPub = DateTime.ParseExact(dataPublicacao, "dd/MM/yyyy HH:mm", null);
        //    DateTime? dataIniRecurso = temRecurso ? DateTime.ParseExact(recInicio, "dd/MM/yyyy HH:mm", null) : (DateTime?)null;
        //    DateTime? dataFimRecurso = temRecurso ? DateTime.ParseExact(recFim, "dd/MM/yyyy HH:mm", null) : (DateTime?)null;

        //    oldConcursoModel concurso = ConcursoSelecionado;

        //    if (idAnexo == 0)
        //    {
        //        idAnexo = ConcursoSelecionado.Anexos.Count == 0 || ConcursoSelecionado.Anexos.Min(ane => ane.Id) > 0 ? -1 : ConcursoSelecionado.Anexos.Min(ane => ane.Id) - 1;

        //        concurso.Anexos.Add(new oldConcursoModel.AnexoModel()
        //        {
        //            Ativo = true,
        //            Id = idAnexo,
        //            IdConcurso = idConcurso,
        //            IdTipo = idTipo,
        //            TemRecurso = temRecurso,
        //            NomeArquivo = nomeArquivo,
        //            NomeOriginalArquivo = fileName.Replace("C:\\fakepath\\", ""),
        //            DataPublicacao = dataPub,
        //            DataInicioRecurso = dataIniRecurso,
        //            DataFimRecurso = dataFimRecurso
        //        });
        //    }
        //    else
        //    {
        //        oldConcursoModel.AnexoModel anexo = concurso.Anexos.SingleOrDefault(ane => ane.Id == idAnexo);
        //        anexo.Ativo = true;
        //        anexo.IdTipo = idTipo;
        //        anexo.TemRecurso = temRecurso;
        //        anexo.DataPublicacao = dataPub;
        //        anexo.DataInicioRecurso = dataIniRecurso;
        //        anexo.DataFimRecurso = dataFimRecurso;
        //        anexo.NomeArquivo = nomeArquivo;
        //        anexo.NomeOriginalArquivo = String.IsNullOrEmpty(fileName) ? anexo.NomeOriginalArquivo : fileName.Replace("C:\\fakepath\\", "");
        //    }

        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult SalvarCargo(int idCargo, int idConcurso, string nomeCargo, bool ehIsento, string valorInscricao)
        //{
        //    if (ConcursoSelecionado == null)
        //        ConcursoSelecionado = new oldConcursoModel(ConcursoBusiness.Obter(idConcurso));

        //    Decimal? vInsc = String.IsNullOrEmpty(valorInscricao) ? (Decimal?)null : Decimal.Parse(valorInscricao.Replace("R$ ", ""));

        //    oldConcursoModel concurso = ConcursoSelecionado;

        //    if (idCargo == 0)
        //    {
        //        idCargo = ConcursoSelecionado.Cargos.Count == 0 || ConcursoSelecionado.Cargos.Min(car => car.Id) > 0 ? -1 : ConcursoSelecionado.Cargos.Min(car => car.Id) - 1;

        //        concurso.Cargos.Add(new oldConcursoModel.CargoModel()
        //        {
        //            Ativo = true,
        //            Id = idCargo,
        //            IdConcurso = idConcurso,
        //            Nome = nomeCargo,
        //            ValorInscricao = vInsc
        //        });
        //    }
        //    else
        //    {
        //        oldConcursoModel.CargoModel cargo = concurso.Cargos.SingleOrDefault(car => car.Id == idCargo);
        //        cargo.Ativo = true;
        //        cargo.Nome = nomeCargo;
        //        cargo.ValorInscricao = vInsc;
        //    }

        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public async Task<JsonResult> UploadAnexoConcurso()
        //{
        //    try
        //    {
        //        foreach (string file in Request.Files)
        //        {
        //            var fileContent = Request.Files[file];
        //            if (fileContent != null && fileContent.ContentLength > 0)
        //            {
        //                MemoryStream ms = new MemoryStream();
        //                Stream stream = fileContent.InputStream;
        //                stream.CopyTo(ms);
        //                ms.Position = 0;

        //                var fileName = fileContent.FileName;

        //                if ((Path.GetExtension(fileContent.FileName).ToLower() != ".csv") || fileContent.ContentLength > 2224000)
        //                    return Json(new { Sucesso = false }, JsonRequestBehavior.AllowGet);

        //                Session[SessionArquivoConcursoUpload] = new { Stream = ms, FileName = fileName };
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //        return Json(new { Sucesso = false }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { Sucesso = true }, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public async Task<JsonResult> UploadAnexoRemessa()
        //{
        //    try
        //    {
        //        foreach (string file in Request.Files)
        //        {
        //            var fileContent = Request.Files[file];
        //            if (fileContent != null && fileContent.ContentLength > 0)
        //            {
        //                MemoryStream ms = new MemoryStream();
        //                Stream stream = fileContent.InputStream;
        //                stream.CopyTo(ms);
        //                ms.Position = 0;

        //                var fileName = fileContent.FileName;

        //                if ((Path.GetExtension(fileContent.FileName).ToLower() != ".ret" && Path.GetExtension(fileContent.FileName).ToLower() != ".csv") || fileContent.ContentLength > 2224000)
        //                    return Json(new { Sucesso = false }, JsonRequestBehavior.AllowGet);

        //                Session[SessionArquivoRemessaUpload] = new { Stream = ms, FileName = fileName };
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //        return Json(new { Sucesso = false }, JsonRequestBehavior.AllowGet);
        //    }

        //    return Json(new { Sucesso = true }, JsonRequestBehavior.AllowGet);
        //}

        //#endregion [ FIM - Concursos ]

        //#region [ Empresas ]

        //public ActionResult Index_Empresa()
        //{
        //    return View();
        //}

        //public ActionResult Cadastro_Empresa(int? id)
        //{
        //    oldConcursoModel.EmpresaModel empresa = new oldConcursoModel.EmpresaModel();

        //    if (id.HasValue)
        //    {
        //        empresa = new oldConcursoModel.EmpresaModel(EmpresaBusiness.Obter(id.Value));
        //    }

        //    return View(empresa);
        //}

        //public ActionResult CarregarEmpresas()
        //{
        //    List<tb_emp_empresa> empresas = EmpresaBusiness.Listar();
        //    List<SelectListItem> empresasLI = empresas.ConvertAll(emp => new SelectListItem() { Text = emp.emp_nom_empresa, Value = emp.emp_idt_empresa.ToString() });

        //    return Json(empresasLI, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult ListarEmpresas()
        //{
        //    List<oldConcursoModel.EmpresaModel> empresas = EmpresaBusiness.Listar(false).ConvertAll(emp => new oldConcursoModel.EmpresaModel(emp));

        //    return PartialView("_ListaEmpresas", empresas);
        //}

        //public ActionResult EditarAtivacaoEmpresao(int id)
        //{
        //    tb_emp_empresa empresa = EmpresaBusiness.EditarAtivacao(id);
        //    return Json(new { Sucesso = empresa != null, Ativo = empresa.emp_bit_ativo }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Salvar_Empresa(int id, string nome, string razaoSocial, string cnpj, short banco, int convenio, int? convenio_cobranca, string agencia, string conta, bool ativo)
        //{
        //    tb_emp_empresa empresa = new tb_emp_empresa()
        //    {
        //        emp_idt_empresa = id,
        //        emp_bit_ativo = ativo,
        //        emp_des_agencia = agencia,
        //        emp_des_cnpj = cnpj,
        //        emp_des_conta_corrente = conta,
        //        emp_des_razao_social = razaoSocial,
        //        emp_nom_empresa = nome,
        //        emp_num_banco = banco,
        //        emp_num_convenio = convenio,
        //        emp_num_convenio_cobranca = convenio_cobranca
        //    };

        //    bool sucesso = EmpresaBusiness.Salvar(empresa) != null;
        //    string mensagem = sucesso ? Resources.Admin.Concurso.Empresa.Cadastro.SalvarSucesso : Resources.Admin.Concurso.Empresa.Cadastro.SalvarErro;

        //    return Json(new { Sucesso = sucesso, Mensagem = mensagem }, JsonRequestBehavior.AllowGet);
        //}

        //#endregion [ FIM - Empresas ]

        //#region [ Inscritos ]

        //public ActionResult Index_Inscrito(int? id)
        //{
        //    int idConcurso = id.HasValue && id.Value > 0 ? id.Value : 0;
        //    oldConcursoModel concurso = TodosConcursos.SingleOrDefault(con => con.Id == idConcurso);

        //    if (concurso == null)
        //        return RedirecionarPagina("Index", "Concurso", "Admin", 0);

        //    ConcursoSelecionado = concurso;

        //    return View(concurso);
        //}

        //public ActionResult AbrirModalEstatisticas()
        //{
        //    oldConcursoModel concurso = ConcursoSelecionado;

        //    if (concurso == null)
        //        return null;

        //    // Busca a lista de inscritos do concurso
        //    List<oldConcursoModel.InscritoModel> inscritos = InscritoConcursoBusiness.Listar(concurso.Id).ConvertAll<oldConcursoModel.InscritoModel>(ico => new oldConcursoModel.InscritoModel(ico));
        //    concurso.Inscritos = inscritos;
        //    concurso.QuantidadeInscritos = inscritos.Count();

        //    oldConcursoModel.ConcursoInscritosEstatisticaModel ciem = new oldConcursoModel.ConcursoInscritosEstatisticaModel(concurso);
        //    string view = BaseController.RenderViewToString(ControllerContext, "~/Areas/Admin/Views/Concurso/_ModalEstatisticas.cshtml", ciem, true);

        //    return Json(new { View = view }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Cadastro_Inscrito(int? id)
        //{
        //    oldConcursoModel concurso = ConcursoSelecionado;

        //    if (!id.HasValue || concurso == null)
        //        return RedirecionarPagina("Index_Inscrito", "Concurso", "Admin", 0);

        //    return View(concurso);
        //}

        //public ActionResult CarregarInscrito(int id)
        //{
        //    oldConcursoModel concurso = ConcursoSelecionado;

        //    if (concurso == null)
        //        return RedirecionarPagina("Index", "Concurso", "Admin", 0);

        //    oldConcursoModel.InscritoModel inscrito = new oldConcursoModel.InscritoModel(InscritoConcursoBusiness.Obter(id));

        //    if (inscrito == null)
        //        return RedirecionarPagina("Index_Inscrito", "Concurso", "Admin", 0);

        //    string dInscrito = concurso.IdTipoLayoutConcurso == 1 ? "_DadosInscrito" : "_DadosInscritoUniFae";

        //    if (concurso.IdTipoLayoutConcurso == 2)
        //        inscrito.InscritoVestibular = new oldConcursoModel.InscritoModel.InscritoVestibularModel(InscritoConcursoBusiness.ObterVestibular(inscrito.Id));

        //    inscrito.IdConcurso = concurso.Id;
        //    inscrito.IdTipoLayoutConcurso = concurso.IdTipoLayoutConcurso;
        //    inscrito.NomeConcurso = concurso.Nome;

        //    string view = BaseController.RenderViewToString(ControllerContext, String.Format("~/Views/Shared/{0}.cshtml", dInscrito), inscrito, true);

        //    return Json(new { View = view }, JsonRequestBehavior.AllowGet);
        //}

        //private List<oldConcursoModel.InscritoModel> ListarInscritos(int idConcurso, int pagina, string ordem, int? idMatricula, string nome, string cpf, bool? ativo, bool? isento)
        //{
        //    int tamanhoPagina = TamanhoPaginaLista;
        //    bool comPaginacao = true;

        //    List<oldConcursoModel.InscritoModel> inscritos = InscritoConcursoBusiness.Listar(idConcurso, pagina, comPaginacao, tamanhoPagina, ordem, idMatricula, nome, cpf, ativo, isento)
        //        .ConvertAll(ico => new oldConcursoModel.InscritoModel(ico, ico.tb_cci_concurso_cargo_inscrito.ToList()));

        //    return inscritos;
        //}

        //public ActionResult PesquisarInscritos(int idConcurso, int pagina, string ordem, int matricula, string nome, string cpf, bool? ativo, bool? isento)
        //{
        //    List<oldConcursoModel.InscritoModel> inscritos = ListarInscritos(idConcurso, pagina, ordem, matricula, nome, cpf, ativo, isento);

        //    try
        //    {
        //        string view = BaseController.RenderViewToString(ControllerContext, "~/Areas/Admin/Views/Concurso/_ListaInscritos.cshtml", inscritos, true);
        //        return Json(new { View = view, TotalItens = inscritos.Count }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //#endregion [ FIM - Inscritos ]

        //public void GerarRemessaBradesco()
        //{
            
        //}
    }

    public class CSVRetornoBradesco
    {
        public string NroPedido         { get; set; }
        public string Status            { get; set; }
        public string DataPedido        { get; set; }
        public string ValorPedido       { get; set; }
        public string DataPagamento     { get; set; }
        public string ValorPagamento    { get; set; }
        public string MeioPagamento     { get; set; }
        public string AtualRegistro     { get; set; }
        public string CodOperacao       { get; set; }
    }

    public sealed class CSVRetornoBradescoMap : CsvClassMap<CSVRetornoBradesco>
    {
        public CSVRetornoBradescoMap()
        {
            //Map(m => m.NroPedido).Name("Nr. do Pedido");
            //Map(m => m.Status).Name("Status");
            //Map(m => m.DataPedido).Name("Data do Pedido");
            //Map(m => m.ValorPedido).Name("Valor do Pedido(R$)");
            //Map(m => m.DataPagamento).Name("Data de Pagto");
            //Map(m => m.ValorPagamento).Name("Valor de Pagto(R$)");
            //Map(m => m.MeioPagamento).Name("Meio de Pagto");
            //Map(m => m.AtualRegistro).Name("Atual Registro");
            //Map(m => m.CodOperacao).Name("Cod. Operacao");
            Map(m => m.NroPedido).Index(0);
            Map(m => m.Status).Index(1);
            Map(m => m.DataPedido).Index(2);
            Map(m => m.ValorPedido).Index(3);
            Map(m => m.DataPagamento).Index(4);
            Map(m => m.ValorPagamento).Index(5);
            Map(m => m.MeioPagamento).Index(6);
            Map(m => m.AtualRegistro).Index(7);
            Map(m => m.CodOperacao).Index(8);
        }
    }
}