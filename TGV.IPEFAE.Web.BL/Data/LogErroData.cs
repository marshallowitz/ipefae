using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TGV.IPEFAE.Web.BL.Data
{
    internal static class LogErroData
    {
        internal static bool Salvar(tb_ler_log_erro logErro)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                tb_ler_log_erro toUpdate = new tb_ler_log_erro();
                db.tb_ler_log_erro.Add(toUpdate);

                toUpdate.ler_idt_log_erro = logErro.ler_idt_log_erro;
                toUpdate.ler_dat_erro = logErro.ler_dat_erro;
                toUpdate.ler_num_codigo_erro = logErro.ler_num_codigo_erro;
                toUpdate.ler_des_inner_exception = logErro.ler_des_inner_exception;
                toUpdate.ler_des_mensagem = logErro.ler_des_mensagem;
                toUpdate.ler_des_nome_metodo = logErro.ler_des_nome_metodo;
                toUpdate.ler_des_source = logErro.ler_des_source;
                toUpdate.ler_des_stack_trace = logErro.ler_des_stack_trace;
                toUpdate.ler_des_tipo_exception = logErro.ler_des_tipo_exception;
                toUpdate.ler_des_url = logErro.ler_des_url;
                toUpdate.usu_idt_usuario = logErro.usu_idt_usuario;

                db.SaveChangesWithErrors();
            }

            return true;
        }
    }
}
