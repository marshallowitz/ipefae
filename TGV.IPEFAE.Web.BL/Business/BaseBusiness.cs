﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TGV.Framework.Criptografia;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.BL.Business
{
    public static class BaseBusiness
    {
        internal static string ParametroSistema { get { return "IP3F@32015Un1F@3"; } }

        internal static System.Web.HttpContext ContextoCorrente { get { return System.Web.HttpContext.Current; } }
        
        public static DateTime DataAgora { get { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time")); } }

        public static string EmailNaoRespondaIPEFAE { get { return ConfigurationManager.AppSettings["EmailNaoRespondaIPEFAE"]; } }
        public static string NomeNaoRespondaIPEFAE  { get { return ConfigurationManager.AppSettings["NomeNaoRespondaIPEFAE"]; } }
        public static string EmailIPEFAE            { get { return ConfigurationManager.AppSettings["EmailIPEFAE"]; } }
        public static string EmailAtendimentoIPEFAE { get { return ConfigurationManager.AppSettings["EmailAtendimentoIPEFAE"]; } }
        public static bool ConsiderarEmailNaoRespondaComoFrom { get { return ConfigurationManager.AppSettings["ConsiderarEmailNaoRespondaComoFrom"] == "true"; } }
        public static bool IsDebug { get { return ConfigurationManager.AppSettings["debug"] == "true"; } }
        public static bool IsTrace { get { return ConfigurationManager.AppSettings["trace"] == "true"; } }

        public static string ConverterObjetoToCSV(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj", "Value can not be null or Nothing!");

            StringBuilder sb = new StringBuilder();
            Type t = obj.GetType();
            System.Reflection.PropertyInfo[] pi = t.GetProperties();

            for (int index = 0; index < pi.Length; index++)
            {
                sb.Append(pi[index].GetValue(obj, null));

                if (index < pi.Length - 1)
                    sb.Append(",");
            }

            return sb.ToString();
        }

        public static string Descriptografar(string texto)
        {
            return texto.Descriptografar(ParametroSistema);
        }

        public static string FormatarCPF(string cpf, bool aceitarNull = false)
        {
            return BaseData.FormatarCPF(cpf, aceitarNull);
        }

        public static string FormatarCNPJ(string cnpj, bool aceitarNull = false)
        {
            return BaseData.FormatarCNPJ(cnpj, aceitarNull);
        }

        public static string FormatarFone(string fone, bool aceitarNull = false)
        {
            return BaseData.FormatarFone(fone, aceitarNull);
        }

        public static dynamic ObterValorWebConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string OnlyNumbers(string valor)
        {
            Regex digitsOnly = new Regex(@"[^\d]");
            return digitsOnly.Replace(valor, "");
        }

        public static string RemoverCaracteresEspeciais(string stringToConvert)
        {
            string temp = stringToConvert.Normalize(NormalizationForm.FormD);
            IEnumerable<char> filtered = temp;
            filtered = filtered.Where(c => char.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark);
            string final = new string(filtered.ToArray()).Replace(" ", "_");

            return final;
        }
    }
}
