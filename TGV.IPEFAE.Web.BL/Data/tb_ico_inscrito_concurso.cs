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
    
    public partial class tb_ico_inscrito_concurso
    {
        public tb_ico_inscrito_concurso()
        {
            this.tb_cci_concurso_cargo_inscrito = new HashSet<tb_cci_concurso_cargo_inscrito>();
            this.tb_rec_recurso = new HashSet<tb_rec_recurso>();
        }
    
        public int ico_idt_inscrito_concurso { get; set; }
        public int cid_idt_cidade { get; set; }
        public int con_idt_concurso { get; set; }
        public int est_idt_estado_rg { get; set; }
        public string ico_nom_inscrito_concurso { get; set; }
        public System.DateTime ico_dat_nascimento { get; set; }
        public System.DateTime ico_dat_inscricao { get; set; }
        public Nullable<System.DateTime> ico_dat_pagamento { get; set; }
        public string ico_des_cpf { get; set; }
        public string ico_des_rg { get; set; }
        public string ico_des_endereco { get; set; }
        public string ico_des_nro_endereco { get; set; }
        public string ico_des_complemento { get; set; }
        public string ico_des_bairro { get; set; }
        public string ico_des_cep { get; set; }
        public string ico_des_telefone { get; set; }
        public string ico_des_celular { get; set; }
        public string ico_des_email { get; set; }
        public string ico_des_outras_solicitacoes { get; set; }
        public string ico_des_tratamento_especial_qual { get; set; }
        public int ico_num_filhos_menores { get; set; }
        public Nullable<decimal> ico_num_valor_pago { get; set; }
        public string ico_flg_deficiencia { get; set; }
        public string ico_flg_estado_civil { get; set; }
        public bool ico_bit_pago { get; set; }
        public bool ico_bit_destro { get; set; }
        public bool ico_bit_isento { get; set; }
        public bool ico_bit_possui_deficiencia { get; set; }
        public bool ico_bit_tratamento_especial { get; set; }
        public bool ico_bit_ativo { get; set; }
        public string ico_des_link_boleto { get; set; }
    
        public virtual ICollection<tb_cci_concurso_cargo_inscrito> tb_cci_concurso_cargo_inscrito { get; set; }
        public virtual tb_cid_cidade tb_cid_cidade { get; set; }
        public virtual tb_con_concurso tb_con_concurso { get; set; }
        public virtual tb_est_estado tb_est_estado { get; set; }
        public virtual tb_icl_inscrito_classificacao tb_icl_inscrito_classificacao { get; set; }
        public virtual tb_icv_inscrito_concurso_vestibular tb_icv_inscrito_concurso_vestibular { get; set; }
        public virtual tb_idt_inscrito_dados_prova tb_idt_inscrito_dados_prova { get; set; }
        public virtual ICollection<tb_rec_recurso> tb_rec_recurso { get; set; }
    }
}
