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

    public class EstadoModel
    {
        public EstadoModel() { }

        public EstadoModel(tb_est_estado estado)
        {
            if (estado == null)
                return;

            this.Id = estado.est_idt_estado;
            this.Nome = estado.est_nom_estado;
            this.Sigla = estado.est_sig_estado;
            this.Ativo = estado.est_bit_ativo;
        }

        public int Id       { get; set; }
        public string Nome  { get; set; }
        public string Sigla { get; set; }
        public bool Ativo   { get; set; }
    }
}
