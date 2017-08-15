using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGV.IPEFAE.Web.BL.Data
{
    internal class EmpresaData
    {
        internal static tb_emp_empresa EditarAtivacao(int emp_idt_empresa)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                tb_emp_empresa toUpdate = db.tb_emp_empresa.SingleOrDefault(emp => emp.emp_idt_empresa == emp_idt_empresa);

                if (toUpdate == null)
                    return null;

                toUpdate.emp_bit_ativo = !toUpdate.emp_bit_ativo;
                db.SaveChangesWithErrors();

                return toUpdate;
            }
        }

        internal static List<tb_emp_empresa> Listar(bool considerarAtivo = true)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from emp in db.tb_emp_empresa
                        where (!considerarAtivo || emp.emp_bit_ativo)
                        orderby emp.emp_nom_empresa
                        select emp).ToList();
            }
        }

        internal static tb_emp_empresa Obter(int emp_idt_empresa)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from emp in db.tb_emp_empresa
                        where emp.emp_idt_empresa == emp_idt_empresa
                        select emp).SingleOrDefault();
            }
        }

        internal static tb_emp_empresa Salvar(tb_emp_empresa empresa)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                tb_emp_empresa toUpdate = db.tb_emp_empresa.SingleOrDefault(emp => emp.emp_idt_empresa == empresa.emp_idt_empresa);

                if (toUpdate != null)
                    toUpdate.emp_idt_empresa = empresa.emp_idt_empresa;
                else
                {
                    toUpdate = new tb_emp_empresa();
                    db.tb_emp_empresa.Add(toUpdate);
                }

                toUpdate.emp_bit_ativo = empresa.emp_bit_ativo;
                toUpdate.emp_des_agencia = empresa.emp_des_agencia;
                toUpdate.emp_des_cnpj = empresa.emp_des_cnpj;
                toUpdate.emp_des_conta_corrente = empresa.emp_des_conta_corrente;
                toUpdate.emp_des_razao_social = empresa.emp_des_razao_social;
                toUpdate.emp_nom_empresa = empresa.emp_nom_empresa;
                toUpdate.emp_num_banco = empresa.emp_num_banco;
                toUpdate.emp_num_convenio = empresa.emp_num_convenio;
                toUpdate.emp_num_convenio_cobranca = empresa.emp_num_convenio_cobranca;

                db.SaveChangesWithErrors();

                empresa = toUpdate;
            }

            return empresa;
        }
    }
}
