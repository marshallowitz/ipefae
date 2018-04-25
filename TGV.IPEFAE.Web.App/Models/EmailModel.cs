using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TGV.IPEFAE.Web.App.Models
{
    public class EmailModel
    {
        public string Nome      { get; set; }
        public string Matricula { get; set; }
        public string Cargo     { get; set; }
        public string Senha     { get; set; }
        public string Email     { get; set; }
        public string Telefone  { get; set; }
        public string Assunto   { get; set; }
        public string Mensagem  { get; set; }
        public bool IsNovo      { get; set; } = false;

        public string UrlSite { get { return ConfigurationManager.AppSettings["URLAmbiente"]; } }

        public string UrlLogo
        {
            get
            {
                if (String.IsNullOrEmpty(this.UrlSite))
                    return String.Empty;

                return String.Format("{0}Content/imagens/logo.png", this.UrlSite);
            }
        }
    }
}