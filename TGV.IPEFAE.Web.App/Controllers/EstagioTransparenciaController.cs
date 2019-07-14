using System.Collections.Generic;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Helper;
using TGV.IPEFAE.Web.App.Models;

namespace TGV.IPEFAE.Web.App.Controllers
{
    public class EstagioTransparenciaController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listar(int tipo)
        {
            var azureFiles = AzureBlobHelper.ListarArquivosAzure(AzureTipoArquivo.Transparencia, tipo);
            List<TransparenciaFilesAzure> arquivos = new List<TransparenciaFilesAzure>();

            foreach (var azureFile in azureFiles)
            {
                string nomeArquivo = azureFile.Name;
                var metadata = azureFile.Metadata;

                if (metadata != null && metadata.Count > 0)
                    metadata.TryGetValue("Nome", out nomeArquivo);

                arquivos.Add(new TransparenciaFilesAzure()
                {
                    tipo_id = tipo,
                    tipo_nome = AzureBlobHelper.ObterPasta((TransparenciaTipo)tipo),
                    nome = nomeArquivo,
                    url = azureFile.Uri.ToString()
                });
            }

            return Json(new { Sucesso = true, Arquivos = arquivos }, JsonRequestBehavior.AllowGet);
        }
    }
}