using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Script.Serialization;

namespace TGV.IPEFAE.Web.BL.Data
{
    public class ColaboradorData
    {
        internal static List<ColaboradorModel> Listar()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                var cols = (from c in db.colaborador
                           select new ColaboradorModel()
                           {
                               id = c.id,
                               nome = c.nome,
                               cpf = c.cpf,
                               email = c.email,
                               ativo = c.ativo,

                               banco_id = c.banco_id,
                               carteira_trabalho_estado_id = c.carteira_trabalho_estado_id,
                               endereco_cidade_id = c.endereco_cidade_id,
                               endereco_estado_id = c.tb_cid_cidade != null ? c.tb_cid_cidade.est_idt_estado : 0,
                               naturalidade_cidade_id = c.naturalidade_cidade_id,
                               naturalidade_estado_id = c.tb_cid_cidade1 != null ? c.tb_cid_cidade1.est_idt_estado : 0,
                               rg = c.rg,
                               carteira_trabalho_nro = c.carteira_trabalho_nro,
                               carteira_trabalho_serie = c.carteira_trabalho_serie,
                               titulo_eleitor_nro = c.titulo_eleitor_nro,
                               titulo_eleitor_zona = c.titulo_eleitor_zona,
                               titulo_eleitor_secao = c.titulo_eleitor_secao,
                               pis_pasep_net = c.pis_pasep_net,
                               data_nascimento = c.data_nascimento,
                               nacionalidade = c.nacionalidade,
                               nome_mae = c.nome_mae,
                               nome_pai = c.nome_pai,
                               sexo_masculino = c.sexo_masculino,
                               estado_civil = c.estado_civil,
                               grau_instrucao_id = c.grau_instrucao_id,
                               raca_id = c.raca_id,
                               telefone_01 = c.telefone_01,
                               telefone_02 = c.telefone_02,
                               senha = c.senha,
                               tipo_conta = c.tipo_conta,
                               agencia = c.agencia,
                               agencia_digito = c.agencia_digito,
                               conta_corrente = c.conta_corrente,
                               conta_corrente_digito = c.conta_corrente_digito,
                               endereco_cep = c.endereco_cep,
                               endereco_logradouro = c.endereco_logradouro,
                               endereco_nro = c.endereco_nro,
                               endereco_bairro = c.endereco_bairro,
                               endereco_complemento = c.endereco_complemento,
                               dados_ok = c.dados_ok
                           });

