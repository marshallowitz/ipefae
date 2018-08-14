using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using TGV.IPEFAE.Web.App.Controllers;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Handlers
{
    public class DownloadCSVHandler : IHttpHandler, IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                int id = Convert.ToInt32(context.Request.QueryString["id"]);
                string tipo = context.Request.QueryString["tipo"];

                if (!String.IsNullOrEmpty(tipo) && (tipo.Equals("est", StringComparison.InvariantCultureIgnoreCase)))
                    ProcessRequestForEstagio(context);
                else if(!String.IsNullOrEmpty(tipo) && (tipo.Equals("col", StringComparison.InvariantCultureIgnoreCase)))
                    ProcessRequestForColaborador(context);
                else
                    ProcessRequestForConcurso(context, id);
            }
            catch (Exception ex)
            {
                int? idUsuario = BaseController.UsuarioLogado.Id;
                LogErroBusiness.Salvar(ex, idUsuario);
                throw ex;
            }
        }

        private void ProcessRequestForColaborador(HttpContext context)
        {
            List<ColaboradorModel> colaboradores = ColaboradorBusiness.Listar();

            string fileName = String.Format("lista_colaboradores_{0}.csv", BaseBusiness.DataAgora.ToString("yyyyMMdd"));
            byte[] fileBytes = null;

            List<tb_est_estado> estados = EstadoBusiness.Listar();
            List<tb_cid_cidade> cidades = CidadeBusiness.ListarTodas();
            List<GrauInstrucaoModel> grausInstrucao = GrauInstrucaoBusiness.Listar();
            List<RacaModel> racas = RacaBusiness.Listar();
            List<BancoModel> bancos = BancoBusiness.Listar();
            List<IRPFModel> irpfs = IRPFBusiness.Listar();
            tb_emp_empresa emitente = EmpresaBusiness.Obter(1);

            List<ColaboradorCSVModel2> cCSVs = colaboradores.ConvertAll(c => new ColaboradorCSVModel2(c, estados, cidades, grausInstrucao, racas, bancos));
            fileBytes = WriteCsvWithHeaderToMemory(cCSVs, true);

            context.Session["GerouCSV"] = true;

            context.Response.Clear();
            MemoryStream ms = new MemoryStream(fileBytes);
            context.Response.ContentType = "text/csv";
            context.Response.AddHeader("content-disposition", String.Format("attachment;filename={0}", fileName));
            context.Response.Buffer = true;
            ms.WriteTo(context.Response.OutputStream);
            context.Response.Flush(); // Sends all currently buffered output to the client.
            context.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
            context.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.
        }

        private void ProcessRequestForConcurso(HttpContext context, int id)
        {
            ConcursoModel concurso = ConcursoBusiness.Obter(id, true);

            if (concurso == null)
                return;

            dynamic result = ColaboradorBusiness.ListarPorConcursoV2(concurso);
            List<ColaboradorModel> colaboradores = (List<ColaboradorModel>)result.Colaboradores;
            List<ConcursoLocalColaboradorModel> cLocaisColaboradores = (List<ConcursoLocalColaboradorModel>)result.LocaisColaboradores;

            string fileName = String.Format("{1}_{0}.csv", BaseBusiness.RemoverCaracteresEspeciais(concurso.nome), BaseBusiness.DataAgora.ToString("yyyyMMdd"));
            byte[] fileBytes = null;

            List<tb_est_estado> estados = EstadoBusiness.Listar();
            List<tb_cid_cidade> cidades = CidadeBusiness.ListarTodas();
            List<GrauInstrucaoModel> grausInstrucao = GrauInstrucaoBusiness.Listar();
            List<RacaModel> racas = RacaBusiness.Listar();
            List<BancoModel> bancos = BancoBusiness.Listar();
            List<IRPFModel> irpfs = IRPFBusiness.Listar();
            tb_emp_empresa emitente = EmpresaBusiness.Obter(1);

            List<ColaboradorRPAModel> colaboradoresRPA = colaboradores.ConvertAll(c => ColaboradorRPAModel.Clone(c, cidades, estados, cLocaisColaboradores, irpfs, emitente));

            List<ColaboradorCSVModel> cCSVs = colaboradores.ConvertAll(c => new ColaboradorCSVModel(concurso, c, colaboradoresRPA, estados, cidades, grausInstrucao, racas, bancos));
            fileBytes = WriteCsvWithHeaderToMemory(cCSVs, true);

            context.Session["GerouCSV"] = true;

            context.Response.Clear();
            MemoryStream ms = new MemoryStream(fileBytes);
            context.Response.ContentType = "text/csv";
            context.Response.AddHeader("content-disposition", String.Format("attachment;filename={0}", fileName));
            context.Response.Buffer = true;
            ms.WriteTo(context.Response.OutputStream);
            context.Response.Flush(); // Sends all currently buffered output to the client.
            context.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
            context.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.
        }

        private void ProcessRequestForEstagio(HttpContext context)
        {
            var listaEstagiarios = context.Session["SessionCSVEstagiarios"] as List<UsuarioEstagioModel>;

            if (listaEstagiarios?.Count <= 0)
                return;

            var listaEstagiariosCSV = listaEstagiarios.ConvertAll(est => new UsuarioEstagioModelCSV(est));

            string fileName = String.Format("{0}.csv", BaseBusiness.DataAgora.ToString("yyyyMMddHHmmss"));
            byte[] fileBytes = WriteCsvWithHeaderToMemory(listaEstagiariosCSV, false);

            context.Session["GerouCSV"] = true;

            context.Response.Clear();
            MemoryStream ms = new MemoryStream(fileBytes);
            context.Response.ContentType = "text/csv";
            context.Response.AddHeader("content-disposition", String.Format("attachment;filename={0}", fileName));
            context.Response.Buffer = true;
            ms.WriteTo(context.Response.OutputStream);
            context.Response.Flush(); // Sends all currently buffered output to the client.
            context.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
            context.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.

        }

        private byte[] WriteCsvWithHeaderToMemory<T>(IEnumerable<T> records, bool tipoConcurso) where T : class
        {
            if (records.Count() == 0)
                return Resources.Admin.Concurso.CSVInscritos.CSVNenhumInscrito.GetBytes();

            string delimitador = ";";

            StringBuilder csvFile = new StringBuilder();

            // Monta o cabecalho
            var properties = from property in records.First().GetType().GetProperties()
                             let orderAttribute = property.GetCustomAttributes(typeof(OrderAttribute), false).SingleOrDefault() as OrderAttribute
                             orderby orderAttribute.Order
                             select property;

            foreach (var propriedade in properties)
                csvFile.AppendFormat("{0}{1}", propriedade.ObterDescricao(tipoConcurso), delimitador);

            csvFile.AppendLine();

            // Monta o corpo
            foreach (var item in records)
            {
                properties = from property in item.GetType().GetProperties()
                             let orderAttribute = property.GetCustomAttributes(typeof(OrderAttribute), false).SingleOrDefault() as OrderAttribute
                             orderby orderAttribute.Order
                             select property;

                foreach (var propriedade in properties)
                {
                    var isTextAtt = propriedade.GetCustomAttributes(typeof(IsTextAttribute), false).SingleOrDefault() as IsTextAttribute;
                    
                    if (isTextAtt == null || !isTextAtt.IsText)
                        csvFile.AppendFormat("{0}{1}", IPEFAEExtension.GetPropValue(item, propriedade.Name), delimitador);
                    else
                        csvFile.AppendFormat("=\"{0}\"{1}", IPEFAEExtension.GetPropValue(item, propriedade.Name), delimitador);
                }

                csvFile.AppendLine();
            }

            return csvFile.ToString().GetBytes();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}