using Ionic.Zip;
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
    public class DownloadPDFHandler : IHttpHandler, IRequiresSessionState
    {
        public void Trace(string mensagem)
        {
            bool habilitado = BaseBusiness.IsTrace;

            if (!habilitado)
                return;

            int? idUsuario = BaseController.UsuarioLogado.Id;

            Exception ex = new Exception(mensagem);
            LogErroBusiness.Salvar(ex, idUsuario, "Trace - DownloadPDFHandler");
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                Trace("entrou ok");

                List<UsuarioEstagioModel> estagsPDF = context.Session["SessionPDFEstagiarios"] as List<UsuarioEstagioModel>;

                Trace($"Total estags: {estagsPDF.Count.ToString()}");

                if (estagsPDF == null || estagsPDF.Count == 0)
                    return;

                string zipFileName = BaseBusiness.DataAgora.ToString("yyyyMMddHHmm");
                string path = context.Server.MapPath(String.Format("~/Anexos/Estagio/Temp/{0}", zipFileName));

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                Trace("diretorio ok");

                var wrapperContext = new HttpContextWrapper(context);

                // Grava todos os currículos
                foreach (UsuarioEstagioModel ueModel in estagsPDF)
                {
                    System.Web.Routing.RouteData rd = new System.Web.Routing.RouteData();
                    rd.Values.Add("controller", "Estagio");
                    rd.Values.Add("action", "GerarPDF");
                    rd.Values.Add("id", ueModel.Id);
                    System.Web.Mvc.ControllerContext ctx = new System.Web.Mvc.ControllerContext(wrapperContext, rd, new BaseEstagioController());

                    Trace("wrapperContext ok");

                    Rotativa.ViewAsPdf vAsPDF = BaseEstagioController.GerarPDFCurriculo(ueModel);

                    Trace("vAsPDF ok");

                    int relatorioId = PDFBusiness.ObterChaveRelatorio();
                    string chaveId = relatorioId.ToString().PadLeft(2, '0');

                    string apiKey = BaseBusiness.ObterValorWebConfig($"apiKeyPDF{chaveId}");
                    byte[] estagPDF = vAsPDF.NewBuildFile(ctx, apiKey);

                    Trace($"estagPDF ok: {estagPDF.Length}");

                    string pdfPath = String.Format("{0}/{1}.pdf", path, ueModel.Id);

                    Trace($"pdfPath ok: {ueModel.Id}");

                    using (FileStream fs = new FileStream(pdfPath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fs.Write(estagPDF, 0, estagPDF.Length);
                    }

                    Trace("FileStreamWrite ok");
                }

                Trace("foreach ok");

                // Cria arquivo Zip
                DirectoryInfo dI = new DirectoryInfo(path);
                string fileName = String.Format("{0}.zip", zipFileName);

                Trace("zip ok");

                context.Response.Clear();

                using (MemoryStream ms = new MemoryStream())
                {
                    using (ZipFile zip = new ZipFile())
                    {
                        String[] filenames = System.IO.Directory.GetFiles(path);

                        foreach (String filename in filenames)
                        {
                            ZipEntry e = zip.AddFile(filename, "");
                        }

                        zip.Save(context.Response.OutputStream);
                    }
                }

                Trace("MemoryStream ok");

                // Apaga os PDFs
                dI.Empty();
                context.Session["GerouPDF"] = true;

                context.Response.ContentType = "application/x-zip-compressed";
                context.Response.AddHeader("content-disposition", String.Format("attachment;filename={0}", fileName));
                context.Response.Buffer = true;

                Trace("Buffer ok");

                context.Response.Flush(); // Sends all currently buffered output to the client.

                Trace("Flush ok");

                context.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.

                Trace("SuppressContent ok");

                context.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.

                Trace("CompleteRequest ok");
            }
            catch (Exception ex)
            {
                int? idUsuario = BaseController.UsuarioLogado.Id;
                LogErroBusiness.Salvar(ex, idUsuario);
                throw ex;
            }
        }

        private byte[] WriteCsvWithHeaderToMemory<T>(IEnumerable<T> records, bool tipoConcurso) where T : class
        {
            if (records.Count() == 0)
                return Resources.Admin.Concurso.CSVInscritos.CSVNenhumInscrito.GetBytes();

            string delimitador = ";";

            StringBuilder csvFile = new StringBuilder();

            // Monta o cabecalho
            foreach (var propriedade in records.First().GetType().GetProperties())
                csvFile.AppendFormat("{0}{1}", propriedade.ObterDescricao(tipoConcurso), delimitador);

            csvFile.AppendLine();

            // Monta o corpo
            foreach (var item in records)
            {
                foreach (var propriedade in item.GetType().GetProperties())
                    csvFile.AppendFormat("{0}{1}", IPEFAEExtension.GetPropValue(item, propriedade.Name), delimitador);

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