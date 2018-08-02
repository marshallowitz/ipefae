using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGV.Framework.Criptografia;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.BL.Business
{
    public class ColaboradorBusiness
    {
        public static List<ColaboradorModel> Listar()
        {
            return ColaboradorData.Listar();
        }

        public static List<ColaboradorModel> ListarPorConcurso(int idConcurso, int inicio, int total)
        {
            ConcursoModel concurso = ConcursoBusiness.Obter(idConcurso, true);

            if (concurso == null)
                return new List<ColaboradorModel>();

            List<ColaboradorModel> colaboradores = new List<ColaboradorModel>();

            foreach (var local in concurso.locais)
            {
                foreach (var col in local.Colaboradores)
                {
                    colaboradores.Add(col.colaborador);
                }
            }

            if (inicio > 0)
                inicio--;

            return colaboradores.Skip(inicio).Take(total).ToList();
        }

        public static dynamic ListarPorConcursoV2(ConcursoModel concurso)
        {
            if (concurso == null)
                return new List<ColaboradorModel>();

            List<ColaboradorModel> colaboradores = new List<ColaboradorModel>();
            List<ConcursoLocalColaboradorModel> cLocaisColaboradores = new List<ConcursoLocalColaboradorModel>();

            foreach (var local in concurso.locais)
            {
                foreach (var col in local.Colaboradores)
                {
                    cLocaisColaboradores.Add(col);
                    colaboradores.Add(col.colaborador);
                }
            }

            dynamic result = new ExpandoObject();
            result.Colaboradores = colaboradores;
            result.Concurso = concurso;
            result.LocaisColaboradores = cLocaisColaboradores;

            return result;
        }

        public static ColaboradorModel Obter(int id)
        {
            ColaboradorModel colaborador = ColaboradorData.Obter(id);

            if (id > 0 && !String.IsNullOrEmpty(colaborador.senha))
                colaborador.senhaDescriptografada = colaborador.senha.Descriptografar(BaseBusiness.ParametroSistema);

            return colaborador;
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

        public static ColaboradorModel ObterPorEmail(string email)
        {
            ColaboradorModel cM = ColaboradorData.ObterPorEmail(email);

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
