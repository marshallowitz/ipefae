﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Models
{
    public class CidadeModel
    {
        public CidadeModel()
        {
            this.Estado = new EstadoModel();
        }

        public CidadeModel(tb_cid_cidade cidade) : this()
        {
            this.Id = cidade.cid_idt_cidade;
            this.Nome = cidade.cid_nom_cidade;

            try { this.Estado = new EstadoModel(cidade.tb_est_estado); }
            catch { }
        }

        public static CidadeModel Clone(tb_cid_cidade cidade)
        {
            if (cidade == null)
                return null;

            CidadeModel cM = new CidadeModel();
            cM.Id = cidade.cid_idt_cidade;
            cM.Nome = cidade.cid_nom_cidade;

            //if (cidade.tb_est_estado != null)
            //    cM.Estado = new EstadoModel() { Id = cidade.tb_est_estado.est_idt_estado, Nome = cidade.tb_est_estado.est_nom_estado, Sigla = cidade.tb_est_estado.est_sig_estado };

            return cM;
        }

        public int Id       { get; set; }
        public string Nome  { get; set; }

        public string CidadeEstado { get { return !String.IsNullOrEmpty(this.Estado.Sigla) ? String.Format("{0} ({1})", this.Nome, this.Estado.Sigla) : this.Nome; } }

        public EstadoModel Estado { get; set; } = new EstadoModel();
    }
}