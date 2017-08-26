using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGV.IPEFAE.Web.BL.Data;
using TGV.Framework.Criptografia;

namespace TGV.IPEFAE.Web.BL.Business
{
    public static class InscritoConcursoBusiness
    {
        public static List<spr_tgv_gerar_lista_inscritos_Result> GerarListaInscritos(int con_idt_concurso)
        {
            return InscritoConcursoData.GerarListaInscritos(con_idt_concurso);
        }

        public static List<tb_ico_inscrito_concurso> Listar(int con_idt_concurso)
        {
            return InscritoConcursoData.Listar(con_idt_concurso);
        }

        public static List<tb_ico_inscrito_concurso> Listar(int con_idt_concurso, int pagina, bool comPaginacao, int tamanhoPagina, string ordem, int? matricula, string nome, string cpf, bool? ativo, bool? isento)
        {
            if (!matricula.HasValue)
                matricula = 0;

            if (String.IsNullOrEmpty(nome))
                nome = String.Empty;

            if (String.IsNullOrEmpty(cpf))
                cpf = String.Empty;

            if (String.IsNullOrEmpty(ordem))
                ordem = "N";

            return InscritoConcursoData.Listar(con_idt_concurso, pagina, comPaginacao, tamanhoPagina, ordem, matricula.Value, nome, cpf, ativo, isento);
        }

        public static List<tb_ico_inscrito_concurso> Listar(int con_idt_concurso, string ico_des_cpf)
        {
            return InscritoConcursoData.Listar(con_idt_concurso, ico_des_cpf);
        }

        public static List<tb_ico_inscrito_concurso> ListarConsiderandoPeriodoInscricoes(int con_idt_concurso, string ico_des_cpf, bool dentroPeriodoInscricoes)
        {
            return InscritoConcursoData.ListarConsiderandoPeriodoInscricoes(con_idt_concurso, ico_des_cpf, dentroPeriodoInscricoes);
        }

        public static List<tb_ico_inscrito_concurso_extension> ListarNaoPagos(int con_idt_concurso)
        {
            return InscritoConcursoData.ListarNaoPagos(con_idt_concurso);
        }

        public static List<tb_ico_inscrito_concurso_extension> ListarPagos(int con_idt_concurso)
        {
            return InscritoConcursoData.ListarPagos(con_idt_concurso);
        }

        public static List<tb_ico_inscrito_concurso> ListarUltimosInscritos(int con_idt_concurso)
        {
            return InscritoConcursoData.ListarUltimosInscritos(con_idt_concurso);
        }

        public static tb_ico_inscrito_concurso Obter(int ico_idt_inscrito_concurso)
        {
            return InscritoConcursoData.Obter(ico_idt_inscrito_concurso);
        }

        public static tb_icv_inscrito_concurso_vestibular ObterVestibular(int ico_idt_inscrito_concurso)
        {
            return InscritoConcursoData.ObterVestibular(ico_idt_inscrito_concurso);
        }

        public static tb_ico_inscrito_concurso SalvarLinkBoleto(int ico_idt_inscrito_concurso, string linkBoleto)
        {
            tb_ico_inscrito_concurso inscrito = Obter(ico_idt_inscrito_concurso);

            if (inscrito == null)
                return null;

            inscrito.ico_des_link_boleto = linkBoleto;

            return Salvar(inscrito);
        }

        public static tb_ico_inscrito_concurso Salvar(tb_ico_inscrito_concurso inscrito)
        {
            return InscritoConcursoData.Salvar(inscrito);
        }

        public static tb_ico_inscrito_concurso Salvar(tb_ico_inscrito_concurso inscrito, int idCargo)
        {
            return InscritoConcursoData.Salvar(inscrito, idCargo);
        }

        public static tb_icv_inscrito_concurso_vestibular Salvar(tb_icv_inscrito_concurso_vestibular inscrito)
        {
            return InscritoConcursoData.Salvar(inscrito);
        }

        public static tb_ico_inscrito_concurso Salvar(tb_ico_inscrito_concurso inscrito, List<tb_cci_concurso_cargo_inscrito> ccis)
        {
            tb_ico_inscrito_concurso ico = InscritoConcursoData.Salvar(inscrito);
            ccis = InscritoConcursoData.SalvarCCIs(ccis, ico.ico_idt_inscrito_concurso);
            ico.tb_cci_concurso_cargo_inscrito = ccis;

            return ico;
        }
    }
}
