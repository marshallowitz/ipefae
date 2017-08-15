using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.BL.Business
{
    public static class LogErroBusiness
    {
        private static int? ObterCodigoException(Exception e)
        {
            System.Web.HttpException hE = e as System.Web.HttpException;

            return hE != null ? hE.GetHttpCode() : (int?)null;
        }

        public static void Salvar(Exception ex, int? idUsuarioLogado)
        {
            Salvar(ex, idUsuarioLogado, String.Empty);
        }

        public static void Salvar(Exception ex, int? idUsuarioLogado, string nomeMetodo)
        {
            Int64 idErro = Convert.ToInt64(BaseBusiness.DataAgora.ToString("yyyyMMddHHmmssfff"));
            string url = BaseBusiness.ContextoCorrente != null ? BaseBusiness.ContextoCorrente.Request.Url.AbsolutePath : null;

            tb_ler_log_erro logErro = new tb_ler_log_erro()
            {
                ler_idt_log_erro = idErro,
                ler_dat_erro = BaseBusiness.DataAgora,
                ler_num_codigo_erro = ObterCodigoException(ex),
                ler_des_nome_metodo = ex?.TargetSite?.Name??nomeMetodo,
                ler_des_source = ex?.Source,
                ler_des_inner_exception = ex?.InnerException?.Message,
                ler_des_mensagem = ex?.Message,
                ler_des_stack_trace = ex?.StackTrace,
                ler_des_tipo_exception = ex?.GetType().ToString(),
                ler_des_url = url,
                usu_idt_usuario = idUsuarioLogado
            };

            LogErroBusiness.Salvar(logErro);
        }

        public static void Salvar(tb_ler_log_erro logErro)
        {
            LogErroData.Salvar(logErro);
        }
    }
}
