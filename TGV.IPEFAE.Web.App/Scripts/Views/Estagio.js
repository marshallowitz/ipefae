var focoValidacao = '';

function abrirTelaPreviewCurriculo(id) {
    abrirJanelaPopUpWait('Carregando Preview do Currículo ...');
    gerarPreview(id);
}

function addValidatorIcons(field)
{
    if (field.parent().find('i.form-control-feedback').hasClass('glyphicon')) {
        if (field.val() == '') {
            field.parent().find('i.form-control-feedback').removeClass('glyphicon-ok').addClass('glyphicon-remove');
            field.parent().removeClass('has-success').addClass('has-error');
            field.parent().find('small.help-block').attr('data-bv-result', 'INVALID');
        }
        else {
            field.parent().find('i.form-control-feedback').addClass('glyphicon-ok').removeClass('glyphicon-remove');
            field.parent().addClass('has-success').removeClass('has-error');
            field.parent().find('small.help-block').attr('data-bv-result', 'VALID');
        }
    }

    field.parent().find('i.form-control-feedback').show();
}

function alterarAdicionarRemover(link, adicionar, idContainer, idLink, nameFocus) {
    // Esconde os links do item atual
    var parentLinkDiv = link.closest('.form-group');
    parentLinkDiv.find(idLink).hide();

    // Exibe os links do item anterior/posterior
    var outroLinkDiv = adicionar ? link.closest(idContainer).next() : link.closest(idContainer).prev();
    outroLinkDiv.find(idLink).show();

    // Exibe o link posterior ou esconde o atual
    var outroDiv = adicionar ? link.closest(idContainer).next() : link.closest(idContainer);

    if (adicionar) {
        link.closest(idContainer).next().show();
        link.closest(idContainer).next().find('input[name="' + nameFocus + '"]:first').focus(); // Define o foco do teclado
    }
    else {
        link.closest(idContainer).hide();
        link.closest(idContainer).find('input:not([name="hdnId"])').val('');
        link.closest(idContainer).prev().find('input[name="' + nameFocus + '"]:first').focus(); // Define o foco do teclado
    }

    $('#btnSalvar').removeAttr('disabled');
}

function atualizarTotalRelatorios()
{
    var url = homePage + 'Admin/Estagio/BuscarTotalRelatorios';

    $.ajax({
        type: "POST",
        url: url,
        success: function (data)
        {
            //console.log(data);
            $('.totalRelatorios').html(data.Total);
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'atualizarTotalRelatorios()', ResponseText: xhr.responseText });
        }
    });
}

function buildComboNumeros(ddl, totalItens, start)
{
    ddl.html('');

    for(var i = 0; i <= totalItens; i++)
    {
        if (i == 0)
            ddl.append($('<option>', { value: '', text: '' }));
        else if (start == undefined || i >= start)
            ddl.append($('<option>', { value: i, text: i + 'º' }));
    }
}

function buildComboPeriodo(considerarIntegral)
{
    $('#ddlPeriodo').html('');
    $('#ddlPeriodo').append($('<option>', { value: '', text: '' }));
    $('#ddlPeriodo').append($('<option>', { value: 1, text: optionManha }));
    $('#ddlPeriodo').append($('<option>', { value: 2, text: optionTarde }));
    $('#ddlPeriodo').append($('<option>', { value: 3, text: optionNoite }));

    if (considerarIntegral)
        $('#ddlPeriodo').append($('<option>', { value: 4, text: optionIntegral }));
}

function buscarCidadesComEstagiario()
{
    var url = homePage + 'Admin/Estagio/BuscarCidadesCadastradas';

    $.ajax({
        type: "POST",
        url: url,
        success: function (cidadesJson) {
            var normalize = function (term) {
                var ret = "";
                for (var i = 0; i < term.length; i++) {
                    ret += accentMap[term.charAt(i)] || term.charAt(i);
                }
                return ret;
            };

            $("#txtCidade").autocomplete({
                source: function (request, response) {
                    var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
                    response($.grep(cidadesJson, function (value) {
                        value = value.label || value.value || value;
                        return matcher.test(value) || matcher.test(normalize(value));
                    }));
                }
                , select: function (event, ui) {
                    event.preventDefault();
                    $("#txtCidade").val(ui.item.label);
                }
                , focus: function (event, ui) {
                    event.preventDefault();
                    $("#txtCidade").val(ui.item.label);
                }
            });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'buscarCidadesComEstagiario()', ResponseText: xhr.responseText });
        }
    });
}

function carregarCursosCapacitacoes(idUsuarioEstagio)
{
    $('div.cursos-capacitacoes').block({ message: "Carregando cursos e capacitações ...", css: cssCarregando });
    var url = homePage + 'Estagio/BuscarCursosCapacitacoes';

    $.ajax({
        type: "POST",
        url: url,
        data: { idUsuarioEstagio: idUsuarioEstagio },
        success: function (retorno) {
            $('.cursos-capacitacoes').html('');
            $('.cursos-capacitacoes').html(retorno.View);
            $('div.cursos-capacitacoes').unblock();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'carregarCursosCapacitacoes()', ResponseText: xhr.responseText });
            $('div.cursos-capacitacoes').unblock();
        }
    });
}

function carregarOutrosConhecimentos(idUsuarioEstagio)
{
    $('div.outros-conhecimentos').block({ message: "Carregando outros conhecimentos ...", css: cssCarregando });
    var url = homePage + 'Estagio/BuscarOutrosConhecimentos';

    $.ajax({
        type: "POST",
        url: url,
        data: { idUsuarioEstagio: idUsuarioEstagio },
        success: function (retorno) {
            $('.outros-conhecimentos').html('');
            $('.outros-conhecimentos').html(retorno.View);
            $('div.outros-conhecimentos').unblock();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'carregarOutrosConhecimentos()', ResponseText: xhr.responseText });
            $('div.outros-conhecimentos').unblock();
        }
    });
}

