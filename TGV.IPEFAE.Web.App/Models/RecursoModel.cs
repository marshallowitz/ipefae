using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Models
{
    public class RecursoModel
    {
        #region [ Enum ]

        public enum StatusRecurso
        {
            Indefinido = 0,
            Novo = 1,
            Aprovado = 2,
            Recusado = 3
        }

        #endregion [ FIM - Enum ]

        public RecursoModel()
        {
            this.Id = Int32.MinValue;
            this.DataAbertura = DateTime.MinValue;
            this.Inscrito = new ConcursoModel.InscritoModel();
            this.Status = StatusRecurso.Indefinido;
            this.Atendente = new UsuarioModel();
            this.Ativo = false;
        }

        public RecursoModel(tb_rec_recurso recurso) : this()
        {
            if (recurso == null)
                return;

            this.Id = recurso.rec_idt_recurso;
            this.IdConcurso = recurso.con_idt_concurso;
            this.DataAbertura = recurso.rec_dat_abertura;
            this.DataAtendimento = recurso.rec_dat_atendimento;
            this.Comentario = recurso.rec_des_comentario;
            this.Mensagem = recurso.rec_des_mensagem;
            this.Ativo = recurso.rec_bit_ativo;

            try { this.Inscrito = new ConcursoModel.InscritoModel(recurso.tb_ico_inscrito_concurso, true); } catch { };
            try { this.Status = (RecursoModel.StatusRecurso)recurso.sre_idt_status_recurso; } catch { };
            try { this.Atendente = new UsuarioModel(recurso.tb_usu_usuario); } catch { };
        }

        public int Id                               { get; set; }
        public string Protocolo                     { get { return String.Format("{0}{1}", DataAbertura.ToString("yyyyMM"), this.Id.ToString().PadLeft(5, '0')); } }
        public int IdConcurso                       { get; set; }
        public ConcursoModel Concurso               { get; set; }
        public DateTime DataAbertura                { get; set; }
        public ConcursoModel.InscritoModel Inscrito { get; set; }
        public StatusRecurso Status                 { get; set; }
        public UsuarioModel Atendente               { get; set; }
        public DateTime? DataAtendimento            { get; set; }
        public string Comentario                    { get; set; }
        public string Mensagem                      { get; set; }
        public bool Ativo                           { get; set; }

        public string DataAberturaString            { get { return DataAbertura.ToString("dd/MM/yyyy"); } }
        public string DataAtendimentoString         { get { return DataAtendimento.HasValue ? DataAtendimento.Value.ToString("dd/MM/yyyy") : String.Empty; } }
        public string PathAnexosAtendente           { get { return this.Id > 0 ? String.Format("~/Anexos/Recurso/{0}/Atendente", this.Id) : String.Empty; } }
        public string PathAnexosRequerente          { get { return this.Id > 0 ? String.Format("~/Anexos/Recurso/{0}/Requerente", this.Id) : String.Empty; } }
        
        public string StatusString
        {
            get
            {
                switch (this.Status)
                {
                    case StatusRecurso.Novo:
                        return Resources.Admin.Recurso.Index.StatusNovo;
                    case StatusRecurso.Aprovado:
                        return Resources.Admin.Recurso.Index.StatusAprovado;
                    case StatusRecurso.Recusado:
                        return Resources.Admin.Recurso.Index.StatusReprovado;
                    default:
                        return String.Empty;
                }
            }
        }
    }

    public static class RecursoModelExtension
    {
        public static tb_rec_recurso ToRec(this RecursoModel recurso)
        {
            tb_rec_recurso rec = new tb_rec_recurso()
            {
                con_idt_concurso = recurso.IdConcurso,
                ico_idt_inscrito_concurso = recurso.Inscrito.Id,
                rec_bit_ativo = recurso.Ativo,
                rec_dat_abertura = recurso.DataAbertura,
                rec_dat_atendimento = recurso.DataAtendimento,
                rec_des_comentario = recurso.Comentario,
                rec_des_mensagem = recurso.Mensagem,
                rec_idt_recurso = recurso.Id,
                sre_idt_status_recurso = (int)recurso.Status,
                usu_idt_usuario = recurso.Atendente.Id
            };

            return rec;
        }
    }
}