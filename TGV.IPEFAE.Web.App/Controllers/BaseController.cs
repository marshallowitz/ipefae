using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Configuration;
using System.Text;
using RazorEngine;
using RazorEngine.Templating;

namespace TGV.IPEFAE.Web.App.Controllers
{
    [HandleError]
    public class BaseController : Controller
    {
        #region [ Membros ]

        protected const string _nomeCookieUsuarioLogado = "userCoookieIPEFAE";
        protected const string _nomeSessaoUsuarioLogado = "SessaoBaseControllerUsuarioLogado";

        #endregion [ FIM - Membros ]

        #region [ Sessions ]

        protected const string _nomeSessionConcursoSelecionado = "ConcursoSelecionadoSession";
        protected const string _nomeSessionTodosConcursos = "TodosConcursosSession";

        protected List<ConcursoModel> TodosConcursos
        {
            get
            {
                if (System.Web.HttpContext.Current.Session == null || System.Web.HttpContext.Current.Session[_nomeSessionTodosConcursos] == null)
                {
                    return new List<ConcursoModel>();
                }

                return (List<ConcursoModel>)System.Web.HttpContext.Current.Session[_nomeSessionTodosConcursos];
            }

            set
            {
                System.Web.HttpContext.Current.Session[_nomeSessionTodosConcursos] = value;
            }
        }

        protected ConcursoModel ConcursoSelecionado
        {
            get
            {
                if (System.Web.HttpContext.Current.Session == null || System.Web.HttpContext.Current.Session[_nomeSessionConcursoSelecionado] == null)
                {
                    return new ConcursoModel();
                }

                return (ConcursoModel)System.Web.HttpContext.Current.Session[_nomeSessionConcursoSelecionado];
            }

            set
            {
                System.Web.HttpContext.Current.Session[_nomeSessionConcursoSelecionado] = value;
            }
        }

        #endregion [ FIM - Sessions ]

        #region [ Propriedades ]

        public static int TamanhoPaginaLista
        {
            get
            {
                int tamanhoPagina = 10;
                Int32.TryParse(ConfigurationManager.AppSettings["TamanhoPaginaLista"], out tamanhoPagina);

                return tamanhoPagina;
            }
        }

        public static UsuarioModel UsuarioLogado
        {
            get
            {
                if (System.Web.HttpContext.Current.Session == null || System.Web.HttpContext.Current.Session[_nomeSessaoUsuarioLogado] == null)
                {
                    HttpCookie userCookie = new HttpCookie(BaseController._nomeCookieUsuarioLogado);
                    userCookie = System.Web.HttpContext.Current.Request.Cookies[BaseController._nomeCookieUsuarioLogado];

                    if (userCookie != null && (userCookie["email"] != null || !String.IsNullOrEmpty(userCookie.Value)))
                    {
                        string email = userCookie["email"] == null ? userCookie.Value.Replace("%40", "@") : userCookie["email"];

                        if (!String.IsNullOrEmpty(email))
                        {
                            tb_usu_usuario usuario = UsuarioBusiness.Obter(email);

                            if (usuario != null)
                            {
                                System.Web.HttpContext.Current.Session[_nomeSessaoUsuarioLogado] = new UsuarioModel(usuario);

                                return (UsuarioModel)System.Web.HttpContext.Current.Session[_nomeSessaoUsuarioLogado];
                            }
                        }
                    }

                    return new UsuarioModel();
                }

                return (UsuarioModel)System.Web.HttpContext.Current.Session[_nomeSessaoUsuarioLogado];
            }

            set
            {
                System.Web.HttpContext.Current.Session[_nomeSessaoUsuarioLogado] = value;
            }
        }