                return cols.ToList();
            }
        }

        internal static List<ColaboradorModel> ListarCSV()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (
                        from c in db.colaborador
                        select new ColaboradorModel()
                        {
                            #region [ Dados Colaborador ]
                            id = c.id,
                            nome = c.nome,
                            cpf = c.cpf,
                            email = c.email,
                            ativo = c.ativo,

                            banco_id = c.banco_id,
                            banco = new BancoModel()
                            {
                                id = c.banco.id,
                                nome = c.banco.nome,
                                ativo = c.banco.ativo
                            },

                            carteira_trabalho_estado_id = c.carteira_trabalho_estado_id,
                            carteira_trabalho_estado = new EstadoModel()
                            {
                                Id = c.tb_est_estado.est_idt_estado,
                                Nome = c.tb_est_estado.est_nom_estado,
                                Sigla = c.tb_est_estado.est_sig_estado,
                                Ativo = c.tb_est_estado.est_bit_ativo
                            },

                            endereco_cidade_id = c.endereco_cidade_id,
                            endereco_estado_id = c.tb_cid_cidade != null ? c.tb_cid_cidade.est_idt_estado : 0,
                            endereco_cidade = new CidadeModel()
                            {
                                Id = c.tb_cid_cidade.cid_idt_cidade,
                                IdEstado = c.tb_cid_cidade.est_idt_estado,
                                Nome = c.tb_cid_cidade.cid_nom_cidade,
                                Ativo = c.tb_cid_cidade.cid_bit_ativo,

                                Estado = new EstadoModel()
                                {
                                    Id = c.tb_cid_cidade.tb_est_estado.est_idt_estado,
                                    Nome = c.tb_cid_cidade.tb_est_estado.est_nom_estado,
                                    Sigla = c.tb_cid_cidade.tb_est_estado.est_sig_estado,
                                    Ativo = c.tb_cid_cidade.tb_est_estado.est_bit_ativo
                                }
                            },

                            naturalidade_cidade_id = c.naturalidade_cidade_id,
                            naturalidade_estado_id = c.tb_cid_cidade1 != null ? c.tb_cid_cidade1.est_idt_estado : 0,
                            naturalidade_cidade = new CidadeModel()
                            {
                                Id = c.tb_cid_cidade1.cid_idt_cidade,
                                IdEstado = c.tb_cid_cidade1.est_idt_estado,
                                Nome = c.tb_cid_cidade1.cid_nom_cidade,
                                Ativo = c.tb_cid_cidade1.cid_bit_ativo,

                                Estado = new EstadoModel()
                                {
                                    Id = c.tb_cid_cidade1.tb_est_estado.est_idt_estado,
                                    Nome = c.tb_cid_cidade1.tb_est_estado.est_nom_estado,
                                    Sigla = c.tb_cid_cidade1.tb_est_estado.est_sig_estado,
                                    Ativo = c.tb_cid_cidade1.tb_est_estado.est_bit_ativo
                                }
                            },

                            grau_instrucao_id = c.grau_instrucao_id,
                            grau_instrucao = new GrauInstrucaoModel()
                            {
                                id = c.grau_instrucao.id,
                                nome = c.grau_instrucao.nome,
                                ativo = c.grau_instrucao.ativo
                            },

                            raca_id = c.raca_id,
                            raca = new RacaModel()
                            {
                                id = c.raca.id,
                                nome = c.raca.nome,
                                ativo = c.raca.ativo
                            },

                            rg = c.rg,
                            carteira_trabalho_nro = c.carteira_trabalho_nro,
                            carteira_trabalho_serie = c.carteira_trabalho_serie,
                            titulo_eleitor_nro = c.titulo_eleitor_nro,
                            titulo_eleitor_zona = c.titulo_eleitor_zona,
                            titulo_eleitor_secao = c.titulo_eleitor_secao,
                            pis_pasep_net = c.pis_pasep_net,
                            data_nascimento = c.data_nascimento,
                            nacionalidade = c.nacionalidade,
                            nome_mae = c.nome_mae,
                            nome_pai = c.nome_pai,
                            sexo_masculino = c.sexo_masculino,
                            estado_civil = c.estado_civil,
                            telefone_01 = c.telefone_01,
                            telefone_02 = c.telefone_02,
                            senha = c.senha,
                            tipo_conta = c.tipo_conta,
                            agencia = c.agencia,
                            agencia_digito = c.agencia_digito,
                            conta_corrente = c.conta_corrente,
                            conta_corrente_digito = c.conta_corrente_digito,
                            endereco_cep = c.endereco_cep,
                            endereco_logradouro = c.endereco_logradouro,
                            endereco_nro = c.endereco_nro,
                            endereco_bairro = c.endereco_bairro,
                            endereco_complemento = c.endereco_complemento,
                            dados_ok = c.dados_ok
                            #endregion [ FIM - Dados Colaborador ]
                        }
                        ).ToList();
            }
        }

        internal static ColaboradorModel Obter(int id)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                var col = (from c in db.colaborador
                           where c.id == id
                           select new ColaboradorModel()
                           {
                               id = c.id,
                               nome = c.nome,
                               cpf = c.cpf,
                               email = c.email,
                               ativo = c.ativo,

                               banco_id = c.banco_id,
                               carteira_trabalho_estado_id = c.carteira_trabalho_estado_id,
                               endereco_cidade_id = c.endereco_cidade_id,
                               endereco_estado_id = c.tb_cid_cidade != null ? c.tb_cid_cidade.est_idt_estado : 0,
                               naturalidade_cidade_id = c.naturalidade_cidade_id,
                               naturalidade_estado_id = c.tb_cid_cidade1 != null ? c.tb_cid_cidade1.est_idt_estado : 0,
                               rg = c.rg,
                               carteira_trabalho_nro = c.carteira_trabalho_nro,
                               carteira_trabalho_serie = c.carteira_trabalho_serie,
                               titulo_eleitor_nro = c.titulo_eleitor_nro,
                               titulo_eleitor_zona = c.titulo_eleitor_zona,
                               titulo_eleitor_secao = c.titulo_eleitor_secao,
                               pis_pasep_net = c.pis_pasep_net,
                               data_nascimento = c.data_nascimento,
                               nacionalidade = c.nacionalidade,
                               nome_mae = c.nome_mae,
                               nome_pai = c.nome_pai,
                               sexo_masculino = c.sexo_masculino,
                               estado_civil = c.estado_civil,
                               grau_instrucao_id = c.grau_instrucao_id,
                               raca_id = c.raca_id,
                               telefone_01 = c.telefone_01,
                               telefone_02 = c.telefone_02,
                               senha = c.senha,
                               tipo_conta = c.tipo_conta,
                               agencia = c.agencia,
                               agencia_digito = c.agencia_digito,
                               conta_corrente = c.conta_corrente,
                               conta_corrente_digito = c.conta_corrente_digito,
                               endereco_cep = c.endereco_cep,
                               endereco_logradouro = c.endereco_logradouro,
                               endereco_nro = c.endereco_nro,
                               endereco_bairro = c.endereco_bairro,
                               endereco_complemento = c.endereco_complemento,
                               dados_ok = c.dados_ok
                           });

                return col.SingleOrDefault();
            }
        }

        internal static ColaboradorModel ObterPorCPF(string cpf)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                var col = (from c in db.colaborador
                           where c.cpf == cpf
                           select new ColaboradorModel()
                           {
                               id = c.id,
                               nome = c.nome,
                               cpf = c.cpf,
                               email = c.email,
                               ativo = c.ativo,

                               banco_id = c.banco_id,
                               carteira_trabalho_estado_id = c.carteira_trabalho_estado_id,
                               endereco_cidade_id = c.endereco_cidade_id,
                               endereco_estado_id = c.tb_cid_cidade != null ? c.tb_cid_cidade.est_idt_estado : 0,
                               naturalidade_cidade_id = c.naturalidade_cidade_id,
                               naturalidade_estado_id = c.tb_cid_cidade1 != null ? c.tb_cid_cidade1.est_idt_estado : 0,
                               rg = c.rg,
                               carteira_trabalho_nro = c.carteira_trabalho_nro,
                               carteira_trabalho_serie = c.carteira_trabalho_serie,
                               titulo_eleitor_nro = c.titulo_eleitor_nro,
                               titulo_eleitor_zona = c.titulo_eleitor_zona,
                               titulo_eleitor_secao = c.titulo_eleitor_secao,
                               pis_pasep_net = c.pis_pasep_net,
                               data_nascimento = c.data_nascimento,
                               nacionalidade = c.nacionalidade,
                               nome_mae = c.nome_mae,
                               nome_pai = c.nome_pai,
                               sexo_masculino = c.sexo_masculino,
                               estado_civil = c.estado_civil,
                               grau_instrucao_id = c.grau_instrucao_id,
                               raca_id = c.raca_id,
                               telefone_01 = c.telefone_01,
                               telefone_02 = c.telefone_02,
                               senha = c.senha,
                               tipo_conta = c.tipo_conta,
                               agencia = c.agencia,
                               agencia_digito = c.agencia_digito,
                               conta_corrente = c.conta_corrente,
                               conta_corrente_digito = c.conta_corrente_digito,
                               endereco_cep = c.endereco_cep,
                               endereco_logradouro = c.endereco_logradouro,
                               endereco_nro = c.endereco_nro,
                               endereco_bairro = c.endereco_bairro,
                               endereco_complemento = c.endereco_complemento,
                               dados_ok = c.dados_ok
                           });

                return col.SingleOrDefault();
            }
        }

        internal static ColaboradorModel ObterPorEmail(string email)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                var col = (from c in db.colaborador
                           where c.email.Equals(email, StringComparison.InvariantCultureIgnoreCase)
                           select new ColaboradorModel()
                           {
                               id = c.id,
                               nome = c.nome,
                               cpf = c.cpf,
                               email = c.email,
                               ativo = c.ativo,

                               banco_id = c.banco_id,
                               carteira_trabalho_estado_id = c.carteira_trabalho_estado_id,
                               endereco_cidade_id = c.endereco_cidade_id,
                               endereco_estado_id = c.tb_cid_cidade != null ? c.tb_cid_cidade.est_idt_estado : 0,
                               naturalidade_cidade_id = c.naturalidade_cidade_id,
                               naturalidade_estado_id = c.tb_cid_cidade1 != null ? c.tb_cid_cidade1.est_idt_estado : 0,
                               rg = c.rg,
                               carteira_trabalho_nro = c.carteira_trabalho_nro,
                               carteira_trabalho_serie = c.carteira_trabalho_serie,
                               titulo_eleitor_nro = c.titulo_eleitor_nro,
                               titulo_eleitor_zona = c.titulo_eleitor_zona,
                               titulo_eleitor_secao = c.titulo_eleitor_secao,
                               pis_pasep_net = c.pis_pasep_net,
                               data_nascimento = c.data_nascimento,
                               nacionalidade = c.nacionalidade,
                               nome_mae = c.nome_mae,
                               nome_pai = c.nome_pai,
                               sexo_masculino = c.sexo_masculino,
                               estado_civil = c.estado_civil,
                               grau_instrucao_id = c.grau_instrucao_id,
                               raca_id = c.raca_id,
                               telefone_01 = c.telefone_01,
                               telefone_02 = c.telefone_02,
                               senha = c.senha,
                               tipo_conta = c.tipo_conta,
                               agencia = c.agencia,
                               agencia_digito = c.agencia_digito,
                               conta_corrente = c.conta_corrente,
                               conta_corrente_digito = c.conta_corrente_digito,
                               endereco_cep = c.endereco_cep,
                               endereco_logradouro = c.endereco_logradouro,
                               endereco_nro = c.endereco_nro,
                               endereco_bairro = c.endereco_bairro,
                               endereco_complemento = c.endereco_complemento,
                               dados_ok = c.dados_ok
                           });

                return col.SingleOrDefault();
            }
        }

        internal static ColaboradorModel ObterPorEmailSenha(string email, string senhaCriptografada)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                var col = (from c in db.colaborador
                           where c.email.Equals(email, StringComparison.InvariantCultureIgnoreCase)
                           && c.senha == senhaCriptografada
                           select new ColaboradorModel()
                           {
                               id = c.id,
                               nome = c.nome,
                               cpf = c.cpf,
                               email = c.email,
                               ativo = c.ativo,

                               banco_id = c.banco_id,
                               carteira_trabalho_estado_id = c.carteira_trabalho_estado_id,
                               endereco_cidade_id = c.endereco_cidade_id,
                               endereco_estado_id = c.tb_cid_cidade != null ? c.tb_cid_cidade.est_idt_estado : 0,
                               naturalidade_cidade_id = c.naturalidade_cidade_id,
                               naturalidade_estado_id = c.tb_cid_cidade1 != null ? c.tb_cid_cidade1.est_idt_estado : 0,
                               rg = c.rg,
                               carteira_trabalho_nro = c.carteira_trabalho_nro,
                               carteira_trabalho_serie = c.carteira_trabalho_serie,
                               titulo_eleitor_nro = c.titulo_eleitor_nro,
                               titulo_eleitor_zona = c.titulo_eleitor_zona,
                               titulo_eleitor_secao = c.titulo_eleitor_secao,
                               pis_pasep_net = c.pis_pasep_net,
                               data_nascimento = c.data_nascimento,
                               nacionalidade = c.nacionalidade,
                               nome_mae = c.nome_mae,
                               nome_pai = c.nome_pai,
                               sexo_masculino = c.sexo_masculino,
                               estado_civil = c.estado_civil,
                               grau_instrucao_id = c.grau_instrucao_id,
                               raca_id = c.raca_id,
                               telefone_01 = c.telefone_01,
                               telefone_02 = c.telefone_02,
                               senha = c.senha,
                               tipo_conta = c.tipo_conta,
                               agencia = c.agencia,
                               agencia_digito = c.agencia_digito,
                               conta_corrente = c.conta_corrente,
                               conta_corrente_digito = c.conta_corrente_digito,
                               endereco_cep = c.endereco_cep,
                               endereco_logradouro = c.endereco_logradouro,
                               endereco_nro = c.endereco_nro,
                               endereco_bairro = c.endereco_bairro,
                               endereco_complemento = c.endereco_complemento,
                               dados_ok = c.dados_ok
                           });

                return col.SingleOrDefault();
            }
        }

        internal static ColaboradorModel Salvar(ColaboradorModel cM)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                colaborador toUpdate = db.colaborador.SingleOrDefault(col => col.id == cM.id);

                if (toUpdate == null) // Nao encontrou
                {
                    toUpdate = new colaborador();
                    db.colaborador.Add(toUpdate);
                    cM.ativo = true;
                }

                toUpdate.agencia = cM.agencia;
                toUpdate.agencia_digito = cM.agencia_digito;
                toUpdate.ativo = cM.ativo;
                toUpdate.banco_id = cM.banco_id;
                toUpdate.carteira_trabalho_estado_id = cM.carteira_trabalho_estado_id;
                toUpdate.carteira_trabalho_nro = cM.carteira_trabalho_nro?.FirstCharOfEachWordToUpper();
                toUpdate.carteira_trabalho_serie = cM.carteira_trabalho_serie?.FirstCharOfEachWordToUpper();
                toUpdate.conta_corrente = cM.conta_corrente;
                toUpdate.conta_corrente_digito = cM.conta_corrente_digito;
                toUpdate.dados_ok = cM.dados_ok;
                toUpdate.raca_id = cM.raca_id;
                toUpdate.cpf = cM.cpf;
                toUpdate.data_nascimento = cM.data_nascimento.AddHours(4);
                toUpdate.email = cM.email;
                toUpdate.endereco_bairro = cM.endereco_bairro?.FirstCharOfEachWordToUpper();
                toUpdate.endereco_cep = cM.endereco_cep;
                toUpdate.endereco_cidade_id = cM.endereco_cidade_id;
                toUpdate.endereco_complemento = cM.endereco_complemento?.FirstCharOfEachWordToUpper();
                toUpdate.endereco_logradouro = cM.endereco_logradouro?.FirstCharOfEachWordToUpper();
                toUpdate.endereco_nro = cM.endereco_nro?.FirstCharOfEachWordToUpper();
                toUpdate.estado_civil = cM.estado_civil?.FirstCharOfEachWordToUpper();
                toUpdate.grau_instrucao_id = cM.grau_instrucao_id;
                toUpdate.nacionalidade = cM.nacionalidade?.FirstCharOfEachWordToUpper();
                toUpdate.naturalidade_cidade_id = cM.naturalidade_cidade_id;
                toUpdate.nome = cM.nome?.FirstCharOfEachWordToUpper();
                toUpdate.nome_mae = cM.nome_mae?.FirstCharOfEachWordToUpper();
                toUpdate.nome_pai = cM.nome_pai?.FirstCharOfEachWordToUpper();
                toUpdate.pis_pasep_net = cM.pis_pasep_net?.FirstCharOfEachWordToUpper();
                toUpdate.rg = cM.rg?.FirstCharOfEachWordToUpper();
                toUpdate.sexo_masculino = cM.sexo_masculino;
                toUpdate.telefone_01 = cM.telefone_01;
                toUpdate.telefone_02 = cM.telefone_02;
                toUpdate.tipo_conta = cM.tipo_conta;
                toUpdate.titulo_eleitor_nro = cM.titulo_eleitor_nro?.FirstCharOfEachWordToUpper();
                toUpdate.titulo_eleitor_secao = cM.titulo_eleitor_secao?.FirstCharOfEachWordToUpper();
                toUpdate.titulo_eleitor_zona = cM.titulo_eleitor_zona?.FirstCharOfEachWordToUpper();

                if (!String.IsNullOrEmpty(cM.senha))
                    toUpdate.senha = cM.senha;

                db.SaveChangesWithErrors();

                return Obter(toUpdate.id);
            }
        }
    }

    public class ColaboradorModel
    {
        public ColaboradorModel() { }

        public ColaboradorModel(colaborador c, bool mini = false)
        {
            if (c == null)
                return;

            this.id = c.id;
            this.nome = c.nome;
            this.cpf = c.cpf;
            this.email = c.email;
            this.ativo = c.ativo;

            if (!mini)
            {
                this.banco_id = c.banco_id;
                this.carteira_trabalho_estado_id = c.carteira_trabalho_estado_id;
                this.endereco_cidade_id = c.endereco_cidade_id;
                this.endereco_estado_id = c.tb_cid_cidade != null ? c.tb_cid_cidade.est_idt_estado : 0;
                this.naturalidade_cidade_id = c.naturalidade_cidade_id;
                this.naturalidade_estado_id = c.tb_cid_cidade1 != null ? c.tb_cid_cidade1.est_idt_estado : 0;
                this.rg = c.rg;
                this.carteira_trabalho_nro = c.carteira_trabalho_nro;
                this.carteira_trabalho_serie = c.carteira_trabalho_serie;
                this.titulo_eleitor_nro = c.titulo_eleitor_nro;
                this.titulo_eleitor_zona = c.titulo_eleitor_zona;
                this.titulo_eleitor_secao = c.titulo_eleitor_secao;
                this.pis_pasep_net = c.pis_pasep_net;
                this.data_nascimento = c.data_nascimento;
                this.nacionalidade = c.nacionalidade;
                this.nome_mae = c.nome_mae;
                this.nome_pai = c.nome_pai;
                this.sexo_masculino = c.sexo_masculino;
                this.estado_civil = c.estado_civil;
                this.grau_instrucao_id = c.grau_instrucao_id;
                this.raca_id = c.raca_id;
                this.telefone_01 = c.telefone_01;
                this.telefone_02 = c.telefone_02;
                this.senha = c.senha;
                this.tipo_conta = c.tipo_conta;
                this.agencia = c.agencia;
                this.agencia_digito = c.agencia_digito;
                this.conta_corrente = c.conta_corrente;
                this.conta_corrente_digito = c.conta_corrente_digito;
                this.endereco_cep = c.endereco_cep;
                this.endereco_logradouro = c.endereco_logradouro;
                this.endereco_nro = c.endereco_nro;
                this.endereco_bairro = c.endereco_bairro;
                this.endereco_complemento = c.endereco_complemento;
                this.dados_ok = c.dados_ok;
            }
        }

        #region [ Propriedades ]

        public int id                           { get; set; } = 0;
        public int banco_id                     { get; set; }
        public int? carteira_trabalho_estado_id { get; set; }
        public int endereco_cidade_id           { get; set; }
        public int endereco_estado_id           { get; set; } = 0;
        public int naturalidade_cidade_id       { get; set; }
        public int naturalidade_estado_id       { get; set; } = 0;
        public string nome                      { get; set; }
        public string cpf                       { get; set; }
        public string rg                        { get; set; }
        public string carteira_trabalho_nro     { get; set; }
        public string carteira_trabalho_serie   { get; set; }
        public string titulo_eleitor_nro        { get; set; }
        public string titulo_eleitor_zona       { get; set; }
        public string titulo_eleitor_secao      { get; set; }
        public string pis_pasep_net             { get; set; }
        public DateTime data_nascimento         { get; set; }
        public string nacionalidade             { get; set; }
        public string nome_mae                  { get; set; }
        public string nome_pai                  { get; set; }
        public bool sexo_masculino              { get; set; }
        public string estado_civil              { get; set; }
        public int grau_instrucao_id            { get; set; }
        public int raca_id                      { get; set; }
        public string telefone_01               { get; set; }
        public string telefone_02               { get; set; }
        public string email                     { get; set; }
        public string senha                     { get; set; }
        public int tipo_conta                   { get; set; }
        public int agencia                      { get; set; }
        public string agencia_digito            { get; set; }
        public string conta_corrente            { get; set; }
        public string conta_corrente_digito     { get; set; }
        public string endereco_cep              { get; set; }
        public string endereco_logradouro       { get; set; }
        public string endereco_nro              { get; set; }
        public string endereco_bairro           { get; set; }
        public string endereco_complemento      { get; set; }
        public bool dados_ok                    { get; set; } = false;
        public bool ativo                       { get; set; }

        public BancoModel banco                     { get; set; } = new BancoModel();
        public EstadoModel carteira_trabalho_estado { get; set; } = new EstadoModel();
        public CidadeModel endereco_cidade          { get; set; } = new CidadeModel();
        public CidadeModel naturalidade_cidade      { get; set; } = new CidadeModel();
        public GrauInstrucaoModel grau_instrucao    { get; set; } = new GrauInstrucaoModel();
        public RacaModel raca                       { get; set; } = new RacaModel();

        [ScriptIgnore]
        public string senhaDescriptografada { get; set; }

        public string codigo { get { return this.id.ToString().PadLeft(6, '0'); } }
        public string cpf_formatado { get { return BaseData.FormatarCPF(this.cpf); } }
        public string telefone_01_formatado { get { return BaseData.FormatarFone(this.telefone_01); } }
        public string telefone_02_formatado { get { return BaseData.FormatarFone(this.telefone_02, true); } }

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public static ColaboradorModel Clone(colaborador col)
        {
            if (col == null)
                return null;

            ColaboradorModel c = new ColaboradorModel(col);

            if (col.tb_cid_cidade != null)
                c.endereco_estado_id = col.tb_cid_cidade.est_idt_estado;

            if (col.tb_cid_cidade1 != null)
                c.naturalidade_estado_id = col.tb_cid_cidade1.est_idt_estado;

            return c;
        }

        #endregion [ FIM - Metodos ]
    }

    public class ColaboradorRPAModel
    {
        #region [ Propriedades ]

        public int id                           { get; set; } = 0;
        public int banco_id                     { get; set; }
        public int? carteira_trabalho_estado_id { get; set; }
        public int endereco_cidade_id           { get; set; }
        public int endereco_estado_id           { get; set; } = 0;
        public int naturalidade_cidade_id       { get; set; }
        public int naturalidade_estado_id       { get; set; } = 0;
        public string nome                      { get; set; }
        public string cpf                       { get; set; }
        public string rg                        { get; set; }
        public string carteira_trabalho_nro     { get; set; }
        public string carteira_trabalho_serie   { get; set; }
        public string titulo_eleitor_nro        { get; set; }
        public string titulo_eleitor_zona       { get; set; }
        public string titulo_eleitor_secao      { get; set; }
        public string pis_pasep_net             { get; set; }
        public DateTime data_nascimento         { get; set; }
        public string nacionalidade             { get; set; }
        public string nome_mae                  { get; set; }
        public string nome_pai                  { get; set; }
        public bool sexo_masculino              { get; set; }
        public string estado_civil              { get; set; }
        public int grau_instrucao_id            { get; set; }
        public int raca_id                      { get; set; }
        public string telefone_01               { get; set; }
        public string telefone_02               { get; set; }
        public string email                     { get; set; }
        public string senha                     { get; set; }
        public int tipo_conta                   { get; set; }
        public int agencia                      { get; set; }
        public string agencia_digito            { get; set; }
        public string conta_corrente            { get; set; }
        public string conta_corrente_digito     { get; set; }
        public string endereco_cep              { get; set; }
        public string endereco_logradouro       { get; set; }
        public string endereco_nro              { get; set; }
        public string endereco_bairro           { get; set; }
        public string endereco_complemento      { get; set; }
        public string endereco_cidade_uf_nome   { get; set; }

        public string codigo                    { get { return this.id.ToString().PadLeft(6, '0'); } }
        public string cpf_formatado             { get { return BaseData.FormatarCPF(this.cpf); } }
        public string telefone_01_formatado     { get { return BaseData.FormatarFone(this.telefone_01); } }
        public string telefone_02_formatado     { get { return BaseData.FormatarFone(this.telefone_02, true); } }

        public string emitente_razao_social     { get; set; }
        public string emitente_cnpj             { get; set; }
        public string mesAno                    { get { return BaseData.DataAgora.ToString("MM/yyyy"); } }
        public string emitente_endereco         { get; set; }
        public string emitente_bairro           { get; set; }
        public string emitente_cidade           { get; set; }

        public string funcao_nome               { get; set; }
        public decimal valor_sem_formatacao     { get; set; }
        public string aliquota_inss             { get { return descontar_inss ? ConfigurationManager.AppSettings["Aliquota_INSS"] : "0"; } }
        public string aliquota_iss              { get { return descontar_iss ? ConfigurationManager.AppSettings["Aliquota_ISS"] : "0"; } }
        public string aliquota_irpf             { get; set; }
        public string deducao_irpf              { get; set; }
        public bool descontar_inss              { get; set; } = true;
        public bool descontar_iss               { get; set; } = true;
        public decimal valor_irpf_sem_formatacao { get; set; }

        public string data_hoje_formatado       { get { return BaseData.DataAgora.ToString("dd/MM/yyyy"); } }
        public string valor_bruto               { get { return String.Format("{0:C2}", this.valor_sem_formatacao); } }
        public string valor_inss                { get { return String.Format("{0:C2}", this.valor_inss_sem_formatacao); } }
        public string valor_iss                 { get { return String.Format("{0:C2}", this.valor_iss_sem_formatacao); } }
        public string valor_irpf                { get { return String.Format("{0:C2}", this.valor_irpf_sem_formatacao); } }
        public string valor_liquido             { get { return String.Format("{0:C2}", this.valor_liquido_sem_formatacao); } }

        public string valor_bruto_5c            { get { return String.Format("{0:C5}", this.valor_sem_formatacao); } }
        public string valor_inss_5c             { get { return String.Format("{0:C5}", this.valor_inss_sem_formatacao); } }
        public string valor_iss_5c              { get { return String.Format("{0:C5}", this.valor_iss_sem_formatacao); } }
        public string valor_irpf_5c             { get { return String.Format("{0:C5}", this.valor_irpf_sem_formatacao); } }
        public string valor_liquido_5c          { get { return String.Format("{0:C5}", this.valor_liquido_sem_formatacao); } }
        public string tipo_conta_texto          { get { return this.tipo_conta == 2 ? "Poupança" : "Corrente"; } }

        private decimal valor_sem_inss          { get { return this.valor_sem_formatacao * this.valor_inss_sem_formatacao / 100; } }
        //public decimal valor_liquido_sem_formatacao   { get { return this.valor_sem_formatacao - this.valor_inss_sem_formatacao - this.valor_iss_sem_formatacao - valor_irpf_sem_formatacao; } }
        public decimal valor_liquido_sem_formatacao { get; set; }
        public string valor_liquido_por_extenso { get { return this.valor_liquido_sem_formatacao.EscreverExtenso(); } }

        public decimal valor_inss_sem_formatacao
        {
            get
            {
                decimal inss = 0;

                Decimal.TryParse(this.aliquota_inss, out inss);

                return this.valor_sem_formatacao * inss / 100;
            }
        }

        public decimal valor_iss_sem_formatacao
        {
            get
            {
                decimal iss = 0;

                Decimal.TryParse(this.aliquota_iss, out iss);

                return this.valor_sem_formatacao * iss / 100;
            }
        }

        private struct struct_irpf
        {
            public decimal irpf_retido  { get; set; }
            public decimal deducao      { get; set; }
            public decimal taxa         { get; set; }
        }

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public static ColaboradorRPAModel Clone(ColaboradorModel col, List<ConcursoLocalColaboradorModel> locaisColaboradores, List<IRPFModel> irpfs, tb_emp_empresa emitente)
        {
            if (col == null)
                return null;

            ColaboradorRPAModel colaborador = col.CopyObject<ColaboradorRPAModel>();

            if (col.endereco_cidade != null && col.endereco_cidade.Estado != null)
                colaborador.endereco_cidade_uf_nome = $"{col.endereco_cidade.Nome}/{col.endereco_cidade.Estado.Sigla}";

            if (locaisColaboradores.Count > 0)
            {
                var localColaborador = locaisColaboradores.FirstOrDefault(lc => lc.colaborador_id == colaborador.id);

                if (localColaborador != null)
                {
                    colaborador.descontar_inss = localColaborador.inss;
                    colaborador.descontar_iss = localColaborador.iss;
                    colaborador.funcao_nome = localColaborador.funcao.funcao;

                    decimal valor_bruto_sem_irpf = 0;

                    if (colaborador.descontar_inss && colaborador.descontar_iss)
                        valor_bruto_sem_irpf = localColaborador.valor * 1.1765m;
                    else if (colaborador.descontar_inss)
                        valor_bruto_sem_irpf = localColaborador.valor * 1.1236m;
                    else if (colaborador.descontar_iss)
                        valor_bruto_sem_irpf = localColaborador.valor * 1.0417m;
                    else
                        valor_bruto_sem_irpf = localColaborador.valor;

                    colaborador.valor_sem_formatacao = valor_bruto_sem_irpf;
                    colaborador.valor_liquido_sem_formatacao = localColaborador.valor;
                }
            }

            if (irpfs.Count > 0)
            {
                struct_irpf sirpf = CalcularAliquotaIRPF(irpfs, colaborador.valor_sem_inss);
                colaborador.aliquota_irpf = sirpf.taxa.ToString();
                colaborador.deducao_irpf = sirpf.deducao.ToString();
                colaborador.valor_irpf_sem_formatacao = sirpf.irpf_retido;
            }

            if (emitente != null)
            {
                colaborador.emitente_razao_social = emitente.emp_des_razao_social;
                colaborador.emitente_cnpj = emitente.CNPJ_Formatado;
                colaborador.emitente_endereco = ConfigurationManager.AppSettings["EnderecoIPEFAE"];
                colaborador.emitente_bairro = ConfigurationManager.AppSettings["BairroIPEFAE"];
                colaborador.emitente_cidade = ConfigurationManager.AppSettings["CidadeIPEFAE"];
            }

            return colaborador;
        }

        private static struct_irpf CalcularAliquotaIRPF(List<IRPFModel> irpfs, decimal valor_sem_inss)
        {
            struct_irpf sirpf = new struct_irpf();
            sirpf.irpf_retido = 0;
            sirpf.deducao = 0;
            sirpf.taxa = 0;

            IRPFModel ir = irpfs.FirstOrDefault(i => i.valor_min <= valor_sem_inss && i.valor_max >= valor_sem_inss);

            if (ir == null)
                return sirpf;

            decimal possivelValor = ((valor_sem_inss * ir.taxa / 100) - ir.deducao);
            sirpf.irpf_retido = possivelValor < 0 ? 0 : possivelValor;
            sirpf.deducao = ir.deducao;
            sirpf.taxa = ir.taxa;

            return sirpf;
        }

        #endregion [ FIM - Metodos ]
    }
}
