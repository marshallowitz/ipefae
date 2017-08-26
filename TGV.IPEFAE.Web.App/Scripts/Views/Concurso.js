var oTable;
var tamanhoInicialAba;
var isMobile = ($(window).width() < 800);

function abrirJanelaPopUpConcurso(idConcurso, nomeConcurso, idInscrito, idTipo, classeParentContainer)
{
    if (idTipo == 1)
        gerarBoletoBancario(idConcurso, idInscrito, true);
    else if (idTipo == 2)
        gerarComprovanteInscricao(nomeConcurso, idInscrito);
    else if (idTipo == 3)
        obterClassificacao(nomeConcurso, idInscrito);
    else if (idTipo == 4)
        obterStatusPagamento(nomeConcurso, idConcurso, idInscrito);

    $(classeParentContainer).modal('hide');
}

function abrirModalAdicionarEmpresa()
{
    window.location.href = homePage + 'Admin/Concurso/Index_Empresa/';
}

function abrirModalAnexo(idAnexo, idConcurso, idTipo) {
    var url = homePage + 'Admin/Concurso/AdicionarAnexo';

    $.ajax({
        type: "POST",
        url: url,
        data: { idAnexo: idAnexo, idConcurso: idConcurso, idTipo: idTipo },
        success: function (retorno) {
            if ($("section#modal-anexos").hasClass('in'))
                return false;

            $('section#modal-anexos').html('');
            $('section#modal-anexos').html(retorno.View);

            $('.modal-anexos').modal('show');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'abrirModalAnexo()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

function abrirModalBuscaInscrito(idConcurso, idTipo, nomeConcurso)
{
    var url = homePage + 'Concurso/AbrirModalBuscaInscrito';

    $.ajax({
        type: "POST",
        url: url,
        data: { idTipo: idTipo },
        success: function (retorno)
        {
            $('section#modal-boleto').html('');
            $('section#modal-boleto').html(retorno.View);

            iniciarTelaModalBuscaInscrito(idTipo, nomeConcurso);
            
            $('#hdnIdConcursoCPF').val(idConcurso);
            var sufixo = obterSufixoTipoModal(idTipo);
            $('.modal-' + sufixo).modal('show');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'abrirModalBuscaInscrito()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

function abrirModalCargo(idCargo, idConcurso) {
    var url = homePage + 'Admin/Concurso/AdicionarCargo';

    $.ajax({
        type: "POST",
        url: url,
        data: { idCargo: idCargo, idConcurso: idConcurso },
        success: function (retorno) {
            $('section#modal-cargos').html('');
            $('section#modal-cargos').html(retorno.View);

            $('.modal-cargos').modal('show');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'abrirModalCargo()', ResponseText: xhr.responseText });
        }
    });
}

function abrirModalEstatisticas() {
    var url = homePage + 'Admin/Concurso/AbrirModalEstatisticas';

    $.ajax({
        type: "POST",
        url: url,
        success: function (retorno) {
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            $('section#modal-estatisticas').html('');
            $('section#modal-estatisticas').html(retorno.View);

            iniciarTelaModalEstatisticas();

            $('.modal-estatisticas').modal('show');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'abrirModalEstatisticas()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

function abrirModalRemessas()
{
    $('#txtUploadFile').val('');
    $('.modal-remessa').modal('show');
}

function abrirModalUpdateConcurso()
{
    $('#txtUploadFileConcurso').val('');
    $('.modal-concurso').modal('show');
}

function abrirPagina (pagina)
{
    window.location.href = homePage + pagina;
}

function boletoInscrito(idConcurso, idInscrito)
{
    abrirJanelaPopUpWait(mensagemGerandoBoleto);
    gerarBoletoBancario(idConcurso, idInscrito, true);
}

function buscarInscritoPorCPF_Ajax(idConcurso, cpf, idInscrito, idTipo, mensagem)
{
    var labelBotao = obterLabelBotaoTipoModal(idTipo);
    var sufixo = obterSufixoTipoModal(idTipo);
    var classeParentContainer = '.modal-' + sufixo;

    // Verifica se existe mais de um inscrito com este cpf para este concurso
    var url = homePage + 'Concurso/BuscarInscritoPorCPF';
    cpf = removerTodosCaracteresMenosNumeros(cpf);

    abrirJanelaPopUpWait(mensagem);

    $.ajax({
        type: "POST",
        url: url,
        data: { idConcurso: idConcurso, cpf: cpf, idMatricula: idInscrito, idTipo: idTipo },
        async: false,
        success: function (retorno)
        {
            if (!retorno.Sucesso)
            {
                if ($(classeParentContainer).find('.resultado-status').length > 0)
                    $(classeParentContainer).find('.resultado-status').hide();

                $(classeParentContainer).find('#txtCPF').val('');
                $(classeParentContainer).find('.cpf-nao-encontrado').fadeIn(1000).fadeOut(3000);
                $(classeParentContainer).find('#txtCPF').focus();

                if (janelaPopUp != null)
                {
                    janelaPopUp.close();
                    janelaPopUp = null;
                }

                $.unblockUI();
                return;
            }
            else if (retorno.MaisUmCargo)
            {
                var classeContainer = (idTipo <= 2) ? '.cpf-segunda-via-' + sufixo : '.cpf-' + sufixo;
                exibirListaCargos(idTipo, idConcurso, cpf, 'buscarInscritoPorCPFComCargo', classeParentContainer, classeContainer, labelBotao, true);
                return;
            }
            //else if (idTipo == 4)
            //{
            //    $(classeParentContainer).find('#btnGerar').hide();
            //    $(classeParentContainer + '-white').find('#txtCPF').attr('readonly', 'readonly');
            //    $(classeParentContainer).find('.resultado-status > .status_dados').html(retorno.DadosStatusPagamento);
            //    $(classeParentContainer).find('.resultado-status > .status').html(retorno.StatusPagamento);
            //    $(classeParentContainer).find('.resultado-status').show();

            //    if (retorno.StatusPagamento.toLowerCase() == 'não pago')
            //        $('.resultado-status > .status-info').show();

            //    $.unblockUI();
            //    return;
            //}
            
            $.unblockUI();
            abrirJanelaPopUpConcurso(idConcurso, retorno.NomeConcurso, retorno.IdInscrito, idTipo, classeParentContainer);
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'buscarInscritoPorCPF()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });

    return false;
}

function buscarInscritoPorCPF(idConcurso, cpf, idInscrito, idTipo)
{
    var mensagem = obterMensagemTipoModal(idTipo);
    $.blockUI({ message: mensagem, css: cssCarregando });
    setTimeout(function () { buscarInscritoPorCPF_Ajax(idConcurso, cpf, idInscrito, idTipo, mensagem); }, 500);
}

function buscarInscritoPorCPFComCargo(idConcurso, nomeConcurso, idTipo)
{
    var sufixo = obterSufixoTipoModal(idTipo);
    var classeParentContainer = '.modal-' + sufixo;

    var mensagem = obterMensagemTipoModal(idTipo);
    var idInscrito = $('input[name=cargo-inscrito]:checked').val();

    if (idInscrito === '' || idInscrito === undefined)
    {
        alert(mensagemSelecioneUmaMatricula);
        return;
    }
    
    abrirJanelaPopUpWait(mensagem);
    abrirJanelaPopUpConcurso(idConcurso, nomeConcurso, idInscrito, idTipo, classeParentContainer);
}

function carregarAlertaRecurso(idConcurso)
{
    var url = homePage + 'Admin/Concurso/CarregarAlertaRecurso';

    $.ajax({
        type: "POST",
        url: url,
        data: { idConcurso: idConcurso },
        success: function (retorno) {
            $('.alertaRecurso').html('');
            $('.alertaRecurso').html(retorno.View);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'carregarAlertaRecurso()', ResponseText: xhr.responseText });
        }
    });
}

function carregarEmpresas(ddl, idEmpresaSelecionada)
{
    var url = homePage + 'Admin/Concurso/CarregarEmpresas';

    $.ajax({
        type: "POST",
        url: url,
        success: function (items)
        {
            ddl.empty();
            var data = items.data ? items.data : items;

            if (data.length > 0)
            {
                ddl.addItems({ data: data, valueName: 'Value', textName: 'Text' });

                if (idEmpresaSelecionada == '0')
                    ddl[0].selectedIndex = 0;
                else
                    ddl.val(idEmpresaSelecionada);
            }
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'carregarEmpresas()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function definirOrdenacao(idConcurso, coluna)
{
    if (coluna.hasClass('selecionado'))
        return;

    if (coluna.hasClass('nome'))
        $('.matricula').removeClass('selecionado').addClass('order');
    else
        $('.nome').removeClass('selecionado').addClass('order');

    coluna.addClass('selecionado').removeClass('order');
    maisInscritos(idConcurso, true);
}

function downloadCSV(id) {
    $.blockUI({ message: mensagemDownloadCSV, css: cssCarregando });

    $('#iframeCSV').attr('src', homePage + 'Handlers/DownloadCSVHandler.ashx?id=' + id);

    setTimeout(function () { $.unblockUI(); }, 10000);
}

function downloadTXT(id)
{
    $.blockUI({ message: mensagemDownloadTXT, css: cssCarregando });

    $('#iframeTXT').attr('src', homePage + 'Handlers/DownloadTXTHandler.ashx?id=' + id + '&tipo=rem');

    setTimeout(function () { $.unblockUI(); }, 10000);
}

function editar(id)
{
    window.location.href = homePage + 'Admin/Concurso/Cadastro/' + id;
}

function editarAtivacao(idCargo, idConcurso) {
    $.blockUI({ message: mensagemDesativando, css: cssCarregando });
    var url = homePage + 'Admin/Concurso/AtivarCargo';

    $.ajax({
        type: "POST",
        url: url,
        data: { idCargo: idCargo, idConcurso: idConcurso },
        success: function (retorno) {
            alert(operacaoMensagemSucesso);
            recarregarTabelaCargos(idConcurso);
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'editarAtivacao()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function editarInscrito(id)
{
    window.location.href = homePage + 'Admin/Concurso/Cadastro_Inscrito/' + id;
}

function excluirAnexo(idAnexo, idConcurso, idTipo)
{
    var url = homePage + 'Admin/Concurso/ExcluirAnexo';

    $.ajax({
        type: "POST",
        url: url,
        data: { idAnexo: idAnexo, idConcurso: idConcurso, idTipo: idTipo },
        success: function (retorno) {
            alert(mensagemArquivoRemovidoSucesso);
            recarregarTabelaAnexos(idConcurso, idTipo);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'excluirAnexo()', ResponseText: xhr.responseText });
        }
    });
}

function excluirCargo(idCargo, idConcurso) {
    var url = homePage + 'Admin/Concurso/ExcluirCargo';

    $.ajax({
        type: "POST",
        url: url,
        data: { idCargo: idCargo, idConcurso: idConcurso },
        success: function (retorno) {
            alert(mensagemCargoRemovidoSucesso);
            recarregarTabelaCargos(idConcurso);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'excluirCargo()', ResponseText: xhr.responseText });
        }
    });
}

function excluirConcurso(id)
{
    if (!confirm('Deseja realmente excluir este concurso?'))
        return false;

    $.blockUI({ message: "Excluindo Concurso...", css: cssCarregando });
    var url = homePage + 'Admin/Concurso/ExcluirConcurso';

    $.ajax({
        type: "POST",
        url: url,
        data: { id: id },
        success: function (items)
        {
            alert("Concurso excluído com sucesso");
            $('#tblConcursos tbody tr#linha_' + id).remove();
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'excluirConcurso()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function exibirListaCargos(idTipo, idConcurso, cpf, callback, classeParentContainer, classeContainer, textBotao, esconderBotao)
{
    var url = homePage + 'Concurso/ExibirModalListaCargos';

    $.ajax({
        type: "POST",
        url: url,
        data: { idTipo: idTipo, idConcurso: idConcurso, cpf: cpf },
        success: function (retorno)
        {
            $(classeParentContainer).find(classeContainer).html('');
            $(classeParentContainer).find(classeContainer).html(retorno.View);

            $(classeParentContainer).find('.modal-dialog').animate({ width: 800 }, 500);
            $(classeParentContainer).find(classeParentContainer + '-white').animate({ height: 300 }, 500);
            $(classeParentContainer).find('.primeira-coluna').show();
            $(classeParentContainer).find('.segunda-coluna').removeClass('col-md-12').addClass('col-md-6');
            $(classeParentContainer).find('.linha-cargos').show();
            $(classeParentContainer).find(classeContainer).find('#btnOkComCargo').val(textBotao);

            $(classeParentContainer + '-white').find('#txtCPF').attr('readonly', 'readonly');
            $(classeParentContainer + '-white').find('#btnGerar').hide();
            $(classeParentContainer + '-white').find('#btnGerar2').show();

            // Se for do tipo status pagamento, exibe a ultima coluna
            //if (classeContainer.indexOf('status-pagamento') >= 0)
            //{
            //    $(classeParentContainer).find(classeContainer).find('.statusPagamento').show();
            //    $(classeParentContainer).find(classeContainer).find('.selecao').hide();
            //}

            if (janelaPopUp != null)
            {
                janelaPopUp.close();
                janelaPopUp = null;
            }
            else // Chamada do recurso
            {
                var contentH = $('.cd-tabs-content').height();
                var newH = $(classeParentContainer).height();
                tabsContentResize(contentH - newH);
                $.unblockUI();
            }

            if (esconderBotao) {
                $(classeParentContainer).find(classeContainer).find('#btnOkComCargo').hide();
                $.unblockUI();
                return;
            }

            var fn = window[callback];
            if (typeof fn === 'function')
                $(classeParentContainer).find('#btnOkComCargo').on('click', function () { fn(idConcurso); });

            tabsContentResize(65);
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'exibirListaCargos()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function incluirAnexo(idConcurso) {
    var url = homePage + 'Admin/Concurso/SalvarAnexo';
    var idAnexo = $('#hdnIdAnexo').val();
    var idTipo = $('#hdnIdTipo').val();
    var nomeArquivo = $('#txtNomeArquivo').val();
    var dataPublicacao = $('#txtDataPublicacao').val();
    var temRecurso = $('#rdRecSim').is(':checked');
    var recInicio = $('#txtDataInicioRecurso').val();
    var recFim = $('#txtDataFimRecurso').val();
    var fileName = $('#txtUploadFile').val();

    $.ajax({
        type: "POST",
        url: url,
        data: { idAnexo: idAnexo, idConcurso: idConcurso, idTipo: idTipo, nomeArquivo: nomeArquivo, dataPublicacao: dataPublicacao, temRecurso: temRecurso, recInicio: recInicio, recFim: recFim, fileName: fileName },
        success: function (retorno) {
            alert(arquivoAnexadoSucesso);
            carregarAlertaRecurso(idConcurso);
            recarregarTabelaAnexos(idConcurso, idTipo);
            $('.modal-anexos').modal('hide');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'incluirAnexo()', ResponseText: xhr.responseText });
        }
    });
}

function incluirCargo(idConcurso) {
    var url = homePage + 'Admin/Concurso/SalvarCargo';
    var idCargo = $('#hdnIdCargo').val();
    var nomeCargo = $('#txtNomeCargo').val();
    var ehIsento = $('#rdIseSim').is(':checked');
    var valorInscricao = $('#txtValorInscricao').val();

    $.ajax({
        type: "POST",
        url: url,
        data: { idCargo: idCargo, idConcurso: idConcurso, nomeCargo: nomeCargo, ehIsento: ehIsento, valorInscricao: valorInscricao },
        success: function (retorno) {
            alert(mensagemOperacaoSucesso);
            recarregarTabelaCargos(idConcurso);
            $('.modal-cargos').modal('hide');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'incluirCargo()', ResponseText: xhr.responseText });
        }
    });
}

function iniciarTelaListaConcursos() {
    $('#txtUploadFile').on('change', function (e) { uploadFileRemessa(e); });
    $('#btnSalvar').on('click', function () { realizarUploadArquivoRemessa(); });

    $('#txtUploadFileConcurso').on('change', function (e) { uploadFileConcurso(e); });
    $('#btnSalvarConcurso').on('click', function () { realizarUploadArquivoConcurso(); });

    listarConcursos();
}

function iniciarTelaListaConcursosUsuario()
{
    if (isMobile)
        textoAbaAbertas = textoAbaAbertasMini;

    listarConcursosUsuario();

    tabsContentResize(65);
    $(window).resize(function () { tabsContentResize(65); });
}

function iniciarTelaDadosConcursosUsuario(idConcurso, temRecurso)
{
    listarDadosConcursosUsuario(idConcurso);

    tabsContentResize(65);
    $(window).resize(function () { tabsContentResize(65); });

    if (temRecurso) {
        $('.recursos').removeClass('disabled');
        $('.recursos').next().removeClass('disabled');
    }
    else {
        $('.recursos').addClass('disabled');
        $('.recursos').next().addClass('disabled');
    }

    
}

function iniciarTelaListaInscritos(idConcurso)
{
    $('#txtMatricula').mask('999999');
    $('#txtCPF').mask('999.999.999-99');
    $('#txtMatricula').focus();

    $('#btnPesquisar').on('click', function () { pesquisarInscritos(idConcurso); });
    $('.order').on('click', function () { definirOrdenacao(idConcurso, $(this)); });
}

function iniciarTelaListaPublicacoes()
{
    if (!inscricoesOnline)
        return;

    var today = new Date();
    var inicio = obterData($('#hdnDataInscricao').val());
    var fim = obterData($('#hdnDataFimInscricao').val());
    var fimBoleto = obterData($('#hdnDataBoleto').val());
    var inicioComprovante = obterData($('#hdnDataInicioComprovante').val());
    var fimComprovante = obterData($('#hdnDataFimComprovante').val());
    var inicioClassificacao = obterData($('#hdnDataInicioClassificacao').val());
    var inicioIsencao = obterData($('#hdnDataInicioIsencao').val());
    var fimIsencao = obterData($('#hdnDataFimIsencao').val());
    var endPagamentoOK = fim.addDays(10);

    var idConcurso = $('#hdnIdConcurso').val();
    var nomeConcurso = $('#hdnNomeConcurso').val();

    var bancoCss = $('#hdnConcursoBancoCss').val();
    $('.logo-banco').addClass(bancoCss);

    // Inscricoes
    if (inicio - today <= 0 && today - fim <= 0)
    {
        $('#btnInscricao').show();
    }

    // Boleto
    if (inicio - today <= 0 && today - fimBoleto <= 0)
    {
        $('#btnBoleto').parent().show();
        $('#btnBoleto').on('click', function () { abrirModalBuscaInscrito(idConcurso, 1, nomeConcurso); });
    }

    // Comprovante pagamento
    if (inicio - today <= 0 && today - endPagamentoOK <= 0)
    {
        $('#btnPagamentoOk').parent().show();
        $('#btnPagamentoOk').on('click', function () { abrirModalBuscaInscrito(idConcurso, 4, nomeConcurso); });
    }

    // Solicitar isenção
    if (inicioIsencao - today <= 0 && today - fimIsencao <= 0)
    {
        $('#btnIsencao').parent().show();
        $('#btnIsencao').on('click', function () { $('a[data-content="recursos"]').trigger('click'); });
    }

    // Comprovante inscrição
    if (inicioComprovante - today <= 0 && today - fimComprovante <= 0)
    {
        $('#btnComprovante').parent().show();
        $('.btnComprovante').on('click', function () { abrirModalBuscaInscrito(idConcurso, 2, nomeConcurso); });
    }

    // Classificação
    if (inicioClassificacao - today <= 0)
    {
        $('#btnClassificacao').parent().show();
        $('.btnClassificacao').on('click', function () { abrirModalBuscaInscrito(idConcurso, 3, nomeConcurso); });
    }

    $('#btnInscricao').on('click', function () { window.location.href = homePage + 'Concurso/' + idConcurso + '/Inscricao'; });
}

function iniciarTelaCadastroConcurso(idConcurso, idTipoLayout, idEmpresa, inscricaoOnline, encerrado, ativo)
{
    $('#txtDataInicio').mask('00/00/0000 00:00');
    $('#datetimepickerInicio').datetimepicker({ locale: 'pt-br' });
    $('#txtDataEncerramentoInscricoes').mask('00/00/0000 00:00');
    $('#datetimepickerEncerramentoInscricoes').datetimepicker({ locale: 'pt-br' });
    $('#txtDataEncerramento').mask('00/00/0000 00:00');
    $('#datetimepickerEncerramento').datetimepicker({ locale: 'pt-br' });
    $('#txtDataInicioComprovante').mask('00/00/0000 00:00');
    $('#datetimepickerInicioComprovante').datetimepicker({ locale: 'pt-br' });
    $('#txtDataEncerramentoComprovante').mask('00/00/0000 00:00');
    $('#datetimepickerEncerramentoComprovante').datetimepicker({ locale: 'pt-br' });
    $('#txtDataInicioClassificacao').mask('00/00/0000 00:00');
    $('#datetimepickerInicioClassificacao').datetimepicker({ locale: 'pt-br' });
    $('#txtDataInicioIsento').mask('00/00/0000 00:00');
    $('#datetimepickerInicioIsento').datetimepicker({ locale: 'pt-br' });
    $('#txtDataEncerramentoIsento').mask('00/00/0000 00:00');
    $('#datetimepickerEncerramentoIsento').datetimepicker({ locale: 'pt-br' });
    $('#txtDataBoleto').mask('00/00/0000 00:00');
    $('#datetimepickerBoleto').datetimepicker({ locale: 'pt-br' });

    carregarEmpresas($('#ddlEmpresa'), idEmpresa);

    if ($('#hdnId').val() <= 0)
    {
        $('#txtDataEncerramentoInscricoes').val('');
        $('#txtDataInicioComprovante').val('');
        $('#txtDataEncerramentoComprovante').val('');
        $('#txtDataInicioClassificacao').val('');
        $('#txtDataInicioIsento').val('');
        $('#txtDataEncerramentoIsento').val('');
        $('#txtDataBoleto').val('');
    }

    $("#datetimepickerInicio").on("dp.change", function (e) { $('#datetimepickerEncerramentoInscricoes').data("DateTimePicker").minDate(e.date); });
    
    $('#ddlTipoLayout').val(idTipoLayout);

    $('.incluirAnexo').on('click', function () { abrirModalAnexo(0, idConcurso, $(this).attr('tag')); });
    $('.incluirCargo').on('click', function () { abrirModalCargo(0, idConcurso); });

    $('input[name="encerrado"]').on('change', function () {
        if ($('#rdEncSim').is(':checked'))
            $('#txtDataEncerramento').parent().parent().show();
        else
        {
            $('#txtDataEncerramento').parent().parent().hide();
            $('#txtDataEncerramento').val('');
        }
    });

    if (inscricaoOnline)
        $('#rdIOSim').attr('checked', 'checked');
    else
        $('#rdIONao').attr('checked', 'checked');

    if (encerrado)
    {
        $('#rdEncSim').attr('checked', 'checked');
        $('#txtDataEncerramento').parent().parent().show();
    }
    else
        $('#rdEncNao').attr('checked', 'checked');

    if (ativo)
        $('#rdAtivoSim').attr('checked', 'checked');
    else
        $('#rdAtivoNao').attr('checked', 'checked');

    carregarAlertaRecurso(idConcurso);
    recarregarTabelaAnexos(idConcurso, 1);
    recarregarTabelaAnexos(idConcurso, 2);
    recarregarTabelaCargos(idConcurso);

    $('.concursoForm')
    .on('init.field.bv', function (e, data) { removerIconeValidator(e, data); })
    .bootstrapValidator({
        container: 'tooltip',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        }
        , fields: {
            nome: {
                validators: { notEmpty: { message: identificacaoObrigatoria } }
            }
            , dataInicio: {
                validators: {
                    notEmpty: { message: dataInicioObrigatorio },
                    date: {
                        message: dataInicioFormatoInvalido,
                        format: 'DD/MM/YYYY HH:mm',
                        min: '01/01/2015 00:00',
                        max: '01/01/2025 00:00'
                    }
                }
            }
            , dataEncerramentoInscricoes: {
                validators: {
                    notEmpty: { message: dataEncerramentoInscricoesObrigatorio },
                    date: {
                        message: dataEncerramentoInscricoesFormatoInvalido,
                        format: 'DD/MM/YYYY HH:mm',
                        min: '01/01/2015 00:00',
                        max: '01/01/2025 00:00'
                    }
                }
            }
            , dataInicioComprovante: {
                validators: {
                    notEmpty: { message: dataInicioComprovanteObrigatorio },
                    date: {
                        message: dataInicioComprovanteFormatoInvalido,
                        format: 'DD/MM/YYYY HH:mm',
                        min: '01/01/2015 00:00',
                        max: '01/01/2025 00:00'
                    }
                }
            }
            , dataEncerramentoComprovante: {
                validators: {
                    notEmpty: { message: dataEncerramentoComprovanteObrigatorio },
                    date: {
                        message: dataEncerramentoComprovanteFormatoInvalido,
                        format: 'DD/MM/YYYY HH:mm',
                        min: '01/01/2015 00:00',
                        max: '01/01/2025 00:00'
                    }
                }
            }
            , dataInicioClassificacao: {
                validators: {
                    notEmpty: { message: dataInicioClassificacaoObrigatorio },
                    date: {
                        message: dataInicioClassificacaoFormatoInvalido,
                        format: 'DD/MM/YYYY HH:mm',
                        min: '01/01/2015 00:00',
                        max: '01/01/2025 00:00'
                    }
                }
            }
            , dataInicioIsento: {
                validators: {
                    notEmpty: { message: dataInicioIsentoObrigatorio },
                    date: {
                        message: dataInicioIsentoFormatoInvalido,
                        format: 'DD/MM/YYYY HH:mm',
                        min: '01/01/2015 00:00',
                        max: '01/01/2025 00:00'
                    }
                }
            }
            , dataEncerramentoIsento: {
                validators: {
                    notEmpty: { message: dataEncerramentoIsentoObrigatorio },
                    date: {
                        message: dataEncerramentoIsentoFormatoInvalido,
                        format: 'DD/MM/YYYY HH:mm',
                        min: '01/01/2015 00:00',
                        max: '01/01/2025 00:00'
                    }
                }
            }
            , dataEncerramento: {
                validators: {
                    notEmpty: { message: dataEncerramentoObrigatorio },
                    date: {
                        message: dataEncerramentoFormatoInvalido,
                        format: 'DD/MM/YYYY HH:mm',
                        min: '01/01/2015 00:00',
                        max: '01/01/2025 00:00'
                    }
                }
            }
            , dataBoleto: {
                validators: {
                    notEmpty: { message: dataBoletoObrigatorio },
                    date: {
                        message: dataBoletoFormatoInvalido,
                        format: 'DD/MM/YYYY HH:mm',
                        min: '01/01/2015 00:00',
                        max: '01/01/2025 00:00'
                    }
                }
            }
        }
    }).on('success.form.bv', function (e) {
        e.preventDefault(); // Prevent form submission
        salvar();
    });

    $('section#modal-anexos').on('shown.bs.modal', function () { $("#txtNomeArquivo").focus(); });
    $('section#modal-cargos').on('shown.bs.modal', function () { $("#txtNomeCargo").focus(); });

    $("#txtNome").focus();
}

function iniciarTelaModalAnexos(idConcurso, temRecurso) {
    $('#txtDataPublicacao').mask('00/00/0000 00:00');
    $('#datetimepickerPublicacao').datetimepicker({ locale: 'pt-br' });
    $('#txtDataInicioRecurso').mask('00/00/0000 00:00');
    $('#datetimepickerInicioRecurso').datetimepicker({ locale: 'pt-br' });
    $('#txtDataFimRecurso').mask('00/00/0000 00:00');
    $('#datetimepickerFimRecurso').datetimepicker({ locale: 'pt-br' });

    $("#datetimepickerInicioRecurso").on("dp.change", function (e) { $('#datetimepickerFimRecurso').data("DateTimePicker").minDate(e.date); });

    $('#txtUploadFile').on('change', function (e) { uploadFileAnexos(e, idConcurso); });

    $('input[name="temRecurso"]').on('change', function () {
        $('.data_recurso').hide();
        $('.data_recurso').find('input[type="text"]').val('');

        if ($(this).attr('id') == 'rdRecSim') {
            $('.data_recurso').show();
            $('#txtDataInicioRecurso').val($('#txtDataPublicacao').val());
            $('#datetimepickerFimRecurso').data("DateTimePicker").minDate($('#txtDataPublicacao').val());
            $('#txtDataInicioRecurso').focus();
        }
    });

    $('.anexoForm')
    .on('init.field.bv', function (e, data) { removerIconeValidator(e, data); })
    .bootstrapValidator({
        container: 'tooltip',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        }
        , fields: {
            dataPublicacao: {
                validators: {
                    notEmpty: { message: dataPublicacaoObrigatorio },
                    date: {
                        message: dataPublicacaoFormatoInvalido,
                        format: 'DD/MM/YYYY H:m',
                        min: '01/01/1920 00:00',
                        max: '01/01/2025 00:00'
                    }
                }
            }
            , nomeArquivo: {
                validators: {
                    notEmpty: { message: nomeArquivoObrigatorio }
                }
            }
            , dataInicio: {
                validators: {
                    notEmpty: { message: dataInicioObrigatorio },
                    date: {
                        message: dataInicioFormatoInvalido,
                        format: 'DD/MM/YYYY H:m',
                        min: '01/01/1920 00:00',
                        max: '01/01/2025 00:00'
                    }
                }
            }
            , dataTermino: {
                validators: {
                    notEmpty: { message: dataTerminoObrigatorio },
                    date: {
                        message: dataTerminoFormatoInvalido,
                        format: 'DD/MM/YYYY H:m',
                        min: 'dataInicio',
                        max: '01/01/2025 00:00'
                    }
                }
            }
            , UploadFile: {
                validators: {
                    notEmpty: { message: arquivoObrigatorio },
                    file: {
                        extension: 'pdf,doc,docx',
                        type: 'application/pdf,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document',
                        maxSize: 4194304,   // 4096 * 1024
                        message: arquivoFormatoInvalido
                    }
                }
            }
        }
    }).on('success.form.bv', function (e) {
        e.preventDefault(); // Prevent form submission
        incluirAnexo(idConcurso);
    });

    $('#rdRecSim').prop("checked", temRecurso);
    $('#rdRecNao').prop("checked", !temRecurso);
    
    if ($('#rdRecSim').is(':checked'))
        $('.data_recurso').show();
    else
        $('.data_recurso').hide();

    $('i[data-bv-icon-for="UploadFile"]').css('top', '25px');

    $('#ddlTipoArquivo').on('change', function () { $('#hdnIdTipo').val($(this).val()); });
}

function iniciarTelaModalBuscaInscrito(idTipo, nomeConcurso)
{
    var sufixo = obterSufixoTipoModal(idTipo);
    $('.modal-' + sufixo).on('hidden.bs.modal', function () { $('.modal-' + sufixo).find('#txtCPF').val(''); });
    $('.modal-' + sufixo).on('show.bs.modal', function ()
    {
        $('.modal-' + sufixo).find('#txtCPF').mask('999.999.999-99');
    });

    $('.modal-' + sufixo).on('shown.bs.modal', function () {
        $('.modal-' + sufixo).find('.cpfForm')
        .on('init.field.bv', function (e, data) { removerIconeValidator(e, data); })
        .bootstrapValidator({
            container: 'tooltip',
            feedbackIcons: {
                valid: 'glyphicon glyphicon-ok',
                invalid: 'glyphicon glyphicon-remove',
                validating: 'glyphicon glyphicon-refresh'
            }
            , fields: {
                cpf: {
                    validators: {
                        notEmpty: { message: cpfObrigatorio }
                    }
                }
            }
        }).on('success.form.bv', function (e) {
            e.preventDefault(); // Prevent form submission
            buscarInscritoPorCPF($('#hdnIdConcursoCPF').val(), $('.modal-' + sufixo).find('#txtCPF').val(), 0, idTipo);
        });

        $('.modal-' + sufixo + '-white').find('#btnGerar2').on('click', function () { buscarInscritoPorCPFComCargo($('#hdnIdConcursoCPF').val(), nomeConcurso, idTipo); });
        $('.modal-' + sufixo).find('#txtCPF').focus();
    });
}

function iniciarTelaModalCargos(idConcurso) {
    $("#txtValorInscricao").maskMoney();

    $('.cargoForm')
    .on('init.field.bv', function (e, data) { removerIconeValidator(e, data); })
    .bootstrapValidator({
        container: 'tooltip',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        }
        , fields: {
            nomeCargo: {
                validators: { notEmpty: { message: nomeObrigatorio } }
            }
            , valorInscricao: {
                validators: {
                    callback: {
                        message: valorInscricaoObrigatorio,
                        callback: function (value, validator, $field) {
                            return value != '' && value != 'R$ 0,00';
                        }
                    }
                }
            }
        }
    }).on('success.form.bv', function (e) {
        e.preventDefault(); // Prevent form submission
        incluirCargo(idConcurso);
    });
}

function iniciarTelaModalEstatisticas() {
    $('section#modal-estatisticas').on('shown.bs.modal', function () {
        $('section#modal-estatisticas').find('#txtEmail').focus();
    });

    $('section#modal-estatisticas').on('hidden.bs.modal', function () {
        $('section#modal-estatisticas').find('#txtEmail').val('');
        $('section#modal-estatisticas').find('#txtSenha').val('');
    });
}

function liberarOpcaoRecursoComCargo(idConcurso) {
    var idInscrito = $('.recursos').find('input[name=cargo-inscrito]:checked').val();

    if (idInscrito === '' || idInscrito === undefined) {
        alert(mensagemSelecioneUmaMatricula);
        return;
    }

    liberarOpcaoRecurso(idConcurso, idInscrito);
}

function liberarOpcaoRecurso(idConcurso, idInscrito)
{
    var mensagem = "Carregando ...";
    $.blockUI({ message: mensagem, css: cssCarregando });

    if (idInscrito === undefined)
        idInscrito = 0;

    var url = homePage + 'Concurso/LiberarOpcaoRecurso';
    var cpf = removerTodosCaracteresMenosNumeros($('.cpf-requerente').find('#txtCPF').val());

    $.ajax({
        type: "POST",
        url: url,
        data: { idConcurso: idConcurso, cpf: cpf, idMatricula: idInscrito },
        success: function (retorno) {
            if (!retorno.Sucesso)
            {
                $('.cpf-nao-encontrado').fadeIn(1000).fadeOut(3000);
                $.unblockUI();
                return;
            }
            else if (retorno.MaisUmCargo)
            {
                exibirListaCargos(0, idConcurso, cpf, 'liberarOpcaoRecursoComCargo', '.recurso-cadastro-usuario', '.cpf-requerente-cargos', labelAbrirRecurso, false);
                $('.cpf-requerente-cargos').find('#txtCPF').attr('readonly', 'readonly');
                return;
            }
            
            iniciarFormularioRecurso({ idConcurso: idConcurso, idInscrito: retorno.IdInscrito, inscricaoRequerente: retorno.InscricaoRequerente, nomeRequerente: retorno.NomeRequerente, cargoRequerente: retorno.CargoRequerente });
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'liberarOpcaoRecurso()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function listarConcursos()
{
    $.blockUI({ message: mensagemCarregando, css: cssCarregando });
    var url = homePage + 'Admin/Concurso/ListarConcursos';

    $.ajax({
        type: "POST",
        url: url,
        success: function (retorno) {
            $('.lista').html('');
            $('.lista').append(retorno);
            montarTabela();

            if ($('.lista').find('table').attr('summary') == naoEncontrado)
                $('.lista').css('marginTop', 0);

            $('.editar').on('click', function () { editar($(this).closest('tr').find('td:first span').html()); });
            $('.excluir').on('click', function () { excluirConcurso($(this).closest('tr').find('td:first span').html()); });
            $('.excel').on('click', function () { downloadCSV($(this).closest('tr').find('td:first span').html()); });
            $('.recurso').on('click', function () { abrirPagina('Admin/Recurso/Index/' + $(this).closest('tr').find('td:first span').html()); });
            $('.inscrito').on('click', function () { abrirPagina('Admin/Concurso/Index_Inscrito/' + $(this).closest('tr').find('td:first span').html()); });
            $('.download-remessa').on('click', function () { downloadTXT($(this).closest('tr').find('td:first span').html()); });

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'listarConcursos()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function listarConcursosUsuario() {
    $.blockUI({ message: mensagemCarregando, css: cssCarregando });
    var url = homePage + 'Concurso/ListarConcursos';

    $.ajax({
        type: "POST",
        url: url,
        success: function (retorno) {
            $('.abertas').html('');
            $('.abertas').append(retorno);
            $('.abertas').html(retorno.Abertas);

            $('.andamento').html('');
            $('.andamento').append(retorno);
            $('.andamento').html(retorno.EmAndamento);

            $('.encerradas').html('');
            $('.encerradas').append(retorno);
            $('.encerradas').html(retorno.Encerradas);

            // Preenche a quantidade nas abas
            $('#abaAbertas').html(textoAbaAbertas + ' (' + retorno.TotalAbertas + ')');
            $('#abaAndamento').html(textoAbaAndamento + ' (' + retorno.TotalEmAndamento + ')');
            $('#abaEncerradas').html(textoAbaEncerradas + ' (' + retorno.TotalEncerradas + ')');

            $('.concurso').on('click', function () { window.location.href = homePage + 'Concurso/' + $(this).find('td:first').html(); });

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'listarConcursosUsuario()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function listarDadosConcursosUsuario(idConcurso)
{
    $.blockUI({ message: mensagemCarregando, css: cssCarregando });
    var url = homePage + 'Concurso/ListarDadosConcurso';

    $.ajax({
        type: "POST",
        url: url,
        data: { idConcurso: idConcurso },
        success: function (retorno) {
            $('.publicacoes').html('');
            $('.publicacoes').append(retorno);
            $('.publicacoes').html(retorno.Publicacoes);

            $('.provas_gabaritos').html('');
            $('.provas_gabaritos').append(retorno);
            $('.provas_gabaritos').html(retorno.ProvasGabaritos);

            if (!isMobile)
            {
                setTabsContentHeight(
                    function ()
                    {
                        $('.publicacoes').find('.panelTabs').height($('.cd-tabs-content').height() - 70);
                        //$('.provas_gabaritos').find('.panelTabs').height($('.cd-tabs-content').height() - 70);
                    },
                    $('.publicacoes').find('.panelTabs').find('tbody'), 0);
            }

            // $('.provas_gabaritos').height($('.provas_gabaritos').find('').height());

            $('a.recursos').on('click', function () {
                $('.cpf-requerente-cargos').html('');
                $('.cpf-requerente').show();
                $('.recursoForm').hide();
                $('.cpf-requerente').find('#txtCPF').val('');
                $('.cpf-requerente').find('#txtCPF').focus();
            });

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'listarDadosConcursosUsuario()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function maisInscritos(idConcurso, limpar)
{
    $('.listagem').block({ message: mensagemCarregando, css: cssCarregando });

    if (limpar !== undefined && limpar)
        $('#hdnPagina').val('0');

    var pagina = $('#hdnPagina').val();
    pagina++;
    $('#hdnPagina').val(pagina);
    var jsonFiltros = obterJsonFiltrosInscritos(idConcurso);
    var url = homePage + 'Admin/Concurso/PesquisarInscritos';

    $.ajax({
        type: "POST",
        url: url,
        data: jsonFiltros,
        success: function (result)
        {
            if (limpar !== undefined && limpar)
                $('.scrollTableBody').html('');

            if (result.TotalItens > 0)
                $('.scrollTableBody').append(result.View);
            else
                $('.mais').hide();

            $('.listagem').unblock();
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'maisInscritos()', ResponseText: xhr.responseText });
            $('.listagem').unblock();
        }
    });
}

function montarTabela()
{
    $.fn.dataTable.moment('DD/MM/YYYY HH:mm');

    oTable = $('#tblConcursos').dataTable(
            {
                "bLengthChange": false,
                "order": [[2, "desc"]],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [0, 6, 7, 8, 9] }],
                "language": {
                    "url": urlDataTable
                },
                "fnDrawCallback": function (oSettings) {
                    var left = $('.dataTables_filter').offset().left;
                    $('.remessa').offset({ left: left - 50 });
                    $('.updateConcurso').offset({ left: left - 100 });
                    $('.adicionarEmpresa').offset({ left: left - 150 });
                    $('.remessa').show();
                    $('.updateConcurso').show();
                    $('.adicionarEmpresa').show();
                }
            }
        );

    

    $('#tblConcursos_filter').parent().removeClass('col-sm-6').addClass('col-sm-10');
    $('#tblConcursos_filter input').focus();
}

function montarTabelaInscritos() {
    oTable = $('#tblInscritos').dataTable(
            {
                "bLengthChange": false,
                "order": [[2, "asc"]],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [0, 9] }],
                "language": {
                    "url": urlDataTable
                },
                "fnDrawCallback": function (oSettings) {
                    //$('#tblInscritos_filter').parent().append($('.estatisticas'));
                    var left = $('#tblInscritos_filter').offset().left;
                    $('.estatisticas').offset({ left: left - 50 });
                    $('.estatisticas').show();

                    $('#tblInscritos_filter input').focus();
                }
            }
        );
}

function obterContentHeight(val, original)
{
    if (original !== undefined && original)
        return tamanhoInicialAba - $('.cd-tabs-content').offset().top - val;
    else
    {
        var contentH = $('.cd-tabs-content').height();
        var newH = window.innerHeight - $('.cd-tabs-content').offset().top - val;
        return newH > contentH ? newH : contentH;
    }
}

function obterData(valData)
{
    var today = new Date();
    var ano = valData.substring(6, 10);
    var mes = valData.substring(3, 5);
    var dia = valData.substring(0, 2);
    var hora = valData.substring(11, 13);
    var minuto = valData.substring(14, 16);
    var dataRetorno = new Date(ano, mes - 1, dia, hora, minuto);

    return dataRetorno;
}

function obterJsonFiltrosInscritos(idConcurso)
{
    var matricula = $('#txtMatricula').val();
    var nome = $.trim($('#txtNome').val());
    var cpf = removerTodosCaracteresMenosNumeros($('#txtCPF').val());
    var ativo = $('#ddlAtivo').val() == '' ? null : $('#ddlAtivo').val();
    var isento = $('#ddlIsento').val() == '' ? null : $('#ddlIsento').val();
    var ordem = $('.selecionado').hasClass('nome') ? 'N' : 'M';
    var pagina = $('#hdnPagina').val();

    if (matricula == "")
        matricula = 0;

    //alert(matricula + " - " + nome + ' - ' + cpf + ' - ' + ativo + ' - ' + isento + ' - ' + ordem + ' - ' + pagina);

    return { idConcurso: idConcurso, pagina: pagina, ordem: ordem, matricula: parseInt(matricula), nome: nome, cpf: cpf, ativo: ativo, isento: isento };
}

function obterLabelBotaoTipoModal(idTipo)
{
    var label = 'Gerar';

    switch (idTipo)
    {
        case 1:
            if (labelGerarBoleto !== undefined)
                label = labelGerarBoleto;
            break;
        case 2:
            if (labelGerarComprovante !== undefined)
                label = labelGerarComprovante;
            break;
        case 3:
            if (labelObterClassificacao !== undefined)
                label = labelObterClassificacao;
            break;
        case 4:
            if (labelGerarStatusPagamento !== undefined)
                label = labelGerarStatusPagamento;
            break;
    }

    return label;
}

function obterMensagemTipoModal(idTipo)
{
    var mensagem = 'Carregando ...';

    switch(idTipo)
    {
        case 1:
            if (mensagemGerandoBoletoBancario !== undefined)
                mensagem = mensagemGerandoBoletoBancario;
            break;
        case 2:
            if (mensagemGerandoComprovante !== undefined)
                mensagem = mensagemGerandoComprovante;
            break;
        case 3:
            if (mensagemObtendoClassificacao !== undefined)
                mensagem = mensagemObtendoClassificacao;
            break;
        case 4:
            if (mensagemGerandoStatusPagamento !== undefined)
                mensagem = mensagemGerandoStatusPagamento;
            break;
    }

    return mensagem;
}

function obterSufixoTipoModal(idTipo)
{
    switch (idTipo)
    {
        case 1:
            return 'boleto';
        case 2:
            return 'comprovante';
        case 3:
            return 'classificacao';
        case 4:
            return 'status-pagamento';
        default:
            return '';
    }
}

function pesquisarInscritos(idConcurso)
{
    $.blockUI({ message: mensagemCarregando, css: cssCarregando });
    $('.matricula').removeClass('selecionado').addClass('order');
    $('.nome').addClass('selecionado').removeClass('order');
    $('#hdnPagina').val('1');
    var jsonFiltros = obterJsonFiltrosInscritos(idConcurso);
    var url = homePage + 'Admin/Concurso/PesquisarInscritos';
    
    $.ajax({
        type: "POST",
        url: url,
        data: jsonFiltros,
        success: function (result)
        {
            $('.scrollTable').hide();
            $('.emptyTable').hide();
            $('.scrollTableBody').html('');

            if (result.TotalItens > 0)
            {
                $('.scrollTableBody').html(result.View);
                $('.scrollTable').fadeIn(1000);
                $('.mais').show();
            }
            else
            {
                $('.emptyTable').fadeIn(1000);
            }

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'pesquisarInscritos()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function realizarUploadArquivoRemessa()
{
    $('.modal-remessa').find('.modal-content').block({ message: mensagemProcessandoArquivoRemessa, css: cssCarregando });
    var url = homePage + 'Admin/Concurso/RealizarUploadArquivoRemessa';

    $.ajax({
        type: "POST",
        url: url,
        success: function (retorno) {
            $('#txtUploadFile').val('');
            $('.modal-remessa').find('.modal-content').unblock();

            if (retorno.Sucesso) {
                $('.modal-remessa').find('.upload-file').find('.sucesso').fadeIn(1000).fadeOut(3000).promise().done(function () { $('.modal-remessa').modal('hide'); });
                listarConcursos();
            }
            else {
                $('.modal-remessa').find('.upload-file').find('.erro').fadeIn(1000).fadeOut(3000);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'realizarUploadArquivoRemessa()', ResponseText: xhr.responseText });
            $('.modal-remessa').find('.modal-content').unblock();
        }
    });

    return false;
}

function realizarUploadArquivoConcurso()
{
    $('.modal-concurso').find('.modal-content').block({ message: mensagemProcessandoArquivoConcurso, css: cssCarregando });
    var url = homePage + 'Admin/Concurso/RealizarUploadArquivoConcurso';

    $.ajax({
        type: "POST",
        url: url,
        success: function (retorno)
        {
            $('#txtUploadFile').val('');
            $('.modal-concurso').find('.modal-content').unblock();

            if (retorno.Sucesso)
            {
                $('.modal-concurso').find('.upload-file').find('.sucesso').fadeIn(1000).fadeOut(3000).promise().done(function () { $('.modal-concurso').modal('hide'); });
                listarConcursos();
            }
            else
            {
                $('.modal-concurso').find('.upload-file').find('.erro').fadeIn(1000).fadeOut(3000);
            }
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'realizarUploadArquivoConcurso()', ResponseText: xhr.responseText });
            $('.modal-concurso').find('.modal-content').unblock();
        }
    });

    return false;
}

function recarregarTabelaAnexos(idConcurso, idTipo) {
    var url = homePage + 'Admin/Concurso/RecarregarTabelaAnexo';

    $.ajax({
        type: "POST",
        url: url,
        data: { idConcurso: idConcurso, idTipo: idTipo },
        success: function (retorno) {
            var idDiv = (idTipo == 1) ? "#divPublicacoes" : "#divProvasGabaritos";
            $(idDiv).html('');
            $(idDiv).html(retorno.View);

            $(idDiv).find('.editarAnexo').on('click', function (e) { e.preventDefault(); abrirModalAnexo($(this).closest('tr').find('td:first').html(), idConcurso, idTipo); return false; });
            $(idDiv).find('.excluirAnexo').on('click', function () { excluirAnexo($(this).closest('tr').find('td:first').html(), idConcurso, idTipo); });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'recarregarTabelaAnexos()', ResponseText: xhr.responseText });
        }
    });
}

function recarregarTabelaCargos(idConcurso) {
    var url = homePage + 'Admin/Concurso/RecarregarTabelaCargo';

    $.ajax({
        type: "POST",
        url: url,
        data: { idConcurso: idConcurso },
        success: function (retorno) {
            var idDiv = "#divCargos";
            $(idDiv).html('');
            $(idDiv).html(retorno.View);

            $('#divCargos').find('.editarCargo').on('click', function () { abrirModalCargo($(this).closest('tr').find('td:first').html(), idConcurso); });
            $('#divCargos').find('.excluirCargo').on('click', function () { excluirCargo($(this).closest('tr').find('td:first').html(), idConcurso); });
            $('#divCargos').find('.editarAtivacao').on('click', function () { editarAtivacao($(this).closest('tr').find('td:first').html(), idConcurso); });
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'recarregarTabelaCargos()', ResponseText: xhr.responseText });
        }
    });
}

function salvar() {
    var id = $('#hdnId').val() <= 0 ? 0 : $('#hdnId').val();
    var idTipoLayout = $('#ddlTipoLayout option:selected').val();
    var emp_idt_empresa = $('#ddlEmpresa').val();
    var nome = $('#txtNome').val();
    var dataInicio = $('#txtDataInicio').val();
    var dataEncerramentoInscricoes = $('#txtDataEncerramentoInscricoes').val();
    var dataEncerramento = $('#rdEncSim').is(':checked') ? $('#txtDataEncerramento').val() : '';
    var dataInicioComprovante = $('#txtDataInicioComprovante').val();
    var dataEncerramentoComprovante = $('#txtDataEncerramentoComprovante').val();
    var dataInicioClassificacao = $('#txtDataInicioClassificacao').val();
    var dataInicioIsento = $('#txtDataInicioIsento').val();
    var dataEncerramentoIsento = $('#txtDataEncerramentoIsento').val();
    var dataBoleto = $('#txtDataBoleto').val();
    var encerrado = $('#rdEncSim').is(':checked');
    var inscricaoOnline = $('#rdIOSim').is(':checked');
    var ativo = $('#rdAtivoSim').is(':checked');

    $.blockUI({ message: mensagemSalvando, css: cssCarregando });
    var url = homePage + 'Admin/Concurso/Salvar';

    $.ajax({
        type: "POST",
        url: url,
        data: { id: id, idTipoLayout: idTipoLayout, emp_idt_empresa: emp_idt_empresa, nome: nome, dataInicio: dataInicio, dataEncerramentoInscricoes: dataEncerramentoInscricoes, dataEncerramento: dataEncerramento, dataInicioComprovante: dataInicioComprovante, dataEncerramentoComprovante: dataEncerramentoComprovante, dataInicioClassificacao: dataInicioClassificacao, dataInicioIsento: dataInicioIsento, dataEncerramentoIsento: dataEncerramentoIsento, dataBoleto: dataBoleto, encerrado: encerrado, inscricaoOnline: inscricaoOnline, ativo: ativo },
        success: function (retorno) {
            alert(retorno.Mensagem);

            if (retorno.Sucesso)
                window.location.href = homePage + 'Admin/Concurso';

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'salvar()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function tabsContentResize(val, original)
{
    var isModalOpened = $('#modal-boleto').find('.modal').is(':visible');

    if (isMobile || isModalOpened)
        return;

    setTimeout(function ()
    {
        $('.cd-tabs-content').height(obterContentHeight(val, original));
    }, 500);
    //if (original !== undefined && original)
    //    $('.cd-tabs-content').height(
    //        function (index, height)
    //        {
    //            return obterContentHeight(val, original);
    //        });
    //else
    //    $('.cd-tabs-content').height(
    //        function (index, height)
    //        {
    //            return obterContentHeight(val, original);
    //        });
}

function uploadFileAnexos(e, idConcurso) {
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
                url: homePage + 'Concurso/UploadAnexosConcurso?idConcurso=' + idConcurso,
                contentType: false,
                processData: false,
                data: data,
                success: function (retorno) {
                    if (retorno.Sucesso)
                        $('.sucesso').fadeIn(1000).fadeOut(3000);
                    else
                        $('.erro').fadeIn(1000).fadeOut(3000);

                    $.unblockUI();
                },
                error: function (xhr, status, p3, p4) {
                    alertaErroJS({ NomeFuncao: 'uploadFileAnexos()', ResponseText: xhr.responseText });
                    $.unblockUI();
                }
            });
        }
    }
}

function uploadFileAnexoRecurso(e)
{
    var files = e.target.files;
    if (files.length > 0) {
        if (window.FormData !== undefined) {
            var data = new FormData();
            for (var x = 0; x < files.length; x++)
                data.append("file" + x, files[x]);

            $.ajax({
                type: "POST",
                url: homePage + 'Concurso/UploadAnexoRecurso',
                contentType: false,
                processData: false,
                data: data,
                success: function (retorno) {},
                error: function (xhr, status, p3, p4) {
                    alertaErroJS({ NomeFuncao: 'enviarRecurso-Anexo()', ResponseText: xhr.responseText });
                    $.unblockUI();
                }
            });
        }
    }
}

function uploadFileRemessa(e) {
    var files = e.target.files;
    if (files.length > 0) {
        if (window.FormData !== undefined) {
            var data = new FormData();
            for (var x = 0; x < files.length; x++)
                data.append("file" + x, files[x]);

            $.ajax({
                type: "POST",
                url: homePage + 'Admin/Concurso/UploadAnexoRemessa',
                contentType: false,
                processData: false,
                data: data,
                success: function (retorno) { },
                error: function (xhr, status, p3, p4) {
                    alertaErroJS({ NomeFuncao: 'uploadFileRemessa()', ResponseText: xhr.responseText });
                    $.unblockUI();
                }
            });
        }
    }
}

function uploadFileConcurso(e)
{
    var files = e.target.files;
    if (files.length > 0)
    {
        if (window.FormData !== undefined)
        {
            var data = new FormData();
            for (var x = 0; x < files.length; x++)
                data.append("file" + x, files[x]);

            $.ajax({
                type: "POST",
                url: homePage + 'Admin/Concurso/UploadAnexoConcurso',
                contentType: false,
                processData: false,
                data: data,
                success: function (retorno) { },
                error: function (xhr, status, p3, p4)
                {
                    alertaErroJS({ NomeFuncao: 'uploadFileConcurso()', ResponseText: xhr.responseText });
                    $.unblockUI();
                }
            });
        }
    }
}