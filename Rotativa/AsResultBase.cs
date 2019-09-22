using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Rotativa.Options;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace Rotativa
{
    public abstract class AsResultBase : ActionResult
    {
        protected AsResultBase()
        {
            this.WkhtmlPath = string.Empty;
            this.FormsAuthenticationCookieName = ".ASPXAUTH";
        }

        /// <summary>
        /// This will be send to the browser as a name of the generated PDF file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Path to wkhtmltopdf\wkhtmltoimage binary.
        /// </summary>
        public string WkhtmlPath { get; set; }

        /// <summary>
        /// Custom name of authentication cookie used by forms authentication.
        /// </summary>
        [Obsolete("Use FormsAuthenticationCookieName instead of CookieName.")]
        public string CookieName
        {
            get { return this.FormsAuthenticationCookieName; }
            set { this.FormsAuthenticationCookieName = value; }
        }

        /// <summary>
        /// Custom name of authentication cookie used by forms authentication.
        /// </summary>
        public string FormsAuthenticationCookieName { get; set; }

        /// <summary>
        /// Sets custom headers.
        /// </summary>
        [OptionFlag("--custom-header")]
        public Dictionary<string, string> CustomHeaders { get; set; }

        /// <summary>
        /// Sets cookies.
        /// </summary>
        [OptionFlag("--cookie")]
        public Dictionary<string, string> Cookies { get; set; }

        /// <summary>
        /// Sets post values.
        /// </summary>
        [OptionFlag("--post")]
        public Dictionary<string, string> Post { get; set; }

        /// <summary>
        /// Indicates whether the page can run JavaScript.
        /// </summary>
        [OptionFlag("-n")]
        public bool IsJavaScriptDisabled { get; set; }

        /// <summary>
        /// Minimum font size.
        /// </summary>
        [OptionFlag("--minimum-font-size")]
        public int? MinimumFontSize { get; set; }

        /// <summary>
        /// Sets proxy server.
        /// </summary>
        [OptionFlag("-p")]
        public string Proxy { get; set; }

        /// <summary>
        /// HTTP Authentication username.
        /// </summary>
        [OptionFlag("--username")]
        public string UserName { get; set; }

        /// <summary>
        /// HTTP Authentication password.
        /// </summary>
        [OptionFlag("--password")]
        public string Password { get; set; }

        /// <summary>
        /// Use this if you need another switches that are not currently supported by Rotativa.
        /// </summary>
        [OptionFlag("")]
        public string CustomSwitches { get; set; }

        [Obsolete(@"Use BuildFile(this.ControllerContext) method instead and use the resulting binary data to do what needed.")]
        public string SaveOnServerPath { get; set; }

        protected abstract string GetUrl(ControllerContext context);

        /// <summary>
        /// Returns properties with OptionFlag attribute as one line that can be passed to wkhtmltopdf binary.
        /// </summary>
        /// <returns>Command line parameter that can be directly passed to wkhtmltopdf binary.</returns>
        protected virtual string GetConvertOptions()
        {
            var result = new StringBuilder();

            var fields = this.GetType().GetProperties();
            foreach (var fi in fields)
            {
                var of = fi.GetCustomAttributes(typeof(OptionFlag), true).FirstOrDefault() as OptionFlag;
                if (of == null)
                    continue;

                object value = fi.GetValue(this, null);
                if (value == null)
                    continue;

                if (fi.PropertyType == typeof(Dictionary<string, string>))
                {
                    var dictionary = (Dictionary<string, string>)value;
                    foreach (var d in dictionary)
                    {
                        result.AppendFormat(" {0} {1} {2}", of.Name, d.Key, d.Value);
                    }
                }
                else if (fi.PropertyType == typeof(bool))
                {
                    if ((bool)value)
                        result.AppendFormat(CultureInfo.InvariantCulture, " {0}", of.Name);
                }
                else
                {
                    result.AppendFormat(CultureInfo.InvariantCulture, " {0} {1}", of.Name, value);
                }
            }

            return result.ToString().Trim();
        }

        private string GetWkParams(ControllerContext context)
        {
            var switches = string.Empty;

            HttpCookie authenticationCookie = null;
            if (context.HttpContext.Request.Cookies != null && context.HttpContext.Request.Cookies.AllKeys.Contains(FormsAuthentication.FormsCookieName))
            {
                authenticationCookie = context.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            }
            if (authenticationCookie != null)
            {
                var authCookieValue = authenticationCookie.Value;
                switches += " --cookie " + this.FormsAuthenticationCookieName + " " + authCookieValue;
            }

            switches += " " + this.GetConvertOptions();

            var url = this.GetUrl(context);
            switches += " " + url;

            return switches;
        }

        protected virtual byte[] CallTheDriver(ControllerContext context)
        {
            var switches = this.GetWkParams(context);

            if (switches == null)
                throw new NullReferenceException("switches - GetWkParams");

            var fileContent = this.WkhtmlConvert(switches);

            if (fileContent == null)
                throw new NullReferenceException("fileContent - WkhtmlConvert");

            return fileContent;
        }

        protected virtual string GetHtmlView(ControllerContext context)
        {
           return "";
        }

        protected abstract byte[] WkhtmlConvert(string switches);

        public byte[] BuildFile(ControllerContext context)
        {
            try
            {
                if (context == null)
                    throw new ArgumentNullException("context");

                if (this.WkhtmlPath == string.Empty)
                    this.WkhtmlPath = HttpContext.Current.Server.MapPath("~/Rotativa");
                
                var fileContent = this.CallTheDriver(context);

                if (string.IsNullOrEmpty(this.SaveOnServerPath) == false)
                {
                    File.WriteAllBytes(this.SaveOnServerPath, fileContent);
                }
                
                return fileContent;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string ApiKey { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            int id = ObterIdChave();
            //var fileContent = BuildFile(context);
            string apiKey = String.IsNullOrEmpty(this.ApiKey) ? ConfigurationManager.AppSettings[$"apiKeyPDF0{id}"] : this.ApiKey;
            var fileContent = NewBuildFile(context, apiKey);
            var response = PrepareResponse(context.HttpContext.Response);
            response.OutputStream.Write(fileContent, 0, fileContent.Length);
        }

        internal static int ObterIdChave()
        {
            int id = 5;
            String cString = ConfigurationManager.ConnectionStrings["IPEFAEEntities_"].ConnectionString;
            using (SqlConnection cnn = new SqlConnection(cString))
            {
                cnn.Open();
                string sql = "SELECT TOP 1 id FROM relatorio_pdf WHERE quantidade < 200";
                SqlCommand command = new SqlCommand(sql, cnn);
                SqlDataReader dr = command.ExecuteReader();

                while(dr.Read())
                {
                    id = (int)dr.GetValue(0);
                }
            }

            return id;
        }

        public byte[] NewBuildFile(ControllerContext context, string apiKey)
        {
            ApiKey = apiKey;
            //string apiKey = "057853b2-16a9-4d04-a4ad-b68c2aa8f5a2";
            string value = GetHtmlView(context);
            using (var client = new System.Net.WebClient())
            {
                NameValueCollection options = new NameValueCollection();
                options.Add("apikey", apiKey);
                options.Add("value", value);

                byte[] result = client.UploadValues("https://api.html2pdfrocket.com/pdf", options);

                return result;
            }
        }

        private static string SanitizeFileName(string name)
        {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidPathChars()) + new string(Path.GetInvalidFileNameChars()));
            string invalidCharsPattern = string.Format(@"[{0}]+", invalidChars);

            string result = Regex.Replace(name, invalidCharsPattern, "_");
            return result;
        }

        protected HttpResponseBase PrepareResponse(HttpResponseBase response)
        {
            response.ContentType = this.GetContentType();

            if (!String.IsNullOrEmpty(this.FileName))
                response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", SanitizeFileName(this.FileName)));

            response.AddHeader("Content-Type", this.GetContentType());

            return response;
        }

        protected abstract string GetContentType();
    }
}