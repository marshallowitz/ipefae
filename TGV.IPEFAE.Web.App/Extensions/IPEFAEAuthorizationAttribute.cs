using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Extensions
{
    public class IPEFAEAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!this.TiposPermissao.Any(tp => tp.TipoPermissao == PermissaoModel.Tipo.Publico))
            {
                if (TGV.IPEFAE.Web.App.Controllers.BaseController.UsuarioLogado == null || TGV.IPEFAE.Web.App.Controllers.BaseController.UsuarioLogado.Id <= 0)
                {
                    RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
                    redirectTargetDictionary.Add("action", "UnauthorizedAccess");
                    redirectTargetDictionary.Add("controller", "Error");
                    redirectTargetDictionary.Add("area", "");
                    redirectTargetDictionary.Add("id", 401);
                    filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
                }
                else if (TGV.IPEFAE.Web.App.Controllers.BaseController.UsuarioLogado != null && !ValidarPermissao())
                {
                    RouteValueDictionary redirectTargetDictionary = new RouteValueDictionary();
                    redirectTargetDictionary.Add("action", "UnauthorizedAccess");
                    redirectTargetDictionary.Add("controller", "Error");
                    redirectTargetDictionary.Add("area", "");
                    redirectTargetDictionary.Add("id", 401);
                    filterContext.Result = new RedirectToRouteResult(redirectTargetDictionary);
                }
            }

            base.OnActionExecuting(filterContext);
        }

        public IPEFAEAuthorizationAttribute() : this(PermissaoModel.Tipo.Administrador, PermissaoModel.Tipo.Concurso, PermissaoModel.Tipo.Estagio, PermissaoModel.Tipo.Vestibular) { }

        public IPEFAEAuthorizationAttribute(params PermissaoModel.Tipo[] tiposPermissao)
        {
            this.TiposPermissao = new List<PermissaoModel>();
            this.TiposPermissao = tiposPermissao.ToList().ConvertAll(p => new PermissaoModel(p));
        }

        public List<PermissaoModel> TiposPermissao { get; set; }

        protected bool ValidarPermissao()
        {
            if (TGV.IPEFAE.Web.App.Controllers.BaseController.UsuarioLogado.IsAdministrador)
                return true;

            bool valido = false;

            this.TiposPermissao.ForEach(tp =>
                {
                    switch (tp.TipoPermissao)
                    {
                        case PermissaoModel.Tipo.Indefinido:
                            valido = false;
                            return;
                        case PermissaoModel.Tipo.Concurso:
                            valido = valido || TGV.IPEFAE.Web.App.Controllers.BaseController.UsuarioLogado.IsConcurso;
                            break;
                        case PermissaoModel.Tipo.Estagio:
                            valido = valido || TGV.IPEFAE.Web.App.Controllers.BaseController.UsuarioLogado.IsEstagio;
                            break;
                        case PermissaoModel.Tipo.Vestibular:
                            valido = valido || TGV.IPEFAE.Web.App.Controllers.BaseController.UsuarioLogado.IsVestibular;
                            break;
                        case PermissaoModel.Tipo.Administrador:
                            valido = valido || TGV.IPEFAE.Web.App.Controllers.BaseController.UsuarioLogado.IsAdministrador;
                            break;
                    }
                });

            return valido;
        }
    }
}