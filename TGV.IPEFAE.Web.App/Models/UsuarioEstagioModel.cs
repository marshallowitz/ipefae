using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.BL.Business;
using TGV.IPEFAE.Web.BL.Data;
using TGV.IPEFAE.Web.Resources.Estagio;
using TGV.IPEFAE.Web.Resources.Shared;

namespace TGV.IPEFAE.Web.App.Models
{
    public class UsuarioEstagioModel
    {
        public UsuarioEstagioModel()
        {
            this.DataCadastro = BaseBusiness.DataAgora;
            this.DataUltimaAtualizacao = BaseBusiness.DataAgora;
            this.Cidade = new CidadeModel();
            this.EstadoCarteiraTrabalho = new EstadoModel();
            this.ConsiderarBusca = true;
            this.EstaEstagiando = false;
            this.PossuiDeficiencia = false;
            this.PossuiExperienciaProfissional = false;
            this.Ativo = true;

            this.DadosEscolares = new DadosEscolaresModel();
            this.CursosCapacitacoes = new List<CursosCapacitacoesModel>();
            this.ExperienciasProfissionais = new List<ExperienciaProfissionalModel>();
            this.OutrosConhecimentos = new List<CursosCapacitacoesModel>();
        }

        public UsuarioEstagioModel(tb_ues_usuario_estagio estagiario) : this()
        {
            if (estagiario == null)
                return;

            this.Id = estagiario.ues_idt_usuario_estagio;
            this.IdCidadeEndereco = estagiario.cid_idt_cidade_endereco;
            this.IdEstadoCarteiraTrabalho = estagiario.est_idt_estado_carteira_trabalho;
            this.IdUsuarioEstagioDadosEscolares = estagiario.ued_idt_usuario_estagio_dados_escolares;
            this.Nome = estagiario.ues_nom_usuario_estagio;
            this.CPF = estagiario.ues_des_cpf;
            this.RG = estagiario.ues_des_rg;
            this.Email = estagiario.ues_des_email;
            this.Senha = estagiario.ues_des_senha;
            this.SenhaDescriptografada = estagiario.ues_des_senha_descriptografada;
            this.CarteiraTrabalhoNumero = estagiario.ues_des_carteira_trabalho_numero;
            this.CarteiraTrabalhoSerie = estagiario.ues_des_carteira_trabalho_serie;
            this.Telefone = estagiario.ues_des_telefone;
            this.Celular = estagiario.ues_des_celular;
            this.Objetivos = estagiario.ues_des_objetivos;
            this.ObservacoesAdmin = estagiario.ues_des_observacoes_admin;
            this.FlgDeficiencia = estagiario.ues_flg_deficiencia;
            this.FlgEstadoCivil = estagiario.ues_flg_estado_civil;
            this.FlgMotivoDesativacao = estagiario.ues_flg_motivo_desativacao;
            this.NumeroFilhos = estagiario.ues_num_quantidade_filhos;

            this.DataNascimento = estagiario.ues_dat_nascimento;
            this.DataCadastro = estagiario.ues_dat_criacao_cv;
            this.DataExpedicaoRG = estagiario.ues_dat_rg_expedicao;
            this.DataUltimaAtualizacao = estagiario.ues_dat_ultima_atualizacao;

            this.ConsiderarBusca = estagiario.ues_bit_considerar_busca;
            this.EstaEstagiando = estagiario.ues_bit_estagiando;
            this.PossuiDeficiencia = estagiario.ues_bit_possui_deficiencia;
            this.PossuiExperienciaProfissional = estagiario.ues_bit_possui_experiencia_profissional;
            this.EhMasculino = estagiario.ues_bit_masculino;
            this.TemFoto = estagiario.ues_bit_tem_foto;
            this.Ativo = estagiario.ues_bit_ativo;

            this.Endereco = estagiario.ues_des_endereco;
            this.NroEndereco = estagiario.ues_des_numero_endereco;
            this.Bairro = estagiario.ues_des_bairro;
            this.Complemento = estagiario.ues_des_complemento;
            this.CEP = estagiario.ues_des_cep;

            try { this.Cidade = new CidadeModel(estagiario.tb_cid_cidade); } catch (Exception) { }
            try { this.EstadoCarteiraTrabalho = new EstadoModel(estagiario.tb_est_estado); } catch (Exception) { }
            try { this.DadosEscolares = new DadosEscolaresModel(estagiario.tb_ued_usuario_estagio_dados_escolares); } catch (Exception) { }
            int posicao = 1;
            try { this.ExperienciasProfissionais = estagiario.tb_uee_usuario_estagio_experiencia.ToList().ConvertAll(uee => new ExperienciaProfissionalModel(uee, posicao++, true)); } catch (Exception) { }
            posicao = 1;
            try { this.CursosCapacitacoes = estagiario.tb_ueo_usuario_estagio_outros.Where(ueo => ueo.ueo_bit_curso).ToList().ConvertAll(ueo => new CursosCapacitacoesModel(ueo, posicao++, true)); } catch (Exception) { }
            posicao = 1;
            try { this.OutrosConhecimentos = estagiario.tb_ueo_usuario_estagio_outros.Where(ueo => !ueo.ueo_bit_curso).ToList().ConvertAll(ueo => new CursosCapacitacoesModel(ueo, posicao++, true)); } catch (Exception) { }
        }

