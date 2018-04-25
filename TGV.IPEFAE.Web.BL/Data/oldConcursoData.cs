using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace TGV.IPEFAE.Web.BL.Data
{
    internal class oldConcursoData
    {
        private static bool ApagarAnexos(int con_idt_concurso, string diretorioBase)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                List<int> recursos = db.tb_rec_recurso.Where(rec => rec.con_idt_concurso == con_idt_concurso).Select(rec => rec.rec_idt_recurso).ToList();

                foreach (int rec_idt_recurso in recursos)
                {
                    string diretorio = String.Format("{1}/Concurso/{0}", con_idt_concurso, diretorioBase);
                    System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(diretorio);

                    directory.Empty();
                }

                // Apaga os anexos do concurso
                db.DeleteWhere<tb_can_concurso_anexo>(can => can.con_idt_concurso == con_idt_concurso);
            }

            return true;
        }

        private static tb_con_concurso EagerLoading(IPEFAEEntities db, IQueryable<tb_con_concurso> cons, bool obterRecursos = true)
        {
            foreach (tb_con_concurso con in cons)
            {
                db.Entry(con).Collection(c => c.tb_rec_recurso).Load();

                db.Entry(con).Collection(c => c.tb_can_concurso_anexo).Load();
                db.Entry(con).Reference(c => c.tb_emp_empresa).Load();
                db.Entry(con).Collection(c => c.tb_cco_cargo_concurso).Load();

                if (obterRecursos)
                {
                    foreach (tb_rec_recurso rec in con.tb_rec_recurso)
                    {
                        db.Entry(rec).Reference(r => r.tb_ico_inscrito_concurso).Load();
                        db.Entry(rec).Reference(r => r.tb_usu_usuario).Load();
                    }
                }
            }
            // .Include("tb_can_concurso_anexo").Include("tb_emp_empresa").Include("tb_cco_cargo_concurso").Include("tb_rec_recurso.tb_ico_inscrito_concurso").Include("tb_rec_recurso.tb_usu_usuario")

            return cons.SingleOrDefault();
        }

        internal static bool ApagarConcurso(int con_idt_concurso, string diretorioBase)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                using (IPEFAEEntities db = BaseData.Contexto)
                {
                    // Apaga os recurso do concurso
                    RecursoData.Apagar(con_idt_concurso, diretorioBase);

                    db.Database.ExecuteSqlCommand(@"
                        DELETE FROM tb_rec_recurso WHERE con_idt_concurso = @p0;
                        DELETE FROM tb_can_concurso_anexo WHERE con_idt_concurso = @p0;
                        DELETE FROM tb_cci_concurso_cargo_inscrito WHERE con_idt_concurso = @p0;
                        DELETE icl FROM tb_icl_inscrito_classificacao icl INNER JOIN tb_ico_inscrito_concurso ico ON icl.ico_idt_inscrito_concurso = ico.ico_idt_inscrito_concurso WHERE ico.con_idt_concurso = @p0;
                        DELETE idt FROM tb_idt_inscrito_dados_prova idt INNER JOIN tb_ico_inscrito_concurso ico ON idt.ico_idt_inscrito_concurso = ico.ico_idt_inscrito_concurso WHERE ico.con_idt_concurso = @p0;
                        DELETE icv FROM tb_icv_inscrito_concurso_vestibular icv INNER JOIN tb_ico_inscrito_concurso ico ON icv.ico_idt_inscrito_concurso = ico.ico_idt_inscrito_concurso WHERE ico.con_idt_concurso = @p0;
                        DELETE FROM tb_ico_inscrito_concurso WHERE con_idt_concurso = @p0;
                        DELETE FROM tb_cco_cargo_concurso WHERE con_idt_concurso = @p0;
                        DELETE FROM tb_con_concurso WHERE con_idt_concurso = @p0;
                    ", new SqlParameter("p0", con_idt_concurso));
                }

                scope.Complete();
            }

            return true;
        }

        internal static bool ApagarConcursosAntigos(List<int> concursos, string diretorioBase)
        {
            TransactionOptions options = new TransactionOptions();
            options.Timeout = TimeSpan.FromMinutes(1);
            options.IsolationLevel = IsolationLevel.Snapshot;

            using (System.Transactions.TransactionScope scope = new System.Transactions.TransactionScope(TransactionScopeOption.Required, options))
            {
                using (IPEFAEEntities db = BaseData.Contexto)
                {
                    foreach (int con_idt_concurso in concursos)
                    {
                        ApagarConcurso(con_idt_concurso, diretorioBase);
                    }
                }

                scope.Complete();
            }

            return true;
        }

        internal static bool EditarAtivacao(int cco_idt_cargo_concurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                tb_cco_cargo_concurso toUpdate = db.tb_cco_cargo_concurso.SingleOrDefault(cco => cco.cco_idt_cargo_concurso == cco_idt_cargo_concurso);

                if (toUpdate == null)
                    return false;

                toUpdate.cco_bit_ativo = !toUpdate.cco_bit_ativo;
                db.SaveChangesWithErrors();

                return true;
            }
        }

        internal static List<tb_con_concurso> Listar()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                var cs =
                    (from con in db.tb_con_concurso.Include("tb_rec_recurso").Include("tb_cco_cargo_concurso").Include("tb_emp_empresa")
                     orderby con.con_dat_concurso descending
                     select con);

                foreach (tb_con_concurso concurso in cs)
                {
                    concurso.TotalInscritos = db.Entry(concurso).Collection(con => con.tb_ico_inscrito_concurso).Query().Count(ico => ico.con_idt_concurso == concurso.con_idt_concurso && ico.ico_bit_ativo);
                    concurso.TotalPagos = db.Entry(concurso).Collection(con => con.tb_ico_inscrito_concurso).Query().Count(ico => ico.con_idt_concurso == concurso.con_idt_concurso && ico.ico_bit_ativo && ico.ico_bit_pago);
                }

                return cs.ToList();
            }
        }

        internal static List<tb_con_concurso> ListarSomenteConcursos(bool considerarAtivo = true)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from con in db.tb_con_concurso.Include("tb_cco_cargo_concurso")
                        where (!considerarAtivo || con.con_bit_ativo)
                        orderby con.con_dat_concurso descending
                        select con).ToList();
            }
        }

        internal static List<tb_cco_cargo_concurso> ListarCargos(int con_idt_concurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from cco in db.tb_cco_cargo_concurso.Include("tb_cci_concurso_cargo_inscrito")
                        where cco.cco_bit_ativo
                        && cco.con_idt_concurso == con_idt_concurso
                        orderby cco.cco_nom_cargo_concurso
                        select cco).ToList();
            }
        }

        internal static List<tb_cco_cargo_concurso> ListarCargosPorId(int cco_idt_cargo_concurso_1, int cco_idt_cargo_concurso_2)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from cco in db.tb_cco_cargo_concurso.Include("tb_cci_concurso_cargo_inscrito")
                        where cco.cco_bit_ativo
                        && (cco.cco_idt_cargo_concurso == cco_idt_cargo_concurso_1 || cco.cco_idt_cargo_concurso == cco_idt_cargo_concurso_2)
                        select cco).ToList();
            }
        }

        internal static tb_con_concurso Obter(int con_idt_concurso, bool considerarAtivo = true, bool obterRecursos = true)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                db.Configuration.ProxyCreationEnabled = false;

                var c = (from con in db.tb_con_concurso
                        where con.con_idt_concurso == con_idt_concurso
                        && (!considerarAtivo || con.con_bit_ativo)
                        select con);

                return EagerLoading(db, c, obterRecursos);
            }
        }

        internal static tb_icl_inscrito_classificacao ObterClassificacao(int ico_idt_inscrito_concurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from icl in db.tb_icl_inscrito_classificacao
                        where icl.icl_bit_ativo
                        && icl.ico_idt_inscrito_concurso == ico_idt_inscrito_concurso
                        && icl.icl_des_situacao != null
                        select icl).SingleOrDefault();
            }
        }

        internal static tb_idt_inscrito_dados_prova ObterDadosProva(int ico_idt_inscrito_concurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from idp in db.tb_idt_inscrito_dados_prova
                        where idp.idt_bit_ativo
                        && idp.ico_idt_inscrito_concurso == ico_idt_inscrito_concurso
                        && idp.idt_dat_prova.HasValue
                        select idp).SingleOrDefault();
            }
        }

        internal static tb_cco_cargo_concurso ObterCargoInscritoCandidato(int ico_idt_inscrito_concurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from cco in db.tb_cco_cargo_concurso
                        join cci in db.tb_cci_concurso_cargo_inscrito on cco.cco_idt_cargo_concurso equals cci.cco_idt_cargo_concurso
                        where cco.cco_bit_ativo
                        && cci.ico_idt_inscrito_concurso == ico_idt_inscrito_concurso
                        select cco).FirstOrDefault();
            }
        }

        internal static tb_con_concurso Salvar(tb_con_concurso concurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                tb_con_concurso toUpdate = db.tb_con_concurso.SingleOrDefault(con => con.con_idt_concurso == concurso.con_idt_concurso);

                if (toUpdate != null)
                    toUpdate.con_idt_concurso = concurso.con_idt_concurso;
                else
                {
                    toUpdate = new tb_con_concurso();
                    db.tb_con_concurso.Add(toUpdate);
                }

                toUpdate.con_bit_ativo = concurso.con_bit_ativo;
                toUpdate.con_bit_encerrado = concurso.con_bit_encerrado;
                toUpdate.con_bit_inscricao_online = concurso.con_bit_inscricao_online;
                toUpdate.con_nom_concurso = concurso.con_nom_concurso;
                toUpdate.con_dat_concurso = concurso.con_dat_concurso;
                toUpdate.con_dat_encerramento_inscricoes = concurso.con_dat_encerramento_inscricoes;
                toUpdate.con_dat_encerramento = concurso.con_dat_encerramento;
                toUpdate.con_dat_inicio_comprovante = concurso.con_dat_inicio_comprovante;
                toUpdate.con_dat_encerramento_comprovante = concurso.con_dat_encerramento_comprovante;
                toUpdate.con_dat_inicio_isento = concurso.con_dat_inicio_isento;
                toUpdate.con_dat_encerramento_isento = concurso.con_dat_encerramento_isento;
                toUpdate.con_dat_inicio_classificacao = concurso.con_dat_inicio_classificacao;
                toUpdate.con_dat_boleto = concurso.con_dat_boleto;
                toUpdate.emp_idt_empresa = concurso.emp_idt_empresa;
                toUpdate.tlc_idt_tipo_layout_concurso = concurso.tlc_idt_tipo_layout_concurso;

                db.SaveChangesWithErrors();

                concurso = toUpdate;
            }

            return concurso;
        }

        internal static void SalvarAnexos(List<tb_can_concurso_anexo> anexos, int con_idt_concurso = 0)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                foreach (tb_can_concurso_anexo anexo in anexos)
                {
                    con_idt_concurso = anexo.con_idt_concurso;

                    tb_can_concurso_anexo toUpdate = db.tb_can_concurso_anexo.SingleOrDefault(can => can.can_idt_concurso_anexo == anexo.can_idt_concurso_anexo);

                    if (toUpdate != null)
                        toUpdate.can_idt_concurso_anexo = anexo.can_idt_concurso_anexo;
                    else
                    {
                        toUpdate = new tb_can_concurso_anexo();
                        db.tb_can_concurso_anexo.Add(toUpdate);
                    }

                    toUpdate.can_bit_ativo = anexo.can_bit_ativo;
                    toUpdate.can_bit_tem_recurso = anexo.can_bit_tem_recurso;
                    toUpdate.can_dat_fim_recurso = anexo.can_dat_fim_recurso;
                    toUpdate.can_dat_inicio_recurso = anexo.can_dat_inicio_recurso;
                    toUpdate.can_dat_publicacao = anexo.can_dat_publicacao;
                    toUpdate.can_des_path_arquivo = anexo.can_des_path_arquivo;
                    toUpdate.can_nom_concurso_anexo = anexo.can_nom_concurso_anexo;
                    toUpdate.con_idt_concurso = anexo.con_idt_concurso;
                    toUpdate.tca_idt_tipo_concurso_anexo = anexo.tca_idt_tipo_concurso_anexo;

                    db.SaveChangesWithErrors();

                    anexo.can_idt_concurso_anexo = toUpdate.can_idt_concurso_anexo;
                }

                List<int> ids = anexos.Select(ane => ane.can_idt_concurso_anexo).ToList();

                db.DeleteWhere<tb_can_concurso_anexo>(can => can.con_idt_concurso == con_idt_concurso && !ids.Contains(can.can_idt_concurso_anexo));
            }
        }

        internal static void SalvarCargos(List<tb_cco_cargo_concurso> cargos)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                int con_idt_concurso = 0;

                foreach (tb_cco_cargo_concurso cargo in cargos)
                {
                    con_idt_concurso = cargo.con_idt_concurso;

                    tb_cco_cargo_concurso toUpdate = db.tb_cco_cargo_concurso.SingleOrDefault(cco => cco.cco_idt_cargo_concurso == cargo.cco_idt_cargo_concurso);

                    if (toUpdate != null)
                        toUpdate.cco_idt_cargo_concurso = cargo.cco_idt_cargo_concurso;
                    else
                    {
                        toUpdate = new tb_cco_cargo_concurso();
                        db.tb_cco_cargo_concurso.Add(toUpdate);
                    }

                    toUpdate.cco_bit_ativo = cargo.cco_bit_ativo;
                    toUpdate.cco_nom_cargo_concurso = cargo.cco_nom_cargo_concurso;
                    toUpdate.cco_num_valor_inscricao = cargo.cco_num_valor_inscricao;
                    toUpdate.con_idt_concurso = cargo.con_idt_concurso;

                    db.SaveChangesWithErrors();

                    cargo.cco_idt_cargo_concurso = toUpdate.cco_idt_cargo_concurso;
                }

                List<int> ids = cargos.Select(car => car.cco_idt_cargo_concurso).ToList();
                db.DeleteWhere<tb_cco_cargo_concurso>(cco => cco.con_idt_concurso == con_idt_concurso && !ids.Contains(cco.cco_idt_cargo_concurso));
            }
        }

        internal static bool SalvarDadosProvaClassificacao(List<tb_ico_inscrito_concurso> inscritos, List<tb_idt_inscrito_dados_prova> dadosProva, List<tb_icl_inscrito_classificacao> classificacoes)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                foreach (tb_ico_inscrito_concurso inscrito in inscritos)
                {
                    tb_ico_inscrito_concurso toUpdate = db.tb_ico_inscrito_concurso.SingleOrDefault(ico => ico.ico_idt_inscrito_concurso == inscrito.ico_idt_inscrito_concurso);

                    if (toUpdate == null)
                        continue;

                    toUpdate.ico_idt_inscrito_concurso = inscrito.ico_idt_inscrito_concurso;
                    toUpdate.ico_nom_inscrito_concurso = inscrito.ico_nom_inscrito_concurso;
                    toUpdate.ico_dat_nascimento = inscrito.ico_dat_nascimento;
                    toUpdate.ico_des_rg = inscrito.ico_des_rg;
                    toUpdate.est_idt_estado_rg = inscrito.est_idt_estado_rg;

                    db.SaveChangesWithErrors();
                }

                foreach (tb_idt_inscrito_dados_prova dadoProva in dadosProva)
                {
                    tb_ico_inscrito_concurso inscrito = db.tb_ico_inscrito_concurso.SingleOrDefault(ico => ico.ico_idt_inscrito_concurso == dadoProva.ico_idt_inscrito_concurso);

                    if (inscrito == null)
                        continue;

                    tb_idt_inscrito_dados_prova toUpdate = db.tb_idt_inscrito_dados_prova.SingleOrDefault(idt => idt.ico_idt_inscrito_concurso == dadoProva.ico_idt_inscrito_concurso);

                    if (toUpdate != null)
                        toUpdate.ico_idt_inscrito_concurso = dadoProva.ico_idt_inscrito_concurso;
                    else
                    {
                        toUpdate = new tb_idt_inscrito_dados_prova();
                        toUpdate.ico_idt_inscrito_concurso = dadoProva.ico_idt_inscrito_concurso;
                        db.tb_idt_inscrito_dados_prova.Add(toUpdate);
                    }

                    toUpdate.idt_bit_ativo = dadoProva.idt_bit_ativo;
                    toUpdate.idt_dat_prova = dadoProva.idt_dat_prova;
                    toUpdate.idt_des_andar = dadoProva.idt_des_andar;
                    toUpdate.idt_des_bairro = dadoProva.idt_des_bairro;
                    toUpdate.idt_des_cep = dadoProva.idt_des_cep;
                    toUpdate.idt_des_cidade = dadoProva.idt_des_cidade;
                    toUpdate.idt_des_endereco = dadoProva.idt_des_endereco;
                    toUpdate.idt_des_local = dadoProva.idt_des_local;
                    toUpdate.idt_des_numero = dadoProva.idt_des_numero;
                    toUpdate.idt_des_sala = dadoProva.idt_des_sala;

                    db.SaveChangesWithErrors();
                }

                foreach (tb_icl_inscrito_classificacao classificacao in classificacoes)
                {
                    tb_ico_inscrito_concurso inscrito = db.tb_ico_inscrito_concurso.SingleOrDefault(ico => ico.ico_idt_inscrito_concurso == classificacao.ico_idt_inscrito_concurso);

                    if (inscrito == null)
                        continue;

                    tb_icl_inscrito_classificacao toUpdate = db.tb_icl_inscrito_classificacao.SingleOrDefault(idt => idt.ico_idt_inscrito_concurso == classificacao.ico_idt_inscrito_concurso);

                    if (toUpdate != null)
                        toUpdate.ico_idt_inscrito_concurso = classificacao.ico_idt_inscrito_concurso;
                    else
                    {
                        toUpdate = new tb_icl_inscrito_classificacao();
                        toUpdate.ico_idt_inscrito_concurso = classificacao.ico_idt_inscrito_concurso;
                        db.tb_icl_inscrito_classificacao.Add(toUpdate);
                    }

                    toUpdate.icl_bit_ativo = classificacao.icl_bit_ativo;
                    toUpdate.icl_des_situacao = classificacao.icl_des_situacao;
                    toUpdate.icl_num_ce = classificacao.icl_num_ce;
                    toUpdate.icl_num_cg = classificacao.icl_num_cg;
                    toUpdate.icl_num_nota = classificacao.icl_num_nota;
                    toUpdate.icl_num_posicao = classificacao.icl_num_posicao;
                    toUpdate.icl_num_pp = classificacao.icl_num_pp;
                    toUpdate.icl_num_tit = classificacao.icl_num_tit;

                    db.SaveChangesWithErrors();
                }
            }

            return true;
        }
    }
}
