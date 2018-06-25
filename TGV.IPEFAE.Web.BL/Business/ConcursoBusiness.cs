using System;
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
        public static bool Excluir(int id)
        {
            return ConcursoData.Excluir(id);
        }

        public static bool Funcao_Excluir(int idFuncao)
        {
            return ConcursoData.Funcao_Excluir(idFuncao);
        }

        public static List<ConcursoFuncaoModel> Funcao_Listar(int idConcurso)
        {
            return ConcursoFuncaoModel.Listar(idConcurso);
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

        public static List<ColaboradorModel> Local_Colaborador_Excluir(int idColaborador)
        {
            return ConcursoData.Local_Colaborador_Excluir(idColaborador);
        }

        public static bool Local_Excluir(int idLocal)
        {
            return ConcursoData.Local_Excluir(idLocal);
        }

        public static ConcursoLocalColaboradorModel Local_Colaborador_Salvar(int idConcursoLocal, ConcursoLocalColaboradorModel clcM)
        {
            clcM.concurso_local_id = idConcursoLocal;

            if (clcM.funcao_id <= 0 && clcM.funcao != null && clcM.funcao.id > 0)
                clcM.funcao_id = clcM.funcao.id;

            if (clcM.colaborador_id <= 0 && clcM.colaborador != null && clcM.colaborador.id > 0)
                clcM.colaborador_id = clcM.colaborador.id;

            return ConcursoData.Local_Colaborador_Salvar(clcM);
        }

        public static ConcursoLocalModel Local_Salvar(int idConcurso, ConcursoLocalModel clM)
        {
            clM.concurso_id = idConcurso;
            return ConcursoData.Local_Salvar(clM);
        }

        public static ConcursoModel Obter(int id, bool ate_colaborador = false)
        {
            return ConcursoData.Obter(id, ate_colaborador);
        }

        public static ConcursoModel Salvar(ConcursoModel cM)
        {
            ConcursoModel retorno = ConcursoData.Salvar(cM);
            return retorno;
        }
    }
}
