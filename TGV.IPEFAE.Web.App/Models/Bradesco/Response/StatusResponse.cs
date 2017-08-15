using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGV.IPEFAE.Web.App.Models.Bradesco.Response
{
    public class StatusResponse
    {
        public int codigo { get; set; }
        public string mensagem { get; set; }
        public string detalhes { get; set; }
    }
}