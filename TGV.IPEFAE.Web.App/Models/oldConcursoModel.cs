using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using TGV.IPEFAE.Web.App.Controllers;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Models
{
    public class oldConcursoModel
    {
        public oldConcursoModel()
        {
            this.Id = Int32.MinValue;
            this.IdTipoLayoutConcurso = 1;
            this.Ativo = true;
            this.Encerrado = false;
            this.InscricoesOnline = true;
            this.Data = BaseBusiness.DataAgora;

            this.Empresa = new EmpresaModel();
            this.Anexos = new List<AnexoModel>();
            this.Cargos = new List<CargoModel>();
            this.Inscritos = new List<InscritoModel>();
            this.Recursos = new List<RecursoModel>();
        }

        public oldConcursoModel(tb_con_concurso concurso, bool noInscrito = false, bool noRecurso = false) : this()
        {
            if (concurso == null)
                return;

            this.Id = concurso.con_idt_concurso;
            this.IdTipoLayoutConcurso = concurso.tlc_idt_tipo_layout_concurso;
            this.Nome = concurso.con_nom_concurso;
            this.Data = concurso.con_dat_concurso;
            this.DataEncerramentoInscricoes = concurso.con_dat_encerramento_inscricoes;
            this.DataEncerramento = concurso.con_dat_encerramento;
            this.DataInicioComprovante = concurso.con_dat_inicio_comprovante;
            this.DataEncerramentoComprovante = concurso.con_dat_encerramento_comprovante;
            this.DataInicioIsento = concurso.con_dat_inicio_isento;
            this.DataEncerramentoIsento = concurso.con_dat_encerramento_isento;
            this.DataInicioClassificacao = concurso.con_dat_inicio_classificacao;
            this.DataBoleto = concurso.con_dat_boleto;
            this.InscricoesOnline = concurso.con_bit_inscricao_online;
            this.Ativo = concurso.con_bit_ativo;
            this.Encerrado = concurso.con_bit_encerrado;
            this.QuantidadeInscritos = concurso.TotalInscritos;
            this.QuantidadePagos = concurso.TotalPagos;

            try { this.Empresa = new EmpresaModel(concurso.tb_emp_empresa); }  catch { };
            try { this.Anexos = concurso.tb_can_concurso_anexo.ToList().ConvertAll(can => new AnexoModel(can)); } catch { };
            try { this.Cargos = concurso.tb_cco_cargo_concurso.OrderBy(car => car.cco_nom_cargo_concurso).ToList().ConvertAll(cco => new CargoModel(cco)); } catch { };

            if (!noInscrito)
                try { this.Inscritos = concurso.tb_ico_inscrito_concurso.ToList().ConvertAll(ico => new InscritoModel(ico)); } catch { };

            if (!noRecurso)
                try { this.Recursos = concurso.tb_rec_recurso.ToList().ConvertAll(rec => new RecursoModel(rec)); } catch { };
        }

        #region [ Propriedades ]

        public int Id                       { get; set; }
        public int IdTipoLayoutConcurso     { get; set; }
        public string Nome                  { get; set; }
        public bool InscricoesOnline        { get; set; }
        public DateTime Data                { get; set; }
        public DateTime DataEncerramentoInscricoes  { get; set; }
        public DateTime? DataEncerramento           { get; set; }
        public DateTime DataInicioComprovante       { get; set; }
        public DateTime DataEncerramentoComprovante { get; set; }
        public DateTime DataInicioClassificacao     { get; set; }
        public DateTime DataInicioIsento            { get; set; }
        public DateTime DataEncerramentoIsento      { get; set; }
        public DateTime DataBoleto                  { get; set; }
        public bool Encerrado                       { get; set; }
        public bool Ativo                           { get; set; }

        public int QuantidadeCargos                 { get { return Cargos.Count(cco => cco.Ativo); } }
        public int QuantidadeInscritos              { get; set; }
        public int QuantidadePagos                  { get; set; }

        public EmpresaModel Empresa             { get; set; }
        public List<AnexoModel> Anexos          { get; set; }
        public List<CargoModel> Cargos          { get; set; }
        public List<InscritoModel> Inscritos    { get; set; }
        public List<RecursoModel> Recursos      { get; set; }

        public string DataString                        { get { return Data.ToString("dd/MM/yyyy HH:mm"); } }
        public string DataEncerramentoInscricoesString  { get { return DataEncerramentoInscricoes.ToString("dd/MM/yyyy HH:mm"); } }
        public string DataEncerramentoString            { get { return DataEncerramento.HasValue ? DataEncerramento.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty; } }
        public string NomeSemEspaco                     { get { return !String.IsNullOrEmpty(this.Nome) ? this.Nome.RemoveSpecialCharacters() : String.Empty; } }

        public string DataInicioComprovanteString       { get { return DataInicioComprovante.ToString("dd/MM/yyyy HH:mm"); } }
        public string DataEncerramentoComprovanteString { get { return DataEncerramentoComprovante.ToString("dd/MM/yyyy HH:mm"); } }
        public string DataInicioClassificacaoString     { get { return DataInicioClassificacao.ToString("dd/MM/yyyy HH:mm"); } }

        public string DataInicioIsentoString            { get { return DataInicioIsento.ToString("dd/MM/yyyy HH:mm"); } }
        public string DataEncerramentoIsentoString      { get { return DataEncerramentoIsento.ToString("dd/MM/yyyy HH:mm"); } }

        public string DataBoletoString                  { get { return DataBoleto.ToString("dd/MM/yyyy HH:mm"); } }

        public string DataInicioRecursoAtual
        {
            get
            {
                if (!ExibirAlertaRecurso)
                    return String.Empty;

                var anes = Anexos.Where(a => a.DataInicioRecurso <= BaseBusiness.DataAgora && a.DataFimRecurso >= BaseBusiness.DataAgora);

                if (anes.Count() == 0)
                    return this.DataInicioIsento.ToString("dd/MM/yyyy HH:mm");

                return anes.Min(a => a.DataInicioRecurso).Value.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public string DataFimRecursoAtual
        {
            get
            {
                if (!ExibirAlertaRecurso)
                    return String.Empty;

                var anes = Anexos.Where(a => a.DataInicioRecurso <= BaseBusiness.DataAgora && a.DataFimRecurso >= BaseBusiness.DataAgora);

                if (anes.Count() == 0)
                    return this.DataEncerramentoIsento.ToString("dd/MM/yyyy HH:mm");

                return anes.Max(a => a.DataFimRecurso).Value.ToString("dd/MM/yyyy HH:mm");
            }
        }

        public bool ExibirAlertaRecurso
        {
            get
            {
                bool dentroPeriodoIsencao = this.DataInicioIsento <= BaseBusiness.DataAgora && this.DataEncerramentoIsento >= BaseBusiness.DataAgora;

                return dentroPeriodoIsencao
                    || Anexos.Count(a => a.DataInicioRecurso <= BaseBusiness.DataAgora && a.DataFimRecurso >= BaseBusiness.DataAgora) > 0;
            }
        }

        public bool ExibirAlertaIsencao { get { return this.DataInicioIsento <= BaseBusiness.DataAgora && this.DataEncerramentoIsento >= BaseBusiness.DataAgora; } }

        public bool PeriodoInscricoesEstaAberto
        {
            get
            {
                if (this.Id <= 0)
                    return false;

                return (this.Data <= BaseBusiness.DataAgora && this.DataEncerramentoInscricoes >= BaseBusiness.DataAgora);
            }
        }

        public string BancoCss                          { get { return Empresa.NumeroBanco == 1 ? "banco-brasil" : "bradesco"; } }

        #endregion [ FIM - Propriedades ]

        #region [ Classes ]

        public class AnexoModel
        {
            public AnexoModel()
            {
                this.Id = 0;
                this.IdConcurso = 0;
                this.IdTipo = 0;
                this.DataPublicacao = DateTime.MaxValue;
                this.TemRecurso = false;
            }

            public AnexoModel(int idConcurso, int idTipo, DateTime dataPublicacao) : this()
            {
                this.IdConcurso = idConcurso;
                this.IdTipo = idTipo;
                this.DataPublicacao = dataPublicacao;
            }

            public AnexoModel(tb_can_concurso_anexo anexo) : this()
            {
                this.Id = anexo.can_idt_concurso_anexo;
                this.IdConcurso = anexo.con_idt_concurso;
                this.IdTipo = anexo.tca_idt_tipo_concurso_anexo;
                this.DataPublicacao = anexo.can_dat_publicacao;
                this.TemRecurso = anexo.can_bit_tem_recurso;
                this.DataInicioRecurso = anexo.can_dat_inicio_recurso;
                this.DataFimRecurso = anexo.can_dat_fim_recurso;
                this.NomeArquivo = anexo.can_nom_concurso_anexo;
                this.NomeOriginalArquivo = anexo.can_des_path_arquivo;
                this.Ativo = anexo.can_bit_ativo;
            }

            public enum TipoEnum
            {
                [Description("Indefinido")]
                Indefinido = 0,
                [Description("Publicações")]
                Publicacoes = 1,
                [Description("Provas")]
                Provas = 2,
                [Description("Gabaritos")]
                Gabaritos = 3
            }

            public int Id                           { get; set; }
            public int IdConcurso                   { get; set; }
            public int IdTipo                       { get; set; }
            public DateTime DataPublicacao          { get; set; }
            public bool TemRecurso                  { get; set; }
            public DateTime? DataInicioRecurso      { get; set; }
            public DateTime? DataFimRecurso         { get; set; }
            public string NomeArquivo               { get; set; }
            public string NomeOriginalArquivo       { get; set; }
            public bool Ativo                       { get; set; }

            public string DataPublicacaoString      { get { return this.DataPublicacao > DateTime.MinValue ? this.DataPublicacao.ToString("dd/MM/yyyy HH:mm") : String.Empty; } }
            public string DataInicioRecursoString   { get { return this.DataInicioRecurso.HasValue ? this.DataInicioRecurso.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty; } }
            public string DataFimRecursoString      { get { return this.DataFimRecurso.HasValue ? this.DataFimRecurso.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty; } }

            public string PathArquivo               { get { return Path.Combine(HttpContext.Current.Server.MapPath(String.Format("~/Anexos/Concurso/{0}", this.IdConcurso)), this.NomeOriginalArquivo); } }
            public string TipoArquivo               { get { return ((TipoEnum)this.IdTipo).GetDescription(); } }
        }

        public class CargoModel
        {
            public CargoModel()
            {
                this.Id = 0;
                this.IdConcurso = 0;
                this.TemInscritos = false;
            }

            public CargoModel(tb_cco_cargo_concurso cargo) : this()
            {
                if (cargo == null)
                    return;

                this.Id = cargo.cco_idt_cargo_concurso;
                this.IdConcurso = cargo.con_idt_concurso;
                this.Nome = cargo.cco_nom_cargo_concurso;
                this.ValorInscricao = cargo.cco_num_valor_inscricao;
                this.Ativo = cargo.cco_bit_ativo;

                try { this.TemInscritos = cargo.tb_cci_concurso_cargo_inscrito.Count > 0; } catch { };
            }

            public int Id                   { get; set; }
            public int IdConcurso           { get; set; }
            public string Nome              { get; set; }
            public decimal? ValorInscricao  { get; set; }
            public bool Ativo               { get; set; }
            public bool TemInscritos        { get; set; }
            
            public string ValorInscricaoString { get { return this.ValorInscricao.HasValue ? String.Format("{0:C}", this.ValorInscricao) : String.Empty; } }
        }

        public class EmpresaModel
        {
            public EmpresaModel()
            { }

            public EmpresaModel(tb_emp_empresa empresa) : this()
            {
                if (empresa == null)
                    return;

                this.Id = empresa.emp_idt_empresa;
                this.Nome = empresa.emp_nom_empresa;
                this.NumeroBanco = empresa.emp_num_banco;
                this.Convenio = empresa.emp_num_convenio;
                this.ConvenioCobranca = empresa.emp_num_convenio_cobranca;
                this.CNPJ = empresa.emp_des_cnpj;
                this.RazaoSocial = empresa.emp_des_razao_social;
                this.Agencia = empresa.emp_des_agencia;
                this.ContaCorrente = empresa.emp_des_conta_corrente;
                this.Ativo = empresa.emp_bit_ativo;
            }

            public int Id               { get; set; }
            public string Nome          { get; set; }
            public Int16 NumeroBanco    { get; set; }
            public int Convenio         { get; set; }
            public int? ConvenioCobranca { get; set; }
            public string CNPJ          { get; set; }
            public string RazaoSocial   { get; set; }
            public string Agencia       { get; set; }
            public string ContaCorrente { get; set; }
            public bool Ativo           { get; set; }

            public string AgenciaConta  { get { return String.Format("{0} / {1}", this.Agencia, this.ContaCorrenteDigito); } }
            public string Banco         { get { return this.NumeroBanco == 1 ? String.Format("Banco do Brasil - {0}", this.NumeroBanco.ToString().PadLeft(3, '0')) : String.Format("Bradesco - {0}", this.NumeroBanco.ToString().PadLeft(3, '0')); } }
            public string CNPJFormatado { get { return String.IsNullOrEmpty(this.CNPJ) ? String.Empty : String.Format(@"{0:00\.000\.000\\0000\-00}", Int64.Parse(this.CNPJ)); } }

            private string ContaCorrenteDigito { get { return String.IsNullOrEmpty(this.ContaCorrente) ? String.Empty : String.Format("{0}-{1}", this.ContaCorrente.Substring(0, this.ContaCorrente.Length - 1), this.ContaCorrente.Substring(this.ContaCorrente.Length - 1)); } }
        }

        public class InscritoModel
        {
            public InscritoModel()
            {
                this.Id = Int32.MinValue;
                this.IdConcurso = 0;
                this.IdCidade = 0;
                this.IdEstadoRG = 0;
                this.DataNascimento = BaseBusiness.DataAgora;
                this.DataInscricao = BaseBusiness.DataAgora;
                this.QuantidadeFilhosMenores = 0;
                this.Isento = false;
                this.Pagou = false;
                this.Ativo = true;

                this.Prova = new ProvaModel();
                this.Cidade = new CidadeModel();
                this.EstadoRG = new EstadoModel();
                this.Cargos = new List<CargoModel>();
                this.InscritoVestibular = new InscritoVestibularModel();
            }

            public InscritoModel(tb_ico_inscrito_concurso inscrito, bool noRecurso = false) : this()
            {
                if (inscrito == null)
                    return;

                this.Id = inscrito.ico_idt_inscrito_concurso;
                this.IdConcurso = inscrito.con_idt_concurso;
                this.IdCidade = inscrito.cid_idt_cidade;
                this.Nome = inscrito.ico_nom_inscrito_concurso;
                this.DataNascimento = inscrito.ico_dat_nascimento;
                this.DataInscricao = inscrito.ico_dat_inscricao;
                this.DataPagamento = inscrito.ico_dat_pagamento;
                this.CPF = inscrito.ico_des_cpf;
                this.RG = inscrito.ico_des_rg;
                this.IdEstadoRG = inscrito.est_idt_estado_rg;
                this.Endereco = inscrito.ico_des_endereco;
                this.NumeroEndereco = inscrito.ico_des_nro_endereco;
                this.Complemento = inscrito.ico_des_complemento;
                this.Bairro = inscrito.ico_des_bairro;
                this.CEP = inscrito.ico_des_cep;
                this.Telefone = inscrito.ico_des_telefone;
                this.Celular = inscrito.ico_des_celular;
                this.Email = inscrito.ico_des_email;
                this.FlgDeficiencia = inscrito.ico_flg_deficiencia;
                this.FlgEstadoCivil = inscrito.ico_flg_estado_civil;
                this.OutrasSolicitacoes = inscrito.ico_des_outras_solicitacoes;
                this.LinkBoleto = inscrito.ico_des_link_boleto;
                this.BrowserCadastro = inscrito.ico_des_browser_cadastro;
                this.QuantidadeFilhosMenores = inscrito.ico_num_filhos_menores;
                this.ValorPago = inscrito.ico_num_valor_pago;
                this.TratamentoEspecialQual = inscrito.ico_des_tratamento_especial_qual;
                this.Isento = inscrito.ico_bit_isento;
                this.Pagou = inscrito.ico_bit_pago;
                this.Destro = inscrito.ico_bit_destro;
                this.PossuiDeficiencia = inscrito.ico_bit_possui_deficiencia;
                this.TratamentoEspecial = inscrito.ico_bit_tratamento_especial;
                this.Ativo = inscrito.ico_bit_ativo;

                try
                {
                    if (inscrito.tb_con_concurso != null && inscrito.tb_con_concurso.con_idt_concurso > 0)
                    {
                        oldConcursoModel concurso = new oldConcursoModel(inscrito.tb_con_concurso, true, noRecurso);
                        this.BancoCss = concurso.BancoCss;
                        this.DataEncerramentoInscricao = concurso.DataEncerramentoInscricoes;
                        this.DataVencimentoBoletoString = concurso.DataBoletoString;
                        this.NomeConcurso = concurso.Nome;
                    }
                }
                catch { }

                try { this.InscritoVestibular = new InscritoVestibularModel(inscrito.tb_icv_inscrito_concurso_vestibular); } catch { };
                try { this.Cidade = new CidadeModel(inscrito.tb_cid_cidade); } catch { };
                try { this.EstadoRG = new EstadoModel(inscrito.tb_est_estado); } catch { };
                try { this.Cargos = inscrito.tb_cci_concurso_cargo_inscrito.ToList().ConvertAll(cci => new CargoModel(cci.tb_cco_cargo_concurso)); } catch { };
            }

            public InscritoModel(tb_ico_inscrito_concurso_extension inscrito, bool noRecurso, bool viaQuery) : this(inscrito, noRecurso)
            {
                if (inscrito == null)
                    return;

                try
                {
                    this.Cidade = new CidadeModel()
                    {
                        Id = inscrito.cid_idt_cidade,
                        Nome = inscrito.cid_nom_cidade,
                        Estado = new EstadoModel()
                        {
                            Id = inscrito.est_idt_estado,
                            Nome = inscrito.est_nom_estado,
                            Sigla = inscrito.est_sig_estado
                        }
                    };
                }
                catch { };

                try
                {
                    this.Cargos = new List<CargoModel>()
                    {
                        new CargoModel()
                        {
                            Id = inscrito.cco_idt_cargo_concurso,
                            IdConcurso = inscrito.con_idt_concurso,
                            Nome = inscrito.cco_nom_cargo_concurso,
                            ValorInscricao = inscrito.cco_num_valor_inscricao,
                            Ativo = inscrito.cco_bit_ativo
                        }
                    };
                }
                catch { };
            }

            public InscritoModel(tb_ico_inscrito_concurso inscrito, List<tb_cci_concurso_cargo_inscrito> ccis) : this(inscrito)
            {
                this.Cargos = ccis.ConvertAll(cci => new CargoModel(cci.tb_cco_cargo_concurso));
            }

            public InscritoModel(spr_tgv_gerar_lista_inscritos_Result inscrito) : this()
            {
                if (inscrito == null)
                    return;

                this.Id = inscrito.ico_idt_inscrito_concurso;
                this.IdConcurso = inscrito.con_idt_concurso;
                this.IdCidade = inscrito.cid_idt_cidade;
                this.Nome = inscrito.ico_nom_inscrito_concurso;
                this.DataNascimento = inscrito.ico_dat_nascimento;
                this.DataInscricao = inscrito.ico_dat_inscricao;
                this.DataPagamento = inscrito.ico_dat_pagamento;
                this.CPF = inscrito.ico_des_cpf;
                this.RG = inscrito.ico_des_rg;
                this.IdEstadoRG = inscrito.est_idt_estado_rg;
                this.SiglaEstadoCSV = inscrito.est_sig_estado_rg;
                this.Endereco = inscrito.ico_des_endereco;
                this.NumeroEndereco = inscrito.ico_des_nro_endereco;
                this.Complemento = inscrito.ico_des_complemento;
                this.Bairro = inscrito.ico_des_bairro;
                this.CEP = inscrito.ico_des_cep;
                this.Telefone = inscrito.ico_des_telefone;
                this.Celular = inscrito.ico_des_celular;
                this.Email = inscrito.ico_des_email;
                this.FlgDeficiencia = inscrito.ico_flg_deficiencia;
                this.FlgEstadoCivil = inscrito.ico_flg_estado_civil;
                this.OutrasSolicitacoes = inscrito.ico_des_outras_solicitacoes;
                this.QuantidadeFilhosMenores = inscrito.ico_num_filhos_menores;
                this.ValorPago = inscrito.ico_num_valor_pago;
                this.TratamentoEspecialQual = inscrito.ico_des_tratamento_especial_qual;
                this.Isento = inscrito.ico_bit_isento;
                this.Pagou = inscrito.ico_bit_pago;
                this.Destro = inscrito.ico_bit_destro;
                this.PossuiDeficiencia = inscrito.ico_bit_possui_deficiencia;
                this.TratamentoEspecial = inscrito.ico_bit_tratamento_especial;
                this.Ativo = inscrito.ico_bit_ativo;

                this.Prova = new ProvaModel();
                this.Prova.Andar = inscrito.idt_des_andar;
                this.Prova.Bairro = inscrito.idt_des_bairro;
                this.Prova.CEP = inscrito.idt_des_cep;
                this.Prova.Cidade = inscrito.idt_des_cidade;
                this.Prova.DataString = inscrito.idt_dat_prova?.ToString("dd/MM/yyyy");
                this.Prova.HoraString = inscrito.idt_dat_prova?.ToString("HH:mm");
                this.Prova.Local = inscrito.idt_des_local;
                this.Prova.Logradouro = inscrito.idt_des_endereco;
                this.Prova.Sala = inscrito.idt_des_sala;

                this.Classificacao = new ClassificacaoModel();
                this.Classificacao.CE = inscrito.icl_num_ce;
                this.Classificacao.CG = inscrito.icl_num_cg;
                this.Classificacao.Nota = inscrito.icl_num_nota;
                this.Classificacao.Posicao = inscrito.icl_num_posicao;
                this.Classificacao.PP = inscrito.icl_num_pp;
                this.Classificacao.Situacao = inscrito.icl_des_situacao;
                this.Classificacao.TIT = inscrito.icl_num_tit;

                this.ResultInscrito = inscrito;

                try { this.InscritoVestibular = new InscritoVestibularModel(inscrito); } catch { };
            }

            #region [ Propriedades ]

            public int Id                       { get; set; }
            public string NroMatricula          { get { return this.Id.ToString().PadLeft(10, '0'); } }
            public int IdConcurso               { get; set; }
            public int IdCargo                  { get { return Cargos.Count > 0 ? Cargos[0].Id : 0; } }
            public int IdEstado                 { get { return Cidade != null && Cidade.Estado != null && Cidade.Estado.Id > 0 ? Cidade.Estado.Id : 0; } }
            public int IdCidade                 { get; set; }
            public string Nome                  { get; set; }
            public DateTime DataNascimento      { get; set; }
            public DateTime DataInscricao       { get; set; }
            public DateTime? DataPagamento      { get; set; }
            public string CPF                   { get; set; }
            public string RG                    { get; set; }
            public int IdEstadoRG               { get; set; }
            public string Endereco              { get; set; }
            public string NumeroEndereco        { get; set; }
            public string Complemento           { get; set; }
            public string Bairro                { get; set; }
            public string CEP                   { get; set; }
            public string Telefone              { get; set; }
            public string Celular               { get; set; }
            public string Email                 { get; set; }
            public string FlgDeficiencia        { get; set; }
            public string FlgEstadoCivil        { get; set; }
            public string OutrasSolicitacoes    { get; set; }
            public string LinkBoleto            { get; set; }
            public string BrowserCadastro       { get; set; }
            public int QuantidadeFilhosMenores  { get; set; }
            public decimal? ValorPago           { get; set; }
            public string TratamentoEspecialQual { get; set; }

            public bool Pagou                   { get; set; }
            public bool Destro                  { get; set; }
            public bool Isento                  { get; set; }
            public bool PossuiDeficiencia       { get; set; }
            public bool TratamentoEspecial      { get; set; }
            public bool Ativo                   { get; set; }

            public ProvaModel Prova             { get; set; }
            public ClassificacaoModel Classificacao { get; set; }

            public CidadeModel Cidade           { get; set; }
            public EstadoModel EstadoRG         { get; set; }
            public List<CargoModel> Cargos      { get; set; }
            public InscritoVestibularModel InscritoVestibular { get; set; }

            public string CPFFormatado          { get { return String.IsNullOrEmpty(CPF) ? String.Empty : String.Format(@"{0:000\.000\.000\-00}", Int64.Parse(CPF)); } }
            public string ValorPagoString       { get { return this.ValorPago.HasValue ? String.Format("{0:C}", this.ValorPago.Value) : String.Empty; } }
            public string DataNascimentoString  { get { return DataNascimento.ToString("dd/MM/yyyy"); } }
            public string SiglaEstado           { get { return this.Cidade != null && this.Cidade.Estado != null ? this.Cidade.Estado.Sigla : String.Empty; } }
            public string SiglaEstadoCSV        { get; set; }
            public string NumeroEnderecoString  { get { return this.NumeroEndereco; } }
            public string NossoNumero           { get { return this.Id.ToString().PadLeft(10, '0'); } }
            public string StatusPagamento       { get { return this.Isento ? Resources.Concurso._ListaCargosInscrito.StatusIsento : (this.Pagou ? Resources.Concurso._ListaCargosInscrito.StatusPago : Resources.Concurso._ListaCargosInscrito.StatusAguardando); } }
            public string BancoCss              { get; set; } = String.Empty;

            public string CargosString
            {
                get
                {
                    if (this.Cargos.Count == 0)
                        return String.Empty;

                    string nomeCargo = String.Empty;
                    string separador = " | ";

                    foreach (CargoModel cargo in this.Cargos)
                        nomeCargo += String.Format("{0}{1}", cargo.Nome, separador);

                    if (nomeCargo.Length > 0)
                        nomeCargo = nomeCargo.Substring(0, nomeCargo.LastIndexOf(separador));

                    return nomeCargo;
                }
            }

            public string ValorInscricaoCargoString
            {
                get
                {
                    if (this.Cargos.Count == 0)
                        return String.Empty;

                    string valorCargo = String.Empty;

                    CargoModel cargo = this.Cargos.FirstOrDefault();

                    if (cargo == null)
                        return String.Empty;

                    return String.IsNullOrEmpty(cargo.ValorInscricaoString) ? Resources.Shared.Geral.ValorInscricaoIsento : cargo.ValorInscricaoString;
                }
            }

            public string EstadoCivil
            {
                get
                {
                    string ec = String.IsNullOrEmpty(this.FlgEstadoCivil) ? String.Empty : this.FlgEstadoCivil.ToUpper();

                    switch (ec)
                    {
                        case "C":
                            return Resources.Shared._DadosInscrito.OptionCasado;
                        case "D":
                            return Resources.Shared._DadosInscrito.OptionDivorciado;
                        case "S":
                            return Resources.Shared._DadosInscrito.OptionSolteiro;
                        case "V":
                            return Resources.Shared._DadosInscrito.OptionViuvo;
                        case "O":
                            return Resources.Shared._DadosInscrito.OptionOutros;
                        default:
                            return String.Empty;
                    }
                }
            }

            public string Deficiencia
            {
                get
                {
                    string def = String.IsNullOrEmpty(this.FlgDeficiencia) ? String.Empty : this.FlgDeficiencia.ToUpper();

                    switch (def)
                    {
                        case "F":
                            return Resources.Shared._DadosInscrito.OptionFisica;
                        case "A":
                            return Resources.Shared._DadosInscrito.OptionAuditiva;
                        case "V":
                            return Resources.Shared._DadosInscrito.OptionVisual;
                        case "M":
                            return Resources.Shared._DadosInscrito.OptionMental;
                        case "O":
                            return Resources.Shared._DadosInscrito.OptionMultipla;
                        default:
                            return String.Empty;
                    }
                }
            }

            public string NomeConcurso  { get; set; }
            public string NomeCargo     { get; set; }
            public string RGComEstado   { get; set; }
            public string NomeCidade    { get; set; }
            public string NomeEstado    { get; set; }
            public DateTime DataEncerramentoInscricao       { get; set; }
            public string DataEncerramentoInscricaoString   { get; set; }
            public string DataVencimentoBoletoString        { get; set; }
            public int IdTipoLayoutConcurso     { get; set; }
            public bool InscricoesEncerradas    { get { return this.DataEncerramentoInscricao < BaseBusiness.DataAgora; } }

            public spr_tgv_gerar_lista_inscritos_Result ResultInscrito { get; set; }

            public bool IsStatusPagamento   { get; set; } = false;

            #endregion [ FIM - Propriedades ]

            public class InscritoVestibularModel
            {
                public InscritoVestibularModel()
                {
                    this.IdInscrito = 0;
                    this.LocalProva = 0;
                    this.DataProva = 0;
                    this.RendaMensal = 0;
                    this.TipoEnsinoMedio = 0;
                    this.TipoConcluiuEnsinoFundamental = 0;
                    this.TipoConcluiuEnsinoMedio = 0;
                    this.ExerceAtividadeRemunerada = 0;
                    this.EscolaridadeMae = 0;
                    this.EscolaridadePai = 0;
                    this.MotivoOptouUniFae = 0;
                    this.MotivoOptouCurso = 0;
                    this.TomouConhecimentoUniFae = 0;
                    this.DataAceitouOsTermos = BaseBusiness.DataAgora;
                    this.Ativo = true;
                }

                public InscritoVestibularModel(tb_icv_inscrito_concurso_vestibular inscrito) : this()
                {
                    if (inscrito == null)
                        return;

                    this.IdInscrito = inscrito.ico_idt_inscrito_concurso;
                    this.IdOpcao2 = inscrito.icv_idt_opcao_2;
                    this.IdOpcao3 = inscrito.icv_idt_opcao_3;
                    this.LocalProva = inscrito.icv_num_local_prova;
                    this.DataProva = inscrito.icv_num_data_prova;
                    this.RendaMensal = inscrito.icv_num_renda_mensal;
                    this.TipoEnsinoMedio = inscrito.icv_num_tipo_ensino_medio;
                    this.TipoConcluiuEnsinoFundamental = inscrito.icv_num_tipo_concluiu_ensino_fundamental;
                    this.TipoConcluiuEnsinoMedio = inscrito.icv_num_tipo_concluiu_ensino_medio;
                    this.ExerceAtividadeRemunerada = inscrito.icv_num_atividade_remunerada;
                    this.EscolaridadeMae = inscrito.icv_num_escolaridade_mae;
                    this.EscolaridadePai = inscrito.icv_num_escolaridade_pai;
                    this.MotivoOptouUniFae = inscrito.icv_num_optar_unifae;
                    this.MotivoOptouCurso = inscrito.icv_num_optar_curso;
                    this.TomouConhecimentoUniFae = inscrito.icv_num_conhecimento_unifae;
                    this.DataAceitouOsTermos = inscrito.icv_dat_aceito_termos;
                    this.EhMasculino = inscrito.icv_bit_eh_masculino;
                    this.Ativo = inscrito.icv_bit_ativo;

                    this.NomeIndicadoPor = inscrito.icv_des_nome_indicado_por;
                    this.CursoIndicadoPor = inscrito.icv_des_curso_indicado_por;
                    this.SemestreCursoIndicadoPor = inscrito.icv_des_semestre_curso_indicado_por;
                    this.TomouConhecimentoUniFaeOutros = inscrito.icv_des_conhecimento_unifae_outros;
                }

                public InscritoVestibularModel(spr_tgv_gerar_lista_inscritos_Result inscrito) : this()
                {
                    this.IdInscrito = inscrito.ico_idt_inscrito_concurso;
                    this.IdOpcao2 = inscrito.icv_idt_opcao_2;
                    this.IdOpcao3 = inscrito.icv_idt_opcao_3;
                    this.LocalProva = inscrito.icv_num_local_prova.HasValue ? inscrito.icv_num_local_prova.Value : 0;
                    this.DataProva = inscrito.icv_num_data_prova.HasValue ? inscrito.icv_num_data_prova.Value : 0;
                    this.RendaMensal = inscrito.icv_num_renda_mensal;
                    this.TipoEnsinoMedio = inscrito.icv_num_tipo_ensino_medio;
                    this.TipoConcluiuEnsinoFundamental = inscrito.icv_num_tipo_concluiu_ensino_fundamental;
                    this.TipoConcluiuEnsinoMedio = inscrito.icv_num_tipo_concluiu_ensino_medio;
                    this.ExerceAtividadeRemunerada = inscrito.icv_num_atividade_remunerada;
                    this.EscolaridadeMae = inscrito.icv_num_escolaridade_mae;
                    this.EscolaridadePai = inscrito.icv_num_escolaridade_pai;
                    this.MotivoOptouUniFae = inscrito.icv_num_optar_unifae;
                    this.MotivoOptouCurso = inscrito.icv_num_optar_curso;
                    this.TomouConhecimentoUniFae = inscrito.icv_num_conhecimento_unifae;
                    this.DataAceitouOsTermos = inscrito.icv_dat_aceito_termos.HasValue ? inscrito.icv_dat_aceito_termos.Value : BaseController.DataAgora;
                    this.EhMasculino = inscrito.icv_bit_eh_masculino.HasValue ? inscrito.icv_bit_eh_masculino.Value : true;
                    this.Ativo = inscrito.icv_bit_ativo.HasValue ? inscrito.icv_bit_ativo.Value : true;

                    this.NomeIndicadoPor = inscrito.icv_des_nome_indicado_por;
                    this.CursoIndicadoPor = inscrito.icv_des_curso_indicado_por;
                    this.SemestreCursoIndicadoPor = inscrito.icv_des_semestre_curso_indicado_por;
                    this.TomouConhecimentoUniFaeOutros = inscrito.icv_des_conhecimento_unifae_outros;
                }

                public int IdInscrito                       { get; set; }
                public int? IdOpcao2                        { get; set; }
                public int? IdOpcao3                        { get; set; }
                public int LocalProva                       { get; set; }
                public int DataProva                        { get; set; }
                public string NomeIndicadoPor               { get; set; }
                public string CursoIndicadoPor              { get; set; }
                public string SemestreCursoIndicadoPor      { get; set; }
                public int? RendaMensal                     { get; set; }
                public int? TipoEnsinoMedio                 { get; set; }
                public int? TipoConcluiuEnsinoMedio         { get; set; }
                public int? TipoConcluiuEnsinoFundamental   { get; set; }
                public int? ExerceAtividadeRemunerada       { get; set; }
                public int? EscolaridadePai                 { get; set; }
                public int? EscolaridadeMae                 { get; set; }
                public int? MotivoOptouUniFae               { get; set; }
                public int? MotivoOptouCurso                { get; set; }
                public int? TomouConhecimentoUniFae         { get; set; }
                public string TomouConhecimentoUniFaeOutros { get; set; }
                public DateTime DataAceitouOsTermos         { get; set; }
                public bool EhMasculino                     { get; set; }
                public bool Ativo                           { get; set; }
            }

            public class ProvaModel
            {
                public ProvaModel() { }

                public ProvaModel(tb_idt_inscrito_dados_prova dados)
                {
                    if (dados == null)
                        return;

                    this.DataString = dados.idt_dat_prova.HasValue ? dados.idt_dat_prova.Value.ToString("dd/MM/yyyy") : String.Empty;
                    this.HoraString = dados.idt_dat_prova.HasValue ? dados.idt_dat_prova.Value.ToString("HH:mm") : String.Empty;
                    this.Local = dados.idt_des_local;
                    this.Sala = dados.idt_des_sala;
                    this.Andar = dados.idt_des_andar;
                    this.Logradouro = String.IsNullOrEmpty(dados.idt_des_endereco) ? String.Empty : String.Format("{0}{1}", dados.idt_des_endereco, String.IsNullOrEmpty(dados.idt_des_numero) ? String.Empty : String.Format(", {0}", dados.idt_des_numero));
                    this.Bairro = dados.idt_des_bairro;
                    this.Cidade = dados.idt_des_cidade;

                    int cep = 0;
                    
                    if (Int32.TryParse(dados.idt_des_cep, out cep))
                        this.CEP = String.Format(@"{0:00000\-000}", cep);
                }

                public string DataString    { get; set; }
                public string Local         { get; set; }
                public string Sala          { get; set; }
                public string HoraString    { get; set; }
                public string Andar         { get; set; }
                public string Logradouro    { get; set; }
                public string Bairro        { get; set; }
                public string CEP           { get; set; }
                public string Cidade        { get; set; }
            }

            public class ClassificacaoModel
            {
                public ClassificacaoModel() { }

                public ClassificacaoModel(tb_icl_inscrito_classificacao classificacao)
                {
                    if (classificacao == null)
                        return;

                    this.Posicao = classificacao.icl_num_posicao;
                    this.Nota = classificacao.icl_num_nota;
                    this.CG = classificacao.icl_num_cg;
                    this.CE = classificacao.icl_num_ce;
                    this.TIT = classificacao.icl_num_tit;
                    this.PP = classificacao.icl_num_pp;
                    this.Situacao = classificacao.icl_des_situacao;
                }

                public int? Posicao     { get; set; }
                public decimal? Nota    { get; set; }
                public decimal? CG      { get; set; }
                public decimal? CE      { get; set; }
                public decimal? TIT     { get; set; }
                public decimal? PP      { get; set; }
                public string Situacao  { get; set; }

                //public string PosicaoString { get { return this.Posicao.HasValue ? String.Format("{0}º", this.Posicao.Value) : "-"; } }
                public string PosicaoString { get { return this.Posicao.HasValue ? this.Posicao.Value.ToString() : "-"; } }
                public string NotaString    { get { return this.Nota.HasValue ? this.Nota.Value.ToString() : "-"; } }
                public string CGString      { get { return this.CG.HasValue ? this.CG.Value.ToString() : "-"; } }
                public string CEString      { get { return this.CE.HasValue ? this.CE.Value.ToString() : "-"; } }
                public string TITString     { get { return this.TIT.HasValue ? this.TIT.Value.ToString() : "-"; } }
                public string PPString      { get { return this.PP.HasValue ? this.PP.Value.ToString() : "-"; } }
            }
        }

        public class InscritoCSVModel
        {
            public InscritoCSVModel(InscritoModel inscrito, oldConcursoModel concurso)
            {
                this.ConcursoDescricao = concurso.Nome.ToUpper();
                this.CargoDescricao = inscrito.ResultInscrito.cco_nom_cargo_concurso;
                this.ValorInscricao = inscrito.ResultInscrito.cco_num_valor_inscricao.HasValue ? String.Format("{0:C}", inscrito.ResultInscrito.cco_num_valor_inscricao.Value) : String.Empty;
                this.Data = inscrito.DataInscricao.ToString("dd/MM/yyyy");
                this.Hora = inscrito.DataInscricao.ToString("HH:mm:ss");
                this.ValorPago = inscrito.Isento ? Resources.Admin.Concurso.CSVInscritos.CSVIsento.ToUpper() : inscrito.ValorPagoString;
                this.NumeroControle = inscrito.Id.ToString().PadLeft(5, '0');
                this.DataBaixa = inscrito.DataPagamento.HasValue ? inscrito.DataPagamento.Value.ToString("dd/MM/yyyy") : String.Empty;
                this.CPF = inscrito.CPFFormatado;
                this.RG = String.Format("{0}", inscrito.RG, inscrito.SiglaEstadoCSV);
                this.RGEstadoEmissor = inscrito.SiglaEstadoCSV;
                this.DataNascimento = inscrito.DataNascimento.ToString("dd/MM/yyyy");
                this.EstadoCivil = inscrito.EstadoCivil.ToUpper();
                this.Nome = inscrito.Nome.ToUpper();
                this.Endereco = String.Format("{0}, {1}", inscrito.Endereco.Replace(';', ' ').ToUpper(), inscrito.NumeroEndereco);
                this.Complemento = inscrito.Complemento.ToUpper();
                this.Bairro = inscrito.Bairro.ToUpper();
                this.Cidade = inscrito.ResultInscrito.cid_nom_cidade;
                this.UF = inscrito.ResultInscrito.est_sig_estado;
                this.CEP = FormatarCEP(inscrito.CEP);
                this.Telefone = IPEFAEExtension.FormatarFone(inscrito.Telefone, false);
                this.Celular = IPEFAEExtension.FormatarFone(inscrito.Celular, false);
                this.Email = inscrito.Email.ToLower();
                this.Deficiencia = String.IsNullOrEmpty(inscrito.Deficiencia) ? Resources.Admin.Concurso.CSVInscritos.CSVSemDeficienciaVazio.ToUpper() : inscrito.Deficiencia.ToUpper();
                this.NecessitaTratamento = inscrito.TratamentoEspecial ? Resources.Admin.Concurso.CSVInscritos.CSVTratEspecialSim.ToUpper() : Resources.Admin.Concurso.CSVInscritos.CSVTratEspecialNao.ToUpper();
                this.TratamentoEspecialQual = inscrito.TratamentoEspecial ? inscrito.TratamentoEspecialQual.ToUpper() : Resources.Admin.Concurso.CSVInscritos.CSVSemTratamento.ToUpper();
                this.QtdadeFilhos = inscrito.QuantidadeFilhosMenores.ToString();

                if (inscrito.Prova != null)
                {
                    this.DataProva = inscrito.Prova.DataString;
                    this.HorarioProva = inscrito.Prova.HoraString;
                    this.LocalEscolaProva = inscrito.Prova.Local;
                    this.SalaProva = inscrito.Prova.Sala;
                }

                if (inscrito.Classificacao != null)
                {
                    this.PontosCG = inscrito.Classificacao.CGString;
                    this.PontosCE = inscrito.Classificacao.CEString;
                    this.PontosTIT = inscrito.Classificacao.TITString;
                    this.PontosPP = inscrito.Classificacao.PPString;
                    this.TotalPontos = inscrito.Classificacao.NotaString;
                    this.Classificacao = inscrito.Classificacao.PosicaoString;
                    this.Situacao = inscrito.Classificacao.Situacao;
                }

                this.Status = inscrito.Ativo ? "Ativo" : "Inativo";
                //this.Outras = String.IsNullOrEmpty(inscrito.OutrasSolicitacoes) ? Resources.Admin.Concurso.CSVInscritos.CSVOutrasSolicitacoesVazio : inscrito.OutrasSolicitacoes;
                //this.Destro = inscrito.Destro ? Resources.Admin.Concurso.CSVInscritos.CSVDestro : Resources.Admin.Concurso.CSVInscritos.CSVCanhoto;

                RemoverPontoVirgula();
            }

            [Order(0)]
            public string ConcursoDescricao     { get; set; }
            [Order(1)]
            public string CargoDescricao        { get; set; }
            [Order(2)]
            public string ValorInscricao        { get; set; }
            [Order(3)]
            public string Data                  { get; set; }
            [Order(4)]
            public string Hora                  { get; set; }
            [Order(5)]
            public string ValorPago             { get; set; }
            [Order(6)]
            public string NumeroControle        { get; set; }
            [Order(7)]
            public string DataBaixa             { get; set; }
            [Order(8)]
            public string CPF                   { get; set; }
            [Order(9), IsText(true)]
            public string RG                    { get; set; }
            [Order(10)]
            public string RGEstadoEmissor       { get; set; }
            [Order(11)]
            public string DataNascimento        { get; set; }
            [Order(12)]
            public string EstadoCivil           { get; set; }
            [Order(13)]
            public string Nome                  { get; set; }
            [Order(14), IsText(true)]
            public string Endereco              { get; set; }
            [Order(15), IsText(true)]
            public string Complemento           { get; set; }
            [Order(16), IsText(true)]
            public string Bairro                { get; set; }
            [Order(17), IsText(true)]
            public string Cidade                { get; set; }
            [Order(18)]
            public string UF                    { get; set; }
            [Order(19)]
            public string CEP                   { get; set; }
            [Order(20)]
            public string Telefone              { get; set; }
            [Order(21)]
            public string Celular               { get; set; }
            [Order(22)]
            public string Email                 { get; set; }
            [Order(23)]
            public string QtdadeFilhos          { get; set; }
            [Order(24)]
            public string Deficiencia           { get; set; }
            [Order(25)]
            public string NecessitaTratamento   { get; set; }
            [Order(26)]
            public string TratamentoEspecialQual { get; set; }

            [Order(27)]
            public string DataProva { get; set; }
            [Order(28)]
            public string HorarioProva { get; set; }
            [Order(29)]
            public string LocalEscolaProva { get; set; }
            [Order(30)]
            public string SalaProva { get; set; }
            [Order(31)]
            public string PontosCG { get; set; }
            [Order(32)]
            public string PontosCE { get; set; }
            [Order(33)]
            public string PontosTIT { get; set; }
            [Order(34)]
            public string PontosPP { get; set; }
            [Order(35)]
            public string TotalPontos { get; set; }
            [Order(36)]
            public string Classificacao { get; set; }
            [Order(37)]
            public string Situacao { get; set; }

            [Order(56)]
            public string Status                { get; set; }

            private string FormatarCEP (string cep)
            {
                if (String.IsNullOrEmpty(cep))
                    return "00000-000";

                if (cep.Length < 8)
                    cep = cep.PadLeft(8, '0');
                else if (cep.Length > 8)
                    cep = cep.Substring(0, 8);

                return String.Format("{0:00000-000}", Convert.ToInt64(cep));
            }

            internal void RemoverPontoVirgula()
            {
                this.ConcursoDescricao = this.ConcursoDescricao.Replace(";", "").ToUpper();
                this.CargoDescricao = this.CargoDescricao.Replace(";", "").ToUpper();
                this.ValorInscricao = this.ValorInscricao.Replace(";", "").ToUpper();
                this.Data = this.Data.Replace(";", "").ToUpper();
                this.Hora = this.Hora.Replace(";", "").ToUpper();
                this.ValorPago = this.ValorPago.Replace(";", "").ToUpper();
                this.NumeroControle = this.NumeroControle.Replace(";", "").ToUpper();
                this.DataBaixa = this.DataBaixa.Replace(";", "").ToUpper();
                this.CPF = this.CPF.Replace(";", "").ToUpper();
                this.RG = this.RG.Replace(";", "").ToUpper();
                this.RGEstadoEmissor = this.RGEstadoEmissor.Replace(";", "").ToUpper();
                this.DataNascimento = this.DataNascimento.Replace(";", "").ToUpper();
                this.EstadoCivil = this.EstadoCivil.Replace(";", "").ToUpper();
                this.Nome = this.Nome.Replace(";", "").ToUpper();
                this.Endereco = this.Endereco.Replace(";", "").ToUpper();
                this.Complemento = this.Complemento.Replace(";", "").ToUpper();
                this.Bairro = this.Bairro.Replace(";", "").ToUpper();
                this.Cidade = this.Cidade.Replace(";", "").ToUpper();
                this.UF = this.UF.Replace(";", "").ToUpper();
                this.CEP = this.CEP.Replace(";", "").ToUpper();
                this.Telefone = this.Telefone.Replace(";", "").ToUpper();
                this.Celular = this.Celular.Replace(";", "").ToUpper();
                this.Email = this.Email.Replace(";", "").ToLower();
                this.Deficiencia = this.Deficiencia.Replace(";", "").ToUpper();
                this.NecessitaTratamento = this.NecessitaTratamento.Replace(";", "").ToUpper();
                this.TratamentoEspecialQual = this.TratamentoEspecialQual.Replace(";", "").ToUpper();
                this.QtdadeFilhos = this.QtdadeFilhos.Replace(";", "").ToUpper();
                this.Status = this.Status.Replace(";", "").ToUpper();
            }

            //public string SolicitacaoVisual     { get; set; }
            //public string SolicitacaoSaude      { get; set; }
            //public string Outras                { get; set; }
            //public string Destro                { get; set; }
        }

        public class InscritoVestibularCSVModel : InscritoCSVModel
        {
            public InscritoVestibularCSVModel(InscritoModel inscrito, oldConcursoModel concurso) : base(inscrito, concurso)
            {
                if (inscrito.InscritoVestibular == null || !inscrito.InscritoVestibular.Ativo)
                    return;

                oldConcursoModel.InscritoModel.InscritoVestibularModel iv = inscrito.InscritoVestibular;

                this.Sexo = iv.EhMasculino ? Resources.Shared._DadosInscritoUniFae.OptionMasculino : Resources.Shared._DadosInscritoUniFae.OptionFeminino;
                this.Opcao2 = !String.IsNullOrEmpty(inscrito.ResultInscrito.cco_nom_cargo_concurso_2) ? inscrito.ResultInscrito.cco_nom_cargo_concurso_2 : Resources.Shared._DadosInscritoUniFae.NaoInformado;
                this.Opcao3 = !String.IsNullOrEmpty(inscrito.ResultInscrito.cco_nom_cargo_concurso_3) ? inscrito.ResultInscrito.cco_nom_cargo_concurso_3 : Resources.Shared._DadosInscritoUniFae.NaoInformado;
                this.LocalProva = iv.LocalProva == 1 ? "São João da Boa Vista" : (iv.LocalProva == 2 ? "Poços de Caldas" : "Mococa");
                this.NomeIndicadoPor = iv.NomeIndicadoPor;
                this.CursoIndicadoPor = iv.CursoIndicadoPor;
                this.SemestreCursoIndicadoPor = iv.SemestreCursoIndicadoPor;
                this.RendaMensal = this.ObterRendaMensal(iv.RendaMensal);
                this.TipoEnsinoMedio = this.ObterTipoEnsinoMedio(iv.TipoEnsinoMedio);
                this.TipoConcluiuEnsinoFundamental = this.ObterTipoConcluiuEnsino(iv.TipoConcluiuEnsinoFundamental);
                this.TipoConcluiuEnsinoMedio = this.ObterTipoConcluiuEnsino(iv.TipoConcluiuEnsinoMedio);
                this.ExerceAtividadeRemunerada = this.ObterExerceAtividadeRemunerada(iv.ExerceAtividadeRemunerada);
                this.EscolaridadePai = this.ObterEscolaridade(iv.EscolaridadePai, true);
                this.EscolaridadeMae = this.ObterEscolaridade(iv.EscolaridadeMae, false);
                this.MotivoOptouUniFae = this.ObterMotivoOptouUniFae(iv.MotivoOptouUniFae);
                this.MotivoOptouCurso = this.ObterMotivoOptouCurso(iv.MotivoOptouCurso);
                this.TomouConhecimentoUniFae = this.ObterTomouConhecimentoUniFae(iv.TomouConhecimentoUniFae);
                this.TomouConhecimentoUniFaeOutros = iv.TomouConhecimentoUniFaeOutros;

                this.RemoverPontoVirgula();
            }

            [Order(38)]
            public string Sexo                          { get; set; }
            [Order(39)]
            public string Opcao2                        { get; set; }
            [Order(40)]
            public string Opcao3                        { get; set; }
            [Order(41)]
            public string LocalProva                    { get; set; }
            [Order(42)]
            public string NomeIndicadoPor               { get; set; }
            [Order(43)]
            public string CursoIndicadoPor              { get; set; }
            [Order(44)]
            public string SemestreCursoIndicadoPor      { get; set; }
            [Order(45)]
            public string RendaMensal                   { get; set; }
            [Order(46)]
            public string TipoEnsinoMedio               { get; set; }
            [Order(47)]
            public string TipoConcluiuEnsinoFundamental { get; set; }
            [Order(48)]
            public string TipoConcluiuEnsinoMedio       { get; set; }
            [Order(49)]
            public string ExerceAtividadeRemunerada     { get; set; }
            [Order(50)]
            public string EscolaridadePai               { get; set; }
            [Order(51)]
            public string EscolaridadeMae               { get; set; }
            [Order(52)]
            public string MotivoOptouUniFae             { get; set; }
            [Order(53)]
            public string MotivoOptouCurso              { get; set; }
            [Order(54)]
            public string TomouConhecimentoUniFae       { get; set; }
            [Order(55)]
            public string TomouConhecimentoUniFaeOutros { get; set; }

            private string ObterEscolaridade(int? escolaridade, bool pai)
            {
                switch (escolaridade)
                {
                    case 1: return pai ? Resources.Shared._DadosInscritoUniFae.OptionEscolaridadePaiNaoAlfabetizado : Resources.Shared._DadosInscritoUniFae.OptionEscolaridadeMaeNaoAlfabetizada;
                    case 2: return Resources.Shared._DadosInscritoUniFae.OptionEscolaridadeFundamentalIncompleto;
                    case 3: return Resources.Shared._DadosInscritoUniFae.OptionEscolaridadeFundamentalCompleto;
                    case 4: return Resources.Shared._DadosInscritoUniFae.OptionEscolaridadeMedioIncompleto;
                    case 5: return Resources.Shared._DadosInscritoUniFae.OptionEscolaridadeMedioCompleto;
                    case 6: return Resources.Shared._DadosInscritoUniFae.OptionEscolaridadeSuperiorIncompleto;
                    case 7: return Resources.Shared._DadosInscritoUniFae.OptionEscolaridadeSuperiorCompleto;
                    case 8: return Resources.Shared._DadosInscritoUniFae.OptionEscolaridadePosGraduacao;
                    default: return Resources.Shared._DadosInscritoUniFae.NaoInformado;
                }
            }

            private string ObterExerceAtividadeRemunerada(int? exerceAtividadeRemunerada)
            {
                switch (exerceAtividadeRemunerada)
                {
                    case 1: return Resources.Shared._DadosInscritoUniFae.OptionSim;
                    case 2: return Resources.Shared._DadosInscritoUniFae.OptionAtividadeRemuneradaNaoNunca;
                    case 3: return Resources.Shared._DadosInscritoUniFae.OptionAtividadeRemuneradaNaoJaExerci;
                    default: return Resources.Shared._DadosInscritoUniFae.NaoInformado;
                }
            }

            private string ObterMotivoOptouCurso(int? motivoOptouCurso)
            {
                switch (motivoOptouCurso)
                {
                    case 1: return Resources.Shared._DadosInscritoUniFae.OptionMotivoOptarCursoVocacao;
                    case 2: return Resources.Shared._DadosInscritoUniFae.OptionMotivoOptarCursoMercadoTrabalho;
                    case 3: return Resources.Shared._DadosInscritoUniFae.OptionMotivoOptarCursoCrescimentoEmpresa;
                    case 4: return Resources.Shared._DadosInscritoUniFae.OptionMotivoOptarCursoExigenciaEmpresa;
                    case 5: return Resources.Shared._DadosInscritoUniFae.OptionMotivoOptarCursoCompetencia;
                    default: return Resources.Shared._DadosInscritoUniFae.NaoInformado;
                }
            }

            private string ObterMotivoOptouUniFae(int? motivoOptouUniFae)
            {
                switch (motivoOptouUniFae)
                {
                    case 1: return Resources.Shared._DadosInscritoUniFae.OptionMotivoOptarUniFaeConceito;
                    case 2: return Resources.Shared._DadosInscritoUniFae.OptionMotivoOptarUniFaeValor;
                    case 3: return Resources.Shared._DadosInscritoUniFae.OptionMotivoOptarUniFaeLocalizacao;
                    default: return Resources.Shared._DadosInscritoUniFae.NaoInformado;
                }
            }

            private string ObterRendaMensal(int? rendaMensal)
            {
                switch (rendaMensal)
                {
                    case 1: return Resources.Shared._DadosInscritoUniFae.OptionRendaMensalAte300;
                    case 2: return Resources.Shared._DadosInscritoUniFae.OptionRendaMensal300a1000;
                    case 3: return Resources.Shared._DadosInscritoUniFae.OptionRendaMensal1001a2000;
                    case 4: return Resources.Shared._DadosInscritoUniFae.OptionRendaMensal2001a3000;
                    case 5: return Resources.Shared._DadosInscritoUniFae.OptionRendaMensalAcima3000;
                    default: return Resources.Shared._DadosInscritoUniFae.NaoInformado;
                }
            }

            private string ObterTipoEnsinoMedio(int? tipoEnsinoMedio)
            {
                switch (tipoEnsinoMedio)
                {
                    case 1: return Resources.Shared._DadosInscritoUniFae.OptionEnsinoMedioEstadual;
                    case 2: return Resources.Shared._DadosInscritoUniFae.OptionEnsinoMedioMunicipal;
                    case 3: return Resources.Shared._DadosInscritoUniFae.OptionEnsinoMedioParticular;
                    default: return Resources.Shared._DadosInscritoUniFae.NaoInformado;
                }
            }

            private string ObterTipoConcluiuEnsino(int? tipoConcluiuEnsino)
            {
                switch (tipoConcluiuEnsino)
                {
                    case 1: return Resources.Shared._DadosInscritoUniFae.OptionConcluiuEnsinoNormal;
                    case 2: return Resources.Shared._DadosInscritoUniFae.OptionConcluiuEnsinoSupletivo;
                    default: return Resources.Shared._DadosInscritoUniFae.NaoInformado;
                }
            }

            private string ObterTomouConhecimentoUniFae(int? tomouConhecimentoUniFae)
            {
                switch (tomouConhecimentoUniFae)
                {
                    case 1: return Resources.Shared._DadosInscritoUniFae.OptionTomouConhecimentoAmigos;
                    case 2: return Resources.Shared._DadosInscritoUniFae.OptionTomouConhecimentoAlunos;
                    case 3: return Resources.Shared._DadosInscritoUniFae.OptionTomouConhecimentoExAlunos;
                    case 4: return Resources.Shared._DadosInscritoUniFae.OptionTomouConhecimentoCartazes;
                    case 5: return Resources.Shared._DadosInscritoUniFae.OptionTomouConhecimentoFlyer;
                    case 6: return Resources.Shared._DadosInscritoUniFae.OptionTomouConhecimentoInternet;
                    case 7: return Resources.Shared._DadosInscritoUniFae.OptionTomouConhecimentoOutdoor;
                    case 8: return Resources.Shared._DadosInscritoUniFae.OptionTomouConhecimentoRadio;
                    case 9: return Resources.Shared._DadosInscritoUniFae.OptionTomouConhecimentoTV;
                    case 10: return Resources.Shared._DadosInscritoUniFae.OptionTomouConhecimentoOutros;
                    default: return Resources.Shared._DadosInscritoUniFae.NaoInformado;
                }
            }

            private void RemoverPontoEVirgula()
            {
                this.Sexo = this.Sexo.Replace(";", "").ToUpper();
                this.Opcao2 = this.Opcao2.Replace(";", "").ToUpper();
                this.Opcao3 = this.Opcao3.Replace(";", "").ToUpper();
                this.LocalProva = this.LocalProva.Replace(";", "").ToUpper();
                this.NomeIndicadoPor = this.NomeIndicadoPor.Replace(";", "").ToUpper();
                this.CursoIndicadoPor = this.CursoIndicadoPor.Replace(";", "").ToUpper();
                this.SemestreCursoIndicadoPor = this.SemestreCursoIndicadoPor.Replace(";", "").ToUpper();
                this.RendaMensal = this.RendaMensal.Replace(";", "").ToUpper();
                this.TipoEnsinoMedio = this.TipoEnsinoMedio.Replace(";", "").ToUpper();
                this.TipoConcluiuEnsinoFundamental = this.TipoConcluiuEnsinoFundamental.Replace(";", "").ToUpper();
                this.TipoConcluiuEnsinoMedio = this.TipoConcluiuEnsinoMedio.Replace(";", "").ToUpper();
                this.ExerceAtividadeRemunerada = this.ExerceAtividadeRemunerada.Replace(";", "").ToUpper();
                this.EscolaridadePai = this.EscolaridadePai.Replace(";", "").ToUpper();
                this.EscolaridadeMae = this.EscolaridadeMae.Replace(";", "").ToUpper();
                this.MotivoOptouUniFae = this.MotivoOptouUniFae.Replace(";", "").ToUpper();
                this.MotivoOptouCurso = this.MotivoOptouCurso.Replace(";", "").ToUpper();
                this.TomouConhecimentoUniFae = this.TomouConhecimentoUniFae.Replace(";", "").ToUpper();
                this.TomouConhecimentoUniFaeOutros = this.TomouConhecimentoUniFaeOutros.Replace(";", "").ToUpper();
            }
        }

        public class ConcursoInscritosEstatisticaModel
        {
            public ConcursoInscritosEstatisticaModel()
            {
                this.TotalInscritos = 0;
                this.InscritosPorCargo = new Dictionary<string, int>();
                this.InscritosPorData = new Dictionary<string, int>();
            }

            public ConcursoInscritosEstatisticaModel(oldConcursoModel concurso) : this()
            {
                this.TotalInscritos = concurso.Inscritos.Count;

                foreach (CargoModel cargo in concurso.Cargos.OrderBy(c => c.Nome))
                {
                    if (!this.InscritosPorCargo.ContainsKey(cargo.Nome))
                        this.InscritosPorCargo.Add(cargo.Nome, 0);
                }

                foreach (InscritoModel inscrito in concurso.Inscritos)
                {
                    foreach (CargoModel cargo in inscrito.Cargos)
                        this.InscritosPorCargo[cargo.Nome] += 1;

                    string dataInscricao = inscrito.DataInscricao.ToString("dd/MM/yyyy");

                    if (this.InscritosPorData.ContainsKey(dataInscricao))
                        this.InscritosPorData[dataInscricao] += 1;
                    else
                        this.InscritosPorData.Add(dataInscricao, 1);
                }
            }

            public int TotalInscritos                           { get; set; }
            public Dictionary<string, int> InscritosPorCargo    { get; set; }
            public Dictionary<string, int> InscritosPorData     { get; set; }
        }

        #endregion [ FIM - Classes ]
    }
}