using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace TGV.IPEFAE.Web.App.Models.BancoBrasil
{
    public class PagamentoModel
    {
        //public PagamentoModel(oldConcursoModel.InscritoModel inscrito, string nomeConcurso, string cargo, string tipo, int idConv, string refTran, double valor, string dtVenc)
        //{
        //    this.msgLoja = String.Format("Pagamento de Taxa de Inscrição<br><br>CONCURSO: {0}<br>{2}: {1}<br><br>Não receber após o vencimento", nomeConcurso, cargo, tipo);
        //    this.idConv = idConv;
        //    this.refTran = refTran;
        //    this.valor = Convert.ToInt32(valor * 100);
        //    this.dtVenc = dtVenc;
        //    this.cpfCnpj = inscrito.CPF;
        //    this.nome = inscrito.Nome;
        //    this.endereco = inscrito.Endereco;
        //    this.cidade = inscrito.Cidade.Nome;
        //    this.uf = inscrito.SiglaEstado;
        //    this.cep = inscrito.CEP.Replace("-", "");
        //}

        public int idConv                   { get; set; }
        public string refTran               { get; set; }
        public int valor                    { get; set; }
        public string dtVenc                { get; set; }
        public string cpfCnpj               { get; set; }
        public string nome                  { get; set; }
        public string endereco              { get; set; }
        public string cidade                { get; set; }
        public string uf                    { get; set; }
        public string cep                   { get; set; }
        public string msgLoja               { get; private set; }
        public int indicadorPessoa          { get; private set; } = 1;
        public int valorDesconto            { get; private set; } = 0;
        public string dataLimiteDesconto    { get; private set; } = String.Empty;
        public string tpDuplicata           { get; private set; } = "DS";
        public int qtdPontos                { get; private set; } = 0;
        public int tpPagamento              { get; private set; } = 21;
        public string urlRetorno            { get; private set; } = "/status";
        public string urlInforma            { get; private set; }

        public string GetQueryString()
        {
            var properties = from p in this.GetType().GetProperties()
                             where p.GetValue(this, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(this, null).ToString());

            return String.Join("&", properties.ToArray());
        }
    }
}