using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TGV.IPEFAE.Web.App.Models
{
    public class PermissaoModel
    {
        public PermissaoModel(Tipo tipo)
        {
            this.TipoPermissao = tipo;
        }

        public enum Tipo
        {
            Indefinido = 0,
            [Description("Administrador")]
            Administrador = 1,
            [Description("Concursos")]
            Concurso = 2,
            [Description("Estágios")]
            Estagio = 4,
            [Description("Vestibulares")]
            Vestibular = 8,
            [Description("Estagiário")]
            Estagiario = 16,
            [Description("Colaborador")]
            Colaborador = 32,
            [Description("Público")]
            Publico = 100
        }

        public Tipo TipoPermissao { get; set; }
    }
}