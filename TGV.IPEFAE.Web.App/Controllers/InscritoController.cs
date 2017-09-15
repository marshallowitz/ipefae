using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TGV.IPEFAE.Web.App.Models;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Controllers
{
    public class InscritoController : BaseController
    {
        private const string SessionDadosInscrito = "DadosCadastroInscrito";
        private const string SessionConcursoSelecionadoUsuario = "ConcursoSelecionadoUsuario";

        public ActionResult CarregarCidades(int est_idt_estado)
        {
            List<tb_cid_cidade> cidades = CidadeBusiness.Listar(est_idt_estado);
            List<SelectListItem> cidadesLI = cidades.ConvertAll(cid => new SelectListItem() { Text = cid.cid_nom_cidade, Value = cid.cid_idt_cidade.ToString() });

            return Json(cidadesLI, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CarregarEstados(bool exibirNome)
        {
            List<tb_est_estado> estados = EstadoBusiness.Listar();
            List<SelectListItem> estadosLI = estados.ConvertAll(est => new SelectListItem() { Text = exibirNome ? est.est_nom_estado : est.est_sig_estado, Value = est.est_idt_estado.ToString() });

            return Json(estadosLI, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Salvar(int id, int idConcurso, int idCargo, string nome, string email, string cpf, string rg, int? estado_rg, string dataNasc, string estadocivil, string telefone, string celular, string endereco, string nroEndereco, string complemento, string bairro, string cep, int idCidade, int filhosmenores, bool possuiDef, string deficiencia_qual, bool necessitaTratEsp, string tratamento_especial_qual, bool ico_bit_ativo, bool ico_bit_pago, DateTime dataInscricao, string dataPagamento, string valorPago, bool isento, bool salvarBanco, string nomeConcurso, string nomeCargo, string estado_rg_string, string nomeCidade, string nomeEstado, int idEstado, string linkBoleto)
        {
            DateTime? dPagamento = String.IsNullOrEmpty(dataPagamento) ? (DateTime?)null : Convert.ToDateTime(dataPagamento);
            DateTime dataNascimento = Convert.ToDateTime(dataNasc);
            decimal? vPago = String.IsNullOrEmpty(valorPago) ? (decimal?)null : Convert.ToDecimal(valorPago);
            int estado_rg_int = estado_rg.HasValue ? estado_rg.Value : 26;
            ico_bit_ativo = id <= 0 ? true : ico_bit_ativo;

            ConcursoModel concurso = (ConcursoModel)Session[SessionConcursoSelecionadoUsuario];

            if (concurso == null)
                concurso = new ConcursoModel(ConcursoBusiness.Obter(idConcurso));

            tb_ico_inscrito_concurso ico = new tb_ico_inscrito_concurso()
            {
                cid_idt_cidade = idCidade,
                con_idt_concurso = idConcurso,
                est_idt_estado_rg = estado_rg_int,
                ico_bit_ativo = ico_bit_ativo,
                ico_bit_destro = true,
                ico_bit_pago = ico_bit_pago,
                ico_bit_possui_deficiencia = possuiDef,
                ico_bit_tratamento_especial = necessitaTratEsp,
                ico_dat_inscricao = dataInscricao,
                ico_dat_nascimento = dataNascimento,
                ico_dat_pagamento = dPagamento,
                ico_des_bairro = bairro,
                ico_des_celular = celular,
                ico_des_cep = cep,
                ico_des_complemento = complemento,
                ico_des_cpf = cpf,
                ico_des_email = email,
                ico_des_endereco = endereco,
                ico_des_outras_solicitacoes = String.Empty,
                ico_des_rg = rg,
                ico_des_telefone = telefone,
                ico_des_tratamento_especial_qual = tratamento_especial_qual,
                ico_flg_deficiencia = deficiencia_qual,
                ico_flg_estado_civil = estadocivil,
                ico_idt_inscrito_concurso = id,
                ico_nom_inscrito_concurso = nome,
                ico_des_nro_endereco = nroEndereco,
                ico_num_filhos_menores = filhosmenores,
                ico_num_valor_pago = vPago,
                ico_bit_isento = isento,
                ico_des_link_boleto = linkBoleto
            };

            if (salvarBanco)
                ico = InscritoConcursoBusiness.Salvar(ico, idCargo);
            else
            {
                ico.tb_cid_cidade = new tb_cid_cidade()
                {
                    cid_idt_cidade = idCidade,
                    cid_nom_cidade = nomeCidade,
                    tb_est_estado = new tb_est_estado() { est_idt_estado = idEstado }
                };

                List<tb_cci_concurso_cargo_inscrito> ccis = new List<tb_cci_concurso_cargo_inscrito>() { new tb_cci_concurso_cargo_inscrito() { con_idt_concurso = idConcurso, cco_idt_cargo_concurso = idCargo, tb_cco_cargo_concurso = new tb_cco_cargo_concurso() { cco_idt_cargo_concurso = idCargo } } };
                
                ConcursoModel.InscritoModel inscrito = new ConcursoModel.InscritoModel(ico, ccis);
                inscrito.NomeConcurso = nomeConcurso;
                inscrito.NomeCargo = nomeCargo;
                inscrito.NomeCidade = nomeCidade;
                inscrito.NomeEstado = nomeEstado;
                inscrito.RGComEstado = String.Format("{0} - {1}", rg, estado_rg_string);
                inscrito.DataEncerramentoInscricaoString = concurso.DataEncerramentoInscricoesString;
                inscrito.DataVencimentoBoletoString = concurso.DataBoletoString;
                Session[SessionDadosInscrito] = new { InscritoModel = inscrito, tb_ico_inscrito_concurso = ico, tb_icv_inscrito_concurso_vestibular = new tb_icv_inscrito_concurso_vestibular(), IdTipoLayoutConcurso = 1 };
            }

            return Json(new { Sucesso = true, Mensagem = Resources.Shared._DadosInscrito.MensagemSucessoSalvar, EhVestibular = concurso.IdTipoLayoutConcurso == 2, Id = ico.ico_idt_inscrito_concurso }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SalvarVestibular(int id, int idConcurso, int? idOpcao2, int? idOpcao3, int localProva, bool ehMasculino, string indicadoPor, string cursoIndicadoPor, string semestreIndicadoPor, int? escolaridadeMae, int? escolaridadePai, int? exerceAtividadeRemunerada, int? motivoOptouCurso, int? motivoOptouUnifae, int? rendaMensal, int? tipoCEF, int? tipoCEM, int? tipoEM, int? tcnf, string tcnfOutros, bool salvarBanco)
        {
            tb_icv_inscrito_concurso_vestibular icv = new tb_icv_inscrito_concurso_vestibular()
            {
                ico_idt_inscrito_concurso = id,
                icv_bit_ativo = true,
                icv_bit_eh_masculino = ehMasculino,
                icv_dat_aceito_termos = DateTime.Now,
                icv_des_conhecimento_unifae_outros = tcnfOutros,
                icv_des_curso_indicado_por = cursoIndicadoPor,
                icv_des_nome_indicado_por = indicadoPor,
                icv_des_semestre_curso_indicado_por = semestreIndicadoPor,
                icv_idt_opcao_2 = idOpcao2,
                icv_idt_opcao_3 = idOpcao3,
                icv_num_atividade_remunerada = exerceAtividadeRemunerada,
                icv_num_conhecimento_unifae = tcnf,
                icv_num_data_prova = 0,
                icv_num_escolaridade_mae = escolaridadeMae,
                icv_num_escolaridade_pai = escolaridadePai,
                icv_num_local_prova = localProva,
                icv_num_optar_curso = motivoOptouCurso,
                icv_num_optar_unifae = motivoOptouUnifae,
                icv_num_renda_mensal = rendaMensal,
                icv_num_tipo_concluiu_ensino_fundamental = tipoCEF,
                icv_num_tipo_concluiu_ensino_medio = tipoCEM,
                icv_num_tipo_ensino_medio = tipoEM
            };

            if (salvarBanco)
                icv = InscritoConcursoBusiness.Salvar(icv);
            else
            {
                if (Session[SessionDadosInscrito] == null)
                    return Json(new { Sucesso = false, Mensagem = Resources.Shared._DadosInscrito.MensagemSucessoSalvar }, JsonRequestBehavior.AllowGet);

                dynamic dadosInscrito = Session[SessionDadosInscrito];
                tb_ico_inscrito_concurso ico = dadosInscrito.tb_ico_inscrito_concurso;
                ConcursoModel.InscritoModel inscrito = dadosInscrito.InscritoModel;

                inscrito.InscritoVestibular = new ConcursoModel.InscritoModel.InscritoVestibularModel()
                {
                    Ativo = true,
                    CursoIndicadoPor = cursoIndicadoPor,
                    DataAceitouOsTermos = DateTime.Now,
                    DataProva = 0,
                    EhMasculino = ehMasculino,
                    EscolaridadeMae = escolaridadeMae,
                    EscolaridadePai = escolaridadePai,
                    ExerceAtividadeRemunerada = exerceAtividadeRemunerada,
                    IdInscrito = id,
                    IdOpcao2 = idOpcao2,
                    IdOpcao3 = idOpcao3,
                    LocalProva = localProva,
                    MotivoOptouCurso = motivoOptouCurso,
                    MotivoOptouUniFae = motivoOptouUnifae,
                    NomeIndicadoPor = indicadoPor,
                    RendaMensal = rendaMensal,
                    SemestreCursoIndicadoPor = semestreIndicadoPor,
                    TipoConcluiuEnsinoFundamental = tipoCEF,
                    TipoConcluiuEnsinoMedio = tipoCEM,
                    TipoEnsinoMedio = tipoEM,
                    TomouConhecimentoUniFae = tcnf,
                    TomouConhecimentoUniFaeOutros = tcnfOutros
                };
     
                Session[SessionDadosInscrito] = new { InscritoModel = inscrito, tb_ico_inscrito_concurso = ico, tb_icv_inscrito_concurso_vestibular = icv, IdTipoLayoutConcurso = 2 };
            }

            return Json(new { Sucesso = true, Mensagem = Resources.Shared._DadosInscrito.MensagemSucessoSalvar }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ValidarCPFExistente(int id, int idConcurso, string cpf)
        {
            List<tb_ico_inscrito_concurso> inscritos = InscritoConcursoBusiness.Listar(idConcurso, cpf);
            bool existe = inscritos.Any(ico => ico.ico_idt_inscrito_concurso != id);

            return Json(new { Existe = existe }, JsonRequestBehavior.AllowGet);
        }
    }
}