using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.BL.Business
{
    public static class CidadeBusiness
    {
        public static List<tb_cid_cidade> Listar(int est_idt_estado)
        {
            return CidadeData.Listar(est_idt_estado);
        }

        public static List<tb_cid_cidade> ListarCidadesComEstagiario()
        {
            return CidadeData.ListarCidadesComEstagiario();
        }
    }
}
