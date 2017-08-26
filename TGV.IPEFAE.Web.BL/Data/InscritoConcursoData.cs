using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGV.IPEFAE.Web.BL.Data
{
    internal class InscritoConcursoData
    {
        private static List<tb_ico_inscrito_concurso> EagerLoading(IPEFAEEntities db, IQueryable<tb_ico_inscrito_concurso> icos)
        {
            foreach (tb_ico_inscrito_concurso ico in icos)
            {
                db.Entry(ico).Reference(i => i.tb_est_estado).Load();
                db.Entry(ico).Reference(i => i.tb_con_concurso).Load();
                db.Entry(ico).Collection(i => i.tb_cci_concurso_cargo_inscrito).Load();

                db.Entry(ico.tb_con_concurso).Collection(c => c.tb_cco_cargo_concurso).Load();

                foreach (tb_cci_concurso_cargo_inscrito cci in ico.tb_cci_concurso_cargo_inscrito)
                {
                    db.Entry(cci).Reference(c => c.tb_cco_cargo_concurso).Load();
                }

                foreach (tb_ico_inscrito_concurso ico2 in ico.tb_con_concurso.tb_ico_inscrito_concurso.Where(ic => ic.ico_idt_inscrito_concurso == ico.ico_idt_inscrito_concurso))
                {
                    db.Entry(ico2).Reference(i => i.tb_est_estado).Load();
                    db.Entry(ico2).Reference(i => i.tb_cid_cidade).Load();
                    db.Entry(ico2.tb_cid_cidade).Reference(c => c.tb_est_estado).Load();
                }
            }

            return icos.ToList();
        }

        internal static List<spr_tgv_gerar_lista_inscritos_Result> GerarListaInscritos(int con_idt_concurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.spr_tgv_gerar_lista_inscritos(con_idt_concurso).ToList();
            }
        }

        internal static List<tb_ico_inscrito_concurso> Listar(int con_idt_concurso, int pagina, bool comPaginacao, int tamanhoPagina, string ordem, int matricula, string nome, string cpf, bool? ativo, bool? isento)
        {
#warning ToDo: Transformar em stored procedure
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                var query = (from ico in db.tb_ico_inscrito_concurso.Include("tb_cid_cidade.tb_est_estado").Include("tb_est_estado").Include("tb_con_concurso").Include("tb_cci_concurso_cargo_inscrito.tb_cco_cargo_concurso")
                        where ico.con_idt_concurso == con_idt_concurso
                         && (matricula == 0 || ico.ico_idt_inscrito_concurso == matricula)
                         && (nome == "" || ico.ico_nom_inscrito_concurso.ToLower().Contains(nome.ToLower()))
                         && (cpf == "" || ico.ico_des_cpf.Contains(cpf))
                         && (!ativo.HasValue || ico.ico_bit_ativo == ativo.Value)
                         && (!isento.HasValue || ico.ico_bit_isento == isento.Value)
                        select ico).ToList();

                var orderedQuery = ordem.Equals("M", StringComparison.InvariantCultureIgnoreCase) ?
                    query.OrderBy(ico => ico.ico_idt_inscrito_concurso) :
                    query.OrderBy(ico => ico.ico_nom_inscrito_concurso);

                if (comPaginacao)
                    return orderedQuery.Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToList();

                return orderedQuery.ToList();
            }
        }

        internal static List<tb_ico_inscrito_concurso> Listar(int con_idt_concurso, string ico_des_cpf)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                db.Configuration.ProxyCreationEnabled = false;

                var icos = (from ico in db.tb_ico_inscrito_concurso
                        where ico.con_idt_concurso == con_idt_concurso
                        && ico.ico_des_cpf == ico_des_cpf
                        && ico.ico_bit_ativo
                        select ico);

                return EagerLoading(db, icos);
            }
        }

        internal static List<tb_ico_inscrito_concurso> ListarConsiderandoPeriodoInscricoes(int con_idt_concurso, string ico_des_cpf, bool dentroPeriodoInscricoes)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                db.Configuration.ProxyCreationEnabled = false;

                var icos = (from ico in db.tb_ico_inscrito_concurso
                        where ico.con_idt_concurso == con_idt_concurso
                        && ico.ico_des_cpf == ico_des_cpf
                        && ico.ico_bit_ativo
                        && (dentroPeriodoInscricoes || ico.ico_bit_pago || ico.ico_bit_isento)
                        select ico);

                return EagerLoading(db, icos);

                //Include("tb_est_estado").Include("tb_con_concurso.tb_ico_inscrito_concurso.tb_est_estado").Include("tb_con_concurso.tb_cco_cargo_concurso").Include("tb_con_concurso.tb_ico_inscrito_concurso.tb_cid_cidade.tb_est_estado").Include("tb_cci_concurso_cargo_inscrito.tb_cco_cargo_concurso")
            }
        }

        internal static List<tb_ico_inscrito_concurso> Listar(int con_idt_concurso, bool considerarSomenteAtivos = true)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from ico in db.tb_ico_inscrito_concurso.Include("tb_cci_concurso_cargo_inscrito.tb_cco_cargo_concurso")
                        where ico.con_idt_concurso == con_idt_concurso
                        && (!considerarSomenteAtivos || ico.ico_bit_ativo)
                        select ico).ToList();
            }
        }

        internal static List<tb_ico_inscrito_concurso_extension> ListarNaoPagos(int con_idt_concurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                db.Configuration.ProxyCreationEnabled = false;
                string query = @"SELECT	DISTINCT
                                        ico.*,
		                                cid.*,
		                                est.*,
		                                cco.*
                                FROM    tb_ico_inscrito_concurso ico
                                INNER JOIN tb_cid_cidade cid ON ico.cid_idt_cidade = cid.cid_idt_cidade
                                INNER JOIN tb_est_estado est ON cid.est_idt_estado = est.est_idt_estado
                                INNER JOIN tb_cci_concurso_cargo_inscrito cci ON cci.ico_idt_inscrito_concurso = ico.ico_idt_inscrito_concurso
                                INNER JOIN tb_cco_cargo_concurso cco ON cci.cco_idt_cargo_concurso = cco.cco_idt_cargo_concurso
                                WHERE ico.con_idt_concurso = @p0
                                AND ico.ico_bit_ativo = 1
                                AND ico.ico_bit_pago = 0;
                                ";

                var icos = db.Database.SqlQuery<tb_ico_inscrito_concurso_extension>(query, con_idt_concurso).ToList();

                return icos;
            }
        }

        internal static List<tb_ico_inscrito_concurso_extension> ListarPagos(int con_idt_concurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                db.Configuration.ProxyCreationEnabled = false;
                string query = @"SELECT	DISTINCT
                                        ico.*,
		                                cid.*,
		                                est.*,
		                                cco.*
                                FROM    tb_ico_inscrito_concurso ico
                                INNER JOIN tb_cid_cidade cid ON ico.cid_idt_cidade = cid.cid_idt_cidade
                                INNER JOIN tb_est_estado est ON cid.est_idt_estado = est.est_idt_estado
                                INNER JOIN tb_cci_concurso_cargo_inscrito cci ON cci.ico_idt_inscrito_concurso = ico.ico_idt_inscrito_concurso
                                INNER JOIN tb_cco_cargo_concurso cco ON cci.cco_idt_cargo_concurso = cco.cco_idt_cargo_concurso
                                WHERE ico.con_idt_concurso = @p0
                                AND ico.ico_bit_ativo = 1
                                AND ico.ico_bit_pago = 1;
                                ";

                var icos = db.Database.SqlQuery<tb_ico_inscrito_concurso_extension>(query, con_idt_concurso).ToList();

                return icos;
            }
        }

        internal static List<tb_ico_inscrito_concurso> ListarUltimosInscritos(int con_idt_concurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from ico in db.tb_ico_inscrito_concurso
                        where ico.con_idt_concurso == con_idt_concurso
                        orderby ico.ico_idt_inscrito_concurso descending
                        select ico).Take(5).ToList();
            }
        }

        internal static tb_ico_inscrito_concurso Obter(int ico_idt_inscrito_concurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from ico in db.tb_ico_inscrito_concurso.Include("tb_cid_cidade.tb_est_estado").Include("tb_est_estado").Include("tb_con_concurso.tb_cco_cargo_concurso").Include("tb_con_concurso.tb_emp_empresa").Include("tb_cci_concurso_cargo_inscrito.tb_cco_cargo_concurso")
                        where ico.ico_idt_inscrito_concurso == ico_idt_inscrito_concurso
                        select ico).SingleOrDefault();
            }
        }

        internal static tb_icv_inscrito_concurso_vestibular ObterVestibular(int ico_idt_inscrito_concurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from icv in db.tb_icv_inscrito_concurso_vestibular
                        where icv.ico_idt_inscrito_concurso == ico_idt_inscrito_concurso
                        select icv).SingleOrDefault();
            }
        }

        internal static tb_ico_inscrito_concurso Salvar(tb_ico_inscrito_concurso inscrito)
        {
            return Salvar(inscrito, 0);
        }

        internal static tb_ico_inscrito_concurso Salvar(tb_ico_inscrito_concurso inscrito, int cco_idt_cargo_concurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                tb_ico_inscrito_concurso toUpdate = db.tb_ico_inscrito_concurso.SingleOrDefault(ico => ico.ico_idt_inscrito_concurso == inscrito.ico_idt_inscrito_concurso);

                if (toUpdate != null)
                    toUpdate.ico_idt_inscrito_concurso = inscrito.ico_idt_inscrito_concurso;
                else
                {
                    toUpdate = new tb_ico_inscrito_concurso();
                    db.tb_ico_inscrito_concurso.Add(toUpdate);
                }

                string celular = !String.IsNullOrEmpty(inscrito.ico_des_celular) && inscrito.ico_des_celular.Length > 11 ? inscrito.ico_des_celular.Substring(0, 11) : inscrito.ico_des_celular;
                string telefone = !String.IsNullOrEmpty(inscrito.ico_des_telefone) && inscrito.ico_des_telefone.Length > 11 ? inscrito.ico_des_telefone.Substring(0, 11) : inscrito.ico_des_telefone;

                toUpdate.cid_idt_cidade = inscrito.cid_idt_cidade;
                toUpdate.con_idt_concurso = inscrito.con_idt_concurso;
                toUpdate.est_idt_estado_rg = inscrito.est_idt_estado_rg;
                toUpdate.ico_bit_ativo = inscrito.ico_bit_ativo;
                toUpdate.ico_bit_destro = inscrito.ico_bit_destro;
                toUpdate.ico_bit_isento = inscrito.ico_bit_isento;
                toUpdate.ico_bit_pago = inscrito.ico_bit_pago;
                toUpdate.ico_bit_possui_deficiencia = inscrito.ico_bit_possui_deficiencia;
                toUpdate.ico_bit_tratamento_especial = inscrito.ico_bit_tratamento_especial;
                toUpdate.ico_dat_inscricao = inscrito.ico_dat_inscricao;
                toUpdate.ico_dat_nascimento = inscrito.ico_dat_nascimento;
                toUpdate.ico_dat_pagamento = inscrito.ico_dat_pagamento;
                toUpdate.ico_des_bairro = inscrito.ico_des_bairro;
                toUpdate.ico_des_celular = celular;
                toUpdate.ico_des_cep = inscrito.ico_des_cep;
                toUpdate.ico_des_complemento = inscrito.ico_des_complemento;
                toUpdate.ico_des_cpf = inscrito.ico_des_cpf;
                toUpdate.ico_des_email = inscrito.ico_des_email;
                toUpdate.ico_des_endereco = inscrito.ico_des_endereco;
                toUpdate.ico_des_link_boleto = inscrito.ico_des_link_boleto;
                toUpdate.ico_des_outras_solicitacoes = inscrito.ico_des_outras_solicitacoes;
                toUpdate.ico_des_rg = inscrito.ico_des_rg;
                toUpdate.ico_des_telefone = telefone;
                toUpdate.ico_des_tratamento_especial_qual = inscrito.ico_des_tratamento_especial_qual;
                toUpdate.ico_flg_deficiencia = inscrito.ico_flg_deficiencia;
                toUpdate.ico_flg_estado_civil = inscrito.ico_flg_estado_civil;
                toUpdate.ico_nom_inscrito_concurso = inscrito.ico_nom_inscrito_concurso;
                toUpdate.ico_des_nro_endereco = inscrito.ico_des_nro_endereco;
                toUpdate.ico_num_filhos_menores = inscrito.ico_num_filhos_menores;
                toUpdate.ico_num_valor_pago = inscrito.ico_num_valor_pago;

                db.SaveChangesWithErrors();

                inscrito = toUpdate;
                inscrito.tb_cid_cidade = db.tb_cid_cidade.Include("tb_est_estado").Single(cid => cid.cid_idt_cidade == inscrito.cid_idt_cidade);
                inscrito.tb_con_concurso = db.tb_con_concurso.Single(con => con.con_idt_concurso == inscrito.con_idt_concurso);
                inscrito.tb_cci_concurso_cargo_inscrito = db.tb_cci_concurso_cargo_inscrito.Include("tb_cco_cargo_concurso").Where(cci => cci.con_idt_concurso == inscrito.con_idt_concurso && cci.ico_idt_inscrito_concurso == inscrito.ico_idt_inscrito_concurso).ToList();

                if (cco_idt_cargo_concurso > 0)
                {
                    tb_cci_concurso_cargo_inscrito ccinsc = new tb_cci_concurso_cargo_inscrito();

                    if (inscrito.tb_cci_concurso_cargo_inscrito.Count >= 0)
                        db.DeleteWhere<tb_cci_concurso_cargo_inscrito>(cci => cci.con_idt_concurso == inscrito.con_idt_concurso && cci.ico_idt_inscrito_concurso == inscrito.ico_idt_inscrito_concurso);

                    db.tb_cci_concurso_cargo_inscrito.Add(ccinsc);
                    ccinsc.con_idt_concurso = inscrito.con_idt_concurso;
                    ccinsc.ico_idt_inscrito_concurso = inscrito.ico_idt_inscrito_concurso;
                    ccinsc.cco_idt_cargo_concurso = cco_idt_cargo_concurso;

                    db.SaveChangesWithErrors();

                    inscrito.tb_cci_concurso_cargo_inscrito = db.tb_cci_concurso_cargo_inscrito.Include("tb_cco_cargo_concurso").Where(cci => cci.con_idt_concurso == inscrito.con_idt_concurso && cci.ico_idt_inscrito_concurso == inscrito.ico_idt_inscrito_concurso).ToList();
                }
            }

            return inscrito;
        }

        internal static tb_icv_inscrito_concurso_vestibular Salvar(tb_icv_inscrito_concurso_vestibular inscrito)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                tb_icv_inscrito_concurso_vestibular toUpdate = db.tb_icv_inscrito_concurso_vestibular.SingleOrDefault(icv => icv.ico_idt_inscrito_concurso == inscrito.ico_idt_inscrito_concurso);

                if (toUpdate == null)
                {
                    toUpdate = new tb_icv_inscrito_concurso_vestibular();
                    db.tb_icv_inscrito_concurso_vestibular.Add(toUpdate);
                }

                toUpdate.ico_idt_inscrito_concurso = inscrito.ico_idt_inscrito_concurso;
                toUpdate.icv_bit_ativo = inscrito.icv_bit_ativo;
                toUpdate.icv_bit_eh_masculino = inscrito.icv_bit_eh_masculino;
                toUpdate.icv_dat_aceito_termos = inscrito.icv_dat_aceito_termos;
                toUpdate.icv_des_conhecimento_unifae_outros = inscrito.icv_des_conhecimento_unifae_outros;
                toUpdate.icv_des_curso_indicado_por = inscrito.icv_des_curso_indicado_por;
                toUpdate.icv_des_nome_indicado_por = inscrito.icv_des_nome_indicado_por;
                toUpdate.icv_des_semestre_curso_indicado_por = inscrito.icv_des_semestre_curso_indicado_por;
                toUpdate.icv_idt_opcao_2 = inscrito.icv_idt_opcao_2;
                toUpdate.icv_idt_opcao_3 = inscrito.icv_idt_opcao_3;
                toUpdate.icv_num_atividade_remunerada = inscrito.icv_num_atividade_remunerada;
                toUpdate.icv_num_conhecimento_unifae = inscrito.icv_num_conhecimento_unifae;
                toUpdate.icv_num_data_prova = inscrito.icv_num_data_prova;
                toUpdate.icv_num_escolaridade_mae = inscrito.icv_num_escolaridade_mae;
                toUpdate.icv_num_escolaridade_pai = inscrito.icv_num_escolaridade_pai;
                toUpdate.icv_num_local_prova = inscrito.icv_num_local_prova;
                toUpdate.icv_num_optar_curso = inscrito.icv_num_optar_curso;
                toUpdate.icv_num_optar_unifae = inscrito.icv_num_optar_unifae;
                toUpdate.icv_num_renda_mensal = inscrito.icv_num_renda_mensal;
                toUpdate.icv_num_tipo_concluiu_ensino_fundamental = inscrito.icv_num_tipo_concluiu_ensino_fundamental;
                toUpdate.icv_num_tipo_concluiu_ensino_medio = inscrito.icv_num_tipo_concluiu_ensino_medio;
                toUpdate.icv_num_tipo_ensino_medio = inscrito.icv_num_tipo_ensino_medio;

                db.SaveChangesWithErrors();

                inscrito = toUpdate;
            }

            return inscrito;
        }

        internal static List<tb_cci_concurso_cargo_inscrito> SalvarCCIs(List<tb_cci_concurso_cargo_inscrito> ccis, int ico_idt_inscrito_concurso)
        {
            List<tb_cci_concurso_cargo_inscrito> retorno = new List<tb_cci_concurso_cargo_inscrito>();

            using (IPEFAEEntities db = BaseData.Contexto)
            {
                foreach (tb_cci_concurso_cargo_inscrito cci in ccis)
                {
                    tb_cci_concurso_cargo_inscrito toInsert = new tb_cci_concurso_cargo_inscrito();
                    db.tb_cci_concurso_cargo_inscrito.Add(toInsert);
                    toInsert.con_idt_concurso = cci.con_idt_concurso;
                    toInsert.cco_idt_cargo_concurso = cci.cco_idt_cargo_concurso;
                    toInsert.ico_idt_inscrito_concurso = ico_idt_inscrito_concurso;

                    db.SaveChangesWithErrors();

                    retorno.Add(toInsert);
                }
            }

            return retorno;
        }
    }
}
