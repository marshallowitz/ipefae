using System.Collections.Generic;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.BL.Business
{
    public class FuncaoBusiness
    {
        public static List<FuncaoModel> Listar()
        {
            return FuncaoData.Listar();
        }
    }
}
