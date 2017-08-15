using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Controllers;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Areas.Admin.Controllers
{
    [IPEFAEAuthorizationAttribute(PermissaoModel.Tipo.Concurso)]
    public class RecursoController : BaseController
    {
        public ActionResult Index(int? id)
        {
            int idConcurso = id.HasValue && id.Value > 0 ? id.Value : 0;

            ConcursoSelecionado = new ConcursoModel(ConcursoBusiness.Obter(idConcurso, false));
            return View(ConcursoSelecionado);
        }

        public ActionResult ListarRecursos()
        {
            List<RecursoModel> recursos = new List<RecursoModel>();

            if (ConcursoSelecionado != null)
                recursos = ConcursoSelecionado.Recursos;

            return PartialView("_ListaRecursos", recursos);
        }

        public ActionResult Cadastro(int? id)
        {
            if (!id.HasValue)
                return null;

            RecursoModel recurso = new RecursoModel(RecursoBusiness.Obter(id.Value));
            recurso.Concurso = ConcursoSelecionado;
            recurso.Inscrito.NomeCargo = String.IsNullOrEmpty(recurso.Inscrito.NomeCargo) ? recurso.Inscrito.CargosString : recurso.Inscrito.NomeCargo;
            
            return View(recurso);
        }

        public ActionResult Enviar(int idRecurso, string comentario, bool deferido)
        {
            RecursoModel recurso = new RecursoModel(RecursoBusiness.Obter(idRecurso));
            recurso.Concurso = ConcursoSelecionado;

            if (recurso == null)
                return RedirecionarPagina("Index", "Recurso", "Admin", 0);

            recurso.Atendente = UsuarioLogado;
            recurso.Comentario = comentario;
            recurso.DataAtendimento = DateTime.Today;
            recurso.Status = deferido ? RecursoModel.StatusRecurso.Aprovado : RecursoModel.StatusRecurso.Recusado;

            tb_rec_recurso rec = recurso.ToRec();
            RecursoBusiness.Salvar(rec);

            ConcursoModel concurso = ConcursoSelecionado;
            concurso.Recursos.ForEach(r =>
            {
                if (r.Id == recurso.Id)
                {
                    r.Atendente = recurso.Atendente;
                    r.Comentario = recurso.Comentario;
                    r.DataAtendimento = recurso.DataAtendimento;
                    r.Status = recurso.Status;
                }
            });

            ConcursoSelecionado = concurso;

            try
            {
                EnviarEmailRecurso(recurso, "_HtmlRespostaRecurso", TGV.IPEFAE.Web.Resources.Email._HtmlRespostaRecurso.TituloEmail, true);
            }
            catch (Exception ex)
            {

            }

            return Json(new { Sucesso = true, Mensagem = Resources.Admin.Recurso.Cadastro.MensagemSucesso }, JsonRequestBehavior.AllowGet);
        }
	}
}