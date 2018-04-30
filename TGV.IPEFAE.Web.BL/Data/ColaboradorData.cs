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
        public int agencia                      { get; set; }
        public string agencia_digito            { get; set; }
        public string conta_corrente            { get; set; }
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

        public string funcao_nome               { get; set; }
        public decimal valor_sem_formatacao     { get; set; }
        public string aliquota_inss             { get { return ConfigurationManager.AppSettings["Aliquota_INSS"]; } }
        public string aliquota_iss              { get { return ConfigurationManager.AppSettings["Aliquota_ISS"]; } }
        public string aliquota_irpf             { get; set; }
        public string deducao_irpf              { get; set; }
        public decimal valor_irpf_sem_formatacao { get; set; }

        public string data_hoje_formatado       { get { return BaseData.DataAgora.ToString("dd/MM/yyyy"); } }
        public string valor                     { get { return String.Format("{0:C2}", this.valor_sem_formatacao); } }
        public string valor_inss                { get { return String.Format("{0:C2}", this.valor_inss_sem_formatacao); } }
        public string valor_iss                 { get { return String.Format("{0:C2}", this.valor_iss_sem_formatacao); } }
        public string valor_irpf                { get { return String.Format("{0:C2}", this.valor_irpf_sem_formatacao); } }
        public string valor_final               { get { return String.Format("{0:C2}", this.valor_final_sem_formatacao); } }
        private decimal valor_sem_inss          { get { return this.valor_sem_formatacao * this.valor_inss_sem_formatacao / 100; } }
        public decimal valor_final_sem_formatacao   { get { return this.valor_sem_formatacao - this.valor_inss_sem_formatacao - this.valor_iss_sem_formatacao - valor_irpf_sem_formatacao; } }
        public string valor_final_por_extenso   { get { return this.valor_final_sem_formatacao.EscreverExtenso(); } }

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

        public static ColaboradorRPAModel Clone(ColaboradorModel col, List<tb_cid_cidade> cidades, List<tb_est_estado> estados, List<ConcursoLocalColaboradorModel> locaisColaboradores, List<IRPFModel> irpfs)
        {
            if (col == null)
                return null;

            ColaboradorRPAModel colaborador = col.CopyObject<ColaboradorRPAModel>();

            if (cidades.Count > 0 && estados.Count > 0)
            {
                var cid = cidades.FirstOrDefault(c => c.cid_idt_cidade == colaborador.endereco_cidade_id);

                if (cid != null)
                {
                    var est = estados.FirstOrDefault(e => e.est_idt_estado == cid.est_idt_estado);
                    var siglaEstado = String.Empty;

                    if (est != null)
                        siglaEstado = $"/{est.est_sig_estado}";

                    colaborador.endereco_cidade_uf_nome = $"{cid.cid_nom_cidade}{siglaEstado}";
                }
            }

            if (locaisColaboradores.Count > 0)
            {
                var localColaborador = locaisColaboradores.FirstOrDefault(lc => lc.colaborador_id == colaborador.id);

                if (localColaborador != null)
                {
                    colaborador.funcao_nome = localColaborador.funcao.funcao;
                    colaborador.valor_sem_formatacao = localColaborador.valor;
                }
            }

            if (irpfs.Count > 0)
            {
                struct_irpf sirpf = CalcularAliquotaIRPF(irpfs, colaborador.valor_sem_inss);
                colaborador.aliquota_irpf = sirpf.taxa.ToString();
                colaborador.deducao_irpf = sirpf.deducao.ToString();
                colaborador.valor_irpf_sem_formatacao = sirpf.irpf_retido;
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
