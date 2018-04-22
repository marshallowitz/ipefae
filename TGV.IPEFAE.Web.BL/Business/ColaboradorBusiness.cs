using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGV.Framework.Criptografia;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.BL.Business
{
    public class ColaboradorBusiness
    {
        public static ColaboradorModel Obter(int id)
        {
            return ColaboradorData.Obter(id);
        }

        public static ColaboradorModel ObterPorCPF(string cpf)
        {
            cpf = BaseBusiness.OnlyNumbers(cpf);

            ColaboradorModel cM = ColaboradorData.ObterPorCPF(cpf);

            if (cM == null)
                return null;

            cM.senhaDescriptografada = BaseBusiness.Descriptografar(cM.senha);

            return cM;
        }

        public static ColaboradorModel RealizarLogin(string email, string senha)
        {
            string senhaCriptografada = senha.Criptografar(BaseBusiness.ParametroSistema);

            return ColaboradorData.ObterPorEmailSenha(email, senhaCriptografada);
        }

        public static ColaboradorModel Salvar(ColaboradorModel cM)
        {
            // Valida salvar por e-mail
            if (String.IsNullOrEmpty(cM.email))
                return null;

            var us = ColaboradorData.ObterPorEmail(cM.email);

            if (us != null && cM.id != us.id)
                return null;

            // Valida salvar por cpf
            if (String.IsNullOrEmpty(cM.cpf))
                return null;

            us = ObterPorCPF(cM.cpf);

            if (us != null && cM.id != us.id)
                return null;

            // Se for criação, criptografa a senha
            if (!String.IsNullOrEmpty(cM.senha))
                cM.senha = cM.senha.Criptografar(BaseBusiness.ParametroSistema);

            ColaboradorModel retorno = ColaboradorData.Salvar(cM);
            retorno.senhaDescriptografada = retorno.senha.Descriptografar(BaseBusiness.ParametroSistema);
            return retorno;
        }
    }
}
