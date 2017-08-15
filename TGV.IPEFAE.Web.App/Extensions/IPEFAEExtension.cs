using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Extensions
{
    public static class IPEFAEExtension
    {
        public static ConcursosExtension ObterConcursosExtension(this List<ConcursoModel> concursos)
        {
            ConcursosExtension ce = new ConcursosExtension();

            concursos.ForEach(con =>
                {
                    if (BaseBusiness.DataAgora < con.DataEncerramentoInscricoes)
                        ce.Abertas.Add(con);
                    else if (BaseBusiness.DataAgora >= con.DataEncerramentoInscricoes && !con.Encerrado)
                        ce.EmAndamento.Add(con);
                    else if (con.Encerrado && con.DataEncerramento >= BaseBusiness.DataAgora)
                        ce.Encerradas.Add(con);
                });

            return ce;
        }

        public static string ToCargoString(this List<ConcursoModel.CargoModel> cargos)
        {
            StringBuilder sb = new StringBuilder();

            cargos.Where(cco => cco.Ativo).ToList().ForEach(c => sb.AppendFormat(" {0} /", c.Nome.ToUpper()));

            string listaCargos = sb.ToString();

            return listaCargos.Substring(0, listaCargos.Length - 1).Trim();
        }

        public static void Empty(this System.IO.DirectoryInfo directory)
        {
            foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
            directory.Delete();
        }

        public static string FixStringUnicode(this string texto)
        {
            if (String.IsNullOrEmpty(texto))
                texto = String.Empty;

            string tRetorno = String.Empty;

            // &#231;&#227;
            int intCtr = 0;

            while (intCtr <= texto.Length - 1)
            {
                if (texto[intCtr] == '&')
                {
                    int charCode = CheckIfUnicode(texto.Substring(intCtr));

                    if (charCode > 0)
                    {
                        char ch = Convert.ToChar(charCode);
                        tRetorno += ch;
                        intCtr += (charCode > 99) ? 6 : 5;
                        continue;
                    }
                }

                tRetorno += texto[intCtr];
                intCtr++;
            }

            return tRetorno;
        }

        private static int CheckIfUnicode(string partialText)
        {
            int charCode = 0;

            if (partialText.Length < 5
                || partialText.Substring(0, 2) != "&#"
                || (!(Int32.TryParse(partialText.Substring(2, 3), out charCode) || Int32.TryParse(partialText.Substring(2, 2), out charCode)))
                || (charCode > 99 ? partialText.Substring(5, 1) != ";" : partialText.Substring(4, 1) != ";"))
                return -1;

            return charCode;
        }

        public static string FormatarFone(string telefone, bool formatoNormal)
        {
            string onzeDigitos = "{0:##-#########}";
            string dezDigitos = "{0:##-########}";

            if (formatoNormal)
            {
                onzeDigitos = "{0:(##) #####-####}";
                dezDigitos = "{0:(##) ####-####}";
            }

            if (String.IsNullOrEmpty(telefone))
                return String.Empty;

            if (telefone.Length >= 11)
                return String.Format(onzeDigitos, Convert.ToInt64(telefone));

            return String.Format(dezDigitos, Convert.ToInt64(telefone));
        }

        /// <summary>
        /// Retrieve the description on the enum, e.g.
        /// [Description("Bright Pink")]
        /// BrightPink = 2,
        /// Then when you pass in the enum, it will retrieve the description
        /// </summary>
        /// <param name="en">The Enumeration</param>
        /// <returns>A string representing the friendly name</returns>
        public static string GetDescription(this Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return en.ToString();
        }

        public static byte[] GetBytes(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            string[] searchPatterns = searchPattern.Split('|');
            List<string> files = new List<string>();
            foreach (string sp in searchPatterns)
                files.AddRange(System.IO.Directory.GetFiles(path, sp, searchOption));
            files.Sort();
            return files.ToArray();
        }

        public static string GetMesInteiro(this DateTime data)
        {
            switch (data.Month)
            {
                case 1:
                    return "Janeiro";
                case 2:
                    return "Fevereiro";
                case 3:
                    return "Março";
                case 4:
                    return "Abril";
                case 5:
                    return "Maio";
                case 6:
                    return "Junho";
                case 7:
                    return "Julho";
                case 8:
                    return "Agosto";
                case 9:
                    return "Setembro";
                case 10:
                    return "Outubro";
                case 11:
                    return "Novembro";
                case 12:
                    return "Dezembro";
                default:
                    return String.Empty;
            }
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static string GetString(this byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static JContainer MergeInto(this List<JArray> listaJObject)
        {
            if (listaJObject.Count == 0)
                return null;

            JContainer jContRetorno = listaJObject[0];
            listaJObject.RemoveAt(0);

            foreach (JArray arr in listaJObject)
            {
                foreach (JToken item in arr)
                {
                    jContRetorno.Add(item);
                }
            }

            return jContRetorno;
        }

        public static string ObterDescricao(this PropertyInfo propriedade, bool tipoConcurso)
        {
            string resourcePath = tipoConcurso ? "TGV.IPEFAE.Web.Resources.Admin.Concurso.CSVInscritos" : "TGV.IPEFAE.Web.Resources.Admin.Estagio.EstagioCSV";

            Assembly localizationAssembly = Assembly.Load("TGV.IPEFAE.Web.Resources");
            ResourceManager _resources = new ResourceManager(resourcePath, localizationAssembly);
            string descricao = _resources.GetString(String.Format("Header{0}", propriedade.Name));

            return String.IsNullOrEmpty(descricao) ? propriedade.Name : descricao;
        }

        public static Int32 ObterIdade(this DateTime dataNascimento)
        {
            var today = BaseBusiness.DataAgora;

            var a = (today.Year * 100 + today.Month) * 100 + today.Day;
            var b = (dataNascimento.Year * 100 + dataNascimento.Month) * 100 + dataNascimento.Day;

            return (a - b) / 10000;
        }

        public static byte[] ReadToEnd(this System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }

        public static DateTime ToCurrentTimeZone(this DateTime data)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(data, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
        }

        public static void AdicionarClassificacao(this List<tb_icl_inscrito_classificacao> classificacoes, string[] values, Dictionary<int, string> dicColunas)
        {
            int posicaoInscricao = dicColunas.First(c => c.Value.Equals("ico_idt_inscrito_concurso", StringComparison.InvariantCultureIgnoreCase)).Key;

            if (!dicColunas.Any(dic => dic.Value.Equals("icl_des_situacao")))
                return;

            tb_icl_inscrito_classificacao classificacao = new tb_icl_inscrito_classificacao();
            classificacao.icl_bit_ativo = true;

            int idt = 0;

            if (values.Length <= posicaoInscricao || !Int32.TryParse(values[posicaoInscricao], out idt) || idt <= 0)
                return;

            classificacao.ico_idt_inscrito_concurso = idt;

            foreach (KeyValuePair<int, string> item in dicColunas)
            {
                if (item.Value.Equals("icl_des_situacao", StringComparison.InvariantCultureIgnoreCase))
                {
                    classificacao.icl_des_situacao = values[item.Key].Truncate(200);
                    continue;
                }
                else if (item.Value.Equals("icl_num_posicao", StringComparison.InvariantCultureIgnoreCase))
                {
                    int intValor = 0;
                    int? intValorBD = null;

                    if (Int32.TryParse(values[item.Key], out intValor))
                        intValorBD = intValor;

                    classificacao.icl_num_posicao = intValorBD;
                    continue;
                }

                decimal valor = 0;
                decimal? valorBD = null;

                if (Decimal.TryParse(values[item.Key], out valor))
                    valorBD = valor;

                switch (item.Value.ToLower())
                {
                    case "icl_num_nota":
                        classificacao.icl_num_nota = valorBD;
                        break;
                    case "icl_num_cg":
                        classificacao.icl_num_cg = valorBD;
                        break;
                    case "icl_num_ce":
                        classificacao.icl_num_ce = valorBD;
                        break;
                    case "icl_num_tit":
                        classificacao.icl_num_tit = valorBD;
                        break;
                    case "icl_num_pp":
                        classificacao.icl_num_pp = valorBD;
                        break;
                }
            }

            if (String.IsNullOrEmpty(classificacao.icl_des_situacao))
                classificacao.icl_bit_ativo = false;

            classificacoes.Add(classificacao);
        }

        public static void AdicionarDadosInscrito(this List<tb_ico_inscrito_concurso> dadosInscrito, List<tb_est_estado> estados, string[] values, Dictionary<int, string> dicColunas)
        {
            int posicaoInscricao = dicColunas.First(c => c.Value.Equals("ico_idt_inscrito_concurso", StringComparison.InvariantCultureIgnoreCase)).Key;

            if (!dicColunas.Any(dic => dic.Value.Equals("ico_nom_inscrito_concurso")))
                return;

            tb_ico_inscrito_concurso inscrito = new tb_ico_inscrito_concurso();

            int idt = 0;

            if (values.Length <= posicaoInscricao || !Int32.TryParse(values[posicaoInscricao], out idt) || idt <= 0)
                return;

            inscrito.ico_idt_inscrito_concurso = idt;
            string data = "";

            foreach (KeyValuePair<int, string> item in dicColunas)
            {
                switch (item.Value.ToLower())
                {
                    case "ico_dat_nascimento":
                        data = values[item.Key];
                        break;
                    case "ico_des_rg":
                        inscrito.ico_des_rg = values[item.Key].Truncate(20);
                        break;
                    case "est_idt_estado_rg":
                        tb_est_estado estado = estados.SingleOrDefault(est => est.est_sig_estado == values[item.Key].Truncate(2));
                        inscrito.est_idt_estado_rg = estado != null ? estado.est_idt_estado : 0;
                        break;
                    case "ico_nom_inscrito_concurso":
                        inscrito.ico_nom_inscrito_concurso = values[item.Key].Truncate(200);
                        break;
                }
            }

            DateTime dataNasc = new DateTime();

            if (DateTime.TryParseExact(String.Format("{0}", data), "dd/MM/yyyy", new CultureInfo("pt-BR"), DateTimeStyles.None, out dataNasc))
                inscrito.ico_dat_nascimento = dataNasc;

            if (inscrito.ico_dat_nascimento != dataNasc || inscrito.est_idt_estado_rg == 0)
                return;

            dadosInscrito.Add(inscrito);
        }

        public static void AdicionarDadosProva(this List<tb_idt_inscrito_dados_prova> dadosProva, string[] values, Dictionary<int, string> dicColunas)
        {
            int posicaoInscricao = dicColunas.First(c => c.Value.Equals("ico_idt_inscrito_concurso", StringComparison.InvariantCultureIgnoreCase)).Key;

            if (!dicColunas.Any(dic => dic.Value.Equals("idt_dat_prova")))
                return;

            tb_idt_inscrito_dados_prova dadoProva = new tb_idt_inscrito_dados_prova();
            dadoProva.idt_bit_ativo = true;

            int idt = 0;

            if (values.Length <= posicaoInscricao || !Int32.TryParse(values[posicaoInscricao], out idt) || idt <= 0)
                return;

            dadoProva.ico_idt_inscrito_concurso = idt;
            string data = "";
            string hora = "";

            foreach (KeyValuePair<int, string> item in dicColunas)
            {
                switch (item.Value.ToLower())
                {
                    case "idt_dat_prova":
                        data = values[item.Key];
                        break;
                    case "idt_dat_prova_hor":
                        hora = values[item.Key];
                        break;
                    case "idt_des_local":
                        dadoProva.idt_des_local = values[item.Key].Truncate(100);
                        break;
                    case "idt_des_sala":
                        dadoProva.idt_des_sala = values[item.Key].Truncate(100);
                        break;
                    case "idt_des_andar":
                        dadoProva.idt_des_andar = values[item.Key].Truncate(100);
                        break;
                    case "idt_des_endereco":
                        dadoProva.idt_des_endereco = values[item.Key].Truncate(500);
                        break;
                    case "idt_des_numero":
                        dadoProva.idt_des_numero = values[item.Key].Truncate(10);
                        break;
                    case "idt_des_bairro":
                        dadoProva.idt_des_bairro = values[item.Key].Truncate(100);
                        break;
                    case "idt_des_cidade":
                        dadoProva.idt_des_cidade = values[item.Key].Truncate(100);
                        break;
                    case "idt_des_cep":
                        string cep = Regex.Replace(values[item.Key], "[^0-9.]", "");
                        dadoProva.idt_des_cep = cep.Truncate(8);
                        break;
                }
            }

            DateTime dataProva = new DateTime();

            if (DateTime.TryParseExact(String.Format("{0} {1}", data, hora), "dd/MM/yyyy HH:mm", new CultureInfo("pt-BR"), DateTimeStyles.None, out dataProva))
                dadoProva.idt_dat_prova = dataProva;
            else
                dadoProva.idt_bit_ativo = false;

            dadosProva.Add(dadoProva);
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }

    public class ConcursosExtension
    {
        public ConcursosExtension()
        {
            this.Abertas = new List<ConcursoModel>();
            this.EmAndamento = new List<ConcursoModel>();
            this.Encerradas = new List<ConcursoModel>();
        }

        public int TotalAbertas     { get { return this.Abertas.Count; } }
        public int TotalEmAndamento { get { return this.EmAndamento.Count; } }
        public int TotalEncerradas  { get { return this.Encerradas.Count; } }

        public List<ConcursoModel> Abertas      { get; set; }
        public List<ConcursoModel> EmAndamento  { get; set; }
        public List<ConcursoModel> Encerradas   { get; set; }
    }

    public static class ControllerExtensions
    {
        public static string PartialViewToString(this Controller controller)
        {
            return controller.PartialViewToString(null, null);
        }

        public static string RenderPartialViewToString(this Controller controller, string viewName)
        {
            return controller.PartialViewToString(viewName, null);
        }

        public static string RenderPartialViewToString(this Controller controller, object model)
        {
            return controller.PartialViewToString(null, model);
        }

        public static string PartialViewToString(this Controller controller, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = controller.ControllerContext.RouteData.GetRequiredString("action");
            }

            controller.ViewData.Model = model;

            using (StringWriter stringWriter = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, stringWriter);
                viewResult.View.Render(viewContext, stringWriter);
                return stringWriter.GetStringBuilder().ToString();
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute
    {
        private readonly int order_;
        public OrderAttribute([CallerLineNumber]int order = 0)
        {
            order_ = order;
        }

        public int Order { get { return order_; } }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class IsTextAttribute : Attribute
    {
        private readonly bool isText_;
        public IsTextAttribute(bool isText = false)
        {
            isText_ = isText;
        }

        public bool IsText { get { return isText_; } }
    }
}