using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace TGV.IPEFAE.Web.App.Extensions
{
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
#warning ToDo: Criar log de erro
            //LogErroBusiness.Salvar(context.Exception, null);
        }
    }
}