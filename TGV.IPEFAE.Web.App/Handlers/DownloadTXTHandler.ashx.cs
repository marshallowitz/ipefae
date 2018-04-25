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
    public class DownloadTXTHandler : IHttpHandler, IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                int id = Convert.ToInt32(context.Request.QueryString["id"]);
                string tipo = context.Request.QueryString["tipo"];

                if (!String.IsNullOrEmpty(tipo))
                {
                    if (tipo.Equals("rem", StringComparison.InvariantCultureIgnoreCase))
                        ProcessRequestForRemessaBoleto(context, id);
                }
            }
            catch (Exception ex)
            {
                int? idUsuario = BaseController.UsuarioLogado.Id;
                LogErroBusiness.Salvar(ex, idUsuario);
                throw ex;
            }
        }

        private void ProcessRequestForRemessaBoleto(HttpContext context, int id)
        {
            //string fileName = String.Format("{0}.txt", BaseBusiness.DataAgora.ToString("yyyyMMddHHmmss"));
            //BoletoModel bm = new BoletoModel();
            //byte[] fileBytes = bm.GerarRemessa(id);

            //context.Session["GerouTXT"] = true;

            //context.Response.Clear();
            //MemoryStream ms = new MemoryStream(fileBytes);
            //context.Response.ContentType = "text/txt";
            //context.Response.AddHeader("content-disposition", String.Format("attachment;filename={0}", fileName));
            //context.Response.Buffer = true;
            //ms.WriteTo(context.Response.OutputStream);
            //context.Response.Flush(); // Sends all currently buffered output to the client.
            //context.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
            //context.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.

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