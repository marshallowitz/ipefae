using System;
using System.Collections.Generic;
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
                return db.colaborador.ToList().ConvertAll(c => c.CopyObject<ColaboradorModel>());
            }
        }

        internal static ColaboradorModel Obter(int id)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return ColaboradorModel.Clone(db.colaborador.Include("tb_cid_cidade").Include("tb_cid_cidade1").SingleOrDefault(col => col.id == id));
            }
        }

        internal static ColaboradorModel ObterPorCPF(string cpf)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.colaborador.SingleOrDefault(col => col.cpf == cpf).CopyObject<ColaboradorModel>();
            }
        }

        internal static ColaboradorModel ObterPorEmail(string email)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.colaborador.SingleOrDefault(col => col.email.Equals(email, StringComparison.InvariantCultureIgnoreCase)).CopyObject<ColaboradorModel>();
            }
        }

        internal static ColaboradorModel ObterPorEmailSenha(string email, string senhaCriptografada)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.colaborador.SingleOrDefault(col => col.email.Equals(email, StringComparison.InvariantCultureIgnoreCase) && col.senha == senhaCriptografada).CopyObject<ColaboradorModel>();
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
                else
                    cM.senha = null;

                toUpdate.agencia = cM.agencia;
                toUpdate.agencia_digito = cM.agencia_digito;
                toUpdate.ativo = cM.ativo;
                toUpdate.banco_id = cM.banco_id;
                toUpdate.carteira_trabalho_estado_id = cM.carteira_trabalho_estado_id;
                toUpdate.carteira_trabalho_nro = cM.carteira_trabalho_nro?.FirstCharOfEachWordToUpper();
                toUpdate.carteira_trabalho_serie = cM.carteira_trabalho_serie?.FirstCharOfEachWordToUpper();
                toUpdate.conta_corrente = cM.conta_corrente?.FirstCharOfEachWordToUpper();
                toUpdate.raca_id = cM.raca_id;
                toUpdate.cpf = cM.cpf;
                toUpdate.data_nascimento = cM.data_nascimento;
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
        #region [ Propriedades ]

        public int id { get; set; } = 0;
        public int banco_id { get; set; }
        public int? carteira_trabalho_estado_id { get; set; }
        public int endereco_cidade_id { get; set; }
        public int endereco_estado_id { get; set; } = 0;
        public int naturalidade_cidade_id { get; set; }
        public int naturalidade_estado_id { get; set; } = 0;
        public string nome { get; set; }
        public string cpf { get; set; }
        public string rg { get; set; }
        public string carteira_trabalho_nro { get; set; }
        public string carteira_trabalho_serie { get; set; }
        public string titulo_eleitor_nro { get; set; }
        public string titulo_eleitor_zona { get; set; }
        public string titulo_eleitor_secao { get; set; }
        public string pis_pasep_net { get; set; }
        public DateTime data_nascimento { get; set; }
        public string nacionalidade { get; set; }
        public string nome_mae { get; set; }
        public string nome_pai { get; set; }
        public bool sexo_masculino { get; set; }
        public string estado_civil { get; set; }
        public int grau_instrucao_id { get; set; }
        public int raca_id { get; set; }
        public string telefone_01 { get; set; }
        public string telefone_02 { get; set; }
        public string email { get; set; }
        public string senha { get; set; }
        public int agencia { get; set; }
        public string agencia_digito { get; set; }
        public string conta_corrente { get; set; }
        public string endereco_cep { get; set; }
        public string endereco_logradouro { get; set; }
        public string endereco_nro { get; set; }
        public string endereco_bairro { get; set; }
        public string endereco_complemento { get; set; }
        public bool ativo { get; set; }

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

            ColaboradorModel c = col.CopyObject<ColaboradorModel>();

            if (col.tb_cid_cidade != null)
                c.endereco_estado_id = col.tb_cid_cidade.est_idt_estado;

            if (col.tb_cid_cidade1 != null)
                c.naturalidade_estado_id = col.tb_cid_cidade1.est_idt_estado;

            return c;
        }

        #endregion [ FIM - Metodos ]
    }
}
