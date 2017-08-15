using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BoletoNet;
using TGV.IPEFAE.Web.BL.Business;
using System.Text;
using System.IO;
using TGV.IPEFAE.Web.App.Models.Bradesco.Request;
using System.Configuration;
using TGV.IPEFAE.Web.App.Models.Bradesco;
using System.Net;
using Newtonsoft.Json;
using TGV.IPEFAE.Web.App.Models.Bradesco.Response;
using TGV.IPEFAE.Web.App.Controllers;
using TGV.IPEFAE.Web.App.Models.BancoBrasil;
using System.Collections.Specialized;

namespace TGV.IPEFAE.Web.App.Models
{
    public class BoletoModel
    {
        //Bradesco:
        //Carteira 26
        //Ag. 0223-2 c/c 3397-9

        //Estabelecimento:INSTITUTO DE PESQUISA ECONOMICAS - IPEFAE
        //CNPJ:00.582.074/0001-83
        //Merchant ID:100005638

        // chave acesso => xvu13e-TmBKoVsjDjb796fZWJ9PdUYwWndE5-0r2ZGo

        public string GerarBoleto(int idConcurso, ConcursoModel.InscritoModel inscrito)
        {
            ConcursoModel concurso = new ConcursoModel(ConcursoBusiness.Obter(idConcurso, true));

            string htmlBoleto = this.GerarBoletoBanco(concurso, inscrito);
            return htmlBoleto;
        }

