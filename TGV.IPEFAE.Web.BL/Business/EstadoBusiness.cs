using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.BL.Business
{
    public static class EstadoBusiness
    {
        public static List<tb_est_estado> Listar()
        {
            return EstadoData.Listar();
        }
    }
}
