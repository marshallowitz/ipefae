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
    
    public partial class tb_icl_inscrito_classificacao
    {
        public int ico_idt_inscrito_concurso { get; set; }
        public Nullable<int> icl_num_posicao { get; set; }
        public Nullable<decimal> icl_num_nota { get; set; }
        public Nullable<decimal> icl_num_cg { get; set; }
        public Nullable<decimal> icl_num_ce { get; set; }
        public Nullable<decimal> icl_num_tit { get; set; }
        public Nullable<decimal> icl_num_pp { get; set; }
        public string icl_des_situacao { get; set; }
        public bool icl_bit_ativo { get; set; }
    
        public virtual tb_ico_inscrito_concurso tb_ico_inscrito_concurso { get; set; }
    }
}