        public byte[] GerarRemessa(int idConcurso)
        {
            try
            {
                ConcursoModel concurso = new ConcursoModel(ConcursoBusiness.Obter(idConcurso, true));

                Int16 codigoBanco = concurso.Empresa.NumeroBanco;
                string tipo = concurso.IdTipoLayoutConcurso == 1 ? "CARGO" : "CURSO";

                int convenio = concurso.Empresa.Convenio;
                string cedenteCNPJ = concurso.Empresa.CNPJ;
                string cedenteNome = concurso.Empresa.RazaoSocial;
                string agencia = concurso.Empresa.Agencia;
                string conta = concurso.Empresa.ContaCorrente;

                Banco banco = new Banco(codigoBanco);
                CedenteModel cedente = new CedenteModel(convenio, cedenteCNPJ, cedenteNome, agencia, conta);
                Cedente c = cedente.GerarCedente();
                ArquivoRemessa arquivo = new ArquivoRemessa(TipoArquivo.CNAB400);
                
                Boletos boletos = ListarBoletos(concurso, c, banco);

                // Valida a Remessa Correspondentes antes de Gerar a mesma...
                string vMsgRetorno = string.Empty;
                bool vValouOK = arquivo.ValidarArquivoRemessa(convenio.ToString(), banco, c, boletos, 1, out vMsgRetorno);

                if (vValouOK)
                {
                    MemoryStream ms = new MemoryStream();
                    arquivo.GerarArquivoRemessa(convenio.ToString(), banco, c, boletos, ms, 1);

                    ms.Flush();
                    return ms.ToArray();
                }
                else
                    throw new Exception(String.Concat("Foram localizados inconsistências na validação da remessa!", vMsgRetorno));
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private string GerarBoletoBanco(ConcursoModel concurso, ConcursoModel.InscritoModel inscrito)
        {
            try
            {
                Int16 codigoBanco = concurso.Empresa.NumeroBanco;
                string carteira = this.ObterCarteira(codigoBanco);
                string tipo = concurso.IdTipoLayoutConcurso == 1 ? "CARGO" : "CURSO";
                string instrucao = String.Format("Pagamento de Taxa de Inscrição<br /><br />CONCURSO: {0}<br />{2}: {1}<br /><br />Não receber após o vencimento", concurso.Nome, inscrito.CargosString, tipo);

                string nossoNumero = inscrito.NossoNumero;
                string numeroDocumento = nossoNumero;
                DateTime vencimento = concurso.DataBoleto.AddDays(1).Date.AddSeconds(-1);
                double valorBoleto = inscrito.Cargos.Count > 0 && inscrito.Cargos[0].ValorInscricao.HasValue ? Convert.ToDouble(inscrito.Cargos[0].ValorInscricao.Value) : 0;

                if (codigoBanco == 36)
                    return this.GerarBoletoBradesco(concurso, inscrito, nossoNumero, valorBoleto, vencimento, tipo);
                else if (codigoBanco == 1)
                    return this.GerarBoletoBancoBrasil(concurso, inscrito, nossoNumero, valorBoleto, vencimento, tipo);

                int convenio = concurso.Empresa.Convenio;
                string cedenteCNPJ = concurso.Empresa.CNPJ;
                string cedenteNome = concurso.Empresa.RazaoSocial;
                string agencia = concurso.Empresa.Agencia;
                string conta = concurso.Empresa.ContaCorrente;

                string sacadoCPF = inscrito.CPF;
                string sacadoNome = inscrito.Nome;
                string endereco = String.Format("{0}, {1}", inscrito.Endereco, inscrito.NumeroEndereco);
                string bairro = inscrito.Bairro;
                string cidade = inscrito.Cidade.Nome;
                string cep = String.Format("{0:00000-000}", Convert.ToInt64(inscrito.CEP));
                string uf = inscrito.Cidade.Estado.Sigla;

                CedenteModel cedente = new CedenteModel(convenio, cedenteCNPJ, cedenteNome, agencia, conta);
                SacadoModel sacado = new SacadoModel(sacadoCPF, sacadoNome, endereco, bairro, cidade, cep, uf);

                // Cria o boleto, e passa os parâmetros usuais
                BoletoBancario bb;

                List<BoletoBancario> boletos = new List<BoletoBancario>();

                bb = new BoletoBancario();
                bb.CodigoBanco = codigoBanco;

                Cedente c = new Cedente(cedente.CPFCNPJ, cedente.Nome, cedente.Agencia, cedente.Conta);
                c.Convenio = cedente.Convenio;

                Boleto b = new Boleto(vencimento, (decimal)valorBoleto, carteira, nossoNumero, c);
                b.NumeroDocumento = numeroDocumento;
                b.Sacado = new Sacado(sacado.CPFCNPJ, sacado.Nome);
                b.Sacado.Endereco.End = sacado.Endereco.Logradouro;
                b.Sacado.Endereco.Bairro = sacado.Endereco.Bairro;
                b.Sacado.Endereco.Cidade = sacado.Endereco.Cidade;
                b.Sacado.Endereco.CEP = sacado.Endereco.CEP;
                b.Sacado.Endereco.UF = sacado.Endereco.UF;

                Instrucao instr = new Instrucao(codigoBanco);
                instr.Descricao = instrucao;
                b.Instrucoes.Add(instr);

                bb.Boleto = b;
                bb.Boleto.Valida();

                boletos.Add(bb);

                return GeraLayout(boletos);
            }
            catch (WebException webex)
            {
                WebResponse errResp = webex.Response;

                using (Stream respStream = errResp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(respStream);
                    string text = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                
            }

            return String.Empty;
        }

        private string GerarBoletoBancoBrasil(ConcursoModel concurso, ConcursoModel.InscritoModel inscrito, string nossoNumero, double valor, DateTime vencimento, string tipo)
        {
            int convenio_comercio_eletronico = concurso.Empresa.Convenio;
            string convenio_cobranca = concurso.Empresa.ConvenioCobranca.HasValue ? concurso.Empresa.ConvenioCobranca.Value.ToString().PadLeft(7, '0') : "0000000";
            string dataVencimento = DateTime.Now > vencimento ? DateTime.Now.AddDays(10).ToString("ddMMyyyy") : vencimento.ToString("ddMMyyyy");
            PagamentoModel pm = new PagamentoModel(inscrito, concurso.Nome, inscrito.CargosString, tipo, convenio_comercio_eletronico, $"{convenio_cobranca}{nossoNumero}", valor, dataVencimento);
            
            string url = "https://mpag.bb.com.br/site/mpag";

            string parameters = pm.GetQueryString();
            byte[] postData = Encoding.UTF8.GetBytes(parameters);

            //string html = BaseController.ObterHtmlFromURL(url, postData, null);
            BaseController bc = new BaseController();
            url = $"{url}?{parameters}";

            string html = bc.ObterHtmlFromURLBB(url, parameters);

            return html;
        }

        private string GerarBoletoBradesco(ConcursoModel concurso, ConcursoModel.InscritoModel inscrito, string nossoNumero, double valor, DateTime vencimento, string tipo)
        {
            // Verifica se o boleto já foi gerado
            string filePath = $"/Boletos/{concurso.Id}/{inscrito.Id}";
            string physicalDirectory = $"{HttpContext.Current.Server.MapPath("~/Boletos/")}{concurso.Id}";
            string physicalFilePah = $"{physicalDirectory}/{inscrito.Id}";
            string virtualPath = $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Authority}{HttpContext.Current.Request.ApplicationPath.TrimEnd('/')}{filePath}";

            if (File.Exists($"{physicalFilePah}.pdf"))
                return $"<html><head>IPEFAE</head><body><embed src='{virtualPath}.pdf' width='800px' height='2100px' /></body></html>";
            else if (File.Exists($"{physicalFilePah}.html"))
                return (new WebClient()).DownloadString($"{virtualPath}.html");

            var valorInteiro = Convert.ToInt32(valor * 100);
            string mensagem = "Pagamento de Taxa de Inscrição";

            string dataEmissao = DateTime.Now.ToString("yyyy-MM-dd");
            string dataVencimento = DateTime.Now > vencimento ? DateTime.Now.AddDays(10).ToString("yyyy-MM-dd") : vencimento.ToString("yyyy-MM-dd");

            string instrucao01 = "Pagamento de Taxa de Inscrição";
            string instrucao02 = $"CONCURSO: {concurso.Nome}";
            string instrucao03 = $"{tipo}: {inscrito.CargosString}";
            string instrucao04 = "Não receber após o vencimento";

            var serviceRequest = new ServiceRequest();
            serviceRequest.merchant_id = ConfigurationManager.AppSettings["BradescoMerchanId"];
            serviceRequest.pedido = new Pedido()
            {
                numero = nossoNumero,
                valor = valorInteiro,
                descricao = mensagem
            };
            serviceRequest.comprador = new Comprador()
            {
                nome = inscrito.Nome,
                documento = inscrito.CPF,
                endereco = new CompradorEndereco()
                {
                    cep = inscrito.CEP,
                    logradouro = inscrito.Endereco,
                    numero = inscrito.NumeroEndereco,
                    complemento = inscrito.Complemento,
                    bairro = inscrito.Bairro,
                    cidade = inscrito.Cidade.Nome,
                    uf = inscrito.Cidade.Estado.Sigla
                },
            };
            serviceRequest.boleto = new BoletoRequest()
            {
                beneficiario = "IPEFAE",
                carteira = "26",
                nosso_numero = nossoNumero,
                data_emissao = dataEmissao,
                data_vencimento = dataVencimento,
                valor_titulo = valorInteiro.ToString(),
                url_logotipo = "http://www.ipefae.org.br/Content/imagens/logo.png",
                mensagem_cabecalho = mensagem,
                tipo_renderizacao = "0",
                instrucoes = new BoletoInstrucoesRequest()
                {
                    instrucao_linha_1 = instrucao01,
                    instrucao_linha_2 = instrucao02,
                    instrucao_linha_3 = instrucao03,
                    instrucao_linha_4 = instrucao04
                }
                //,registro = new BoletoRegistroRequest()
                //{
                //    agencia_pagador = "00014",
                //    razao_conta_pagador = "07050",
                //    conta_pagador = "12345679",
                //    controle_participante = "Segurança arquivo remessa",
                //    tipo_ocorrencia = "02",
                //    especie_titulo = "01",
                //    primeira_instrucao = "00",
                //    segunda_instrucao = "00",
                //    tipo_inscricao_pagador = "01",
                //    sequencia_registro = "00001"
                //}
            };
            //serviceRequest.token_request_confirmacao_pagamento = "21323dsd23434ad12178DDasY";

            string chaveSeguranca = ConfigurationManager.AppSettings["BradescoChaveSeguranca"];

            var mediaType = "application/json";
            var charSet = "UTF-8";
            var urlPost = ConfigurationManager.AppSettings["BradescoBoletoUrl"];
            var request = (HttpWebRequest)WebRequest.Create(urlPost);

            //Conteudo da requisicao
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(serviceRequest));

            //Configuracao do cabecalho da requisicao
            request.Method = "POST";
            request.ContentType = mediaType + ";charset=" + charSet;
            request.ContentLength = data.Length;
            request.Accept = mediaType;
            request.Headers.Add(HttpRequestHeader.AcceptCharset, charSet);

            //Credenciais de Acesso
            String header = serviceRequest.merchant_id + ":" + chaveSeguranca;
            String headerBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(header));
            request.Headers.Add(HttpRequestHeader.Authorization, "Basic " + headerBase64);
            request.GetRequestStream().Write(data, 0, data.Length);
            var response = (HttpWebResponse)request.GetResponse();

            //Verifica resposta do servidor
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                throw new Exception("Retorno da requisicao dif 200/201. HttpStatusCode: " + response.StatusCode.ToString());

            //Obtem a resposta do servidor
            ServiceResponse serviceResponse = null;

            using (var jsonTextReader = new JsonTextReader(new StreamReader(response.GetResponseStream())))
                serviceResponse = new JsonSerializer().Deserialize<ServiceResponse>(jsonTextReader);

            if (serviceResponse.boleto == null)
                throw new Exception($"Erro ao gerar Boleto - {inscrito.Id} - {serviceResponse.status.mensagem}");

            string html = BaseController.ObterHtmlFromURL(serviceResponse.boleto.url_acesso);

            // Grava o arquivo html
            if (!Directory.Exists(physicalDirectory))
                Directory.CreateDirectory(physicalDirectory);

            using (StreamWriter arquivoHtml = File.CreateText($"{physicalDirectory}/{inscrito.Id}.html"))
            {
                arquivoHtml.Write(html);
            }

            return html;
        }

