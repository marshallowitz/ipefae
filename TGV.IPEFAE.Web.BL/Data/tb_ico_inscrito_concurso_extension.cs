using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGV.IPEFAE.Web.BL.Data
{
    [NotMapped]
    public partial class tb_ico_inscrito_concurso_extension : tb_ico_inscrito_concurso
    {
        public int est_idt_estado { get; set; }
        public string cid_nom_cidade { get; set; }
        public bool cid_bit_capital { get; set; }
        public bool cid_bit_ativo { get; set; }

        public string est_nom_estado { get; set; }
        public string est_sig_estado { get; set; }
        public bool est_bit_ativo { get; set; }

        public int cco_idt_cargo_concurso { get; set; }
        public string cco_nom_cargo_concurso { get; set; }
        public decimal cco_num_valor_inscricao { get; set; }
        public bool cco_bit_ativo { get; set; }
    }
}
