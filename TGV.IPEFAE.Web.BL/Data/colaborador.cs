//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TGV.IPEFAE.Web.BL.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class colaborador
    {
        public colaborador()
        {
            this.concurso_local_colaborador = new HashSet<concurso_local_colaborador>();
        }
    
        public int id { get; set; }
        public int banco_id { get; set; }
        public Nullable<int> carteira_trabalho_estado_id { get; set; }
        public int endereco_cidade_id { get; set; }
        public int grau_instrucao_id { get; set; }
        public int naturalidade_cidade_id { get; set; }
        public int raca_id { get; set; }
        public string nome { get; set; }
        public string cpf { get; set; }
        public string rg { get; set; }
        public string carteira_trabalho_nro { get; set; }
        public string carteira_trabalho_serie { get; set; }
        public string titulo_eleitor_nro { get; set; }
        public string titulo_eleitor_zona { get; set; }
        public string titulo_eleitor_secao { get; set; }
        public string pis_pasep_net { get; set; }
        public System.DateTime data_nascimento { get; set; }
        public string nacionalidade { get; set; }
        public string nome_mae { get; set; }
        public string nome_pai { get; set; }
        public bool sexo_masculino { get; set; }
        public string estado_civil { get; set; }
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
        public bool dados_ok { get; set; }
        public bool ativo { get; set; }
    
        public virtual banco banco { get; set; }
        public virtual grau_instrucao grau_instrucao { get; set; }
        public virtual raca raca { get; set; }
        public virtual tb_cid_cidade tb_cid_cidade { get; set; }
        public virtual tb_cid_cidade tb_cid_cidade1 { get; set; }
        public virtual tb_est_estado tb_est_estado { get; set; }
        public virtual ICollection<concurso_local_colaborador> concurso_local_colaborador { get; set; }
    }
}
