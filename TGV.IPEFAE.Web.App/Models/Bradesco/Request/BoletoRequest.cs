using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGV.IPEFAE.Web.App.Models.Bradesco.Request
{
    public class BoletoRequest
    {
        public string beneficiario                  { get; set; }
        public string carteira                      { get; set; } = "26";
        public string nosso_numero                  { get; set; }
        public string data_emissao                  { get; set; }
        public string data_vencimento               { get; set; }
        public string valor_titulo                  { get; set; }
        public string url_logotipo                  { get; set; }
        public string mensagem_cabecalho            { get; set; }
        public string tipo_renderizacao             { get; set; } = "2";
        public BoletoInstrucoesRequest instrucoes   { get; set; } = new BoletoInstrucoesRequest();
        public BoletoRegistroRequest registro       { get; set; } = new BoletoRegistroRequest();
    }
}