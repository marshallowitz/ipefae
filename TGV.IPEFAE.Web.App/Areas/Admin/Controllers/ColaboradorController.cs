using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Controllers;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.App.Helper;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Areas.Admin.Controllers
{
    [IPEFAEAuthorizationAttribute()]
    public class ColaboradorController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastro(int? id)
        {
            int idt = id.HasValue && id.Value > 0 ? id.Value : 0;

            ColaboradorModel cM = ColaboradorBusiness.Obter(idt);

            if (cM == null)
                cM = new ColaboradorModel();

            return View("Cadastro", cM);
        }

        public ActionResult GerarCSV()
        {
            Session["GerouCSV"] = null;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GerarCSVConfirmacao()
        {
            bool? gerou = Session["GerouCSV"] as Nullable<Boolean>;
            bool retorno = gerou.HasValue && gerou.Value;

            if (retorno)
                Session["GerouCSV"] = null;

            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarColaboradores()
        {
            List<ColaboradorModel> colaboradores = ColaboradorBusiness.Listar();
            return PartialView("_ListaColaboradores", colaboradores);
        }

        public ActionResult Salvar(ColaboradorModel cM)
        {
            int id = cM.id;
            ColaboradorModel colaborador = ColaboradorBusiness.Salvar(cM);

            return Json(new { Colaborador = colaborador, Sucesso = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UploadPlanilha()
        {
            var listaBancos = BancoBusiness.Listar();
            var listaFuncoes = ConcursoFuncaoModel.ListarTodas();
            string txt = String.Empty;

            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;

                var lines = ReadLines(() => file.InputStream, System.Text.Encoding.UTF8).ToArray();
                txt += GerarArquivoContadorTXT(lines, listaBancos, listaFuncoes);
            }

            return Json(new { Sucesso = true, Arquivo = txt }, JsonRequestBehavior.AllowGet);
        }

        private string GerarArquivoContadorTXT(string[] linhasCSV, List<BancoModel> listaBancos, List<FuncaoModel> listaFuncoes)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbErros = new StringBuilder();

            linhasCSV = linhasCSV.Skip(1).ToArray(); // Remove o cabeçalho
            int linhaCtr = 1;

            foreach (var linhaCSV in linhasCSV)
            {
                linhaCtr++;
                var itensLinha = linhaCSV.Split(';');

                if (itensLinha.Length < 48)
                {
                    sbErros.AppendLine($"Coluna faltante na linha {linhaCtr}");
                    continue;
                }

                sb.Append("190".PadLeft(7, '0')); // Código da empresa

                #region [ CPF ]

                var cpf = ValidacaoPlanilhaContadorHelper.ObterCPF(itensLinha[12]);

                if (cpf.Item1)
                    sb.Append(cpf.Item2); // CPF do contribuinte
                else
                    sbErros.AppendLine($"{cpf.Item2} na linha {linhaCtr}");

                #endregion [ FIM - CPF ]

                #region [ PIS ]

                var pis = ValidacaoPlanilhaContadorHelper.ObterPIS(itensLinha[20]);

                if (pis.Item1)
                    sb.Append(pis.Item2); // PIS do contribuinte
                else
                    sbErros.AppendLine($"{pis.Item2} na linha {linhaCtr}");

                #endregion [ FIM - PIS ]

                #region [ Nome ]

                var nome = ValidacaoPlanilhaContadorHelper.ObterNomeContribuinte(itensLinha[11]);

                if (nome.Item1)
                    sb.Append(nome.Item2); // Nome do contribuinte
                else
                    sbErros.AppendLine($"{nome.Item2} na linha {linhaCtr}");

                #endregion [ FIM - Nome ]

                sb.Append("1".PadLeft(10, '0')); // Código da empresa

                #region [ Cargo ]

                var cargo = ValidacaoPlanilhaContadorHelper.ObterCodigoCargo(itensLinha[5], listaFuncoes);

                if (cargo.Item1)
                    sb.Append(cargo.Item2); // Código do Cargo
                else
                    sbErros.AppendLine($"{cargo.Item2} na linha {linhaCtr}");

                #endregion [ FIM - Cargo ]

                sb.Append("9".PadLeft(10, '0')); // Código do departamento
                sb.Append("1".PadLeft(10, '0')); // Código do centro de custo

                #region [ Data Inicio Contrato ]

                var dataInicioContrato = ValidacaoPlanilhaContadorHelper.ObterData(itensLinha[1], "Data Início Contrato");

                if (dataInicioContrato.Item1)
                    sb.Append(dataInicioContrato.Item2); // Data Início Contrato
                else
                    sbErros.AppendLine($"{dataInicioContrato.Item2} na linha {linhaCtr}");

                #endregion [ FIM - Data Inicio Contrato ]

                sb.Append("13".PadLeft(2, '0')); // Categoria Sefip
                sb.Append("7"); // Tipo de contribuinte - Autônomo Microempreendedor Individual

                sb.Append("8909".PadLeft(6, '0')); // Código da rubrica

                #region [ Valor Salario ]

                var valorSalario = ValidacaoPlanilhaContadorHelper.ObterValorSalario(itensLinha[6]);

                if (valorSalario.Item1)
                    sb.Append(valorSalario.Item2); // Valor do Salário
                else
                    sbErros.AppendLine($"{valorSalario.Item2} na linha {linhaCtr}");

                #endregion [ FIM - Valor Salario ]

                sb.Append("N"); // Tipo Serviço - Não Especificado

                #region [ Data Nascimento ]

                var dataNascimento = ValidacaoPlanilhaContadorHelper.ObterData(itensLinha[21], "Data Nascimento");

                if (dataNascimento.Item1)
                    sb.Append(dataNascimento.Item2); // Data Nascimento
                else
                    sbErros.AppendLine($"{dataNascimento.Item2} na linha {linhaCtr}");

                #endregion [ FIM - Data Nascimento ]

                sb.Append("2"); // Forma de Pagamento - Crédito em Conta

                #region [ Codigo Banco ]

                var codigoBanco = ValidacaoPlanilhaContadorHelper.ObterCodigoBanco(itensLinha[34], listaBancos);

                if (codigoBanco.Item1)
                    sb.Append(codigoBanco.Item2); // Código do Banco
                else
                    sbErros.AppendLine($"{codigoBanco.Item2} na linha {linhaCtr}");

                #endregion [ FIM - Codigo Banco ]

                #region [ Numero Conta ]

                var numeroConta = ValidacaoPlanilhaContadorHelper.ObterNumeroConta(itensLinha[38]);

                if (numeroConta.Item1)
                    sb.Append(numeroConta.Item2); // Número da Conta
                else
                    sbErros.AppendLine($"{numeroConta.Item2} na linha {linhaCtr}");

                #endregion [ FIM - Numero Conta ]

                #region [ Digito Conta ]

                var numeroContaDigito = ValidacaoPlanilhaContadorHelper.ObterNumeroContaDigito(itensLinha[39]);

                if (numeroContaDigito.Item1)
                    sb.Append(numeroContaDigito.Item2); // Digito da Conta
                else
                    sbErros.AppendLine($"{numeroContaDigito.Item2} na linha {linhaCtr}");

                #endregion [ FIM - Digito Conta ]

                #region [ Tipo Conta ]

                var tipoConta = ValidacaoPlanilhaContadorHelper.ObterNumeroContaTipo(itensLinha[35]);

                if (tipoConta.Item1)
                    sb.Append(tipoConta.Item2); // Tipo da Conta
                else
                    sbErros.AppendLine($"{tipoConta.Item2} na linha {linhaCtr}");

                #endregion [ FIM - Tipo Conta ]

                sb.AppendLine();
            }

            if (sbErros.Length > 0)
                return sbErros.ToString();
            else
                return sb.ToString();
        }

        private bool ValidarAcesso(int usu_idt_usuario)
        {
            if (UsuarioLogado.IsAdministrador)
                return true;

            tb_usu_usuario usuario = UsuarioBusiness.Obter(usu_idt_usuario);

            if (usuario != null)
            {
                UsuarioModel u = new UsuarioModel(usuario);

                return (UsuarioLogado.IsConcurso && u.IsConcurso)
                    || (UsuarioLogado.IsEstagio && u.IsEstagio)
                    || (UsuarioLogado.IsVestibular && u.IsVestibular);
            }

            return true;
        }
	}
}