﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class IPEFAEEntities : DbContext
    {
        public IPEFAEEntities()
            : base("name=IPEFAEEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tb_usu_usuario> tb_usu_usuario { get; set; }
        public virtual DbSet<tb_per_permissao> tb_per_permissao { get; set; }
        public virtual DbSet<tb_est_estado> tb_est_estado { get; set; }
        public virtual DbSet<tb_cci_concurso_cargo_inscrito> tb_cci_concurso_cargo_inscrito { get; set; }
        public virtual DbSet<tb_cid_cidade> tb_cid_cidade { get; set; }
        public virtual DbSet<tb_sre_status_recurso> tb_sre_status_recurso { get; set; }
        public virtual DbSet<tb_rec_recurso> tb_rec_recurso { get; set; }
        public virtual DbSet<tb_tlc_tipo_layout_concurso> tb_tlc_tipo_layout_concurso { get; set; }
        public virtual DbSet<tb_cco_cargo_concurso> tb_cco_cargo_concurso { get; set; }
        public virtual DbSet<tb_can_concurso_anexo> tb_can_concurso_anexo { get; set; }
        public virtual DbSet<tb_ued_usuario_estagio_dados_escolares> tb_ued_usuario_estagio_dados_escolares { get; set; }
        public virtual DbSet<tb_uee_usuario_estagio_experiencia> tb_uee_usuario_estagio_experiencia { get; set; }
        public virtual DbSet<tb_ueo_usuario_estagio_outros> tb_ueo_usuario_estagio_outros { get; set; }
        public virtual DbSet<tb_ues_usuario_estagio> tb_ues_usuario_estagio { get; set; }
        public virtual DbSet<tb_ler_log_erro> tb_ler_log_erro { get; set; }
        public virtual DbSet<tb_icv_inscrito_concurso_vestibular> tb_icv_inscrito_concurso_vestibular { get; set; }
        public virtual DbSet<tb_idt_inscrito_dados_prova> tb_idt_inscrito_dados_prova { get; set; }
        public virtual DbSet<tb_icl_inscrito_classificacao> tb_icl_inscrito_classificacao { get; set; }
        public virtual DbSet<tb_con_concurso> tb_con_concurso { get; set; }
        public virtual DbSet<tb_emp_empresa> tb_emp_empresa { get; set; }
        public virtual DbSet<tb_ema_email> tb_ema_email { get; set; }
        public virtual DbSet<tb_ico_inscrito_concurso> tb_ico_inscrito_concurso { get; set; }
        public virtual DbSet<relatorio_pdf> relatorio_pdf { get; set; }
        public virtual DbSet<banco> banco { get; set; }
        public virtual DbSet<grau_instrucao> grau_instrucao { get; set; }
        public virtual DbSet<raca> raca { get; set; }
        public virtual DbSet<concurso_local> concurso_local { get; set; }
        public virtual DbSet<concurso> concurso { get; set; }
        public virtual DbSet<colaborador> colaborador { get; set; }
        public virtual DbSet<concurso_funcao> concurso_funcao { get; set; }
        public virtual DbSet<concurso_local_colaborador> concurso_local_colaborador { get; set; }
    
        public virtual ObjectResult<spr_tgv_gerar_lista_inscritos_Result> spr_tgv_gerar_lista_inscritos(Nullable<int> con_idt_concurso)
        {
            var con_idt_concursoParameter = con_idt_concurso.HasValue ?
                new ObjectParameter("con_idt_concurso", con_idt_concurso) :
                new ObjectParameter("con_idt_concurso", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spr_tgv_gerar_lista_inscritos_Result>("spr_tgv_gerar_lista_inscritos", con_idt_concursoParameter);
        }
    }
}
