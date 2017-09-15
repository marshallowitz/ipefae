using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGV.IPEFAE.Web.BL.Data
{
    internal class UsuarioEstagioData
    {
        internal static List<tb_ues_usuario_estagio> Listar(int pagina, bool comPaginacao, int tamanhoPagina, string nome, string curso, int? semAno, bool? estagiando, string cpf, bool visualizacao, string cidade, string ordem)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                var query = (from ues in db.tb_ues_usuario_estagio.Include("tb_cid_cidade.tb_est_estado").Include("tb_est_estado").Include("tb_ued_usuario_estagio_dados_escolares").Include("tb_uee_usuario_estagio_experiencia").Include("tb_ueo_usuario_estagio_outros")
                             where ((visualizacao && ues.ues_bit_considerar_busca && ues.ues_bit_ativo) || (!visualizacao && (!ues.ues_bit_considerar_busca || !ues.ues_bit_ativo)))
                             && (nome == "" || ues.ues_nom_usuario_estagio.ToLower().Contains(nome.ToLower()))
                             && (curso == "" || ues.tb_ued_usuario_estagio_dados_escolares.ued_des_nome_curso.ToLower().Contains(curso.ToLower()))
                             && (!semAno.HasValue || ues.tb_ued_usuario_estagio_dados_escolares.ued_num_ano_semestre == semAno.Value)
                             && (!estagiando.HasValue || ues.ues_bit_estagiando == estagiando.Value)
                             && ues.ues_des_cpf.Contains(cpf)
                             && ues.tb_cid_cidade.cid_nom_cidade.Contains(cidade)
                             select ues);

                var orderedQuery = ordem.Equals("N", StringComparison.InvariantCultureIgnoreCase) ? 
                    query.OrderBy(ues => ues.ues_nom_usuario_estagio) :
                    query.OrderByDescending(ues => ues.ues_dat_ultima_atualizacao);

                if (comPaginacao)
                    return orderedQuery.Skip((pagina - 1) * tamanhoPagina).Take(tamanhoPagina).ToList();

                return orderedQuery.ToList();
            }
        }

        internal static tb_ues_usuario_estagio Obter(int ues_idt_usuario_estagio)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from ues in db.tb_ues_usuario_estagio.Include("tb_cid_cidade.tb_est_estado").Include("tb_est_estado").Include("tb_ued_usuario_estagio_dados_escolares").Include("tb_uee_usuario_estagio_experiencia").Include("tb_ueo_usuario_estagio_outros")
                        where ues.ues_idt_usuario_estagio == ues_idt_usuario_estagio
                        select ues).SingleOrDefault();
            }
        }

        internal static tb_ues_usuario_estagio Obter(string ues_des_email, string ues_des_senha)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from ues in db.tb_ues_usuario_estagio
                        where ues.ues_bit_ativo
                        && ues.ues_des_email.ToLower() == ues_des_email.ToLower()
                        && ues.ues_des_senha == ues_des_senha
                        select ues).SingleOrDefault();
            }
        }

        internal static tb_ues_usuario_estagio ObterPorCPF(string ues_des_cpf)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.tb_ues_usuario_estagio.LastOrDefault(ues => ues.ues_des_cpf == ues_des_cpf);
            }
        }

        internal static tb_ues_usuario_estagio ObterPorEmail(string ues_des_email)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.tb_ues_usuario_estagio.SingleOrDefault(ues => ues.ues_des_email == ues_des_email && ues.ues_bit_ativo);
            }
        }

        internal static tb_ues_usuario_estagio Salvar(tb_ues_usuario_estagio usuario, int ued_idt_usuario_estagio_dados_escolares, List<tb_uee_usuario_estagio_experiencia> exps, List<tb_ueo_usuario_estagio_outros> ccs, List<tb_ueo_usuario_estagio_outros> ocs, bool ehAdmin)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                tb_ues_usuario_estagio toUpdate = db.tb_ues_usuario_estagio.SingleOrDefault(ues => ues.ues_idt_usuario_estagio == usuario.ues_idt_usuario_estagio);

                if (toUpdate != null)
                    toUpdate.ues_idt_usuario_estagio = usuario.ues_idt_usuario_estagio;
                else
                {
                    toUpdate = new tb_ues_usuario_estagio();
                    db.tb_ues_usuario_estagio.Add(toUpdate);
                }

                toUpdate.cid_idt_cidade_endereco = usuario.cid_idt_cidade_endereco;
                toUpdate.est_idt_estado_carteira_trabalho = usuario.est_idt_estado_carteira_trabalho;
                toUpdate.ued_idt_usuario_estagio_dados_escolares = ued_idt_usuario_estagio_dados_escolares;
                toUpdate.ues_bit_masculino = usuario.ues_bit_masculino;
                toUpdate.ues_bit_possui_deficiencia = usuario.ues_bit_possui_deficiencia;
                toUpdate.ues_bit_possui_experiencia_profissional = usuario.ues_bit_possui_experiencia_profissional;
                toUpdate.ues_bit_tem_foto = usuario.ues_bit_tem_foto;
                toUpdate.ues_dat_criacao_cv = usuario.ues_dat_criacao_cv;
                toUpdate.ues_dat_nascimento = usuario.ues_dat_nascimento;
                toUpdate.ues_dat_rg_expedicao = usuario.ues_dat_rg_expedicao;
                toUpdate.ues_dat_ultima_atualizacao = usuario.ues_dat_ultima_atualizacao;
                toUpdate.ues_des_bairro = usuario.ues_des_bairro;
                toUpdate.ues_des_carteira_trabalho_numero = usuario.ues_des_carteira_trabalho_numero;
                toUpdate.ues_des_carteira_trabalho_serie = usuario.ues_des_carteira_trabalho_serie;
                toUpdate.ues_des_celular = usuario.ues_des_celular;
                toUpdate.ues_des_cep = usuario.ues_des_cep;
                toUpdate.ues_des_complemento = usuario.ues_des_complemento;
                toUpdate.ues_des_cpf = usuario.ues_des_cpf;
                toUpdate.ues_des_email = usuario.ues_des_email;
                toUpdate.ues_des_endereco = usuario.ues_des_endereco;
                toUpdate.ues_des_numero_endereco = usuario.ues_des_numero_endereco;
                toUpdate.ues_des_objetivos = usuario.ues_des_objetivos;
                toUpdate.ues_des_rg = usuario.ues_des_rg;
                toUpdate.ues_des_telefone = usuario.ues_des_telefone;
                toUpdate.ues_flg_deficiencia = usuario.ues_flg_deficiencia;
                toUpdate.ues_flg_estado_civil = usuario.ues_flg_estado_civil;
                toUpdate.ues_nom_usuario_estagio = usuario.ues_nom_usuario_estagio;
                toUpdate.ues_num_quantidade_filhos = usuario.ues_num_quantidade_filhos;

                if (ehAdmin || usuario.ues_idt_usuario_estagio == 0)
                {
                    toUpdate.ues_bit_ativo = usuario.ues_bit_ativo;
                    toUpdate.ues_bit_considerar_busca = usuario.ues_bit_considerar_busca;
                    toUpdate.ues_bit_estagiando = usuario.ues_bit_estagiando;
                    toUpdate.ues_flg_motivo_desativacao = usuario.ues_flg_motivo_desativacao;
                    toUpdate.ues_des_observacoes_admin = usuario.ues_des_observacoes_admin;
                }

                if (!String.IsNullOrEmpty(usuario.ues_des_senha))
                    toUpdate.ues_des_senha = usuario.ues_des_senha;

                db.SaveChangesWithErrors();

                usuario = toUpdate;

                // Salva as 3 listas
                Salvar(exps, usuario.ues_idt_usuario_estagio);
                Salvar(ccs, usuario.ues_idt_usuario_estagio, true);
                Salvar(ocs, usuario.ues_idt_usuario_estagio, false);
            }

            return Obter(usuario.ues_idt_usuario_estagio);
        }

        internal static tb_ued_usuario_estagio_dados_escolares Salvar(tb_ued_usuario_estagio_dados_escolares dadosEscolares)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                tb_ued_usuario_estagio_dados_escolares toUpdate = db.tb_ued_usuario_estagio_dados_escolares.SingleOrDefault(ued => ued.ued_idt_usuario_estagio_dados_escolares == dadosEscolares.ued_idt_usuario_estagio_dados_escolares);

                if (toUpdate != null)
                    toUpdate.ued_idt_usuario_estagio_dados_escolares = dadosEscolares.ued_idt_usuario_estagio_dados_escolares;
                else
                {
                    toUpdate = new tb_ued_usuario_estagio_dados_escolares();
                    db.tb_ued_usuario_estagio_dados_escolares.Add(toUpdate);
                }

                toUpdate.ued_bit_ativo = dadosEscolares.ued_bit_ativo;
                toUpdate.ued_bit_ead = dadosEscolares.ued_bit_ead;
                toUpdate.ued_des_nome_curso = dadosEscolares.ued_des_nome_curso;
                toUpdate.ued_des_nome_escola = dadosEscolares.ued_des_nome_escola;
                toUpdate.ued_flg_periodo = dadosEscolares.ued_flg_periodo;
                toUpdate.ued_flg_tipo_dados_escolares = dadosEscolares.ued_flg_tipo_dados_escolares;
                toUpdate.ued_flg_tipo_profissionalizante = dadosEscolares.ued_flg_tipo_profissionalizante;
                toUpdate.ued_num_ano_inicio = dadosEscolares.ued_num_ano_inicio;
                toUpdate.ued_num_ano_semestre = dadosEscolares.ued_num_ano_semestre;
                toUpdate.ued_num_ano_termino = dadosEscolares.ued_num_ano_termino;
                toUpdate.ued_num_mes_inicio = dadosEscolares.ued_num_mes_inicio;
                toUpdate.ued_num_mes_termino = dadosEscolares.ued_num_mes_termino;

                db.SaveChangesWithErrors();

                dadosEscolares = toUpdate;
            }

            return dadosEscolares;
        }

        private static void Salvar(List<tb_uee_usuario_estagio_experiencia> exps, int ues_idt_usuario_estagio)
        {
            int itemToSkip = 0;

            foreach (tb_uee_usuario_estagio_experiencia exp in exps)
            {
                using (IPEFAEEntities db = BaseData.Contexto)
                {
                    tb_uee_usuario_estagio_experiencia toUpdate = exp.uee_idt_usuario_estagio_experiencia > 0 ?
                        db.tb_uee_usuario_estagio_experiencia.SingleOrDefault(uee => uee.uee_idt_usuario_estagio_experiencia == exp.uee_idt_usuario_estagio_experiencia) :
                        db.tb_uee_usuario_estagio_experiencia.Where(e => e.ues_idt_usuario_estagio == ues_idt_usuario_estagio).OrderBy(e => e.uee_idt_usuario_estagio_experiencia).Skip(itemToSkip++).Take(1).SingleOrDefault();

                    if (toUpdate == null)
                    {
                        toUpdate = new tb_uee_usuario_estagio_experiencia();
                        db.tb_uee_usuario_estagio_experiencia.Add(toUpdate);
                    }

                    toUpdate.uee_bit_ativo = exp.uee_bit_ativo;
                    toUpdate.uee_des_atividades_desenvolvidas = exp.uee_des_atividades_desenvolvidas;
                    toUpdate.uee_des_cargo = exp.uee_des_cargo;
                    toUpdate.uee_des_nome_empresa = exp.uee_des_nome_empresa;
                    toUpdate.uee_num_ano_inicio = exp.uee_num_ano_inicio;
                    toUpdate.uee_num_ano_termino = exp.uee_num_ano_termino;
                    toUpdate.uee_num_mes_inicio = exp.uee_num_mes_inicio;
                    toUpdate.uee_num_mes_termino = exp.uee_num_mes_termino;
                    toUpdate.ues_idt_usuario_estagio = ues_idt_usuario_estagio;

                    db.SaveChangesWithErrors();
                }
            }
        }

        private static void Salvar(List<tb_ueo_usuario_estagio_outros> ueos, int ues_idt_usuario_estagio, bool ehCurso)
        {
            int itemToSkip = 0;

            foreach (tb_ueo_usuario_estagio_outros ueo in ueos)
            {
                using (IPEFAEEntities db = BaseData.Contexto)
                {
                    tb_ueo_usuario_estagio_outros toUpdate = ueo.ueo_idt_usuario_estagio_outros > 0 ?
                        db.tb_ueo_usuario_estagio_outros.SingleOrDefault(o => o.ueo_idt_usuario_estagio_outros == ueo.ueo_idt_usuario_estagio_outros) :
                        db.tb_ueo_usuario_estagio_outros.Where(o => o.ues_idt_usuario_estagio == ues_idt_usuario_estagio && o.ueo_bit_curso == ehCurso).OrderBy(u => u.ueo_idt_usuario_estagio_outros).Skip(itemToSkip++).Take(1).SingleOrDefault();

                    if (toUpdate == null)
                    {
                        toUpdate = new tb_ueo_usuario_estagio_outros();
                        db.tb_ueo_usuario_estagio_outros.Add(toUpdate);
                    }

                    toUpdate.ueo_bit_ativo = ueo.ueo_bit_ativo;
                    toUpdate.ueo_bit_curso = ehCurso;
                    toUpdate.ueo_des_duracao = ueo.ueo_des_duracao;
                    toUpdate.ueo_nom_usuario_estagio_outros = ueo.ueo_nom_usuario_estagio_outros;
                    toUpdate.ues_idt_usuario_estagio = ues_idt_usuario_estagio;

                    db.SaveChangesWithErrors();
                }
            }
        }
    }

    public partial class tb_ues_usuario_estagio
    {
        public string ues_des_senha_descriptografada { get; set; }
    }
}
