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
    
    public partial class concurso_local
    {
        public concurso_local()
        {
            this.concurso_local_colaborador = new HashSet<concurso_local_colaborador>();
        }
    
        public int id { get; set; }
        public int concurso_id { get; set; }
        public string local { get; set; }
        public bool ativo { get; set; }
    
        public virtual ICollection<concurso_local_colaborador> concurso_local_colaborador { get; set; }
        public virtual concurso concurso { get; set; }
    }
}