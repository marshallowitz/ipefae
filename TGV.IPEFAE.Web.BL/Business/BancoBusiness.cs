using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGV.Framework.Criptografia;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.BL.Business
{
    public class BancoBusiness
    {
        public static List<BancoModel> Listar()
        {
            return BancoData.Listar();
        }
    }
}
