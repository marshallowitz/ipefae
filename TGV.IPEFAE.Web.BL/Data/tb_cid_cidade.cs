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
    
    public partial class tb_cid_cidade
    {
        public tb_cid_cidade()
        {
            this.tb_ues_usuario_estagio = new HashSet<tb_ues_usuario_estagio>();
            this.tb_ico_inscrito_concurso = new HashSet<tb_ico_inscrito_concurso>();
        }
    
        public int cid_idt_cidade { get; set; }
        public int est_idt_estado { get; set; }
        public string cid_nom_cidade { get; set; }
        public bool cid_bit_capital { get; set; }
        public bool cid_bit_ativo { get; set; }
    
        public virtual tb_est_estado tb_est_estado { get; set; }
        public virtual ICollection<tb_ues_usuario_estagio> tb_ues_usuario_estagio { get; set; }
        public virtual ICollection<tb_ico_inscrito_concurso> tb_ico_inscrito_concurso { get; set; }
    }
}
