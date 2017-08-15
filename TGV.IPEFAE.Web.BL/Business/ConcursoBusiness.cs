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
        public static bool ApagarConcursosAntigos(int diasExclusao, string diretorioBase)
        {
            // Busca todos os concursos cuja data de encerramento seja maior do que este período
            List<int> concursos = ConcursoBusiness.ListarSomenteConcursos(false)
                                    //.Where(con => con.con_dat_encerramento.HasValue && con.con_dat_encerramento.Value.AddDays(diasExclusao) <= BaseBusiness.DataAgora)
                                    .Where(con => con.con_idt_concurso == 3)
                                    .Select(con => con.con_idt_concurso).ToList();

            //return ConcursoData.ApagarConcursosAntigos(concursos, diretorioBase);
            return true;
        }

        public static bool EditarAtivacao(int cco_idt_cargo_concurso)
        {
            return ConcursoData.EditarAtivacao(cco_idt_cargo_concurso);
        }

        public static bool Excluir(int con_idt_concurso, string diretorioBase)
        {
            ConcursoData.ApagarConcurso(con_idt_concurso, diretorioBase);

            return true;
        }

        public static List<tb_con_concurso> Listar()
        {
            return ConcursoData.Listar();
        }

        public static List<tb_con_concurso> ListarSomenteConcursos(bool considerarAtivo = true)
        {
            return ConcursoData.ListarSomenteConcursos(considerarAtivo);
        }

        public static List<tb_cco_cargo_concurso> ListarCargos(int con_idt_concurso)
        {
            return ConcursoData.ListarCargos(con_idt_concurso);
        }

        public static List<tb_cco_cargo_concurso> ListarCargosPorId(int? cco_idt_cargo_concurso_1, int? cco_idt_cargo_concurso_2)
        {
            if (!cco_idt_cargo_concurso_1.HasValue && !cco_idt_cargo_concurso_2.HasValue)
                return new List<tb_cco_cargo_concurso>();

            int opcao1 = cco_idt_cargo_concurso_1.HasValue ? cco_idt_cargo_concurso_1.Value : 0;
            int opcao2 = cco_idt_cargo_concurso_2.HasValue ? cco_idt_cargo_concurso_2.Value : 0;

            return ConcursoData.ListarCargosPorId(opcao1, opcao2);
        }

        public static tb_con_concurso Obter(int con_idt_concurso, bool considerarAtivo = true, bool obterRecursos = true)
        {
            return ConcursoData.Obter(con_idt_concurso, considerarAtivo, obterRecursos);
        }

        public static tb_cco_cargo_concurso ObterCargoInscritoCandidato(int ico_idt_inscrito_concurso)
        {
            return ConcursoData.ObterCargoInscritoCandidato(ico_idt_inscrito_concurso);
        }

        public static tb_icl_inscrito_classificacao ObterClassificacao(int ico_idt_inscrito_concurso)
        {
            return ConcursoData.ObterClassificacao(ico_idt_inscrito_concurso);
        }

        public static tb_idt_inscrito_dados_prova ObterDadosProva(int ico_idt_inscrito_concurso)
        {
            return ConcursoData.ObterDadosProva(ico_idt_inscrito_concurso);
        }

        public static tb_con_concurso Salvar(tb_con_concurso concurso, List<tb_can_concurso_anexo> anexos, List<tb_cco_cargo_concurso> cargos)
        {
            concurso = ConcursoData.Salvar(concurso);

            anexos.ForEach(can => can.con_idt_concurso = concurso.con_idt_concurso);
            ConcursoData.SalvarAnexos(anexos, concurso.con_idt_concurso);

            cargos.ForEach(cco => cco.con_idt_concurso = concurso.con_idt_concurso);
            ConcursoData.SalvarCargos(cargos);

            return concurso;
        }

        public static bool Salvar(List<tb_ico_inscrito_concurso> inscritos, List<tb_idt_inscrito_dados_prova> dadosProva, List<tb_icl_inscrito_classificacao> classificacoes)
        {
            return ConcursoData.SalvarDadosProvaClassificacao(inscritos, dadosProva, classificacoes);
        }
    }
}