        #region [ Propriedades ]

        public int Id                               { get; set; }
        public int? IdEstadoCarteiraTrabalho        { get; set; }
        public int IdUsuarioEstagioDadosEscolares   { get; set; }
        public int IdEstado                         { get { return Cidade != null && Cidade.Estado != null && Cidade.Estado.Id > 0 ? Cidade.Estado.Id : 0; } }
        public string Nome                          { get; set; }
        public string CPF                           { get; set; }
        public string RG                            { get; set; }
        public string Email                         { get; set; }
        public string Senha                         { get; set; }
        public string SenhaDescriptografada         { get; set; }
        public string CarteiraTrabalhoNumero        { get; set; }
        public string CarteiraTrabalhoSerie         { get; set; }
        public string Telefone                      { get; set; }
        public string TelefoneFormatado             { get { return String.IsNullOrEmpty(Telefone) ? String.Empty : (Telefone.Length == 10) ? String.Format(@"{0:(00) 0000-0000}", Int64.Parse(Telefone)) : String.Format(@"{0:(00) 00000-0000}", Int64.Parse(Telefone.PadLeft(11, '0'))); } }
        public string Celular                       { get; set; }
        public string CelularFormatado              { get { return String.IsNullOrEmpty(Celular) ? String.Empty : (Celular.Length == 10) ? String.Format(@"{0:(00) 0000-0000}", Int64.Parse(Celular)) : String.Format(@"{0:(00) 00000-0000}", Int64.Parse(Celular.PadLeft(11, '0'))); } }
        public string Objetivos                     { get; set; }
        public string ObservacoesAdmin              { get; set; }
        public string FlgDeficiencia                { get; set; }
        public string FlgEstadoCivil                { get; set; }
        public string FlgMotivoDesativacao          { get; set; }
        public int NumeroFilhos                     { get; set; }

        public DateTime DataNascimento              { get; set; }
        public DateTime DataCadastro                { get; set; }
        public DateTime DataUltimaAtualizacao       { get; set; }
        public DateTime DataExpedicaoRG             { get; set; }

        public bool ConsiderarBusca                 { get; set; }
        public bool EstaEstagiando                  { get; set; }
        public bool PossuiDeficiencia               { get; set; }
        public bool PossuiExperienciaProfissional   { get; set; }
        public bool EhMasculino                     { get; set; }
        public bool TemFoto                         { get; set; }
        public bool Ativo                           { get; set; }
        
        public EstadoModel EstadoCarteiraTrabalho   { get; set; }
        public DadosEscolaresModel DadosEscolares   { get; set; }

