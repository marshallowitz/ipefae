using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGV.IPEFAE.Web.App.Models.Bradesco
{
    public class Comprador
    {
        public string nome { get; set; }
        public string documento { get; set; }
        public CompradorEndereco endereco { get; set; } = new CompradorEndereco();
        public string ip { get; set; }
        public string user_agent { get; set; }
    }
}