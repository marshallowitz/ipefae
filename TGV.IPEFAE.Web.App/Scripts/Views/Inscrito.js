var oTable;

function atualizarCursos()
{
    var ddl1 = $('select[name="curso"]');
    var ddl2 = $('select[name="curso2"]');
    var ddl3 = $('select[name="curso3"]');

    ddl1.find('option').show();
    ddl2.find('option').show();
    ddl3.find('option').show();

    var val1 = ddl1.val();
    var val2 = ddl2.val();
    var val3 = ddl3.val();

    if (val1 > 0)
    {
        ddl2.find('option[value="' + val1 + '"]').hide();
        ddl3.find('option[value="' + val1 + '"]').hide();
    }

    if (val2 > 0)
    {
        ddl1.find('option[value="' + val2 + '"]').hide();
        ddl3.find('option[value="' + val2 + '"]').hide();
    }

    if (val3 > 0)
    {
        ddl1.find('option[value="' + val3 + '"]').hide();
        ddl2.find('option[value="' + val3 + '"]').hide();
    }
}

function carregarCargosConcurso(idConcurso, idCargo)
{
    var ddl = $('#ddlCargo');
    var url = homePage + 'Concurso/CarregarCargos';

    // Carrega a lista de cargos existentes no concurso
    $.ajax({
        type: "POST",
        url: url,
        data: { idConcurso: idConcurso },
        async: false,
        success: function (items) {
            ddl.empty();
            var data = items.data ? items.data : items;

            if (data.length > 0) {
                ddl.addItems({ data: data, valueName: 'Value', textName: 'Text' });
                ddl.val(idCargo);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'carregarCargosConcurso()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function carregarOpcoesCurso(idConcurso, idOpcao, idDdl)
{
    var ddl = $('#' + idDdl);
    var url = homePage + 'Concurso/CarregarCargos';

    // Carrega a lista de cargos existentes no concurso
    $.ajax({
        type: "POST",
        url: url,
        data: { idConcurso: idConcurso },
        async: false,
        success: function (items)
        {
            ddl.empty();
            var data = items.data ? items.data : items;

            if (data.length > 0)
            {
                ddl.addItems({ data: data, valueName: 'Value', textName: 'Text' });
                ddl.val(idOpcao);

                ddl.prepend("<option value='0' selected='selected'></option>");
            }
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'carregarOpcoesCurso()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function carregarEstados(ddl, idEstado, exibirNome, idCidade) {
    var url = homePage + 'Inscrito/CarregarEstados';

    $.ajax({
        type: "POST",
        url: url,
        data: { exibirNome: exibirNome },
        success: function (items) {
            ddl.empty();
            var data = items.data ? items.data : items;

            if (data.length > 0) {
                ddl.addItems({ data: data, valueName: 'Value', textName: 'Text' });
                ddl.val(idEstado);

                if (ddl.attr('id') == 'ddlEstado' && idEstado > 0 && idCidade > 0) {
                    $('#ddlEstado').trigger('change');
                    $('#ddlCidade').val(idCidade);
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'carregarEstados()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function carregarTelaDadosInscrito(idConcurso, idCargo, idEstadoRG, idEstadoCivil, idEstado, idCidade, ativo, isento, possuiDef, flgDef, necTratEsp)
{
    var ehVestibular = $('#IdTipoLayoutConcurso').val() == 2;
    carregarCargosConcurso(idConcurso, idCargo);
    carregarEstados($('#ddlEstado'), idEstado, true, idCidade);
    carregarEstados($('#ddlEstadoRG'), idEstadoRG, false);

    $('#txtCPF').mask('999.999.999-99');

    var SPMaskBehavior = function (val) { return val.replace(/\D/g, '').length === 11 ? '(00) 00000-0000' : '(00) 0000-00009'; };
    var spOptions = { onKeyPress: function (val, e, field, options) { field.mask(SPMaskBehavior.apply({}, arguments), options); } };
    $('#txtTelefone').mask(SPMaskBehavior, spOptions);
    $('#txtCelular').mask(SPMaskBehavior, spOptions);

    $('#txtDataNascimento').mask('00/00/0000');
    $('#txtCEP').mask('00000-000');
    $('#txtFilhosMenores').mask('00');
    //$('#txtNroEndereco').mask('00000');

    if ($('#Id').val() < 0)
        $('#txtDataNascimento').val('');

    var paramsCidade = {};
    paramsCidade['est_idt_estado'] = idEstado;
    $('#ddlEstado').cascade({ url: homePage + 'Inscrito/CarregarCidades', params: paramsCidade, paramName: 'est_idt_estado', childSelect: $('#ddlCidade'), dadosMinimo: 1, emptyText: 'Selecione uma cidade', carregando: "Carregando cidades...", async: false });

    $('input[name="deficiencia"]').on('change', function () {
        if ($('#rdDefSim').is(':checked'))
            $('.tem-deficiencia').slideDown(500);
        else {
            $('.tem-deficiencia').slideUp(500);
            $('input[name="deficiencia_qual"]').prop('checked', false);
            $('input[name="tratamento_especial"]').prop('checked', false);
            $('input[name="tratamento_especial_qual"]').val('');
        }
    });

    $('input[name="tratamento_especial"]').on('change', function () {
        if ($('#rdTraSim').is(':checked'))
            $('.necessita-tratamento').slideLeft(true, 500);
        else {
            $('.necessita-tratamento').slideLeft(false, 500);
            $('input[name="tratamento_especial_qual"]').val('');
        }
    });

    if (idEstadoCivil != null && idEstadoCivil != undefined && idEstadoCivil != '')
    { $('#ddlEstadoCivil').val(idEstadoCivil); }
    
    if (ativo)
        $('#rdAtivoSim').attr('checked', 'checked');
    else
        $('#rdAtivoNao').attr('checked', 'checked');

    if (isento)
        $('#rdIsentoSim').attr('checked', 'checked');
    else
        $('#rdIsentoNao').attr('checked', 'checked');

    if (possuiDef) {
        $('#rdDefSim').attr('checked', 'checked');
        $('input[name="deficiencia_qual"][value=' + flgDef + ']').prop('checked', true);
        $('input[name="deficiencia"]').trigger('change');

        if (necTratEsp)
        {
            $('#rdTraSim').attr('checked', 'checked');
            $('input[name="tratamento_especial"]').trigger('change');
        }
        else
            $('#rdTraNao').attr('checked', 'checked');
    }
    else
        $('#rdDefNao').attr('checked', 'checked');

    var minDataNasc = new Date();
    minDataNasc.dateAdd('year', -14);

    var cpfJaExistenteMessage = cpfJaExistente + (ehVestibular ? 'vestibular' : 'concurso');

    if (typeof ignorarValidacao === 'undefined' || !ignorarValidacao) {
        $('.inscritoForm')
            .on('init.field.bv', function (e, data) { removerIconeValidator(e, data); })
            .bootstrapValidator({
                container: 'tooltip',
                feedbackIcons: {
                    valid: 'glyphicon glyphicon-ok',
                    invalid: 'glyphicon glyphicon-remove',
                    validating: 'glyphicon glyphicon-refresh'
                }
                , fields: {
                    cargo: { validators: { notEmpty: { message: cargoObrigatorio } } }
                    , curso: { validators: { notEmpty: { message: cursoObrigatorio } } }
                    //, curso2: { validators: { notEmpty: { message: curso2Obrigatorio } } }
                    //, curso3: { validators: { notEmpty: { message: curso3Obrigatorio } } }
                    , nome: { validators: { notEmpty: { message: nomeObrigatorio } } }
                    , email: { validators: { notEmpty: { message: emailObrigatorio }, emailAddress: { message: emailFormatoInvalido } } }
                    , cpf: {
                        validators: {
                            notEmpty: { message: cpfObrigatorio }
                            , callback: {
                                callback: function (value)
                                {
                                    if (!validarCpf(value))
                                        return { valid: false, message: cpfInvalido };

                                    var id = $('#Id').val();
                                    var cpfExiste = false;

                                    // Verifica se é vestibular
                                    if ($('#ddlCurso2').length > 0)
                                        cpfExiste = validarCPFExistente(id, idConcurso, value);

                                    return !cpfExiste ? true : { valid: false, message: cpfJaExistenteMessage };
                                }
                            }
                        }
                    }
                    , rg: { validators: { notEmpty: { message: rgObrigatorio } } }
                    , dataNasc: { validators: { notEmpty: { message: dataNascObrigatorio }, date: { message: dataNascFormatoInvalido, format: 'DD/MM/YYYY', min: '01/01/1920', max: formatDateToDDMMYYYY(minDataNasc) } } }
                    , telefone: { validators: { notEmpty: { message: telefoneObrigatorio } } }
                    , endereco: { validators: { notEmpty: { message: enderecoObrigatorio } } }
                    , nroEndereco: { validators: { notEmpty: { message: nroEnderecoObrigatorio } } }
                    , bairro: { validators: { notEmpty: { message: bairroObrigatorio } } }
                    , cep: { validators: { notEmpty: { message: cepObrigatorio }, callback: { message: 'CEP inválido', callback: function (value, validator, $field) { return IsCEP(value, false); } } } }
                    , estado: { validators: { notEmpty: { message: estadoObrigatorio } } }
                    , cidade: { validators: { notEmpty: { message: cidadeObrigatorio } } }
                    , sexo: { validators: { notEmpty: { message: sexoObrigatorio } } }
                    , filhosmenores: { validators: {notEmpty: { message: filhosMenoresObrigatorio } } }
                    , deficiencia_qual: { validators: { notEmpty: { message: qualDeficienciaObrigatorio } } }
                    , tratamento_especial: { validators: { notEmpty: { message: tratamentoEspecialObrigatorio } } }
                    , tratamento_especial_qual: { validators: { notEmpty: { message: qualTratamentoEspecialObrigatorio } } }
                    , rendaMensal: { validators: { notEmpty: { message: rendaMensalObrigatorio } } }
                    , ensinoMedioEm: { validators: { notEmpty: { message: fEMObrigatorio } } }
                    , concluiuEnsinoFundamental: { validators: { notEmpty: { message: cEFObrigatorio } } }
                    , concluiuEnsinoMedio: { validators: { notEmpty: { message: cEMObrigatorio } } }
                    , atividadeRemunerada: { validators: { notEmpty: { message: atividadeRemuneradaObrigatorio } } }
                    , escolaridadePai: { validators: { notEmpty: { message: escolaridadePaiObrigatorio } } }
                    , escolaridadeMae: { validators: { notEmpty: { message: escolaridadaMaeObrigatorio } } }
                    , motivoOptarUniFae: { validators: { notEmpty: { message: motivoUniFaeObrigatorio } } }
                    , motivoOptarCurso: { validators: { notEmpty: { message: motivoCursoObrigatorio } } }
                    , tomouConhecimento: { validators: { notEmpty: { message: ficouSabendoObrigatorio } } }
                    , tomouConhecimentoOutros: { validators: { notEmpty: { message: ficouSabendoOutrosObrigatorio } } }
                }
            }).on('success.form.bv', function (e) {
                e.preventDefault(); // Prevent form submission
                salvar();
            });
    }

    $('i[data-bv-icon-for="cargo"]').css('left', '250px');
    $('i[data-bv-icon-for="nroEndereco"]').css('left', '50px');
    $('i[data-bv-icon-for="filhosmenores"]').css('left', '50px');
    $('i[data-bv-icon-for="cidade"]').css('left', '250px');
    $('i[data-bv-icon-for="deficiencia_qual"]').css('top', '5px');
    $('i[data-bv-icon-for="tratamento_especial"]').css('top', '10px').css('left', '100px');
}

function carregarTelaDadosInscritoVestibular(id, idConcurso, opcao2, opcao3, localProva, rendaFamiliar, tipoEnsinoMedio, ondeFundamental, ondeMedio, exerceAtividadeRemunerada, escolaridadePai, escolaridadeMae, motivoUnifae, motivoCurso, tomouConhecimento, ehMasculino)
{
    carregarOpcoesCurso(idConcurso, opcao2, 'ddlCurso2');
    carregarOpcoesCurso(idConcurso, opcao3, 'ddlCurso3');

    $('#ddlCurso2').val(opcao2);
    $('#ddlCurso3').val(opcao3);
    $('#ddlLocalProva').val(localProva);
    $('#ddlRendaMensal').val(rendaFamiliar);
    $('#ddlEnsinoMedioEm').val(tipoEnsinoMedio);
    $('#ddlConcluiuEnsinoFundamental').val(ondeFundamental);
    $('#ddlConcluiuEnsinoMedio').val(ondeMedio);
    $('#ddlAtividadeRemunerada').val(exerceAtividadeRemunerada);
    $('#ddlEscolaridadePai').val(escolaridadePai);
    $('#ddlEscolaridadeMae').val(escolaridadeMae);
    $('#ddlMotivoOptarUniFae').val(motivoUnifae);
    $('#ddlMotivoOptarCurso').val(motivoCurso);
    $('#ddlTomouConhecimento').val(tomouConhecimento);
    $('#ddlSexo').val(id < 0 ? "" : (ehMasculino ? "M" : "F"));
}

function changeComoFicouSabendo()
{
    $('#BotaoAvancar').removeAttr('disabled');

    if ($('#ddlTomouConhecimento').val() == "10")
    {
        $('.tomou-conhecimento-quais').show();
        $('.tomou-conhecimento-quais').find('input').focus();
    }
    else
    {
        $('.tomou-conhecimento-quais').hide();
        $('.tomou-conhecimento-quais').find('input').val('');
    }
}

function confirmar(idConcurso)
{
    $.blockUI({ message: mensagemSalvando, css: cssCarregando });
    var url = homePage + 'Concurso/ConfirmarInscricao';

    $.ajax({
        type: "POST",
        url: url,
        success: function (retorno) {
            if (retorno.Mensagem != undefined)
                alert(retorno.Mensagem);

            window.location.href = homePage + 'Concurso/InscricaoConfirmada';
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'confirmar()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function imprimirBoletoInscrito(idConcurso, idInscrito)
{
    abrirJanelaPopUpWait(mensagemGerandoBoletoBancario);
    gerarBoletoBancario(idConcurso, idInscrito, false);
}

function iniciarTelaCadastroInscrito(idConcurso, idInscrito)
{
    $.blockUI({ message: mensagemCarregando, css: cssCarregando });
    var url = homePage + 'Admin/Concurso/CarregarInscrito';

    $.ajax({
        type: "POST",
        url: url,
        data: { id: idInscrito },
        success: function (retorno) {
            $('.cadastro-inscrito').html('');
            $('.cadastro-inscrito').html(retorno.View);

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'iniciarTelaCadastroInscrito()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function salvar() {
    var id = $('#Id').val();
    var idConcurso = $('#IdConcurso').val();
    var cargo = $('#ddlCargo').val();
    var nome = $('#txtNomeCompleto').val();
    var email = $.trim($('#txtEmail').val());
    var cpf = removerTodosCaracteresMenosNumeros($('#txtCPF').val());
    var rg = $.trim($('#txtRG').val());
    var estado_rg = $('#ddlEstadoRG').val();
    var dataNasc = $('#txtDataNascimento').val();
    var estadocivil = $('#ddlEstadoCivil').val();
    var telefone = removerTodosCaracteresMenosNumeros($('#txtTelefone').val());
    var celular = removerTodosCaracteresMenosNumeros($('#txtCelular').val());
    var endereco = $('#txtEndereco').val();
    var nroEndereco = $('#txtNroEndereco').val();
    var complemento = $('#txtComplemento').val();
    var bairro = $('#txtBairro').val();
    var cep = removerTodosCaracteresMenosNumeros($('#txtCEP').val());
    var cidade = isInteger($('#ddlCidade').val()) ? $('#ddlCidade').val() : 9401;
    var filhosmenores = removerTodosCaracteresMenosNumeros($('#txtFilhosMenores').val());
    var possuiDef = $('#rdDefSim').is(':checked');
    var deficiencia_qual = possuiDef ? $('input[name="deficiencia_qual"]:checked').val() : null;
    var necessitaTratEsp = possuiDef ? $('#rdTraSim').is(':checked') : false;
    var tratamento_especial_qual = necessitaTratEsp ? $('#txtTratamentoEspecialQual').val() : null;
    var ico_bit_ativo = id <= 0 ? true : $('#rdAtivoSim').is(':checked');
    var ico_bit_pago = $('#Pagou').val() != '' ? eval($('#Pagou').val().toLowerCase()) : false;
    var ico_des_link_boleto = $('#txtLinkBoleto').val() === undefined ? '' : $('#txtLinkBoleto').val();
    var ico_des_browser_cadastro = id <= 0 ? detectarBrowse() : ($('#txtBrowserCadastro').val() === undefined ? '' : $('#txtBrowserCadastro').val());
    //console.log(ico_des_browser_cadastro);
    //console.log(id);
    var dataInscricao = $('#DataInscricao').val();
    var dataPagamento = $('#DataPagamento').val();
    var valorPago = $('#ValorPago').val();
    var isento = $('#rdIsentoSim').is(':checked');
    var nomeConcurso = $('#hdnNomeConcurso').length == 0 ? "" : $('#hdnNomeConcurso').val();
    var nomeCargo = $('#ddlCargo option:selected').html();
    var estado_rg_string = $('#ddlEstadoRG option:selected').html();
    var nomeCidade = $('#ddlCidade option:selected').html();
    var nomeEstado = $('#ddlEstado option:selected').html();
    var idEstado = $('#ddlEstado').val();
    var salvarBanco = $('#hdnSalvarBanco').length == 0 ? true : $('#hdnSalvarBanco').val();

    if (email === null || email === undefined || email == '')
    {
        alert(emailObrigatorio);
        return;
    }

    $.blockUI({ message: mensagemSalvando, css: cssCarregando });
    var url = homePage + 'Inscrito/Salvar';

    $.ajax({
        type: "POST",
        url: url,
        data: {
            id: id, idConcurso: idConcurso, idCargo: cargo, nome: nome, email: email, cpf: cpf, rg: rg, estado_rg: estado_rg, dataNasc: dataNasc, estadocivil: estadocivil, telefone: telefone, celular: celular, endereco: endereco, nroEndereco: nroEndereco, complemento: complemento, bairro: bairro, cep: cep, idCidade: cidade, filhosmenores: filhosmenores, possuiDef: possuiDef, deficiencia_qual: deficiencia_qual, necessitaTratEsp: necessitaTratEsp, tratamento_especial_qual: tratamento_especial_qual, ico_bit_ativo: ico_bit_ativo, ico_bit_pago: ico_bit_pago, dataInscricao: dataInscricao, dataPagamento: dataPagamento, valorPago: valorPago, isento: isento, salvarBanco: salvarBanco, nomeConcurso: nomeConcurso, nomeCargo: nomeCargo, estado_rg_string: estado_rg_string, nomeCidade: nomeCidade, nomeEstado: nomeEstado, idEstado: idEstado, linkBoleto: ico_des_link_boleto, browserCadastro: ico_des_browser_cadastro
        },
        success: function (retorno) {
            if (eval(salvarBanco) && retorno.Mensagem != undefined)
                alert(retorno.Mensagem);

            if (retorno.EhVestibular)
                salvarVestibular(retorno.Id);

            var tRetorno = telaRetornoSalvar;

            if (tRetorno === '')
                tRetorno = homePage + 'Admin/Concurso/Index_Inscrito/' + idConcurso;

            if (retorno.Sucesso)
                window.location.href = tRetorno;

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'salvar()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function salvarVestibular(id)
{
    var idConcurso = $('#IdConcurso').val();
    var idOpcao2 = $('#ddlCurso2').val() == "" ? null : $('#ddlCurso2').val();
    var idOpcao3 = $('#ddlCurso3').val() == "" ? null : $('#ddlCurso3').val();
    var localProva = isInteger($('#ddlLocalProva').val()) ? $('#ddlLocalProva').val() : 1;
    var ehMasculino = $('#ddlSexo').val() == "M";
    var indicadoPor = $('#txtNomeIndicado').val();
    var cursoIndicadoPor = $('#txtCursoIndicado').val();
    var semestreIndicadoPor = $('#txtSemestreCursoIndicado').val();
    var escolaridadeMae = $('#ddlEscolaridadeMae').val() == "0" ? null : $('#ddlEscolaridadeMae').val();
    var escolaridadePai = $('#ddlEscolaridadePai').val() == "0" ? null : $('#ddlEscolaridadePai').val();
    var exerceAtividadeRemunerada = $('#ddlAtividadeRemunerada').val() == "0" ? null : $('#ddlAtividadeRemunerada').val();
    var motivoOptouCurso = $('#ddlMotivoOptarCurso').val() == "0" ? null : $('#ddlMotivoOptarCurso').val();
    var motivoOptouUnifae = $('#ddlMotivoOptarUniFae').val() == "0" ? null : $('#ddlMotivoOptarUniFae').val();
    var rendaMensal = $('#ddlRendaMensal').val() == "0" ? null : $('#ddlRendaMensal').val();
    var tipoCEF = $('#ddlConcluiuEnsinoFundamental').val() == "" ? null : $('#ddlConcluiuEnsinoFundamental').val();
    var tipoCEM = $('#ddlConcluiuEnsinoMedio').val() == "0" ? null : $('#ddlConcluiuEnsinoMedio').val();
    var tipoEM = $('#ddlEnsinoMedioEm').val() == "0" ? null : $('#ddlEnsinoMedioEm').val();
    var tcnf = $('#ddlTomouConhecimento').val() == "0" ? null : $('#ddlTomouConhecimento').val();
    var tcnfOutros = $('#txtTomouConhecimentoOutros').val();
    var salvarBanco = $('#hdnSalvarBanco').length == 0 ? true : $('#hdnSalvarBanco').val();

    var url = homePage + 'Inscrito/SalvarVestibular';

    $.ajax({
        type: "POST",
        url: url,
        async: false,
        data: {
            id: id, idConcurso: idConcurso, idOpcao2: idOpcao2, idOpcao3: idOpcao3, localProva: localProva, ehMasculino: ehMasculino, indicadoPor: indicadoPor, cursoIndicadoPor: cursoIndicadoPor, semestreIndicadoPor: semestreIndicadoPor, escolaridadeMae: escolaridadeMae, escolaridadePai: escolaridadePai, exerceAtividadeRemunerada: exerceAtividadeRemunerada, motivoOptouCurso: motivoOptouCurso, motivoOptouUnifae: motivoOptouUnifae, rendaMensal: rendaMensal, tipoCEF: tipoCEF, tipoCEM: tipoCEM, tipoEM: tipoEM, tcnf: tcnf, tcnfOutros: tcnfOutros, salvarBanco: salvarBanco
        },
        success: function (retorno)
        { },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'salvarVestibular()', ResponseText: xhr.responseText });
        }
    });
}

function validarCPFExistente(id, idConcurso, cpf)
{
    var existe = false;
    var url = homePage + 'Inscrito/ValidarCPFExistente';

    $.ajax({
        type: "POST",
        url: url,
        async: false,
        data: { id: id, idConcurso: idConcurso, cpf: removerTodosCaracteresMenosNumeros(cpf) },
        success: function (retorno)
        { existe = retorno.Existe; },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'validarCPFExistente()', ResponseText: xhr.responseText });
        }
    });

    return existe;
}