        public List<CursosCapacitacoesModel> CursosCapacitacoes             { get; set; }
        public List<ExperienciaProfissionalModel> ExperienciasProfissionais { get; set; }
        public List<CursosCapacitacoesModel> OutrosConhecimentos            { get; set; }

        public string CPFFormatado                  { get { return String.IsNullOrEmpty(CPF) ? String.Empty : String.Format(@"{0:000\.000\.000\-00}", Int64.Parse(CPF)); } }
        public string DataCadastroString            { get { return DataCadastro.ToString("dd/MM/yyyy HH:mm"); } }
        public string DataExpedicaoRGString         { get { return DataExpedicaoRG.ToString("dd/MM/yyyy"); } }
        public string DataNascimentoString          { get { return DataNascimento.ToString("dd/MM/yyyy"); } }
        public string DataUltimaAtualizacaoString   { get { return DataUltimaAtualizacao.ToString("dd/MM/yyyy HH:mm"); } }
        public string Idade                         { get { return String.Format("{0} {1}", this.DataNascimento.ObterIdade(), Preview.LabelIdadeAnos); } }
        public string UrlFoto                       { get { return this.TemFoto ? String.Format("Anexos/Estagio/{0}.jpg", this.Id) : String.Empty; } }

        #region [ Endereço ]

        public int IdCidadeEndereco                 { get; set; }
        public string Endereco                      { get; set; }
        public int NroEndereco                      { get; set; }
        public string Bairro                        { get; set; }
        public string Complemento                   { get; set; }
        public string CEP                           { get; set; }

        public string NumeroEnderecoString          { get { return this.NroEndereco <= 0 ? String.Empty : this.NroEndereco.ToString(); } }

        public CidadeModel Cidade                   { get; set; }

        #endregion [ FIM - Endereço ]

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

        #endregion [ FIM - Propriedades ]

        #region [ Classes ]

        public class CursosCapacitacoesModel
        {
            public CursosCapacitacoesModel()
            {
                this.Ativo = true;
                this.EhCurso = true;
            }

            public CursosCapacitacoesModel(tb_ueo_usuario_estagio_outros cursoCapacitacao, int posicao, bool visivel)
            {
                if (cursoCapacitacao == null)
                    return;

                this.Id = cursoCapacitacao.ueo_idt_usuario_estagio_outros;
                this.IdUsuarioEstagio = cursoCapacitacao.ues_idt_usuario_estagio;
                this.EhCurso = cursoCapacitacao.ueo_bit_curso;
                this.Ativo = cursoCapacitacao.ueo_bit_ativo;
                this.Posicao = posicao;
                this.Visivel = this.Ativo && visivel;

                if (!this.Ativo || String.IsNullOrEmpty(cursoCapacitacao.ueo_nom_usuario_estagio_outros))
                    return;

                this.NomeCurso = cursoCapacitacao.ueo_nom_usuario_estagio_outros;
                this.Duracao = cursoCapacitacao.ueo_des_duracao;
            }

            public int Id               { get; set; }
            public int IdUsuarioEstagio { get; set; }
            public string NomeCurso     { get; set; }
            public string Duracao       { get; set; }
            public bool EhCurso         { get; set; }
            public bool Ativo           { get; set; }
            public int Posicao          { get; set; }
            public bool Visivel         { get; set; }
        }

        public class DadosEscolaresModel
        {
            public DadosEscolaresModel()
            {
                this.AnoInicio = DateTime.Today.Year;
                this.AnoTermino = DateTime.Today.Year;
            }

