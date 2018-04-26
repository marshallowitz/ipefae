﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGV.IPEFAE.Web.BL.Data;
using TGV.Framework.Criptografia;

namespace TGV.IPEFAE.Web.BL.Business
{
    public static class ConcursoBusiness
    {
        public static bool Funcao_Excluir(int idFuncao)
        {
            return ConcursoData.Funcao_Excluir(idFuncao);
        }

        public static ConcursoFuncaoModel Funcao_Salvar(int idConcurso, ConcursoFuncaoModel cfM)
        {
            cfM.concurso_id = idConcurso;
            return ConcursoData.Funcao_Salvar(cfM);
        }

        public static List<ConcursoModel> Listar()
        {
            return ConcursoData.Listar();
        }

        public static bool Local_Colaborador_Excluir(int idConcursoLocal, int idColaborador)
        {
            return ConcursoData.Local_Colaborador_Excluir(idConcursoLocal, idColaborador);
        }

        public static bool Local_Excluir(int idLocal)
        {
            return ConcursoData.Local_Excluir(idLocal);
        }

        public static ConcursoLocalColaboradorModel Local_Colaborador_Salvar(int idConcurso, ConcursoLocalColaboradorModel clcM)
        {
            return ConcursoData.Local_Colaborador_Salvar(clcM);
        }

        public static ConcursoLocalModel Local_Salvar(int idConcurso, ConcursoLocalModel clM)
        {
            clM.concurso_id = idConcurso;
            return ConcursoData.Local_Salvar(clM);
        }

        public static ConcursoModel Obter(int id)
        {
            return ConcursoData.Obter(id);
        }

        public static ConcursoModel Salvar(ConcursoModel cM)
        {
            ConcursoModel retorno = ConcursoData.Salvar(cM);
            return retorno;
        }
    }
}
