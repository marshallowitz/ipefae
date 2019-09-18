using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Controllers;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.App.Helper;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Areas.Admin.Controllers
{
    [IPEFAEAuthorizationAttribute(PermissaoModel.Tipo.Estagio)]
    public class EstagioTransparenciaController : BaseController
    {
        private string NomeSessionUploadPDF = "NomeSessionUploadPDF";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Apagar(string url)
        {
            var dirName = Path.GetFileName(Path.GetDirectoryName(url));
            var fileName = Path.GetFileName(url);
            var path = $"{dirName}/{fileName}";

            AzureBlobHelper.RemoverArquivoAzure(AzureTipoArquivo.Transparencia, path);

            return Json(new { Sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Listar(int tipo, string nome)
        {
            if (String.IsNullOrEmpty(nome))
                nome = String.Empty;

            var azureFiles = AzureBlobHelper.ListarArquivosAzure(AzureTipoArquivo.Transparencia, tipo);
            List<TransparenciaFilesAzure> arquivos = new List<TransparenciaFilesAzure>();

            foreach (var azureFile in azureFiles)
            {
                string nomeArquivo = azureFile.Name;
                var metadata = azureFile.Metadata;

                if (metadata != null && metadata.Count > 0)
                    metadata.TryGetValue("Nome", out nomeArquivo);

                if (!nomeArquivo.ToLower().Contains(nome.ToLower()))
                    continue;

                string pasta = AzureBlobHelper.ObterPasta((TransparenciaTipo)tipo);

                if (pasta == "Ata")
                    pasta = "Extrato Parcerias Celebradas";
                else if (pasta == "Contabil")
                    pasta = "Prestação de Contas";
                else if (pasta == "Termo_Remuneracao")
                    pasta = "Publicidade OSC";

                arquivos.Add(new TransparenciaFilesAzure()
                {
                    tipo_id = tipo,
                    tipo_nome = pasta,
                    nome = nomeArquivo,
                    url = azureFile.Uri.ToString()
                });
            }

            return Json(new { Sucesso = true, Arquivos = arquivos }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Salvar(int tipo, string nome)
        {
            if (Session[NomeSessionUploadPDF] == null)
                return Json(new { Sucesso = false, Mensagem = "Não foi encontrado nenhum documento anexado" }, JsonRequestBehavior.AllowGet);

            // Faz o upload do arquivo para o Azure
            dynamic arquivo = Session[NomeSessionUploadPDF];
            var fileName = arquivo.FileName;
            var fileStream = arquivo.Stream;
            AzureBlobHelper.GravarArquivoTransparenciaAzure((TransparenciaTipo)tipo, fileName, fileStream, new KeyValuePair<string, string>("Nome", nome));

            return Json(new { Sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> UploadPDF()
        {
            string pathPDF = String.Empty;

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

                        if (fileContent.ContentLength > 5000000)
                            return Json(new { Sucesso = false, PathPDF = "", Mensagem = "O arquivo deve ter menos do que 4MB de tamanho" }, JsonRequestBehavior.AllowGet);

                        if (new FileInfo(fileName).Extension.ToLower() != ".pdf")
                            return Json(new { Sucesso = false, PathPDF = "", Mensagem = "O formato do arquivo deve ser PDF" }, JsonRequestBehavior.AllowGet);

                        Session[NomeSessionUploadPDF] = new { Stream = ms, FileName = fileName };

                        pathPDF = Convert.ToBase64String(ms.ReadToEnd());
                    }
                }
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Sucesso = false, PathFoto = "" }, JsonRequestBehavior.AllowGet);
            }

            JsonResult result = Json(new { Sucesso = true, PathPDF = String.Format("data:application/pdf;base64,{0}", pathPDF) }, JsonRequestBehavior.AllowGet);
            result.MaxJsonLength = int.MaxValue;
            return result;
        }
    }
}