            public DadosEscolaresModel(tb_ued_usuario_estagio_dados_escolares dados_escolares) : this()
            {
                if (dados_escolares == null)
                    return;

                this.Id = dados_escolares.ued_idt_usuario_estagio_dados_escolares;
                this.Ano_Semestre = dados_escolares.ued_num_ano_semestre;
                this.NomeEscola = dados_escolares.ued_des_nome_escola;
                this.NomeCurso = dados_escolares.ued_des_nome_curso;
                this.FlagPeriodo = dados_escolares.ued_flg_periodo;
                this.FlagTipo = dados_escolares.ued_flg_tipo_dados_escolares;
                this.FlagTipoProfissionalizante = dados_escolares.ued_flg_tipo_profissionalizante;
                this.MesInicio = dados_escolares.ued_num_mes_inicio;
                this.AnoInicio = dados_escolares.ued_num_ano_inicio;
                this.MesTermino = dados_escolares.ued_num_mes_termino;
                this.AnoTermino = dados_escolares.ued_num_ano_termino;
                this.EhEAD = dados_escolares.ued_bit_ead;
                this.Ativo = dados_escolares.ued_bit_ativo;
            }

            public int Id                               { get; set; }
            public int Ano_Semestre                     { get; set; }
            public string NomeEscola                    { get; set; }
            public string NomeCurso                     { get; set; }
            public string FlagPeriodo                   { get; set; }
            public string FlagTipo                      { get; set; }
            public string FlagTipoProfissionalizante    { get; set; }
            public int? MesInicio                       { get; set; }
            public int AnoInicio                        { get; set; }
            public int? MesTermino                      { get; set; }
            public int AnoTermino                       { get; set; }
            public bool? EhEAD                          { get; set; }
            public bool Ativo                           { get; set; }

            public string DataInicioString  { get { return MesInicio.HasValue ? String.Format("{0}/{1}", MesInicio.Value.ToString().PadLeft(2, '0'), AnoInicio) : AnoInicio.ToString(); } }
            public string DataTerminoString { get { return MesTermino.HasValue ? String.Format("{0}/{1}", MesTermino.Value.ToString().PadLeft(2, '0'), AnoTermino) : AnoTermino.ToString(); } }

            public string InicioString
            {
                get
                {
                    return (this.Id > 0 && this.MesInicio.HasValue) ?
                        String.Format("{0}/{1}", new DateTime(AnoInicio, MesInicio.Value, 1).GetMesInteiro(), AnoInicio) :
                        String.Empty;
                }
            }

            public string TerminoString
            {
                get
                {
                    return (this.Id > 0 && this.MesTermino.HasValue) ?
                        String.Format(" {2} {0}/{1}", new DateTime(AnoTermino, MesTermino.Value, 1).GetMesInteiro(), AnoTermino, Preview.LabelExperienciaTerminoAte) :
                        String.Empty;
                }
            }

            public string Ano_SemestreString
            {
                get
                {
                    switch (this.FlagTipo)
                    {
                        case "M":
                            return String.Format("{0}º {1}", this.Ano_Semestre, Preview.LabelTipoAno);
                        case "T":
                            switch (this.FlagTipoProfissionalizante)
	                        {
                                case "1":
                                    return String.Format("{0}º {1}", this.Ano_Semestre, _DadosEstagiario.OptionTipoModulo);
                                case "2":
                                    return String.Format("{0}º {1}", this.Ano_Semestre, _DadosEstagiario.OptionTipoTermo);
                                case "3":
                                    return String.Format("{0}º {1}", this.Ano_Semestre, _DadosEstagiario.OptionTipoSemestre);
	                        }
                            return String.Empty;
                        case "E":
                            return String.Format("{0}º {1} {2}", this.Ano_Semestre, Preview.LabelTipoAno, this.FlagTipoProfissionalizante == "2" ? _DadosEstagiario.OptionEJAEF : _DadosEstagiario.OptionEJAEM);
                        case "S":
                            return String.Format("{0}º {1}", this.Ano_Semestre, Preview.LabelTipoSemestre);
                        default:
                            return "";
                    }
                }
            }

