using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Models
{
    public class EstadoModel
    {
        public EstadoModel() { }

        public EstadoModel(tb_est_estado estado) : this()
        {
            this.Id = estado.est_idt_estado;
            this.Sigla = estado.est_sig_estado;
            this.Nome = estado.est_nom_estado;
        }

        public int Id       { get; set; }
        public string Sigla { get; set; }
        public string Nome  { get; set; }
    }
}