        private string GeraLayout(IEnumerable<BoletoBancario> boletos)
        {
            var html = new StringBuilder();

            try
            {
                foreach (var o in boletos)
                {
                    html.Append(o.MontaHtmlEmbedded());
                    html.Append("</br></br></br></br></br></br></br></br></br></br>");
                }
            }
            catch (Exception ex)
            {
            }

            return html.ToString();
        }

        private Boletos ListarBoletos(ConcursoModel concurso, Cedente cedente, Banco banco)
        {
            Boletos boletos = new Boletos();
            Int16 codigoBanco = concurso.Empresa.NumeroBanco;
            string carteira = this.ObterCarteira(codigoBanco);

            DateTime vencimento = concurso.DataBoleto.AddDays(1).Date.AddSeconds(-1);

            //List<ConcursoModel.InscritoModel> inscritos = InscritoConcursoBusiness.ListarPagos(concurso.Id).ConvertAll(ico => new ConcursoModel.InscritoModel(ico, true));
            List<ConcursoModel.InscritoModel> inscritos = InscritoConcursoBusiness.ListarNaoPagos(concurso.Id).ConvertAll(ico => new ConcursoModel.InscritoModel(ico, false, true));

            foreach (var inscrito in inscritos)
            {
                double valorBoleto = inscrito.Cargos.Count > 0 && inscrito.Cargos[0].ValorInscricao.HasValue ? Convert.ToDouble(inscrito.Cargos[0].ValorInscricao.Value) : 0;

                Boleto b = new Boleto(vencimento, (decimal)valorBoleto, carteira, inscrito.NossoNumero, cedente);
                b.NumeroDocumento = inscrito.NossoNumero;
                b.Sacado = new Sacado(inscrito.CPFFormatado, inscrito.Nome);
                b.Sacado.Endereco.End = inscrito.NumeroEnderecoString;
                b.Sacado.Endereco.Bairro = inscrito.Bairro;
                b.Sacado.Endereco.Cidade = inscrito.NomeCidade;
                b.Sacado.Endereco.CEP = inscrito.CEP;
                b.Sacado.Endereco.UF = inscrito.SiglaEstado;

                switch (banco.Codigo)
                {
                    case 1:
                        b.EspecieDocumento = new EspecieDocumento_BancoBrasil("1");
                        break;
                    case 237:
                        b.EspecieDocumento = new EspecieDocumento_Santander("17");
                        break;
                    default:
                        break;
                }
                
                b.Banco = banco;
                boletos.Add(b);
            }

            return boletos;
        }