            public string Ano_Semestre_CursoString
            {
                get
                {
                    switch (this.FlagTipo)
                    {
                        case "M":
                            return String.Format("{0}º {1}", this.Ano_Semestre, Preview.LabelTipoAno);
                        case "T":
                            switch (this.FlagTipoProfissionalizante)
                            {
                                case "1":
                                    return String.Format("{0}º {1} - {2}", this.Ano_Semestre, _DadosEstagiario.OptionTipoModulo, this.NomeCurso);
                                case "2":
                                    return String.Format("{0}º {1} - {2}", this.Ano_Semestre, _DadosEstagiario.OptionTipoTermo, this.NomeCurso);
                                case "3":
                                    return String.Format("{0}º {1} - {2}", this.Ano_Semestre, _DadosEstagiario.OptionTipoSemestre, this.NomeCurso);
                            }
                            return String.Empty;
                        case "E":
                            return String.Format("{0}º {1} {2}", this.Ano_Semestre, Preview.LabelTipoAno, this.FlagTipoProfissionalizante == "2" ? _DadosEstagiario.OptionEJAEF : _DadosEstagiario.OptionEJAEM);
                        case "S":
                            return String.Format("{0}º {1} - {2}", this.Ano_Semestre, Preview.LabelTipoSemestre, this.NomeCurso);
                        default:
                            return "";
                    }
                }
            }

            public string TipoEnsino
            {
                get
                {
                    switch (this.FlagTipo)
                    {
                        case "M":
                            return _DadosEstagiario.OptionEnsinoMedio;
                        case "T":
                            return _DadosEstagiario.OptionTecnico;
                        case "E":
                            return _DadosEstagiario.OptionEJA;
                        case "S":
                            return _DadosEstagiario.OptionEnsinoSuperior;
                        default:
                            return "";
                    }
                }
            }
        }

        public class ExperienciaProfissionalModel
        {
            public ExperienciaProfissionalModel() { }

            public ExperienciaProfissionalModel(tb_uee_usuario_estagio_experiencia experiencia_profissional, int posicao, bool visivel) : this()
            {
                if (experiencia_profissional == null)
                    return;

                this.Id = experiencia_profissional.uee_idt_usuario_estagio_experiencia;
                this.IdUsuarioEstagio = experiencia_profissional.ues_idt_usuario_estagio;
                this.Posicao = posicao;
                this.Ativo = experiencia_profissional.uee_bit_ativo;
                this.Visivel = this.Ativo && visivel;

                if (!this.Ativo || String.IsNullOrEmpty(experiencia_profissional.uee_des_nome_empresa))
                    return;

                this.MesInicio = experiencia_profissional.uee_num_mes_inicio;
                this.AnoInicio = experiencia_profissional.uee_num_ano_inicio;
                this.MesTermino = experiencia_profissional.uee_num_mes_termino;
                this.AnoTermino = experiencia_profissional.uee_num_ano_termino;
                this.NomeEmpresa = experiencia_profissional.uee_des_nome_empresa;
                this.Cargo = experiencia_profissional.uee_des_cargo;
                this.AtividadesDesenvolvidas = experiencia_profissional.uee_des_atividades_desenvolvidas;
            }

            public int Id                           { get; set; }
            public int IdUsuarioEstagio             { get; set; }
            public int? MesInicio                   { get; set; }
            public int? AnoInicio                   { get; set; }
            public int? MesTermino                  { get; set; }
            public int? AnoTermino                  { get; set; }
            public string NomeEmpresa               { get; set; }
            public string Cargo                     { get; set; }
            public string AtividadesDesenvolvidas   { get; set; }
            public bool Ativo                       { get; set; }
            public int Posicao                      { get; set; }
            public bool Visivel                     { get; set; }

            public string Inicio
            {
                get
                {
                    return (this.Id > 0 && this.MesInicio.HasValue && this.AnoInicio.HasValue) ?
                        String.Format("{0}/{1}", new DateTime(AnoInicio.Value, MesInicio.Value, 1).GetMesInteiro(), AnoInicio) :
                        String.Empty;
                }
            }
            
