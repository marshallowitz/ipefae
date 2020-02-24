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
        public static List<ColaboradorModel> Listar(bool mini = false)
        {
            return ColaboradorData.Listar(mini);
        }

        public static List<ColaboradorModel> ListarCSV()
        {
            return ColaboradorData.ListarCSV();
        }

        public static List<ColaboradorModel> ListarPorConcurso(int idConcurso, int inicio, int total)
        {
            ConcursoModel concurso = ConcursoBusiness.Obter(idConcurso, true);

            if (concurso == null)
                return new List<ColaboradorModel>();

            List<ColaboradorModel> colaboradores = new List<ColaboradorModel>();

            using (IPEFAEEntities db = BaseData.Contexto)
            {
                colaboradores = (from c in db.colaborador
                                 where c.concurso_local_colaborador.Any(clc => clc.concurso_local.concurso_id == concurso.id)
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
                                 }).ToList();
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

            using (IPEFAEEntities db = BaseData.Contexto)
            {
                colaboradores = (from c in db.colaborador
                                 where c.concurso_local_colaborador.Any(clc => clc.concurso_local.concurso_id == concurso.id)
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
                                 }).ToList();

                cLocaisColaboradores = (from clc in db.concurso_local_colaborador
                                        where clc.concurso_local.concurso_id == concurso.id
                                        select new ConcursoLocalColaboradorModel()
                                        {
                                            #region [ Dados ConcursoLocalColaborador]

                                            id = clc.id,
                                            concurso_local_id = clc.concurso_local_id,
                                            concurso_local = new ConcursoLocalModel()
                                            {
                                                id = clc.concurso_local.id,
                                                concurso_id = clc.concurso_local.concurso_id,
                                                ativo = clc.concurso_local.ativo,
                                                local = clc.concurso_local.local
                                            },

                                            colaborador_id = clc.colaborador_id,

                                            funcao_id = clc.funcao_id,
                                            funcao = new ConcursoFuncaoModel()
                                            {
                                                ativo = clc.concurso_funcao.ativo,
                                                concurso_id = clc.concurso_funcao.concurso_id,
                                                funcao = clc.concurso_funcao.funcao,
                                                id = clc.concurso_funcao.id,
                                                sem_desconto = clc.concurso_funcao.sem_desconto,
                                                valor_liquido = clc.concurso_funcao.valor_liquido
                                            },

                                            valor = clc.valor,
                                            inss = clc.inss,
                                            iss = clc.iss,
                                            ativo = clc.ativo

                                            #endregion [ FIM - Dados ConcursoLocalColaborador]
                                        }).ToList();
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

        public static ColaboradorModel Salvar(ColaboradorModel cM, bool novaSenha = false)
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
            if (!novaSenha && !String.IsNullOrEmpty(cM.senha))
                cM.senha = cM.senha.Criptografar(BaseBusiness.ParametroSistema);
            else if (novaSenha)
                cM.senha = cM.senhaDescriptografada.Criptografar(BaseBusiness.ParametroSistema);

            ColaboradorModel retorno = ColaboradorData.Salvar(cM);
            retorno.senhaDescriptografada = retorno.senha.Descriptografar(BaseBusiness.ParametroSistema);
            return retorno;
        }
    }
}
