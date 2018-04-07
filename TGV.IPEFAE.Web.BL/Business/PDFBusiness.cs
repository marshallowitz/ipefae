using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.BL.Business
{
    public class PDFBusiness
    {
        public static int ObterChaveRelatorio()
        {
            return PDFData.ObterChaveRelatorio();
        }

        public static void Atualizar5Chave()
        {
            PDFData.Atualizar5Chave();
        }

        public static int ListarTotalGeradosMesCorrente()
        {
            return PDFData.ListarTotalGeradosMesCorrente();
        }
    }
}
