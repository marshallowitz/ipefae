using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.BL.Business
{
    public static class EmpresaBusiness
    {
        public static tb_emp_empresa EditarAtivacao(int emp_idt_empresa)
        {
            return EmpresaData.EditarAtivacao(emp_idt_empresa);
        }

        public static List<tb_emp_empresa> Listar(bool considerarAtivo = true)
        {
            return EmpresaData.Listar(considerarAtivo);
        }

        public static tb_emp_empresa Obter(int emp_idt_empresa)
        {
            return EmpresaData.Obter(emp_idt_empresa);
        }

        public static tb_emp_empresa Salvar(tb_emp_empresa empresa)
        {
            return EmpresaData.Salvar(empresa);
        }
    }
}
