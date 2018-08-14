using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Models
{
    public class ColaboradorCSVModel
    {
        public ColaboradorCSVModel(ConcursoModel concurso, ColaboradorModel colaborador, List<ColaboradorRPAModel> colaboradoresRPA, List<tb_est_estado> Estados, List<tb_cid_cidade> Cidades, List<GrauInstrucaoModel> grausInstrucao, List<RacaModel> racas, List<BancoModel> bancos)
        {
            if (colaborador == null)
                return;

            var estado_ct = Estados.FirstOrDefault(e => e.est_idt_estado == colaborador.carteira_trabalho_estado_id);
            var cidade_nat = Cidades.FirstOrDefault(c => c.cid_idt_cidade == colaborador.naturalidade_cidade_id);
            var estado_nat = cidade_nat == null ? null : Estados.FirstOrDefault(e => e.est_idt_estado == cidade_nat.est_idt_estado);
            var grin = grausInstrucao.FirstOrDefault(gi => gi.id == colaborador.grau_instrucao_id);
            var ra = racas.FirstOrDefault(r => r.id == colaborador.raca_id);
            var ba = bancos.FirstOrDefault(b => b.id == colaborador.banco_id);
            var cidade_end = Cidades.FirstOrDefault(c => c.cid_idt_cidade == colaborador.endereco_cidade_id);
            var estado_end = cidade_end == null ? null : Estados.FirstOrDefault(e => e.est_idt_estado == cidade_end.est_idt_estado);

            ColaboradorRPAModel cRPA = colaboradoresRPA.FirstOrDefault(cr => cr.id == colaborador.id);
            ConcursoLocalModel clm = concurso.locais.FirstOrDefault(c => c.Colaboradores.Any(co => co.colaborador_id == colaborador.id));

            this.nome_concurso = concurso.nome;
            this.data_concurso = concurso.data.ToString("ddMMyyyy");
            this.local_concurso = clm != null ? clm.local : String.Empty;
            this.funcao_colaborador = cRPA != null ? cRPA.funcao_nome : String.Empty;
            this.valor_bruto = cRPA != null ? String.Format("{0:N}", cRPA.valor_bruto_5c) : String.Empty;
            this.valor_liquido = cRPA != null ? String.Format("{0:N}", cRPA.valor_liquido_5c) : String.Empty;
            this.descontar_inss = cRPA != null && cRPA.descontar_inss ? "Sim" : "Não";
            this.descontar_iss = cRPA != null && cRPA.descontar_iss ? "Sim" : "Não";

            this.codigo = colaborador.codigo;
            this.nome = colaborador.nome;
            this.cpf = BaseBusiness.FormatarCPF(colaborador.cpf);
            this.rg = colaborador.rg;
            this.carteira_trabalho_nro = String.IsNullOrEmpty(colaborador.carteira_trabalho_nro) ? String.Empty : colaborador.carteira_trabalho_nro;
            this.carteira_trabalho_serie = String.IsNullOrEmpty(colaborador.carteira_trabalho_serie) ? String.Empty : colaborador.carteira_trabalho_serie;
            this.carteira_trabalho_estado = estado_ct == null ? String.Empty : estado_ct.est_sig_estado;
            this.titulo_eleitor_nro = String.IsNullOrEmpty(colaborador.titulo_eleitor_nro) ? String.Empty : colaborador.titulo_eleitor_nro;
            this.titulo_eleitor_zona = String.IsNullOrEmpty(colaborador.titulo_eleitor_zona) ? String.Empty : colaborador.titulo_eleitor_zona;
            this.titulo_eleitor_secao = String.IsNullOrEmpty(colaborador.titulo_eleitor_secao) ? String.Empty : colaborador.titulo_eleitor_secao;
            this.pis_pasep_net = colaborador.pis_pasep_net;
            this.data_nascimento = colaborador.data_nascimento.ToString("ddMMyyyy");
            this.naturalidade_cidade = cidade_nat == null ? String.Empty : cidade_nat.cid_nom_cidade;
            this.naturalidade_estado = estado_nat == null ? String.Empty : estado_nat.est_sig_estado;
            this.nacionalidade = colaborador.nacionalidade;
            this.nome_mae = colaborador.nome_mae;
            this.nome_pai = String.IsNullOrEmpty(colaborador.nome_pai) ? String.Empty : colaborador.nome_pai;
            this.sexo = colaborador.sexo_masculino ? "Masculino" : "Feminino";
            this.estado_civil = ObterEstadoCivil(colaborador.estado_civil);
            this.grau_instrucao = grin == null ? String.Empty : grin.nome;
            this.raca = ra == null ? String.Empty : ra.nome;
            this.telefone_01 = BaseBusiness.FormatarFone(colaborador.telefone_01);
            this.telefone_02 = BaseBusiness.FormatarFone(colaborador.telefone_02, true);
            this.email = colaborador.email;
            this.banco = ba == null ? String.Empty : ba.nome;
            this.agencia = colaborador.agencia.ToString();
            this.agencia_digito = colaborador.agencia_digito;
            this.conta_corrente = colaborador.conta_corrente;
            this.endereco_logradouro = colaborador.endereco_logradouro;
            this.endereco_nro = colaborador.endereco_nro;
            this.endereco_complemento = String.IsNullOrEmpty(colaborador.endereco_complemento) ? String.Empty : colaborador.endereco_complemento;
            this.endereco_bairro = colaborador.endereco_bairro;
            this.endereco_cidade = cidade_end == null ? String.Empty : cidade_end.cid_nom_cidade;
            this.endereco_estado = estado_end == null ? String.Empty : estado_end.est_sig_estado;
            this.endereco_cep = colaborador.endereco_cep;

            RemoverPontoVirgula();
        }

        #region [ Propriedades ]

        [Order(0)]
        public string nome_concurso { get; set; }
        [Order(1)]
        public string data_concurso { get; set; }
        [Order(2)]
        public string local_concurso { get; set; }
        [Order(3)]
        public string funcao_colaborador { get; set; }
        [Order(4)]
        public string valor_bruto { get; set; }
        [Order(5)]
        public string valor_liquido { get; set; }
        [Order(6)]
        public string descontar_inss { get; set; }
        [Order(7)]
        public string descontar_iss { get; set; }
        [Order(8)]
        public string codigo { get; set; }
        [Order(9)]
        public string nome { get; set; }
        [Order(10)]
        [IsText(true)]
        public string cpf { get; set; }
        [Order(11)]
        [IsText(true)]
        public string rg { get; set; }
        [Order(12)]
        public string carteira_trabalho_nro { get; set; }
        [Order(13)]
        [IsText(true)]
        public string carteira_trabalho_serie { get; set; }
        [Order(14)]
        public string carteira_trabalho_estado { get; set; }
        [Order(15)]
        [IsText(true)]
        public string titulo_eleitor_nro { get; set; }
        [Order(16)]
        public string titulo_eleitor_zona { get; set; }
        [Order(17)]
        public string titulo_eleitor_secao { get; set; }
        [Order(18)]
        public string pis_pasep_net { get; set; }
        [Order(19)]
        [IsText(true)]
        public string data_nascimento { get; set; }
        [Order(20)]
        public string naturalidade_cidade { get; set; }
        [Order(21)]
        public string naturalidade_estado { get; set; }
        [Order(22)]
        public string nacionalidade { get; set; }
        [Order(23)]
        public string nome_mae { get; set; }
        [Order(24)]
        public string nome_pai { get; set; }
        [Order(25)]
        public string sexo { get; set; }
        [Order(26)]
        public string estado_civil { get; set; }
        [Order(27)]
        public string grau_instrucao { get; set; }
        [Order(28)]
        public string raca { get; set; }
        [Order(29)]
        public string telefone_01 { get; set; }
        [Order(30)]
        public string telefone_02 { get; set; }
        [Order(31)]
        public string email { get; set; }
        [Order(32)]
        public string banco { get; set; }
        [Order(33)]
        [IsText(true)]
        public string agencia { get; set; }
        [Order(34)]
        [IsText(true)]
        public string agencia_digito { get; set; }
        [Order(35)]
        [IsText(true)]
        public string conta_corrente { get; set; }
        [Order(36)]
        public string endereco_logradouro { get; set; }
        [Order(37)]
        [IsText(true)]
        public string endereco_nro { get; set; }
        [Order(38)]
        public string endereco_complemento { get; set; }
        [Order(39)]
        public string endereco_bairro { get; set; }
        [Order(40)]
        public string endereco_cidade { get; set; }
        [Order(41)]
        public string endereco_estado { get; set; }
        [Order(42)]
        public string endereco_cep { get; set; }

        #endregion [ FIM - Propriedades ]

        internal string ObterEstadoCivil(string sigla)
        {
            switch (sigla?.ToUpper())
            {
                case "C":
                    return "Casado";
                case "D":
                    return "Divorciado";
                case "S":
                    return "Solteiro";
                case "V":
                    return "Viúvo";
                default:
                    return "Outro";
            }
        }

        internal void RemoverPontoVirgula()
        {
            this.banco = this.banco.Replace(";", "");
            this.rg = this.rg.Replace(";", "");
            this.carteira_trabalho_nro = this.carteira_trabalho_nro.Replace(";", "");
            this.carteira_trabalho_serie = this.carteira_trabalho_serie.Replace(";", "");
            this.titulo_eleitor_nro = this.titulo_eleitor_nro.Replace(";", "");
            this.titulo_eleitor_zona = this.titulo_eleitor_zona.Replace(";", "");
            this.titulo_eleitor_secao = this.titulo_eleitor_secao.Replace(";", "");
            this.pis_pasep_net = this.pis_pasep_net.Replace(";", "");
            this.nacionalidade = this.nacionalidade.Replace(";", "");
            this.nome_mae = this.nome_mae.Replace(";", "");
            this.nome_pai = this.nome_pai.Replace(";", "");
            this.nome = this.nome.Replace(";", "");
            this.email = this.email.Replace(";", "");
            this.agencia = this.agencia.Replace(";", "");
            this.agencia_digito = this.agencia_digito.Replace(";", "");
            this.conta_corrente = this.conta_corrente.Replace(";", "");
            this.endereco_logradouro = this.endereco_logradouro.Replace(";", "");
            this.endereco_nro = this.endereco_nro.Replace(";", "");
            this.endereco_complemento = this.endereco_complemento.Replace(";", "");
            this.endereco_bairro = this.endereco_bairro.Replace(";", "");
        }
    }

    public class ColaboradorCSVModel2
    {
        public ColaboradorCSVModel2(ColaboradorModel colaborador, List<tb_est_estado> Estados, List<tb_cid_cidade> Cidades, List<GrauInstrucaoModel> grausInstrucao, List<RacaModel> racas, List<BancoModel> bancos)
        {
            if (colaborador == null)
                return;

            var estado_ct = Estados.FirstOrDefault(e => e.est_idt_estado == colaborador.carteira_trabalho_estado_id);
            var cidade_nat = Cidades.FirstOrDefault(c => c.cid_idt_cidade == colaborador.naturalidade_cidade_id);
            var estado_nat = cidade_nat == null ? null : Estados.FirstOrDefault(e => e.est_idt_estado == cidade_nat.est_idt_estado);
            var grin = grausInstrucao.FirstOrDefault(gi => gi.id == colaborador.grau_instrucao_id);
            var ra = racas.FirstOrDefault(r => r.id == colaborador.raca_id);
            var ba = bancos.FirstOrDefault(b => b.id == colaborador.banco_id);
            var cidade_end = Cidades.FirstOrDefault(c => c.cid_idt_cidade == colaborador.endereco_cidade_id);
            var estado_end = cidade_end == null ? null : Estados.FirstOrDefault(e => e.est_idt_estado == cidade_end.est_idt_estado);

            this.codigo = colaborador.codigo;
            this.nome = colaborador.nome;
            this.cpf = BaseBusiness.FormatarCPF(colaborador.cpf);
            this.rg = colaborador.rg;
            this.carteira_trabalho_nro = String.IsNullOrEmpty(colaborador.carteira_trabalho_nro) ? String.Empty : colaborador.carteira_trabalho_nro;
            this.carteira_trabalho_serie = String.IsNullOrEmpty(colaborador.carteira_trabalho_serie) ? String.Empty : colaborador.carteira_trabalho_serie;
            this.carteira_trabalho_estado = estado_ct == null ? String.Empty : estado_ct.est_sig_estado;
            this.titulo_eleitor_nro = String.IsNullOrEmpty(colaborador.titulo_eleitor_nro) ? String.Empty : colaborador.titulo_eleitor_nro;
            this.titulo_eleitor_zona = String.IsNullOrEmpty(colaborador.titulo_eleitor_zona) ? String.Empty : colaborador.titulo_eleitor_zona;
            this.titulo_eleitor_secao = String.IsNullOrEmpty(colaborador.titulo_eleitor_secao) ? String.Empty : colaborador.titulo_eleitor_secao;
            this.pis_pasep_net = colaborador.pis_pasep_net;
            this.data_nascimento = colaborador.data_nascimento.ToString("ddMMyyyy");
            this.naturalidade_cidade = cidade_nat == null ? String.Empty : cidade_nat.cid_nom_cidade;
            this.naturalidade_estado = estado_nat == null ? String.Empty : estado_nat.est_sig_estado;
            this.nacionalidade = colaborador.nacionalidade;
            this.nome_mae = colaborador.nome_mae;
            this.nome_pai = String.IsNullOrEmpty(colaborador.nome_pai) ? String.Empty : colaborador.nome_pai;
            this.sexo = colaborador.sexo_masculino ? "Masculino" : "Feminino";
            this.estado_civil = ObterEstadoCivil(colaborador.estado_civil);
            this.grau_instrucao = grin == null ? String.Empty : grin.nome;
            this.raca = ra == null ? String.Empty : ra.nome;
            this.telefone_01 = BaseBusiness.FormatarFone(colaborador.telefone_01);
            this.telefone_02 = BaseBusiness.FormatarFone(colaborador.telefone_02, true);
            this.email = colaborador.email;
            this.banco = ba == null ? String.Empty : ba.nome;
            this.agencia = colaborador.agencia.ToString();
            this.agencia_digito = colaborador.agencia_digito;
            this.conta_corrente = colaborador.conta_corrente;
            this.endereco_logradouro = colaborador.endereco_logradouro;
            this.endereco_nro = colaborador.endereco_nro;
            this.endereco_complemento = String.IsNullOrEmpty(colaborador.endereco_complemento) ? String.Empty : colaborador.endereco_complemento;
            this.endereco_bairro = colaborador.endereco_bairro;
            this.endereco_cidade = cidade_end == null ? String.Empty : cidade_end.cid_nom_cidade;
            this.endereco_estado = estado_end == null ? String.Empty : estado_end.est_sig_estado;
            this.endereco_cep = colaborador.endereco_cep;
            this.dados_ok = colaborador.dados_ok ? "Sim" : "Não";
            this.ativo = colaborador.ativo ? "Sim" : "Não";

            RemoverPontoVirgula();
        }

        #region [ Propriedades ]

        [Order(0)]
        public string codigo { get; set; }
        [Order(1)]
        [IsText(true)]
        public string nome { get; set; }
        [Order(2)]
        [IsText(true)]
        public string cpf { get; set; }
        [Order(3)]
        [IsText(true)]
        public string rg { get; set; }
        [Order(4)]
        public string carteira_trabalho_nro { get; set; }
        [Order(5)]
        [IsText(true)]
        public string carteira_trabalho_serie { get; set; }
        [Order(6)]
        public string carteira_trabalho_estado { get; set; }
        [Order(7)]
        [IsText(true)]
        public string titulo_eleitor_nro { get; set; }
        [Order(8)]
        public string titulo_eleitor_zona { get; set; }
        [Order(9)]
        public string titulo_eleitor_secao { get; set; }
        [Order(10)]
        public string pis_pasep_net { get; set; }
        [Order(11)]
        [IsText(true)]
        public string data_nascimento { get; set; }
        [Order(12)]
        public string naturalidade_cidade { get; set; }
        [Order(13)]
        public string naturalidade_estado { get; set; }
        [Order(14)]
        public string nacionalidade { get; set; }
        [Order(15)]
        public string nome_mae { get; set; }
        [Order(16)]
        public string nome_pai { get; set; }
        [Order(17)]
        public string sexo { get; set; }
        [Order(18)]
        public string estado_civil { get; set; }
        [Order(19)]
        public string grau_instrucao { get; set; }
        [Order(20)]
        public string raca { get; set; }
        [Order(21)]
        public string telefone_01 { get; set; }
        [Order(22)]
        public string telefone_02 { get; set; }
        [Order(23)]
        public string email { get; set; }
        [Order(24)]
        public string banco { get; set; }
        [Order(25)]
        [IsText(true)]
        public string agencia { get; set; }
        [Order(26)]
        [IsText(true)]
        public string agencia_digito { get; set; }
        [Order(27)]
        [IsText(true)]
        public string conta_corrente { get; set; }
        [Order(28)]
        public string endereco_logradouro { get; set; }
        [Order(29)]
        [IsText(true)]
        public string endereco_nro { get; set; }
        [Order(30)]
        public string endereco_complemento { get; set; }
        [Order(31)]
        public string endereco_bairro { get; set; }
        [Order(32)]
        public string endereco_cidade { get; set; }
        [Order(33)]
        public string endereco_estado { get; set; }
        [Order(34)]
        public string endereco_cep { get; set; }
        [Order(35)]
        public string dados_ok { get; set; }
        [Order(36)]
        public string ativo { get; set; }

        #endregion [ FIM - Propriedades ]

        internal string ObterEstadoCivil(string sigla)
        {
            switch (sigla?.ToUpper())
            {
                case "C":
                    return "Casado";
                case "D":
                    return "Divorciado";
                case "S":
                    return "Solteiro";
                case "V":
                    return "Viúvo";
                default:
                    return "Outro";
            }
        }

        internal void RemoverPontoVirgula()
        {
            this.banco = this.banco.Replace(";", "");
            this.rg = this.rg.Replace(";", "");
            this.carteira_trabalho_nro = this.carteira_trabalho_nro.Replace(";", "");
            this.carteira_trabalho_serie = this.carteira_trabalho_serie.Replace(";", "");
            this.titulo_eleitor_nro = this.titulo_eleitor_nro.Replace(";", "");
            this.titulo_eleitor_zona = this.titulo_eleitor_zona.Replace(";", "");
            this.titulo_eleitor_secao = this.titulo_eleitor_secao.Replace(";", "");
            this.pis_pasep_net = this.pis_pasep_net.Replace(";", "");
            this.nacionalidade = this.nacionalidade.Replace(";", "");
            this.nome_mae = this.nome_mae.Replace(";", "");
            this.nome_pai = this.nome_pai.Replace(";", "");
            this.nome = this.nome.Replace(";", "");
            this.email = this.email.Replace(";", "");
            this.agencia = this.agencia.Replace(";", "");
            this.agencia_digito = this.agencia_digito.Replace(";", "");
            this.conta_corrente = this.conta_corrente.Replace(";", "");
            this.endereco_logradouro = this.endereco_logradouro.Replace(";", "");
            this.endereco_nro = this.endereco_nro.Replace(";", "");
            this.endereco_complemento = this.endereco_complemento.Replace(";", "");
            this.endereco_bairro = this.endereco_bairro.Replace(";", "");
        }
    }
}