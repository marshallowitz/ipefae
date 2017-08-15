using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGV.IPEFAE.Web.App.Models.Bradesco.Response
{
    public class ServiceResponse
    {
        public string merchant_id { get; set; }
        public string meio_pagamento { get; set; }
        public Pedido pedido { get; set; } = new Pedido();
        public BoletoResponse boleto { get; set; } = new BoletoResponse();
        public StatusResponse status { get; set; } = new StatusResponse();
    }
}