using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Controllers;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;
using CsvHelper.Configuration;

namespace TGV.IPEFAE.Web.App.Areas.Admin.Controllers
{
    [IPEFAEAuthorizationAttribute()]
    public class ConcursoController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastro(int? id)
        {
            int idt = id.HasValue && id.Value > 0 ? id.Value : 0;

            ConcursoModel cM = ConcursoBusiness.Obter(idt);

            if (cM == null)
                cM = new ConcursoModel();

            return View("Cadastro", cM);
        }

        public ActionResult Excluir(int id)
        {
            ConcursoBusiness.Excluir(id);
            return ListarConcursos();
        }

        public ActionResult Funcao_Excluir(int idFuncao)
        {
            ConcursoBusiness.Funcao_Excluir(idFuncao);

            return Json(new { Sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Funcao_Listar(int idConcurso)
        {
            List<ConcursoFuncaoModel> funcoes = ConcursoFuncaoModel.Listar(idConcurso);
            return Json(new { Sucesso = true, Funcoes = funcoes }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Funcao_Salvar(int idConcurso, ConcursoFuncaoModel cfM)
        {
            ConcursoFuncaoModel funcao = ConcursoBusiness.Funcao_Salvar(idConcurso, cfM);

            return Json(new { Funcao = funcao, Sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GerarListaColaboradores(int id)
        {
            Session["GerouCSV"] = null;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GerarCSVListaColaboradoresConfirmacao()
        {
            bool? gerou = Session["GerouCSV"] as Nullable<Boolean>;
            bool retorno = gerou.HasValue && gerou.Value;

            if (retorno)
                Session["GerouCSV"] = null;

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RPA(int id)
        {
            return View(id);
        }

        public ActionResult ListarColaboradores(int idConcurso)
        {
            List<tb_cid_cidade> cidades = CidadeBusiness.ListarTodas();
            List<tb_est_estado> estados = EstadoBusiness.Listar();
            List<IRPFModel> irpfs = IRPFBusiness.Listar();
            List<ConcursoLocalColaboradorModel> locaisColaboradores = ConcursoLocalColaboradorModel.Listar(idConcurso);
            tb_emp_empresa emitente = EmpresaBusiness.Obter(1);

            List<ColaboradorModel> cs = ColaboradorBusiness.ListarPorConcurso(idConcurso);
            List<ColaboradorRPAModel> colaboradores = cs.ConvertAll(c => ColaboradorRPAModel.Clone(c, cidades, estados, locaisColaboradores, irpfs, emitente));
            return Json(new { Colaboradores = colaboradores }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarConcursos()
        {
            List<ConcursoModel> concursos = ConcursoBusiness.Listar();
            return PartialView("_ListaConcursos", concursos);
        }

        public ActionResult Local_Colaborador_Excluir(int idColaborador)
        {
            List<ColaboradorModel> colaboradores = ConcursoBusiness.Local_Colaborador_Excluir(idColaborador);

            return Json(new { Sucesso = true, Colaboradores = colaboradores }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Local_Excluir(int idLocal)
        {
            ConcursoBusiness.Local_Excluir(idLocal);

            return Json(new { Sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Local_Colaborador_Salvar(int idConcursoLocal, ConcursoLocalColaboradorModel clcM)
        {
            ConcursoLocalColaboradorModel colaborador = ConcursoBusiness.Local_Colaborador_Salvar(idConcursoLocal, clcM);

            return Json(new { Colaborador = colaborador, Sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Local_Salvar(int idConcurso, ConcursoLocalModel clM)
        {
            ConcursoLocalModel local = ConcursoBusiness.Local_Salvar(idConcurso, clM);

            return Json(new { Local = local, Sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Obter(int id)
        {
            ConcursoModel cM = ConcursoBusiness.Obter(id);
            List<ColaboradorModel> colaboradores = ColaboradorBusiness.Listar(); 
            bool sucesso = (cM != null);

            return Json(new { Sucesso = sucesso, Concurso = cM, Colaboradores = colaboradores }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Salvar(ConcursoModel cM)
        {
            int id = cM.id;
            ConcursoModel concurso = ConcursoBusiness.Salvar(cM);

            return Json(new { Concurso = concurso, Sucesso = true }, JsonRequestBehavior.AllowGet);
        }
	}
}