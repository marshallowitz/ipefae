using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGV.IPEFAE.Web.App.Models.Bradesco.Request
{
    public class ServiceRequest
    {
        public string merchant_id                           { get; set; }
        public string meio_pagamento                        { get; set; } = "300";
        public Pedido pedido                                { get; set; } = new Pedido();
        public Comprador comprador                          { get; set; } = new Comprador();
        public BoletoRequest boleto                         { get; set; }
        public string token_request_confirmacao_pagamento   { get; set; }
    }
}