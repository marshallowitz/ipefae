﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGV.IPEFAE.Web.BL.Data
{
    internal class CidadeData
    {
        internal static List<tb_cid_cidade> Listar(int est_idt_estado)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from cid in db.tb_cid_cidade
                        where cid.cid_bit_ativo
                        && cid.est_idt_estado == est_idt_estado
                        orderby cid.cid_nom_cidade
                        select cid).ToList();
            }
        }

        internal static List<tb_cid_cidade> ListarTodas()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from cid in db.tb_cid_cidade
                        where cid.cid_bit_ativo
                        orderby cid.cid_nom_cidade
                        select cid).ToList();
            }
        }

        internal static List<tb_cid_cidade> ListarCidadesComEstagiario()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from cid in db.tb_cid_cidade
                        join ues in db.tb_ues_usuario_estagio on cid.cid_idt_cidade equals ues.cid_idt_cidade_endereco
                        where ues.ues_bit_ativo
                        orderby cid.cid_nom_cidade
                        select cid).Distinct().ToList();
            }
        }
    }

    public class CidadeModel
    {
        public CidadeModel() { }
        
        public CidadeModel(tb_cid_cidade cidade)
        {
            if (cidade == null)
                return;

            this.Id = cidade.cid_idt_cidade;
            this.IdEstado = cidade.est_idt_estado;
            this.Estado = new EstadoModel(cidade.tb_est_estado);
            this.Nome = cidade.cid_nom_cidade;
            this.Ativo = cidade.cid_bit_ativo;
        }

        public int Id               { get; set; }
        public int IdEstado         { get; set; }
        public string Nome          { get; set; }
        public bool Ativo           { get; set; }

        public EstadoModel Estado   { get; set; } = new EstadoModel();
    }
}