        private string ObterCarteira(short codigoBanco)
        {
            switch (codigoBanco)
            {
                //case 1:
                //    return "18-019";
                case 1:
                    return "11";
                case 237:
                    return "26";
                default:
                    return String.Empty;
            }
        }

        #region [ SubClasses ]

        public class CedenteModel
        {
            public CedenteModel(int convenio, string cpfCnpj, string nome, string agencia, string conta)
            {
                this.Convenio = convenio;
                this.CPFCNPJ = cpfCnpj;
                this.Nome = nome;
                this.Agencia = agencia;
                this.Conta = conta;
            }

            public int Convenio         { get; set; }
            public string CPFCNPJ       { get; set; }
            public string Nome          { get; set; }
            public string Agencia       { get; set; }
            public string Conta         { get; set; }

            public Cedente GerarCedente()
            {
                string conta = this.Conta.Substring(0, this.Conta.Length - 1);
                string digitoConta = this.Conta.Substring(this.Conta.Length - 1);

                Cedente c = new Cedente();
                c.ContaBancaria = new ContaBancaria(this.Agencia, "0", conta, digitoConta);
                c.CPFCNPJ = this.CPFCNPJ;
                c.Nome = this.Nome;
                c.Codigo = this.Convenio.ToString(); //String.Concat(this.Agencia, conta.DigitoAgencia, conta.OperacaConta, conta.Conta, conta.DigitoConta);

                return c;
            }
        }

        public class SacadoModel
        {
            public SacadoModel(string cpfCnpj, string nome, string logradouro, string bairro, string cidade, string cep, string uf)
            {
                this.CPFCNPJ = cpfCnpj;
                this.Nome = nome;
                this.Endereco = new EnderecoModel(logradouro, bairro, cidade, cep, uf);
            }

            public string CPFCNPJ       { get; set; }
            public string Nome              { get; set; }
            public EnderecoModel Endereco   { get; set; }

            public class EnderecoModel
            {
                public EnderecoModel(string logradouro, string bairro, string cidade, string cep, string uf)
                {
                    this.Logradouro = logradouro;
                    this.Bairro = bairro;
                    this.Cidade = cidade;
                    this.CEP = cep;
                    this.UF = uf;
                }

                public string Logradouro { get; set; }
                public string Bairro { get; set; }
                public string Cidade { get; set; }
                public string CEP { get; set; }
                public string UF { get; set; }
            }
        }

        #endregion [ FIM - SubClasses ]
    }
}