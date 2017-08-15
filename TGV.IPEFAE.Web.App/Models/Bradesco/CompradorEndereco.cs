using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGV.IPEFAE.Web.App.Models.Bradesco
{
    public class CompradorEndereco
    {
        public string cep { get; set; }
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
    }
}