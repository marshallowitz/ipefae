using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGV.IPEFAE.Web.BL.Data
{
    internal class EstadoData
    {
        internal static List<tb_est_estado> Listar()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from est in db.tb_est_estado
                        where est.est_bit_ativo
                        orderby est.est_nom_estado
                        select est).ToList();
            }
        }
    }
}