            public string Termino
            {
                get
                {
                    return (this.Id > 0 && this.MesTermino.HasValue && this.AnoTermino.HasValue) ?
                        String.Format(" {2} {0}/{1}", new DateTime(AnoTermino.Value, MesTermino.Value, 1).GetMesInteiro(), AnoTermino, Preview.LabelExperienciaTerminoAte) :
                        String.Empty;
                }
            }
        }

        #endregion [ FIM - Classes ]
    }

    public class UsuarioEstagioModelCSV
    {
        public UsuarioEstagioModelCSV(UsuarioEstagioModel estagiario)
        {
            if (estagiario == null)
                return;

            this.Nome = estagiario.Nome;
            this.CPF = estagiario.CPFFormatado;
            this.RG = estagiario.RG;
            this.DataExpedicaoRG = estagiario.DataExpedicaoRG.ToString("dd/MM/yyyy");
            this.CTPSNro = estagiario.CarteiraTrabalhoNumero;
            this.CTPSSerie = $"{estagiario.CarteiraTrabalhoSerie}-{estagiario.EstadoCarteiraTrabalho.Sigla}";
            this.DataNascimento = estagiario.DataNascimento.ToString("dd/MM/yyyy");
            this.Fone01 = estagiario.TelefoneFormatado;
            this.Fone02 = estagiario.CelularFormatado;
            this.Email = estagiario.Email;
            this.EnderecoNro = $"{estagiario.Endereco}, {estagiario.NroEndereco}";
            this.Complemento = estagiario.Complemento;
            this.Bairro = estagiario.Bairro;
            this.Cidade = estagiario.Cidade.Nome;
            this.Estado = estagiario.Cidade.Estado.Sigla;
            this.CEP = FormatarCEP(estagiario.CEP);
            this.TipoEnsino = estagiario.DadosEscolares?.TipoEnsino;
            this.NomeEscola = estagiario.DadosEscolares?.NomeEscola;
            this.NomeCurso = estagiario.DadosEscolares?.NomeCurso;
            this.SemestreCurso = estagiario.DadosEscolares?.Ano_SemestreString;
            this.InicioCurso = estagiario.DadosEscolares?.DataInicioString;
            this.TerminoCurso = estagiario.DadosEscolares?.DataTerminoString;
        }

        [Order(0)]
        public string Nome { get; set; }

        [Order(1)]
        public string CPF { get; set; }

        [Order(2)]
        [IsText(true)]
        public string RG { get; set; }

        [Order(3)]
        public string DataExpedicaoRG { get; set; }

        [Order(4)]
        [IsText(true)]
        public string CTPSNro { get; set; }

        [Order(5)]
        public string CTPSSerie { get; set; }

        [Order(6)]
        public string DataNascimento { get; set; }

        [Order(7)]
        public string Fone01 { get; set; }

        [Order(8)]
        public string Fone02 { get; set; }

        [Order(9)]
        public string Email { get; set; }

        [Order(10)]
        public string EnderecoNro { get; set; }

        [Order(11)]
        public string Complemento { get; set; }

        [Order(12)]
        public string Bairro { get; set; }

        [Order(13)]
        public string Cidade { get; set; }

        [Order(14)]
        public string Estado { get; set; }

        [Order(15)]
        public string CEP { get; set; }

        [Order(16)]
        public string TipoEnsino { get; set; }

        [Order(17)]
        public string NomeEscola { get; set; }

        [Order(18)]
        public string NomeCurso { get; set; }

        [Order(19)]
        public string SemestreCurso { get; set; }

        [Order(20)]
        public string InicioCurso { get; set; }

        [Order(21)]
        public string TerminoCurso { get; set; }

        private string FormatarCEP(string cep)
        {
            if (String.IsNullOrEmpty(cep))
                return "00000-000";

            if (cep.Length < 8)
                cep = cep.PadLeft(8, '0');
            else if (cep.Length > 8)
                cep = cep.Substring(0, 8);

            return String.Format("{0:00000-000}", Convert.ToInt64(cep));
        }
    }
}