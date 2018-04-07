using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGV.IPEFAE.Web.BL.Data;
using TGV.Framework.Criptografia;

namespace TGV.IPEFAE.Web.BL.Business
{
    public static class UsuarioEstagioBusiness
    {
        public static List<tb_ues_usuario_estagio> Listar(int pagina, bool comPaginacao, int tamanhoPagina, string nome, string curso, int? semAno, bool? estagiando, string cpf, bool visualizacao, string cidade, string ordem)
        {
            if (String.IsNullOrEmpty(nome))
                nome = String.Empty;

            if (String.IsNullOrEmpty(curso))
                curso = String.Empty;

            if (String.IsNullOrEmpty(cpf))
                cpf = String.Empty;

            if (String.IsNullOrEmpty(cidade))
                cidade = String.Empty;

            if (String.IsNullOrEmpty(ordem))
                ordem = "N";

            return UsuarioEstagioData.Listar(pagina, comPaginacao, tamanhoPagina, nome, curso, semAno, estagiando, cpf, visualizacao, cidade, ordem);
        }

        public static tb_ues_usuario_estagio Obter(int ues_idt_usuario_estagio)
        {
            return UsuarioEstagioData.Obter(ues_idt_usuario_estagio);
        }

        public static tb_ues_usuario_estagio ObterPorCPF(string ues_des_cpf)
        {
            tb_ues_usuario_estagio ues = UsuarioEstagioData.ObterPorCPF(ues_des_cpf);

            if (ues == null)
                return null;

            ues.ues_des_senha_descriptografada = TGVCripto.Descriptografar(ues.ues_des_senha, BaseBusiness.ParametroSistema);

            return ues;
        }

        public static tb_ues_usuario_estagio ObterPorEmail(string ues_des_email)
        {
            tb_ues_usuario_estagio ues = UsuarioEstagioData.ObterPorEmail(ues_des_email);

            return ues;
        }

        public static tb_ues_usuario_estagio RealizarLogin(string ues_des_email, string ues_des_senha)
        {
            string senhaCriptografada = ues_des_senha.Criptografar(BaseBusiness.ParametroSistema);

            return UsuarioEstagioData.Obter(ues_des_email, senhaCriptografada);
        }

        public static tb_ues_usuario_estagio Salvar(tb_ues_usuario_estagio usuario, tb_ued_usuario_estagio_dados_escolares dadosEscolares, List<tb_uee_usuario_estagio_experiencia> exps, List<tb_ueo_usuario_estagio_outros> ccs, List<tb_ueo_usuario_estagio_outros> ocs, bool ehAdmin)
        {
            if (!String.IsNullOrEmpty(usuario.ues_des_email))
                return null;

            var us = UsuarioEstagioData.ObterPorEmail(usuario.ues_des_email);

            if (us != null)
                usuario.ues_idt_usuario_estagio = us.ues_idt_usuario_estagio;

            // Salva os dados escolares
            dadosEscolares = UsuarioEstagioData.Salvar(dadosEscolares);

            // Salva o usuário
            if (!String.IsNullOrEmpty(usuario.ues_des_senha))
                usuario.ues_des_senha = usuario.ues_des_senha.Criptografar(BaseBusiness.ParametroSistema);

            return UsuarioEstagioData.Salvar(usuario, dadosEscolares.ued_idt_usuario_estagio_dados_escolares, exps, ccs, ocs, ehAdmin);
        }
    }
}
