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
    
    public partial class concurso_funcao
    {
        public int concurso_id { get; set; }
        public int funcao_id { get; set; }
        public decimal valor_liquido { get; set; }
        public bool ativo { get; set; }
    
        public virtual concurso concurso { get; set; }
        public virtual funcao funcao { get; set; }
    }
}