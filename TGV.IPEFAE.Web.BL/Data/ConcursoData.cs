using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web.Script.Serialization;

namespace TGV.IPEFAE.Web.BL.Data
{
    public class ConcursoData
    {
        internal static bool Excluir(int id)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    db.DeleteWhere<concurso_local_colaborador>(clc => clc.concurso_local.concurso_id == id);
                    db.DeleteWhere<concurso_local>(cl => cl.concurso_id == id);
                    db.DeleteWhere<concurso_funcao>(cf => cf.concurso_id == id);
                    db.DeleteWhere<concurso>(c => c.id == id);

                    scope.Complete();
                }

                return true;
            }
        }

        internal static bool Funcao_Excluir(int idFuncao)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                db.DeleteWhere<concurso_funcao>(cf => cf.id == idFuncao);
                return true;
            }
        }

        internal static ConcursoFuncaoModel Funcao_Salvar(ConcursoFuncaoModel cfM)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                concurso_funcao toUpdate = db.concurso_funcao.SingleOrDefault(cf => cf.id == cfM.id);

                if (toUpdate == null) // Nao encontrou
                {
                    toUpdate = new concurso_funcao();
                    db.concurso_funcao.Add(toUpdate);
                    cfM.ativo = true;
                }

                toUpdate.ativo = cfM.ativo;
                toUpdate.concurso_id = cfM.concurso_id;
                toUpdate.funcao = cfM.funcao?.FirstCharOfEachWordToUpper();
                toUpdate.valor_liquido = cfM.valor_liquido;

                db.SaveChangesWithErrors();

                cfM.id = toUpdate.id;

                return cfM;
            }
        }

        internal static List<ConcursoModel> Listar()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.concurso.ToList().ConvertAll(c => c.CopyObject<ConcursoModel>());
            }
        }

        internal static bool Local_Colaborador_Excluir(int idColaborador)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
               db.DeleteWhere<concurso_local_colaborador>(clc => clc.id == idColaborador);

                return true;
            }
        }

        internal static bool Local_Excluir(int idLocal)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    db.DeleteWhere<concurso_local_colaborador>(clc => clc.concurso_local.id == idLocal);
                    db.DeleteWhere<concurso_local>(cl => cl.id == idLocal);

                    scope.Complete();
                }
                
                return true;
            }
        }

        internal static ConcursoLocalColaboradorModel Local_Colaborador_Salvar(ConcursoLocalColaboradorModel clcM)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                concurso_local_colaborador toUpdate = db.concurso_local_colaborador.SingleOrDefault(clc => clc.id == clcM.id);

                if (toUpdate == null) // Nao encontrou
                {
                    toUpdate = new concurso_local_colaborador();
                    db.concurso_local_colaborador.Add(toUpdate);
                    clcM.ativo = true;
                }

                toUpdate.ativo = clcM.ativo;
                toUpdate.colaborador_id = clcM.colaborador_id;
                toUpdate.concurso_local_id = clcM.concurso_local_id;
                toUpdate.funcao_id = clcM.funcao_id;
                toUpdate.inss = clcM.inss;
                toUpdate.iss = clcM.iss;
                toUpdate.valor = clcM.valor;

                db.SaveChangesWithErrors();

                clcM.id = toUpdate.id;

                return clcM;
            }
        }

        internal static ConcursoLocalModel Local_Salvar(ConcursoLocalModel clM)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                concurso_local toUpdate = db.concurso_local.SingleOrDefault(cl => cl.id == clM.id);

                if (toUpdate == null) // Nao encontrou
                {
                    toUpdate = new concurso_local();
                    db.concurso_local.Add(toUpdate);
                    clM.ativo = true;
                }

                toUpdate.ativo = clM.ativo;
                toUpdate.concurso_id = clM.concurso_id;
                toUpdate.local = clM.local?.FirstCharOfEachWordToUpper();

                db.SaveChangesWithErrors();

                clM.id = toUpdate.id;

                return clM;
            }
        }

        internal static ConcursoModel Obter(int id, bool ate_colaborador = false)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                if (!ate_colaborador)
                {
                    var query = @"
                                SELECT	DISTINCT
		                                c.id AS concurso_id,
		                                c.nome AS concurso_nome,
		                                c.[data] AS concurso_data,
		                                c.ativo AS concurso_ativo,
		                                cf.id AS concurso_funcao_id,
		                                cf.funcao AS concurso_funcao_funcao,
		                                cf.valor_liquido AS concurso_funcao_valor_liquido,
		                                cf.sem_desconto AS concurso_funcao_sem_desconto,
		                                cf.ativo AS concurso_funcao_ativo,
		                                CAST((CASE WHEN clc.id IS NOT NULL THEN 1 ELSE 0 END) AS BIT) AS concurso_funcao_tem_associacao,
		                                cl.id AS concurso_local_id,
		                                cl.[local] AS concurso_local_local,
		                                cl.ativo AS concurso_local_ativo,
		                                clc.id AS concurso_local_colaborador_id,
		                                clc.colaborador_id AS concurso_local_colaborador_colaborador_id,
		                                clc.funcao_id AS concurso_local_colaborador_funcao_id,
		                                clc.valor AS concurso_local_colaborador_valor,
		                                clc.inss AS concurso_local_colaborador_inss,
		                                clc.iss AS concurso_local_colaborador_iss,
		                                clc.ativo AS concurso_local_colaborador_ativo,
		                                co.id AS colaborador_id,
		                                co.nome AS colaborador_nome,
		                                co.cpf AS colaborador_cpf,
		                                co.email AS colaborador_email,
		                                co.ativo AS colaborador_ativo
                                FROM	concurso c
                                LEFT JOIN concurso_funcao cf ON c.id = cf.concurso_id AND cf.ativo = 1
                                LEFT JOIN concurso_local cl ON c.id = cl.concurso_id AND cl.ativo = 1
                                LEFT JOIN concurso_local_colaborador clc ON cl.id = clc.concurso_local_id AND cf.id = clc.funcao_id AND clc.ativo = 1
                                LEFT JOIN colaborador co ON clc.colaborador_id = co.id AND co.ativo = 1
                                WHERE	c.id = @concurso_id
                                AND		c.ativo = 1
                                ORDER BY c.nome, cl.[local], co.nome;";

                    // Lista as funções do concurso
                    var funcoes = db.concurso_funcao.Include("concurso_local_colaborador").Where(cf => cf.concurso_id == id && cf.ativo);

                    using (SqlConnection connection = new SqlConnection(BaseData.ConnectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@concurso_id", id);

                        try
                        {
                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();

                            var ctrLinhas = 0;
                            ConcursoModel concurso = new ConcursoModel();

                            while (reader.Read())
                            {
                                if (ctrLinhas == 0)
                                {
                                    // Inicializa o concurso
                                    concurso = new ConcursoModel()
                                    {
                                        id = Convert.ToInt32(reader["concurso_id"]),
                                        nome = reader["concurso_nome"].ToString(),
                                        data = Convert.ToDateTime(reader["concurso_data"]),
                                        ativo = Convert.ToBoolean(reader["concurso_ativo"]),
                                        funcoes = funcoes.ToList().ConvertAll(cf => new ConcursoFuncaoModel(cf)).OrderBy(cf => cf.funcao).ThenBy(cf => cf.valor_liquido).ToList()
                                    };
                                }

                                // Monta os locais e colaboradores
                                // Se não existir esse local, adiciona ao concurso
                                if (!String.IsNullOrEmpty(reader["concurso_local_id"].ToString()) && !concurso.locais.Any(cl => cl.id == Convert.ToInt32(reader["concurso_local_id"])))
                                {
                                    var local = new ConcursoLocalModel()
                                    {
                                        id = Convert.ToInt32(reader["concurso_local_id"]),
                                        concurso_id = id,
                                        local = reader["concurso_local_local"].ToString(),
                                        ativo = Convert.ToBoolean(reader["concurso_local_ativo"])
                                    };

                                    concurso.locais.Add(local);
                                }

                                var concurso_local = concurso.locais.FirstOrDefault(cl => cl.id == Convert.ToInt32(reader["concurso_local_id"]));

                                if (concurso_local != null)
                                {
                                    // Se não existir esse concurso_local_colaborador, adiciona ao local
                                    if (!String.IsNullOrEmpty(reader["concurso_local_colaborador_id"].ToString()) && !concurso_local.Colaboradores.Any(clc => clc.id == Convert.ToInt32(reader["concurso_local_colaborador_id"])))
                                    {
                                        var local_colaborador = new ConcursoLocalColaboradorModel()
                                        {
                                            id = Convert.ToInt32(reader["concurso_local_colaborador_id"]),
                                            colaborador_id = Convert.ToInt32(reader["concurso_local_colaborador_colaborador_id"]),
                                            funcao_id = Convert.ToInt32(reader["concurso_local_colaborador_funcao_id"]),
                                            valor = Convert.ToDecimal(reader["concurso_local_colaborador_valor"]),
                                            inss = Convert.ToBoolean(reader["concurso_local_colaborador_inss"]),
                                            iss = Convert.ToBoolean(reader["concurso_local_colaborador_iss"]),
                                            ativo = Convert.ToBoolean(reader["concurso_local_colaborador_ativo"])
                                        };

                                        concurso_local.Colaboradores.Add(local_colaborador);
                                    }

                                    // Adiciona o colaborador
                                    var concurso_local_colaborador = concurso_local.Colaboradores.FirstOrDefault(clc => clc.id == Convert.ToInt32(reader["concurso_local_colaborador_id"]));

                                    if (concurso_local_colaborador != null)
                                    {
                                        concurso_local_colaborador.colaborador = new ColaboradorModel()
                                        {
                                            id = Convert.ToInt32(reader["colaborador_id"]),
                                            nome = reader["colaborador_nome"].ToString(),
                                            cpf = reader["colaborador_cpf"].ToString(),
                                            email = reader["colaborador_email"].ToString(),
                                            ativo = Convert.ToBoolean(reader["colaborador_ativo"])
                                        };
                                    }
                                }

                                ctrLinhas++;
                            }

                            reader.Close();

                            return ctrLinhas > 0 ? concurso : null;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                else
                    return ConcursoModel.Clone(db.concurso.Include("concurso_funcao").Include("concurso_local.concurso_local_colaborador.colaborador").SingleOrDefault(con => con.id == id));
            }
        }

        internal static ConcursoModel Salvar(ConcursoModel cM)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                concurso toUpdate = db.concurso.SingleOrDefault(con => con.id == cM.id);

                if (toUpdate == null) // Nao encontrou
                {
                    toUpdate = new concurso();
                    db.concurso.Add(toUpdate);
                    cM.ativo = true;
                }

                toUpdate.ativo = cM.ativo;
                toUpdate.data = cM.data.ToUniversalTime().AddHours(12);
                toUpdate.nome = cM.nome?.FirstCharOfEachWordToUpper();

                db.SaveChangesWithErrors();

                return Obter(toUpdate.id);
            }
        }
    }

    public class ConcursoModel
    {
        #region [ Propriedades ]

        public int id                           { get; set; } = 0;
        public string nome                      { get; set; }
        public DateTime data                    { get; set; }
        public string nacionalidade             { get; set; }
        public bool ativo                       { get; set; }

        public string codigo                    { get { return this.id.ToString().PadLeft(6, '0'); } }
        public string data_formatada            { get { return this.data.ToString("dd/MM/yyyy"); } }

        public List<ConcursoFuncaoModel> funcoes   { get; set; } = new List<ConcursoFuncaoModel>();
        public List<ConcursoLocalModel> locais     { get; set; } = new List<ConcursoLocalModel>();

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public static ConcursoModel Clone(concurso con, bool miniColaborador = false)
        {
            if (con == null)
                return null;

            ConcursoModel c = con.CopyObject<ConcursoModel>();

            if (con.concurso_funcao != null)
                c.funcoes = con.concurso_funcao.ToList().ConvertAll(cf => new ConcursoFuncaoModel(cf));

            if (con.concurso_local != null)
                c.locais = con.concurso_local.ToList().ConvertAll(cl => new ConcursoLocalModel(cl, miniColaborador));

            return c;
        }

        #endregion [ FIM - Metodos ]
    }

    public class ConcursoFuncaoModel
    {
        public ConcursoFuncaoModel() { }

        public ConcursoFuncaoModel(concurso_funcao cf)
        {
            if (cf == null)
                return;

            this.id = cf.id;
            this.concurso_id = cf.concurso_id;
            this.funcao = cf.funcao;
            this.valor_liquido = cf.valor_liquido;
            this.sem_desconto = cf.sem_desconto;
            this.ativo = cf.ativo;

            if (cf.concurso_local_colaborador != null && cf.concurso_local_colaborador.Count > 0)
                this.temAssociacao = true;
        }

        #region [ Propriedades ]

        public int id                   { get; set; } = 0;
        public int concurso_id          { get; set; } = 0;
        public string funcao            { get; set; }
        public decimal valor_liquido    { get; set; }
        public bool sem_desconto        { get; set; }
        public bool ativo               { get; set; }

        public bool modoEdicao          { get; set; } = false;
        public bool temAssociacao       { get; set; } = false;

        public string valor_liquido_formatado   { get { return String.Format("{0:C2}", this.valor_liquido); } }

        public string ordernar_por      { get { return $"{this.funcao}_{this.valor_liquido.ToString().PadLeft(10, '0')}"; } }

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public static ConcursoFuncaoModel Clone(concurso_funcao cf)
        {
            if (cf == null)
                return null;

            ConcursoFuncaoModel funcao = cf.CopyObject<ConcursoFuncaoModel>();

            if (cf.concurso_local_colaborador != null && cf.concurso_local_colaborador.Count > 0)
                funcao.temAssociacao = true;

            return funcao;
        }

        public static List<ConcursoFuncaoModel> Listar(int idConcurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                //return db.concurso_funcao
                //    .Include("concurso_local_colaborador")
                //    .Where(cf => cf.concurso_id == idConcurso)
                //    .ToList()
                //    .ConvertAll(cf => ConcursoFuncaoModel.Clone(cf));

                return (from cf in db.concurso_funcao
                        where cf.concurso_id == idConcurso
                        select new ConcursoFuncaoModel()
                        {
                            ativo = cf.ativo,
                            concurso_id = cf.concurso_id,
                            funcao = cf.funcao,
                            id = cf.id,
                            sem_desconto = cf.sem_desconto,
                            temAssociacao = cf.concurso_local_colaborador.Any(),
                            valor_liquido = cf.valor_liquido
                        }).ToList();
            }
        }

        public static List<FuncaoModel> ListarTodas()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                var funcoes = (from f in db.funcao
                               select new FuncaoModel()
                               {
                                   is_active = f.is_active,
                                   nome = f.nome,
                                   cbo = f.cbo,
                                   codigo = f.codigo,
                                   id = f.id
                               }).ToList();

                return funcoes;
            }
        }

        #endregion [ FIM - Metodos ]
    }

    public class ConcursoLocalModel
    {
        public ConcursoLocalModel() { }

        public ConcursoLocalModel(concurso_local cl, bool miniColaborador = false)
        {
            if (cl == null)
                return;

            this.id = cl.id;
            this.concurso_id = cl.concurso_id;
            this.local = cl.local;
            this.ativo = cl.ativo;

            if (cl.concurso_local_colaborador != null)
                this.Colaboradores = cl.concurso_local_colaborador.ToList().ConvertAll(clc => ConcursoLocalColaboradorModel.Clone(clc, miniColaborador));
        }

        #region [ Propriedades ]

        public int id           { get; set; } = 0;
        public int concurso_id  { get; set; } = 0;
        public string local     { get; set; }
        public bool ativo       { get; set; } = true;

        public List<ConcursoLocalColaboradorModel> Colaboradores { get; set; } = new List<ConcursoLocalColaboradorModel>();

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public static ConcursoLocalModel Clone(concurso_local cl)
        {
            if (cl == null)
                return null;

            ConcursoLocalModel local = cl.CopyObject<ConcursoLocalModel>();

            if (cl.concurso_local_colaborador != null)
                local.Colaboradores = cl.concurso_local_colaborador.ToList().ConvertAll(clc => ConcursoLocalColaboradorModel.Clone(clc));

            return local;
        }

        #endregion [ FIM - Metodos ]
    }

    public class ConcursoLocalColaboradorModel
    {
        public ConcursoLocalColaboradorModel() { }

        public ConcursoLocalColaboradorModel(concurso_local_colaborador clc)
        {
            if (clc == null)
                return;

            this.id = clc.id;
            this.concurso_local_id = clc.concurso_local_id;
            this.colaborador_id = clc.colaborador_id;
            this.funcao_id = clc.funcao_id;
            this.valor = clc.valor;
            this.inss = clc.inss;
            this.iss = clc.iss;
            this.ativo = clc.ativo;
        }

        #region [ Propriedades ]

        public int id                   { get; set; } = 0;
        public int concurso_local_id    { get; set; } = 0;
        public int colaborador_id       { get; set; } = 0;
        public int funcao_id            { get; set; }
        public decimal valor            { get; set; }
        public bool inss                { get; set; } = true;
        public bool iss                 { get; set; } = true;
        public bool ativo               { get; set; }

        public bool modoEdicao          { get; set; } = false;

        public ColaboradorModel colaborador { get; set; } = new ColaboradorModel();
        public ConcursoFuncaoModel funcao { get; set; } = new ConcursoFuncaoModel();

        public ConcursoLocalModel concurso_local { get; set; } = new ConcursoLocalModel();

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public static ConcursoLocalColaboradorModel Clone(concurso_local_colaborador clc, bool mini = false)
        {
            if (clc == null)
                return null;

            //ConcursoLocalColaboradorModel local_colaborador = clc.CopyObject<ConcursoLocalColaboradorModel>();
            ConcursoLocalColaboradorModel local_colaborador = new ConcursoLocalColaboradorModel(clc);

            if (clc.colaborador != null)
                local_colaborador.colaborador = new ColaboradorModel(clc.colaborador, mini);

            if (clc.concurso_funcao != null)
                local_colaborador.funcao = new ConcursoFuncaoModel(clc.concurso_funcao);

            return local_colaborador;
        }

        public static List<ConcursoLocalColaboradorModel> Listar(int idConcurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                var lista = (from clc in db.concurso_local_colaborador
                             where clc.concurso_local.concurso_id == idConcurso
                             select new ConcursoLocalColaboradorModel()
                             {
                                 id = clc.id,
                                 concurso_local_id = clc.concurso_local_id,
                                 colaborador_id = clc.colaborador_id,
                                 funcao_id = clc.funcao_id,
                                 valor = clc.valor,
                                 inss = clc.inss,
                                 iss = clc.iss,
                                 ativo = clc.ativo,

                                 colaborador = new ColaboradorModel()
                                 {
                                     id = clc.colaborador.id,
                                     nome = clc.colaborador.nome,
                                     cpf = clc.colaborador.cpf,
                                     email = clc.colaborador.email,
                                     ativo = clc.colaborador.ativo,

                                     banco_id = clc.colaborador.banco_id,
                                     carteira_trabalho_estado_id = clc.colaborador.carteira_trabalho_estado_id,
                                     endereco_cidade_id = clc.colaborador.endereco_cidade_id,
                                     endereco_estado_id = clc.colaborador.tb_cid_cidade != null ? clc.colaborador.tb_cid_cidade.est_idt_estado : 0,
                                     endereco_cidade = clc.colaborador.tb_cid_cidade != null && clc.colaborador.tb_cid_cidade.tb_est_estado != null ? new CidadeModel()
                                     {
                                         Id = clc.colaborador.tb_cid_cidade.cid_idt_cidade,
                                         IdEstado = clc.colaborador.tb_cid_cidade.est_idt_estado,
                                         Nome = clc.colaborador.tb_cid_cidade.cid_nom_cidade,
                                         Ativo = clc.colaborador.tb_cid_cidade.cid_bit_ativo,

                                         Estado = new EstadoModel()
                                         {
                                             Id = clc.colaborador.tb_cid_cidade.tb_est_estado.est_idt_estado,
                                             Nome = clc.colaborador.tb_cid_cidade.tb_est_estado.est_nom_estado,
                                             Sigla = clc.colaborador.tb_cid_cidade.tb_est_estado.est_sig_estado,
                                             Ativo = clc.colaborador.tb_cid_cidade.tb_est_estado.est_bit_ativo
                                         }
                                     } : null,
                                     naturalidade_cidade_id = clc.colaborador.naturalidade_cidade_id,
                                     naturalidade_estado_id = clc.colaborador.tb_cid_cidade1 != null ? clc.colaborador.tb_cid_cidade1.est_idt_estado : 0,
                                     rg = clc.colaborador.rg,
                                     carteira_trabalho_nro = clc.colaborador.carteira_trabalho_nro,
                                     carteira_trabalho_serie = clc.colaborador.carteira_trabalho_serie,
                                     titulo_eleitor_nro = clc.colaborador.titulo_eleitor_nro,
                                     titulo_eleitor_zona = clc.colaborador.titulo_eleitor_zona,
                                     titulo_eleitor_secao = clc.colaborador.titulo_eleitor_secao,
                                     pis_pasep_net = clc.colaborador.pis_pasep_net,
                                     data_nascimento = clc.colaborador.data_nascimento,
                                     nacionalidade = clc.colaborador.nacionalidade,
                                     nome_mae = clc.colaborador.nome_mae,
                                     nome_pai = clc.colaborador.nome_pai,
                                     sexo_masculino = clc.colaborador.sexo_masculino,
                                     estado_civil = clc.colaborador.estado_civil,
                                     grau_instrucao_id = clc.colaborador.grau_instrucao_id,
                                     raca_id = clc.colaborador.raca_id,
                                     telefone_01 = clc.colaborador.telefone_01,
                                     telefone_02 = clc.colaborador.telefone_02,
                                     senha = clc.colaborador.senha,
                                     tipo_conta = clc.colaborador.tipo_conta,
                                     agencia = clc.colaborador.agencia,
                                     agencia_digito = clc.colaborador.agencia_digito,
                                     conta_corrente = clc.colaborador.conta_corrente,
                                     conta_corrente_digito = clc.colaborador.conta_corrente_digito,
                                     endereco_cep = clc.colaborador.endereco_cep,
                                     endereco_logradouro = clc.colaborador.endereco_logradouro,
                                     endereco_nro = clc.colaborador.endereco_nro,
                                     endereco_bairro = clc.colaborador.endereco_bairro,
                                     endereco_complemento = clc.colaborador.endereco_complemento,
                                     dados_ok = clc.colaborador.dados_ok
                                 },

                                 funcao = new ConcursoFuncaoModel()
                                 {
                                     id = clc.concurso_funcao.id,
                                     concurso_id = clc.concurso_funcao.concurso_id,
                                     funcao = clc.concurso_funcao.funcao,
                                     valor_liquido = clc.concurso_funcao.valor_liquido,
                                     sem_desconto = clc.concurso_funcao.sem_desconto,
                                     ativo = clc.concurso_funcao.ativo,

                                     temAssociacao = clc.concurso_funcao.concurso_local_colaborador.Count > 0
                                 }
                             });

                return lista.ToList();
            }
        }

        #endregion [ FIM - Metodos ]
    }
}