function carregarEstados(ddl, idEstado, exibirNome, idCidade) {
    var url = homePage + 'Estagio/CarregarEstados';

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

function changeDE01() {
    if ($('#rdTecnico').is(':checked')) {
        if ($('#ddlDE01')[0].selectedIndex > 0) {
            $('#lblDE02').html($('#ddlDE01 option:selected').text());
            $('#ddlDE02').removeAttr('disabled');
        }
        else {
            $('#lblDE02').html(labelSelecioneET01);
            $('#ddlDE02')[0].selectedIndex = 0;
            $('#ddlDE02').attr('disabled', 'disabled');
        }
    }
    else if ($('#rdEJA').is(':checked')) {
        switch ($('#ddlDE01')[0].selectedIndex) {
            case 0:
                $('#ddlDE02').html('');
                $('#ddlDE02').attr('disabled', 'disabled');
                break;
            case 1:
                buildComboNumeros($('#ddlDE02'), 3);
                $('#ddlDE02').removeAttr('disabled');
                break;
            case 2:
                buildComboNumeros($('#ddlDE02'), 9, 7);
                $('#ddlDE02').removeAttr('disabled');
                break;
        }
    }

    addValidatorIcons($('#ddlDE01'));
}

function definirOrdenacao(coluna)
{
    if (coluna.hasClass('selecionado'))
        return;

    if (coluna.hasClass('nome'))
        $('.atualizacao').removeClass('selecionado').addClass('order');
    else
        $('.nome').removeClass('selecionado').addClass('order');

    coluna.addClass('selecionado').removeClass('order');
    mais(true);
}

function excluir(id)
{
    if (!confirm('Deseja realmente remover este cadastro?'))
        return;

    $.blockUI({ message: 'Excluindo o Cadastro', css: cssCarregando });
    var url = homePage + 'Admin/Estagio/Excluir';

    $.ajax({
        type: "POST",
        url: url,
        data: { id: id },
        success: function (retorno)
        {
            $.unblockUI();

            pesquisar();
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'excluir()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function exibirSenha()
{
    if ($('#chkVer').is(':checked'))
        $('#txtSenhaAdmin').attr('type', 'text');
    else
        $('#txtSenhaAdmin').attr('type', 'password');
}

function gerarCSVPesquisa()
{
    $.blockUI({ message: mensagemGerarCSV, css: cssCarregando });
    var url = homePage + 'Admin/Estagio/GerarCSVPesquisa';
    var jsonFiltros = obterJsonFiltros();

    $.ajax({
        type: "POST",
        url: url,
        data: jsonFiltros,
        success: function (result)
        {
            if (result)
            {
                $('#iframeCSV').attr('src', homePage + 'Handlers/DownloadCSVHandler.ashx?tipo=est');
                $('#iframeCSV').load();

                setTimeout(function () { terminouDownload(homePage + 'Admin/Estagio/GerarCSVPesquisaConfirmacao'); }, 2000);
            }
            else
            {
                alert(mensagemNenhumEncontrado);
                $.unblockUI();
            }
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'gerarCSVPesquisa()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function gerarPDFPesquisa()
{
    $.blockUI({ message: mensagemGerarPDF, css: cssCarregando });
    var url = homePage + 'Admin/Estagio/GerarPDFPesquisa';
    var jsonFiltros = obterJsonFiltros();

    $.ajax({
        type: "POST",
        url: url,
        data: jsonFiltros,
        success: function (result)
        {
            if (result)
            {
                $('#iframePDF').attr('src', homePage + 'Handlers/DownloadPDFHandler.ashx');
                $('#iframePDF').load();

                setTimeout(function () { terminouDownload(homePage + 'Admin/Estagio/GerarPDFPesquisaConfirmacao'); atualizarTotalRelatorios(); }, 2000);
            }
            else
            {
                alert(mensagemNenhumEncontrado);
                $.unblockUI();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'gerarPDFPesquisa()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function gerarPreview(id)
{
    var url = homePage + 'Estagio/PreviewCurriculo';

    $.ajax({
        type: "POST",
        url: url,
        data: { id: id },
        success: function (retorno) {
            var w = janelaPopUp;
            gerarPreviewChamadaFuncao(w, retorno.View);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'gerarPreview()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

function gerarPreviewChamadaFuncao(w, preview)
{
    if (!janelaPopUp.getTitulo || janelaPopUp.getTitulo() === '')
        setTimeout(function () { gerarPreviewChamadaFuncao(w, preview); }, 1000);
    else {
        w.document.title = ".:: IPEFAE ::.";
        $(w.document.body).html(preview);
    }
}

function incluirFoto(e) {
    $.blockUI({ message: mensagemRealizandoUpload, css: cssCarregando });
    var files = e.target.files;
    if (files.length > 0) {
        if (window.FormData !== undefined) {
            var data = new FormData();
            for (var x = 0; x < files.length; x++) {
                data.append("file" + x, files[x]);
            }

            $.ajax({
                type: "POST",
                url: homePage + '/Estagio/UploadFotoCurriculo',
                contentType: false,
                processData: false,
                data: data,
                success: function (retorno) {
                    $('#txtUploadFoto').val('');

                    if (retorno.Sucesso)
                    {
                        $('#fotoEstagio').attr('src', retorno.PathFoto);
                        $('#hdnTemFoto').val('true');
                        $('.remover-foto').show();
                    }

                    $.unblockUI();
                },
                error: function (xhr, status, p3, p4) {
                    alertaErroJS({ NomeFuncao: 'incluirFoto()', ResponseText: xhr.responseText });
                    $.unblockUI();
                }
            });
        }
    }
}

function iniciarTelaAdminListagem()
{
    atualizarTotalRelatorios();
    buscarCidadesComEstagiario();
    $('#txtCPF').mask('999.999.999-99');
    $('#txtNome').focus();

    $('#btnPesquisar').on('click', function () { pesquisar(); });
    $('#btnCSV').on('click', function () { gerarCSVPesquisa(); });
    $('#btnPDF').on('click', function () { gerarPDFPesquisa(); });

    $('.order').on('click', function () { definirOrdenacao($(this)); });
}

function iniciarTelaCadastroEstagio(idEstado, idCidade, idEstadoCarteiraTrabalho, possuiDef, possuiExp, ehMasc, tipo_estudo, periodo, anosemestre, ehEad, flgTipo, flgEstadoCivil, motivoDesativacao)
{
    inicializarValidationTelaCadastro();

    $('#ddlDE01').on('change', function () { changeDE01(); });

    var id = $('#hdnId').val();
    $('.remover-foto').on('click', function () { removerFoto() });

    carregarEstados($('#ddlEstado'), idEstado, true, idCidade);
    //carregarEstados($('#ddlCarteiraTrabalhoUF'), idEstadoCarteiraTrabalho, false);

    $('#txtCPF').mask('999.999.999-99');

    var SPMaskBehavior = function (val) { return val.replace(/\D/g, '').length === 11 ? '(00) 00000-0000' : '(00) 0000-00009'; };
    var spOptions = { onKeyPress: function (val, e, field, options) { field.mask(SPMaskBehavior.apply({}, arguments), options); } };
    $('#txtTelefone').mask(SPMaskBehavior, spOptions);
    $('#txtCelular').mask(SPMaskBehavior, spOptions);

    $('#txtDataRGExpedicao').mask('00/00/0000');
    $('#txtDataNascimento').mask('00/00/0000');
    $('#txtInicioDE').mask('00/0000');
    $('#txtTerminoDE').mask('00/0000');
    $('input[name="dataInicio"]').mask('00/0000');
    $('input[name="dataTermino"]').mask('00/0000');
    $('#txtCEP').mask('00000-000');
    $('#txtQtdadeFilhos').mask('00');
    $('#txtNroEndereco').mask('00000');

    var paramsCidade = {};
    paramsCidade['est_idt_estado'] = idEstado;
    $('#ddlEstado').cascade({ url: homePage + 'Estagio/CarregarCidades', params: paramsCidade, paramName: 'est_idt_estado', childSelect: $('#ddlCidade'), dadosMinimo: 1, emptyText: 'Selecione uma cidade', carregando: "Carregando cidades...", async: false });

    $('#txtEmail').on('change', function () { verificarEmailExistente() });

    $('input[name="deficiencia"]').on('change', function () {
        if ($('#rdDefSim').is(':checked'))
            $('.tem-deficiencia').slideDown(500);
        else {
            $('.tem-deficiencia').slideUp(500);
            $('input[name="deficiencia_qual"]').prop('checked', false);
        }
    });

    $('input[name="tipo_ensino"]').on('change', function () { mudarTipoEnsino(true, null); });

    $('#txtUploadFoto').on('change', function (e) { incluirFoto(e); });

    $('#txtDataNascimento').on('change', function (e) {
        var idade = calcularIdade($(this).val());
        $('#idade').html('(' + idade + labelIdadeAnos + ')');
    });

    $('input[name="experiencia_profissional"]').on('change', function () { mudarExperienciaProfissional(id); });

    if (id <= 0) {
        $('#txtDataRGExpedicao').val('');
        $('#txtDataNascimento').val('');
    }
    else
    {
        if (ehMasc)
            $('#ddlSexo').val('M');
        else
            $('#ddlSexo').val('F');

        $('#ddlEstadoCivil').val(flgEstadoCivil);

        $('#txtDataNascimento').trigger('change');

        $('input[name="tipo_ensino"][value="' + tipo_estudo + '"]').attr('checked', 'checked');
        mudarTipoEnsino(false, flgTipo);

        $('#ddlPeriodo').val(periodo);
        $('#ddlPeriodo').val(periodo);

        switch(tipo_estudo)
        {
            case 'M':
                $('#ddlDE01').val(anosemestre);
                break;
            case 'T':
                $('#ddlDE02').val(anosemestre);
            case 'E':
                $('#ddlDE02').val(anosemestre);
                $('#ddlDE02').removeAttr('disabled');
                break;
            case 'S':
                $('#ddlDE01').val(anosemestre);
                var idEAD = ehEad ? 2 : 1;
                $('#ddlDE02').val(idEAD);
                break;
        }

        if (eval($('#hdnTemFoto').val()))
        {
            $('#fotoEstagio').attr('src', homePage + 'Anexos/Estagio/' + id + '.jpg');
            $('.remover-foto').show();
        }

        // Area administrativa
        if ($('.area_adm').length > 0)
        {
            var ativo = eval($('#hdnAt').val());
            var consideraPesquisa = eval($('#hdnCP').val());
            var estaEstagiando = eval($('#hdnEE').val());

            if (ativo)
                $('#rdAtiSim').attr('checked', 'checked');
            else
                $('#rdAtiNao').attr('checked', 'checked');

            if (consideraPesquisa)
                $('#rdPesSim').attr('checked', 'checked');
            else
                $('#rdPesNao').attr('checked', 'checked');

            if (estaEstagiando)
                $('#rdEstSim').attr('checked', 'checked');
            else
                $('#rdEstNao').attr('checked', 'checked');

            $('#ddlMotivoDesativacao').val(motivoDesativacao);
        }
    }

    if (possuiDef) {
        $('#rdDefSim').attr('checked', 'checked');
        $('input[name="deficiencia_qual"][value=' + flgDef + ']').prop('checked', true);
        $('input[name="deficiencia"]').trigger('change');
    }
    else
        $('#rdDefNao').attr('checked', 'checked');

    if (possuiExp)
    {
        $('#rdExpSim').attr('checked', 'checked');
        mudarExperienciaProfissional(id);
    }
    else
        $('#rdExpNao').attr('checked', 'checked');

    carregarCursosCapacitacoes(id);
    carregarOutrosConhecimentos(id);
    
    $('.modal-senha').on('show.bs.modal', function () {
        $('.form-group').removeClass('has-error').removeClass('has-success');
        $('.form-group').find('small.help-block').hide();
        $('.form-group').find('i.form-control-feedback').hide();
        $('.modal-senha').find('#txtSenha').val('');
        $('.modal-senha').find('#txtConfirmacaoSenha').val('');
    });
    $('.modal-senha').on('shown.bs.modal', function () { $('.modal-senha').find('#txtSenha').focus(); });

    // Autocomplete
    var cursosArray = $.map(cursosScript, function (value, key) { return { value: value, data: key }; });
    var normalize = function (term) {
        var ret = "";
        for (var i = 0; i < term.length; i++) {
            ret += accentMap[term.charAt(i)] || term.charAt(i);
        }
        return ret;
    };

    $("#txtNomeCurso").autocomplete({
        source: function (request, response) {
            if ($('#rdEnsinoSuperior').is(':checked')) {
                var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
                response($.grep(cursosArray, function (value) {
                    value = value.label || value.value || value;
                    return matcher.test(value) || matcher.test(normalize(value));
                }));
            }
        },
        change: function (event, ui) {
            if (!ui.item && $('#rdEnsinoSuperior').is(':checked'))
            {
                $("#txtNomeCurso").val('');
                $("#txtNomeCurso").parent('div').removeClass('has-success');
                $("#txtNomeCurso").parent('div').addClass('has-error');
                $("#txtNomeCurso").parent('div').find('i').removeClass('glyphicon-ok');
                $("#txtNomeCurso").parent('div').find('i').addClass('glyphicon-remove');
                $("#txtNomeCurso").parent('div').find('small').attr('data-bv-for', 'INVALID');
            }
        }
    });

    $('#txtNomeCompleto').focus();
}

function inicializarValidationTelaCadastro()
{
    $('.estCadastroForm')
    .on('init.field.bv', function (e, data) { removerIconeValidator(e, data); })
    .bootstrapValidator({
        container: 'tooltip',
        excluded: [':disabled', ':hidden', ':not(:visible)'],
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        }
        , live: 'disabled'
        , fields: {
            nome: { validators: { notEmpty: { message: nomeObrigatorio } } }
            , email: { validators: { notEmpty: { message: emailObrigatorio }, emailAddress: { message: formatoInvalidoEmail } } }
            , cpf: { validators: { notEmpty: { message: cpfObrigatorio } } }
            , rg: { validators: { notEmpty: { message: rgObrigatorio } } }
            , dataRGExpedicao: {
                validators: {
                    notEmpty: { message: dataExpedicaoObrigatoria },
                    date: { message: dataExpedicaoFormatoInvalido, format: 'DD/MM/YYYY' }
                }
            }
            , carteiraTrabalhoNumero: { validators: { notEmpty: { message: carteiraTrabalhoNumeroObrigatorio } } }
            , carteiraTrabalhoSerie: { validators: { notEmpty: { message: carteiraTrabalhoSerieObrigatorio } } }
            , carteiraTrabalhoUF: { validators: { notEmpty: { message: carteiraTrabalhoUFObrigatorio } } }
            , sexo: { validators: { notEmpty: { message: sexoObrigatorio } } }
            , dataNasc: {
                validators: {
                    notEmpty: { message: dataNascObrigatorio },
                    date: { message: dataNascFormatoInvalido, format: 'DD/MM/YYYY' }
                }
            }
            , telefone: { validators: { notEmpty: { message: telefoneObrigatorio } } }
            , endereco: { validators: { notEmpty: { message: enderecoObrigatorio } } }
            , nroEndereco: { validators: { notEmpty: { message: nroEnderecoObrigatorio } } }
            , bairro: { validators: { notEmpty: { message: bairroObrigatorio } } }
            , cep: { validators: { notEmpty: { message: cepObrigatorio } } }
            , estado: { validators: { notEmpty: { message: estadoObrigatorio } } }
            , cidade: { validators: { notEmpty: { message: cidadeObrigatorio } } }
            , qtdadeFilhos: { validators: { notEmpty: { message: qtdadeFilhosObrigatorio } } }
            , deficiencia_qual: { validators: { notEmpty: { message: deficiencia_qualObrigatorio } } }
            , tipo_ensino: { validators: { notEmpty: { message: tipo_ensinoObrigatorio } } }
            , nomeEscola: { validators: { notEmpty: { message: nomeEscolaObrigatorio } } }
            , de01: { validators: { notEmpty: { message: de01Obrigatorio } } }
            , de02: { validators: { notEmpty: { message: de02Obrigatorio } } }
            , periodo: { validators: { notEmpty: { message: periodoObrigatorio } } }
            , nomeCurso: { validators: { notEmpty: { message: nomeCursoObrigatorio } } }
            , dataInicioDE: {
                validators: {
                    notEmpty: { message: dataInicioDEObrigatorio },
                    callback: {
                        message: dataInicioDEFormatoInvalido,
                        callback: function (value, validator) {
                            var data = value.split('/');
                            return (data.length == 2 && data[0] <= 12 && data[1] > 1999);
                        }
                    }
                }
            }
            , dataTerminoDE: {
                validators: {
                    notEmpty: { message: dataTerminoDEObrigatorio },
                    callback: {
                        message: dataTerminoDEFormatoInvalido,
                        callback: function (value, validator) {
                            var data = value.split('/');

                            if (!(data.length == 2 && data[0] <= 12 && data[1] > 1999))
                                return { valid: false, message: dataTerminoDEFormatoInvalido };

                            var dataInicio = $('#txtInicioDE').val().split('/');

                            if (!(dataInicio.length == 2 && dataInicio[0] <= 12 && dataInicio[1] > 1999))
                                return true;

                            if (data[1] < dataInicio[1] || (data[1] == dataInicio[1] && data[0] <= dataInicio[0]))
                                return { valid: false, message: dataTerminoMaiorDataInicio };
                            else
                                return true;
                        }
                    }
                }
            }
            , nomeEmpresa: { validators: { notEmpty: { message: nomeEmpresaObrigatorio } } }
            , cargo: { validators: { notEmpty: { message: cargoObrigatorio } } }
            , atividadesDesenvolvidas: { validators: { notEmpty: { message: atividadesDesenvolvidasObrigatorio } } }
            , dataInicio: {
                validators: {
                    notEmpty: { message: dataInicioDEObrigatorio },
                    callback: {
                        message: dataInicioDEFormatoInvalido,
                        callback: function (value, validator) {
                            var data = value.split('/');
                            return (data.length == 2 && data[0] <= 12 && data[1] > 1999);
                        }
                    }
                }
            }
            , dataTermino: {
                validators: {
                    callback: {
                        message: dataTerminoDEFormatoInvalido,
                        callback: function (value, validator, field) {
                            if (field.attr('tag') == 1 && value == '')
                                return true;

                            if (value == '')
                                return { valid: false, message: dataTerminoDEObrigatorio };

                            var data = value.split('/');
                            return (data.length == 2 && data[0] <= 12 && data[1] > 1999);
                        }
                    }
                }
            }
            , UploadFile: {
                validators: {
                    callback: {
                        message: arquivoObrigatorio,
                        callback: function (value, validator, field) {
                            if (value == '' && ($('#fotoEstagio').attr('src') == '' || $('#fotoEstagio').attr('src').indexOf('empty.png') >= 0))
                                return { valid: false, message: arquivoObrigatorio };

                            return true;
                        }
                    }
                    , file: {
                        extension: 'jpg,png,bmp',
                        type: 'image/*',
                        maxSize: 2097152,   // 2048 * 1024
                        message: arquivoFormatoInvalido
                    }
                }
            }
        }
    }).on('success.form.bv', function (e) {
        e.preventDefault(); // Prevent form submission
        focoValidacao = '';

        if (!validateFoto()) {
            focoValidacao.focus();
            return;
        }

        if (!validateDadosEscolares(true)) {
            focoValidacao.focus();
            return;
        }

        if (!validateExperienciasProfissionais()) {
            focoValidacao.focus();
            return;
        }

        $('#btnSalvar').removeAttr('disabled');
        salvar(false);
    });

    $('.frmSenha')
    .on('init.field.bv', function (e, data) { removerIconeValidator(e, data); })
    .bootstrapValidator({
        container: 'tooltip',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        }
        , fields: {
            senha: { validators: { notEmpty: { message: senhaObrigatorio } } }
            , confirmacaoSenha: { validators: { notEmpty: { message: confirmacaoSenhaObrigatorio }, identical: { field: 'senha', message: senhaConfirmacaoDiferente } } }
        }
    }).on('success.form.bv', function (e) {
        e.preventDefault(); // Prevent form submission
        salvar(true);
    });
}

function iniciarTelaInicialEstagio() {
    $('.modal-reenviar-senha').find('#txtCPF').mask('999.999.999-99');

    $('.estLoginForm')
    .on('init.field.bv', function (e, data) { removerIconeValidator(e, data); })
    .bootstrapValidator({
        container: 'tooltip',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        }
        , fields: {
            email: {
                validators: {
                    notEmpty: { message: emailObrigatorio },
                    emailAddress: { message: formatoInvalidoEmail }
                }
            },
            senha: {
                validators: {
                    notEmpty: { message: senhaObrigatoria }
                }
            }
        }
    }).on('success.form.bv', function (e) {
        e.preventDefault(); // Prevent form submission
        realizarLogin();
    });

    var validatorCPF = $('.cpfForm')
    .on('init.field.bv', function (e, data) { removerIconeValidator(e, data); })
    .bootstrapValidator({
        container: 'tooltip',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        }
        , fields: { cpf: { validators: { notEmpty: { message: cpfObrigatorio } } } }
    }).on('success.form.bv', function (e) {
        e.preventDefault(); // Prevent form submission
        reenviarSenha();
    });

    $('.modal-reenviar-senha').on('hide.bs.modal', function () {
        $('.form-group').removeClass('has-error').removeClass('has-success');
        $('.form-group').find('small.help-block').hide();
        $('.form-group').find('i.form-control-feedback').hide();
        $('.modal-reenviar-senha').find('#txtCPF').val('');
    });
    $('.modal-reenviar-senha').on('shown.bs.modal', function () { $('.modal-reenviar-senha').find('#txtCPF').focus(); });
}

function mais(limpar) {
    $('.listagem').block({ message: mensagemPesquisando, css: cssCarregando });

    if (limpar !== undefined && limpar)
        $('#hdnPagina').val('0');

    var pagina = $('#hdnPagina').val();
    pagina++;
    $('#hdnPagina').val(pagina);

    var jsonFiltros = obterJsonFiltros();
    var url = homePage + 'Admin/Estagio/Pesquisar';

    $.ajax({
        type: "POST",
        url: url,
        data: jsonFiltros,
        success: function (result) {
            if (limpar !== undefined && limpar)
                $('.scrollTableBody').html('');

            if (result.TotalItens > 0)
                $('.scrollTableBody').append(result.View);
            else
                $('.mais').hide();

            $('.listagem').unblock();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'mais()', ResponseText: xhr.responseText });
            $('.listagem').unblock();
        }
    });
}

function montarListaCursos(ehCurso) {
    var container = ehCurso ? ".container_curso_capacitacao_" : ".container_outro_conhecimento_";
    var lista = "";

    for (var i = 1; i <= 5; i++) {
        lista += "||";
        lista += "item:" + i + ",";
        lista += "id:" + $(container + i).find('input[name="hdnId"]').val() + ",";
        lista += "nomeCurso:" + $(container + i).find('input[name="nomeCurso"]').val();

        if (ehCurso)
            lista += ",duracao:" + $(container + i).find('input[name="duracao"]').val();
    }

    return lista;
}

function montarListaExperiencia() {
    var lista = "";

    for (var i = 1; i <= 3; i++) {
        lista += "||";
        lista += "item:" + i + ",";
        lista += "id:" + $('.container_experiencia_' + i).find('input[name="hdnId"]').val() + ",";
        lista += "nomeEmpresa:" + $('.container_experiencia_' + i).find('input[name="nomeEmpresa"]').val() + ",";
        lista += "cargo:" + $('.container_experiencia_' + i).find('input[name="cargo"]').val() + ",";
        lista += "datainicio:" + $('.container_experiencia_' + i).find('input[name="dataInicio"]').val() + ",";
        lista += "datatermino:" + $('.container_experiencia_' + i).find('input[name="dataTermino"]').val() + ",";
        lista += "atividadesDesenvolvidas:" + encodeURIComponent($('.container_experiencia_' + i).find('textarea[name="atividadesDesenvolvidas"]').val());
    }

    return lista;
}

function mudarExperienciaProfissional(idUsuarioEstagio)
{
    if ($('#rdExpNao').is(':checked'))
        $('.tem-experiencia-profissional').fadeOut(500).promise().done(function () {
            validateExperienciaProfissional(1, false);
            validateExperienciaProfissional(2, false);
            validateExperienciaProfissional(3, false);

            $('.container_experiencia_2').hide();
            $('.container_experiencia_3').hide();
        });
    else {
        validateExperienciaProfissional(1, true);
        $('.tem-experiencia-profissional').fadeIn(500);
        $('.container_experiencia_1').find('input[name="nomeEmpresa"]').focus();
    }

    $('#btnSalvar').removeAttr('disabled');

    return false;
}

function mudarTipoEnsino(limparDados, flgTipo)
{
    if (limparDados) {
        $('.tipo-ensino input').val('');
        $('#ddlDE02').html('');
        $('#txtNomeCurso').val('');
    }

    $('.tipo-ensino').fadeOut(500).promise().done(function () { $('.tipo-ensino').slideDown(1000); });
    $('.terceira-coluna').show();
    $('#ddlDE01').off('blur');
    $('#ddlDE02').removeAttr('disabled');
    $('#txtNomeCurso').removeAttr('readonly');
    $('#txtNomeEscola').focus();

    if ($('#rdEnsinoMedio').is(':checked')) {
        $('#txtNomeCurso').val(optionEnsinoMedio);
        $('#txtNomeCurso').attr('readonly', 'readonly');

        // Carrega o combo com os três anos
        $('#lblDE01').html(labelEM01);
        buildComboNumeros($('#ddlDE01'), 3);

        // Esconde a terceira coluna
        $('.terceira-coluna').hide();

        // Carrega o período com apenas três tipos
        $('#lblPeriodo').html(labelPeriodo);
        buildComboPeriodo(false);
    }
    else if ($('#rdTecnico').is(':checked')) {
        // Carrega o tipo com os 3 valores
        $('#lblDE02').html(labelSelecioneET01);
        $('#lblDE01').html(labelET01);
        $('#ddlDE01').html('');
        $('#ddlDE01').append($('<option>', { value: '', text: '' }));
        $('#ddlDE01').append($('<option>', { value: 1, text: optionTipoModulo }));
        $('#ddlDE01').append($('<option>', { value: 2, text: optionTipoTermo }));
        $('#ddlDE01').append($('<option>', { value: 3, text: optionTipoSemestre }));

        // Carrega o combo com 10 anos
        buildComboNumeros($('#ddlDE02'), 10);
        $('#ddlDE02').attr('disabled', 'disabled');
        
        if (flgTipo != null) {
            $('#ddlDE01').val(flgTipo);
            $('#ddlDE01').trigger('change');
        }

        // Carrega o período com apenas três tipos
        buildComboPeriodo(false);
    }
    else if ($('#rdEJA').is(':checked')) {
        $('#txtNomeCurso').val(optionEJA);
        $('#txtNomeCurso').attr('readonly', 'readonly');

        // Carrega o combo Ensino Médio / Ensino Fundamental
        $('#lblDE01').html(labelEJA01);
        $('#ddlDE01').html('');
        $('#ddlDE01').append($('<option>', { value: '', text: '' }));
        $('#ddlDE01').append($('<option>', { value: 1, text: optionEJAEM }));
        $('#ddlDE01').append($('<option>', { value: 2, text: optionEJAEF }));

        // Carrega a terceira coluna com 1,2,3 se ensino medio e 7, 8, 9 se ensino fundamental
        $('#lblDE02').html(labelEJA02);
        $('#ddlDE02').attr('disabled', 'disabled');

        if (flgTipo != null) {
            $('#ddlDE01').val(flgTipo);
            $('#ddlDE01').trigger('change');
        }

        // Carrega o período com apenas três tipos
        buildComboPeriodo(false);
    }
    else // if ($('#rdEnsinoSuperior').is(':checked'))
    {
        // Carrega o combo com 10 semestres
        $('#lblDE01').html(labelES01);
        buildComboNumeros($('#ddlDE01'), 10);

        // Carrega a terceira coluna com as modalidades presencial e ead
        $('#lblDE02').html(labelES02);
        $('#ddlDE02').html('');
        $('#ddlDE02').append($('<option>', { value: '', text: '' }));
        $('#ddlDE02').append($('<option>', { value: 1, text: optionModalidadePresencial }));
        $('#ddlDE02').append($('<option>', { value: 2, text: optionModalidadeEAD }));

        // Carrega o período com quatro tipos
        buildComboPeriodo(true);
    }

    validateDadosEscolares(true);
    $('#btnSalvar').removeAttr('disabled');
}

function obterJsonFiltros() {
    var nome = $('#txtNome').val();
    var curso = $('#txtCurso').val();
    var semAno = $('#ddlAno').val() == '' ? null : $('#ddlAno').val();
    var estagiando = $('#ddlSituacao').val() == '' ? null : eval($('#ddlSituacao').val());
    var visualizacao = eval($('#ddlVisualizacao').val());
    var cpf = removerTodosCaracteresMenosNumeros($('#txtCPF').val());
    var cidade = $('#txtCidade').val();
    var ordem = $('.selecionado').hasClass('nome') ? 'N' : 'A';

    var pagina = $('#hdnPagina').val();

    return { nome: nome, curso: curso, semAno: semAno, estagiando: estagiando, cpf: cpf, visualizacao: visualizacao, cidade: cidade, ordem: ordem, pagina: pagina };
}

function pesquisar() {
    $.blockUI({ message: mensagemPesquisando, css: cssCarregando });
    $('.atualizacao').removeClass('selecionado').addClass('order');
    $('.nome').addClass('selecionado').removeClass('order');
    $('#hdnPagina').val('1');
    var jsonFiltros = obterJsonFiltros();
    var url = homePage + 'Admin/Estagio/Pesquisar';

    $.ajax({
        type: "POST",
        url: url,
        data: jsonFiltros,
        success: function (result) {
            $('.scrollTable').fadeOut(500);
            $('.emptyTable').fadeOut(500);
            $('.scrollTableBody').html('');

            if (result.TotalItens > 0) {
                $('.scrollTableBody').html(result.View);
                $('.scrollTable').fadeIn(1000);
                $('#btnPDF').show();
                $('.mais').show();
            }
            else
            {
                $('.emptyTable').fadeIn(1000);
                $('#btnPDF').hide();
            }

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'pesquisar()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function realizarLogin() {
    $.blockUI({ message: mensagemBlockUILogin, css: cssCarregando });
    var url = homePage + 'Estagio/RealizarLogin';
    var email = $('.estLoginForm').find('#txtEmail').val();
    var senha = $('.estLoginForm').find('#txtSenha').val();

    $.ajax({
        type: "POST",
        url: url,
        data: { email: email, senha: senha },
        success: function (retorno) {
            if (retorno.Sucesso)
                window.location.href = homePage + 'Estagio/Cadastro/' + retorno.IdEstagiario;
            else {
                alert(erroLogin);
                $('#txtEmail').focus();
            }

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
            alertaErroJS({ NomeFuncao: 'realizarLogin()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

function reenviarSenha() {
    $('.modal-reenviar-senha').find('.modal-content').block({ message: mensagemReenviandoSenha, css: cssCarregando });
    var url = homePage + 'Estagio/EnviarSenhaPorEmail';
    var cpf = removerTodosCaracteresMenosNumeros($('.modal-reenviar-senha').find('#txtCPF').val());

    $.ajax({
        type: "POST",
        url: url,
        data: { cpf: cpf },
        success: function (retorno) {
            if (retorno)
                alert(mensagemEmailEnviadoSucesso);
            else {
                $('.modal-reenviar-senha').find('.cpf-nao-encontrado').fadeIn(1000).fadeOut(3000);
                $('.modal-reenviar-senha').find('#txtCPF').focus();
            }

            $('.modal-reenviar-senha').find('.modal-content').unblock();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $('.modal-reenviar-senha').find('.modal-content').unblock();
            alertaErroJS({ NomeFuncao: 'reenviarSenha()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

function removerFoto() {
    $('#txtUploadFoto').val('');
    $('#fotoEstagio').attr('src', homePage + 'Content/imagens/empty.png');
    //$('#fotoEstagio').hide();
    $('#hdnTemFoto').val('false');
    $('.remover-foto').hide();
}

function salvar(gravouSenha)
{
    var id = $('#hdnId').val();

    if (id <= 0 && !gravouSenha)
        $('.modal-senha').modal('show');
    else
        salvarUsuarioExistente(id);
}

function salvarUsuarioExistente(id)
{
    // Esconde o popUp da senha se estiver aberto
    $('.modal-senha').modal('hide');

    var nome = $('#txtNomeCompleto').val();
    var email = $('#txtEmail').val();
    var cpf = removerTodosCaracteresMenosNumeros($('#txtCPF').val());
    var rg = $('#txtRG').val();
    var dataExpedicao = $('#txtDataRGExpedicao').val();
    var nroCarteira = $('#txtCarteiraTrabalhoNumero').val();
    var serieCarteira = $('#txtCarteiraTrabalhoSerie').val();
    var ufCarteira = $('#ddlCarteiraTrabalhoUF').val();
    var estadoCivil = $('#ddlEstadoCivil').val();
    var dataNasc = $('#txtDataNascimento').val();
    var telefone01 = removerTodosCaracteresMenosNumeros($('#txtTelefone').val());
    var telefone02 = removerTodosCaracteresMenosNumeros($('#txtCelular').val());
    var endereco = $('#txtEndereco').val();
    var nroEndereco = $('#txtNroEndereco').val();
    var complemento = $('#txtComplemento').val();
    var bairro = $('#txtBairro').val();
    var cep = removerTodosCaracteresMenosNumeros($('#txtCEP').val());
    var idCidade = $('#ddlCidade').val();
    var qtdadeFilhos = $('#txtQtdadeFilhos').val();
    var possuiDef = $('#rdDefSim').is(':checked');
    var defQual = possuiDef ? $('input[name="deficiencia_qual"]:checked').val() : null;
    var objetivos = $('#txtObjetivos').val();
    var idDadosEscolares = $("#hdnIdDadosEstagio").val();
    var tipo_ensino = $('input[name="tipo_ensino"]:checked').val();
    var nomeEscola = $('#txtNomeEscola').val();
    var de01 = $('#ddlDE01').val();
    var de02 = $('#ddlDE02').val();
    var periodo = $('#ddlPeriodo').val();
    var nomeCursoEscola = $('#txtNomeCurso').val();
    var dataInicioDE = $('#txtInicioDE').val();
    var dataTerminoDE = $('#txtTerminoDE').val();
    var possuiExp = $('#rdExpSim').is(':checked');
    var listaExp = montarListaExperiencia();
    var listaCursos = montarListaCursos(true);
    var listaOutros = montarListaCursos(false);
    var ehMasc = $('#ddlSexo').val() == "M";
    var temFoto = $('#hdnTemFoto').val();
    var senha = $('#txtSenha').val();

    var ehAdmin = $('.area_adm').length > 0;
    var considerar_busca = true;
    var estagiando = false;
    var ativo = true;
    var motivoDesativacao = null;
    var observacoesAdmin = null;

    if (ehAdmin)
    {
        considerar_busca = $('#rdPesSim').is(':checked');
        estagiando = $('#rdEstSim').is(':checked');
        ativo = $('#rdAtiSim').is(':checked');
        motivoDesativacao = $('#ddlMotivoDesativacao').val();
        observacoesAdmin = $('#txtObsAdmin').val();
    }

    $('.container.marginBottom-30').addClass('whirl');
    var url = homePage + 'Estagio/Salvar';

    $.ajax({
        type: "POST",
        url: url,
        data:
        {
            id: id, nome: nome, email: email, senha: senha, cpf: cpf, rg: rg, dataExpedicao: dataExpedicao, nroCarteira: nroCarteira, serieCarteira: serieCarteira, ufCarteira: ufCarteira, estadoCivil: estadoCivil, dataNasc: dataNasc, telefone01: telefone01, telefone02: telefone02, endereco: endereco, nroEndereco: nroEndereco, complemento: complemento, bairro: bairro, cep: cep, idCidade: idCidade, qtdadeFilhos: qtdadeFilhos, possuiDef: possuiDef, defQual: defQual, objetivos: objetivos, idDadosEscolares: idDadosEscolares, tipo_ensino: tipo_ensino, nomeEscola: nomeEscola, de01: de01, de02: de02, periodo: periodo, nomeCursoEscola: nomeCursoEscola, dataInicioDE: dataInicioDE, dataTerminoDE: dataTerminoDE, possuiExp: possuiExp, listaExp: listaExp, listaCursos: listaCursos, listaOutros: listaOutros, ehMasc: ehMasc, temFoto: temFoto, ehAdmin: ehAdmin, considerar_busca: considerar_busca, estagiando: estagiando, ativo: ativo, motivoDesativacao: motivoDesativacao, observacoesAdmin: observacoesAdmin
        },
        success: function (retorno) {
            // Exibe mensagem na tela
            alert(curriculoSalvoSucesso);
            window.location.href = '/Estagio/Cadastro/' + retorno.Id;

            //$('#hdnId').val(retorno.Id);
            //$('#hdnIdDadosEstagio').val(retorno.IdDadosEscolares);

            //var href = $('#btnPreview').attr('href');

            //if (href !== undefined)
            //    $('#btnPreview').attr('href', href.replace('/0', '/' + retorno.Id));

            //href = $('#btnPDF').attr('href');

            //if (href !== undefined)
            //    $('#btnPDF').attr('href', href.replace('/0', '/' + retorno.Id));

            //$('#btnPreview').show();
            //$('#btnPDF').show();
            //$.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
            alertaErroJS({ NomeFuncao: 'salvarUsuarioExistente()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

function validateDadosEscolares(validar) {
    var valido = true;
    var container = $('.tipo-ensino');

    if (!validar) {
        container.find('.has-feedback').removeClass('has-success').removeClass('has-error');
        container.find('small.help-block').attr('data-bv-result', 'NOT_VALIDATED');
        container.find('i.form-control-feedback').hide();
    }
    else {
        var field = container.find('input[name="nomeEscola"]');

        if (field.val() == '') {
            valido = false;

            addValidatorIcons(field);

            if (focoValidacao == '')
                focoValidacao = field;
        }

        field = container.find('select[name="de01"]');

        if (field.val() == '') {
            valido = false;

            addValidatorIcons(field);

            if (focoValidacao == '')
                focoValidacao = field;
        }

        field = container.find('select[name="de02"]');

        if (field.is(':visible') && field.val() == '') {
            valido = false;

            addValidatorIcons(field);

            if (focoValidacao == '')
                focoValidacao = field;
        }

        field = container.find('select[name="periodo"]');

        if (field.val() == '') {
            valido = false;

            addValidatorIcons(field);

            if (focoValidacao == '')
                focoValidacao = field;
        }

        field = container.find('input[name="nomeCurso"]');
        addValidatorIcons(field); // Unico caso em que chama fora do if = ""

        if (field.val() == '') {
            valido = false;

            if (focoValidacao == '')
                focoValidacao = field;
        }

        field = container.find('input[name="dataInicioDE"]');

        if (field.val() == '') {
            valido = false;

            addValidatorIcons(field);

            if (focoValidacao == '')
                focoValidacao = field;
        }

        field = container.find('input[name="dataTerminoDE"]');

        if (field.val() == '') {
            valido = false;

            addValidatorIcons(field);

            if (focoValidacao == '')
                focoValidacao = field;
        }
    }

    return valido;
}

function validateExperienciaProfissional(containerId, validar) {
    var valido = true;
    var container = $('.container_experiencia_' + containerId);

    if (!validar) {
        container.find('input[name="nomeEmpresa"]').val('');
        container.find('input[name="cargo"]').val('');
        container.find('input[name="dataInicio"]').val('');
        container.find('input[name="dataTermino"]').val('');
        container.find('textarea[name="atividadesDesenvolvidas"]').val('');

        container.find('.has-feedback').removeClass('has-success').removeClass('has-error');
        container.find('small.help-block').attr('data-bv-result', 'NOT_VALIDATED');
        container.find('i.form-control-feedback').hide();
    }
    else {
        var field = container.find('input[name="nomeEmpresa"]');

        if (field.val() == '') {
            valido = false;

            addValidatorIcons(field);

            if (focoValidacao == '')
                focoValidacao = field;
        }

        field = container.find('input[name="cargo"]');

        if (field.val() == '') {
            valido = false;

            addValidatorIcons(field);

            if (focoValidacao == '')
                focoValidacao = field;
        }

        field = container.find('input[name="dataInicio"]');

        if (field.val() == '') {
            valido = false;

            addValidatorIcons(field);

            if (focoValidacao == '')
                focoValidacao = field;
        }

        field = container.find('textarea[name="atividadesDesenvolvidas"]');

        if (field.val() == '') {
            valido = false;

            addValidatorIcons(field);

            if (focoValidacao == '')
                focoValidacao = field;
        }
    }

    return valido;
}

function validateExperienciasProfissionais() {
    var valido = true;

    if ($('.tem-experiencia-profissional').is(':visible'))
    {
        valido = validateExperienciaProfissional(1, true);

        if (!valido) {
            focoValidacao.focus();
            return valido;
        }

        if ($('.container_experiencia_2').is(':visible')) {
            valido = validateExperienciaProfissional(2, true);

            if (!valido) {
                focoValidacao.focus();
                return valido;
            }

            if ($('.container_experiencia_3').is(':visible')) {
                valido = validateExperienciaProfissional(3, true);

                if (!valido) {
                    focoValidacao.focus();
                    return valido;
                }
            }
        }
    }

    return valido;
}

function validateFoto()
{
    var valido = true;
    var field = $('input[name="UploadFile"]');
    var image = $('#fotoEstagio');

    if (field.val() == '' && (image.attr('src') == '' || image.attr('src').indexOf('empty.png') >= 0)) {
        valido = false;

        if (field.parent().parent().find('i.form-control-feedback').hasClass('glyphicon')) {
            if (field.val() == '') {
                field.parent().parent().find('i.form-control-feedback').removeClass('glyphicon-ok').addClass('glyphicon-remove');
                field.parent().parent().removeClass('has-success').addClass('has-error');
                field.parent().parent().find('small.help-block').attr('data-bv-result', 'INVALID');
            }
            else {
                field.parent().parent().find('i.form-control-feedback').addClass('glyphicon-ok').removeClass('glyphicon-remove');
                field.parent().parent().addClass('has-success').removeClass('has-error');
                field.parent().parent().find('small.help-block').attr('data-bv-result', 'VALID');
            }
        }

        field.parent().parent().find('i.form-control-feedback').show();

        if (focoValidacao == '')
            focoValidacao = field;
    }

    return valido;
}

function verificarEmailExistente()
{
    $('#txtEmail').block({ message: mensagemValidandoEmail, css: cssCarregando });
    var url = homePage + 'BaseEstagio/ValidarEmailExistente';
    var id = $('#hdnId').val();
    var email = $('#txtEmail').val();

    $.ajax({
        type: "POST",
        url: url,
        data: { id: id, email: email },
        success: function (retorno) {
            if (retorno)
            {
                $('#txtEmail').val('');
                alert(mensagemEmailExistente);
                $('#txtEmail').focus();
            }

            $('#txtEmail').unblock();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $('#txtEmail').unblock();
            alertaErroJS({ NomeFuncao: 'verificarEmailExistente()', ResponseText: xhr.responseText });
        }
    });

    return false;
}