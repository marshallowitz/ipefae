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
    
    public partial class concurso_local_colaborador
    {
        public int id { get; set; }
        public int concurso_local_id { get; set; }
        public int colaborador_id { get; set; }
        public int funcao_id { get; set; }
        public decimal valor { get; set; }
        public bool tem_empresa { get; set; }
        public bool ativo { get; set; }
    
        public virtual colaborador colaborador { get; set; }
        public virtual concurso_funcao concurso_funcao { get; set; }
        public virtual concurso_local concurso_local { get; set; }
    }
}