        public static string VersionNumber
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public ActionResult Captcha(string sessionName)
        {
            if (String.IsNullOrEmpty(sessionName) || Session[sessionName] != null)
                return Json(false, JsonRequestBehavior.AllowGet);

            Session[sessionName] = "OK";
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CaptchaCheck(string sessionName)
        {
            if (String.IsNullOrEmpty(sessionName) || Session[sessionName] == null || Session[sessionName].ToString() != "OK")
                return Json(false, JsonRequestBehavior.AllowGet);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Download(string path)
        {
            string fileName = Path.GetFileName(path);
            string folder = Path.GetFullPath(path);

            if (System.IO.File.Exists(path))
                return File(new FileStream(path, FileMode.Open), "content-dispostion", fileName);

            return RedirecionarPagina("NotFound", "Error", "", 404);
        }

        public ActionResult EnviarContato(string nome, string email, string telefone, string mensagem)
        {
            if (String.IsNullOrEmpty(nome) || String.IsNullOrEmpty(email) || String.IsNullOrEmpty(telefone) || String.IsNullOrEmpty(mensagem))
                return Json(false, JsonRequestBehavior.AllowGet);

            // Envia e-mail
            string tituloEmail = String.Format(TGV.IPEFAE.Web.Resources.Email._HtmlContato.TituloEmail, nome);
            string corpoEmail = MontarCorpoEmailContato(nome, email, telefone, tituloEmail, mensagem);
            string emailFrom = BaseBusiness.ConsiderarEmailNaoRespondaComoFrom ? BaseBusiness.EmailNaoRespondaIPEFAE : email;
            EmailBusiness.EnviarEmail(BaseBusiness.EmailAtendimentoIPEFAE, emailFrom, nome, email, tituloEmail, corpoEmail, false, true, new List<string>());

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public void EnviarEmailRecurso(RecursoModel recurso, string partialViewEmail, string tituloEmailResource, bool atendente)
        {
            // Envia e-mail
            string tituloEmail = String.Format(tituloEmailResource, recurso.Concurso.Nome, recurso.Protocolo);
            string email = recurso.Inscrito.Email;
            string corpoEmail = MontarCorpoEmailRecurso(recurso, tituloEmail, partialViewEmail);

            List<string> anexos = new List<string>();
            string pathFiles = atendente ? recurso.PathAnexosAtendente : recurso.PathAnexosRequerente;
            string pathTotal = Server.MapPath(pathFiles);

            if (Directory.Exists(pathTotal))
            {
                string[] files = IPEFAEExtension.GetFiles(pathTotal, "*.pdf|*.doc|*.docx", SearchOption.TopDirectoryOnly);

                foreach (string filePath in files)
                    anexos.Add(filePath);
            }

            EmailBusiness.EnviarEmail(email, BaseBusiness.EmailNaoRespondaIPEFAE, BaseBusiness.NomeNaoRespondaIPEFAE, tituloEmail, corpoEmail, true, anexos);
        }

        public ActionResult ListarAnexos(string pathAnexos)
        {
            string pathTotal = Server.MapPath(pathAnexos);

            if (!Directory.Exists(pathTotal))
                Directory.CreateDirectory(pathTotal);

            string[] files = Directory.GetFiles(pathTotal, "*.*");
            List<AnexoModel> anexos = new List<AnexoModel>();

            foreach (string file in files)
                anexos.Add(new AnexoModel(file));

            return Json(new { View = this.PartialViewToString("~/Views/Shared/_ListaAnexos.cshtml", anexos) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogarErroClient(string clientMethod, string mensagem)
        {
#warning ToDo: Criar método LogarErroClient
            //UsuarioModel usuarioLogado = UsuarioLogado;
            //long? idUsuarioLogado = usuarioLogado != null && usuarioLogado.UsuarioId > 0 ? usuarioLogado.UsuarioId : (long?)null;
            //Int64 idErro = Convert.ToInt64(BaseBusiness.DataAgora.ToString("yyyyMMddHHmmssfff"));
            //string url = HttpContext.Request.Url.AbsolutePath;

            //tb_ler_log_erro logErro = new tb_ler_log_erro()
            //{
            //    ler_idt_log_erro = idErro,
            //    ler_dat_erro = BaseBusiness.DataAgora,
            //    ler_num_codigo_erro = null,
            //    ler_des_nome_metodo = clientMethod,
            //    ler_des_source = clientMethod,
            //    ler_des_inner_exception = null,
            //    ler_des_mensagem = mensagem,
            //    ler_des_stack_trace = "Javascript",
            //    ler_des_tipo_exception = "Javascript",
            //    ler_des_url = url,
            //    usu_idt_usuario = idUsuarioLogado
            //};

            //LogErroBusiness.Salvar(logErro);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private string MontarCorpoEmailRecurso(RecursoModel recurso, string assunto, string partialViewEmail)
        {
            EmailModel eModel = new EmailModel();
            eModel.Nome = recurso.Inscrito.Nome;
            eModel.Matricula = recurso.Inscrito.NroMatricula;
            eModel.Email = recurso.Inscrito.Email;
            eModel.Assunto = assunto;
            eModel.Recurso = recurso;
            eModel.Recurso.Mensagem = eModel.Recurso.Mensagem.Replace("\n", "<br />");
            eModel.Recurso.Comentario = String.IsNullOrEmpty(eModel.Recurso.Comentario) ? String.Empty : eModel.Recurso.Comentario.Replace("\n", "<br />");

            // Caso não tenha cargo, vai buscar
            string nomeCargo = recurso.Inscrito.NomeCargo;

            if (String.IsNullOrEmpty(nomeCargo))
            {
                tb_cco_cargo_concurso cco = ConcursoBusiness.ObterCargoInscritoCandidato(recurso.Inscrito.Id);

                if (cco != null)
                    nomeCargo = cco.cco_nom_cargo_concurso;
            }

            eModel.Cargo = nomeCargo;

            return System.Web.HttpUtility.HtmlDecode(this.PartialViewToString(String.Format("~/Views/Email/{0}.cshtml", partialViewEmail), eModel));
        }

        public static string ObterHtmlFromURL(string urlAddress, byte[] postData = null, string parameters = null)
        {
            string data = String.Empty;

            if (parameters != null)
            {
                urlAddress = $"{urlAddress}?{parameters}";
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);

            if (postData != null)
            {
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postData.Length;
                request.Method = "POST";

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(postData, 0, postData.Length);
                dataStream.Close();
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;

                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, System.Text.Encoding.GetEncoding(response.CharacterSet));
                    }

                    data = readStream.ReadToEnd();

                    //response.Close();
                    readStream.Close();
                }
            }

            return data;
        }

        public string ObterHtmlFromURLBB(string urlAddress, string parameters = null)
        {
            if (parameters != null)
                urlAddress = $"{urlAddress}?{parameters}";

            var pathTemplate = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Views/Shared/_BoletoBB.cshtml");

            string view = System.IO.File.ReadAllText(pathTemplate);
            var html = view.Replace("|*|URL|*|", urlAddress);

            //var html = Engine.Razor.RunCompile(view, $"templateEmail_{BaseBusiness.DataAgora.ToString("yyyyMMddHHmmss")}", typeof(string), urlAddress);

            return html;
        }

        public ActionResult RemoverArquivo(string pathArquivo)
        {
            if (System.IO.File.Exists(pathArquivo))
                System.IO.File.Delete(pathArquivo);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        protected static string RenderViewToString(ControllerContext context, string viewPath, object model = null, bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial)
                viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            else
                viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null)
                throw new FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                            context.Controller.ViewData,
                                            context.Controller.TempData,
                                            sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        protected ActionResult RedirecionarPaginaEmConstrucao()
        {
            return RedirecionarPagina("EmConstrucao", "Error", "", 404);
        }

        protected ActionResult RedirecionarPagina(string action, string controller, string area, int id)
        {
            System.Web.Routing.RouteValueDictionary redirectTargetDictionary = new System.Web.Routing.RouteValueDictionary();
            redirectTargetDictionary.Add("action", action);
            redirectTargetDictionary.Add("controller", controller);
            redirectTargetDictionary.Add("area", area);
            redirectTargetDictionary.Add("id", id);
            
            return new RedirectToRouteResult(redirectTargetDictionary);
        }

        protected void SalvarArquivo(string diretorio, string fileName, Stream stream)
        {
            var path = Path.Combine(diretorio, fileName);
            try
            {
                if (!Directory.Exists(diretorio))
                    Directory.CreateDirectory(diretorio);

                using (var fileStream = System.IO.File.Create(path))
                    stream.CopyTo(fileStream);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<JsonResult> UploadFiles(int idRecurso, bool atendente)
        {
            try
            {
                string diretorio = Server.MapPath(String.Format("~/Anexos/Recurso/{0}/{1}", idRecurso, atendente ? "Atendente" : "Requerente"));

                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        var stream = fileContent.InputStream;
                        var fileName = fileContent.FileName;

                        if ((Path.GetExtension(fileContent.FileName).ToLower() != ".pdf" && Path.GetExtension(fileContent.FileName).ToLower() != ".doc" && Path.GetExtension(fileContent.FileName).ToLower() != ".docx") ||
                            fileContent.ContentLength > 2224000)
                            return Json(new { Sucesso = false }, JsonRequestBehavior.AllowGet);

                        SalvarArquivo(diretorio, fileName, stream);
                    }
                }
            }
            catch (Exception)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Sucesso = false }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerEmail(int? id)
        {
            int idt = id.HasValue ? id.Value : 1;
            if (idt <= 0)
                idt = 1;

            return View(idt);
        }

        public ActionResult VerEmailRetorno(int? id)
        {
            int idt = id.HasValue ? id.Value : 1;
            if (idt <= 0)
                idt = 1;

            EmailModel eModel = new EmailModel();
            eModel.Nome = "Nome";
            eModel.Matricula = "00798";
            eModel.Cargo = "Nome do Cargo";
            eModel.Email = "seu@email.com.br";
            eModel.Telefone = "(11) 91234.5678";
            eModel.Assunto = "Assunto contato";
            eModel.Mensagem = "Teste de mensagem de contato";

            switch (idt)
            {
                case 2:
                    eModel.Recurso = new RecursoModel()
                    {
                        Comentario = "Teste de comentário de resposta",
                        Concurso = new ConcursoModel() { Nome = "Nome do Concurso" },
                        DataAbertura = DateTime.Today,
                        Inscrito = new ConcursoModel.InscritoModel() { Nome = "João da Silva" },
                        Mensagem = eModel.Mensagem,
                        Status = RecursoModel.StatusRecurso.Recusado
                    };

                    return Json(System.Web.HttpUtility.HtmlDecode(this.PartialViewToString("~/Views/Email/_HtmlRespostaRecurso.cshtml", eModel)), JsonRequestBehavior.AllowGet);
                case 3:
                    eModel.Recurso = new RecursoModel()
                    {
                        Comentario = "Teste de comentário de resposta",
                        Concurso = new ConcursoModel() { Nome = "Nome do Concurso" },
                        DataAbertura = DateTime.Today,
                        Inscrito = new ConcursoModel.InscritoModel() { Nome = "João da Silva" },
                        Mensagem = eModel.Mensagem,
                        Status = RecursoModel.StatusRecurso.Recusado
                    };

                    return Json(System.Web.HttpUtility.HtmlDecode(this.PartialViewToString("~/Views/Email/_HtmlAberturaRecurso.cshtml", eModel)), JsonRequestBehavior.AllowGet);
                default:
                    return Json(System.Web.HttpUtility.HtmlDecode(this.PartialViewToString("~/Views/Email/_HtmlContato.cshtml", eModel)), JsonRequestBehavior.AllowGet);
            }
        }

        private string MontarCorpoEmailContato(string nome, string email, string telefone, string assunto, string mensagem)
        {
            EmailModel eModel = new EmailModel();
            eModel.Nome = nome;
            eModel.Email = email;
            eModel.Telefone = telefone;
            eModel.Assunto = assunto;
            eModel.Mensagem = mensagem.Replace("\n", "<br />");

            return System.Web.HttpUtility.HtmlDecode(this.PartialViewToString("~/Views/Email/_HtmlContato.cshtml", eModel));
        }

        #endregion [ FIM - Metodos ]

        #region [ Eventos ]

        protected override void OnException(ExceptionContext filterContext)
        {
            int? idUsuario = UsuarioLogado != null && UsuarioLogado.Id > 0 ? UsuarioLogado.Id : (int?)null;
            LogErroBusiness.Salvar(filterContext.Exception, idUsuario);
        }

        #endregion [ FIM - Eventos ]
    }
}