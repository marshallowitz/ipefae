using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGV.IPEFAE.Web.App.Models
{
    public class AnexoModel
    {
        public AnexoModel(string path)
        {
            this.PathArquivo = path;
        }

        public string PathArquivo { get; set; }
        public string NomeArquivo { get { return System.IO.Path.GetFileName(this.PathArquivo).Length > 10 ? String.Format("{0}....{1}", System.IO.Path.GetFileName(this.PathArquivo).Substring(0, 7), System.IO.Path.GetExtension(this.PathArquivo)) : System.IO.Path.GetFileName(this.PathArquivo); } }
    }
}