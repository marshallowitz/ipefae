using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TGV.IPEFAE.Web.App.Helper
{
    public class AzureBlobHelper
    {
        private const string c_container_transparencia_name = "transparencia";
        private const string c_storage_conn_string = "StorageConnectionString";

        public static void GravarArquivoTransparenciaAzure(TransparenciaTipo tipo, string fileName, Stream fileStream, KeyValuePair<string, string>? metadataItem = null)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string pasta = ObterPasta(tipo);
            string path = $"{pasta}/{DateTime.Now.ToString("yyyyMMddHHmmsss")}_{fileNameWithoutExtension}.pdf";

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting(c_storage_conn_string));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(c_container_transparencia_name); // Obtém o container da área de transparencia
            bool novoContainer = container.CreateIfNotExists(); // Caso o container ainda não exista, cria

            if (novoContainer)
                container.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob }); // Seta as permissões de acesso

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(path);

            if (metadataItem.HasValue)
                blockBlob.Metadata.Add(metadataItem.Value);

            blockBlob.UploadFromStream(fileStream);
        }

        public static string ObterPasta(TransparenciaTipo tipo)
        {
            switch (tipo)
            {
                case TransparenciaTipo.Atas:
                    return "Ata";
                case TransparenciaTipo.CNPJ:
                    return "CNPJ";
                case TransparenciaTipo.DREs:
                    return "DRE";
                case TransparenciaTipo.Estatuto:
                    return "Estatuto";
                case TransparenciaTipo.Remuneracoes:
                    return "Remuneracao";
                default:
                    return "Ata";
            }
        }

        public static void RemoverArquivoAzure(AzureTipoArquivo tipo, string path)
        {
            string container_name = tipo == AzureTipoArquivo.Transparencia ? c_container_transparencia_name : String.Empty;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting(c_storage_conn_string));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(container_name); // Obtém o container com os arquivos

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(path);
            var result = blockBlob.DeleteIfExists();
        }


        public static IEnumerable<dynamic> ListarArquivosAzure(AzureTipoArquivo tipo, int identificador)
        {
            string container_name = String.Empty;
            string pasta = String.Empty;

            switch (tipo)
            {
                case AzureTipoArquivo.Transparencia:
                    container_name = c_container_transparencia_name;
                    pasta = ObterPasta((TransparenciaTipo)identificador);
                    break;
                default:
                    return new List<dynamic>();
            }

            string connString = CloudConfigurationManager.GetSetting(c_storage_conn_string);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(container_name); // Obtém o container com os arquivos

            var directory = container.GetDirectoryReference($"{pasta}");
            var arquivos = directory.ListBlobs(false, BlobListingDetails.Metadata).OfType<CloudBlockBlob>();
            return arquivos;
        }
    }

    public enum AzureTipoArquivo
    {
        Transparencia = 1
    }

    public enum TransparenciaTipo
    {
        Atas = 1,
        CNPJ = 2,
        DREs = 3,
        Estatuto = 4,
        Remuneracoes = 5
    }
}