using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGV.IPEFAE.Web.BL.Data
{
    public partial class tb_emp_empresa
    {
        public string CNPJ_Formatado { get { return BaseData.FormatarCNPJ(this.emp_des_cnpj); } }
    }
}
