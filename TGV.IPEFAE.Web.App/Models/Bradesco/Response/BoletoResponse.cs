﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGV.IPEFAE.Web.App.Models.Bradesco.Response
{
    public class BoletoResponse
    {
        public int valor_titulo { get; set; }
        public string data_geracao { get; set; }
        public string data_atualizacao { get; set; }
        public string linha_digitavel { get; set; }
        public string linha_digitavel_formatada { get; set; }
        public string token { get; set; }
        public string url_acesso { get; set; }
    }
}