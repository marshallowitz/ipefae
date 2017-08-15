using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGV.IPEFAE.Web.BL.Data;
using TGV.Framework.Criptografia;

namespace TGV.IPEFAE.Web.BL.Business
{
    public static class UsuarioBusiness
    {
        public static tb_usu_usuario EditarAtivacao(int usu_idt_usuario)
        {
            return UsuarioData.EditarAtivacao(usu_idt_usuario);
        }

        public static List<tb_usu_usuario> Listar(int per_idt_permissao)
        {
            return UsuarioData.Listar(per_idt_permissao);
        }

        public static tb_usu_usuario Obter(int usu_idt_usuario)
        {
            return UsuarioData.Obter(usu_idt_usuario);
        }

        public static tb_usu_usuario Obter(string usu_des_email)
        {
            return UsuarioData.Obter(usu_des_email);
        }

        public static tb_usu_usuario RealizarLogin(string usu_des_email, string usu_des_senha)
        {
            string senhaCriptografada = usu_des_senha.Criptografar(BaseBusiness.ParametroSistema);

            return UsuarioData.Obter(usu_des_email, senhaCriptografada);
        }

        public static tb_usu_usuario Salvar(tb_usu_usuario usuario)
        {
            if (!String.IsNullOrEmpty(usuario.usu_des_senha))
                usuario.usu_des_senha = usuario.usu_des_senha.Criptografar(BaseBusiness.ParametroSistema);

            return UsuarioData.Salvar(usuario);
        }
    }
}
