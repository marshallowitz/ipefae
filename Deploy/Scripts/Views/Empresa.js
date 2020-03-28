var oTable;

function editar(id)
{
    window.location.href = homePage + 'Admin/Concurso/Cadastro_Empresa/' + id;
}

function editarAtivacao(divEditarAtivacao, id)
{
    $.blockUI({ message: mensagemDesativando, css: cssCarregando });
    var url = homePage + 'Admin/Concurso/EditarAtivacaoEmpresao';

    $.ajax({
        type: "POST",
        url: url,
        data: { id: id },
        success: function (retorno) {
            if (retorno.Sucesso) {
                divEditarAtivacao.removeClass('checked');
                divEditarAtivacao.removeClass('unchecked');

                if (retorno.Ativo)
                    divEditarAtivacao.addClass('checked');
                else
                    divEditarAtivacao.addClass('unchecked');

                alert(operacaoMensagemSucesso);
            }
            else
                alert(operacaoMensagemErro);

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'editarAtivacao()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function iniciarTelaListaEmpresas() {
    listarEmpresas();
}

function iniciarTelaCadastroEmpresa()
{
    $('.userForm')
    .on('init.field.bv', function (e, data) { removerIconeValidator(e, data); })
    .bootstrapValidator({
        container: 'tooltip',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        }
        , fields: {
            RazaoSocial: { validators: { notEmpty: { message: razaoSocialObrigatorio } } }
            , Nome: { validators: { notEmpty: { message: nomeObrigatorio } } }
            , cnpj: { validators: { notEmpty: { message: cnpjObrigatorio } } }
            , banco: { validators: { notEmpty: { message: bancoObrigatorio } } }
            , Agencia: { validators: { notEmpty: { message: agenciaObrigatorio } } }
            , ContaCorrente: { validators: { notEmpty: { message: contaObrigatorio } } }
            , Convenio: { validators: { notEmpty: { message: convenioObrigatorio } } }
        }
    })
    .on('success.form.bv', function (e) {
        e.preventDefault(); // Prevent form submission
        salvar();
        return false;
    });

    if ($('.cadastro-empresa').find('#Convenio').val() == '0')
        $('.cadastro-empresa').find('#Convenio').val('');

    $('.cadastro-empresa').find('#txtCNPJ').mask("00.000.000/0000-00");
    $('.cadastro-empresa').find('#Convenio').mask("0######");
    $('.cadastro-empresa').find('#Agencia').mask("0###");
    $('.cadastro-empresa').find('#ContaCorrente').mask("0#######");

    if ($('#Id').val() <= 0)
    {
        $('#chkAtivo').prop("checked", true);
        $('#chkAtivo').attr("disabled", true);
    }
    else
        $('#ddlBanco').val(bancoSelecionado);
}

function listarEmpresas()
{
    $.blockUI({ message: mensagemCarregando, css: cssCarregando });
    var url = homePage + 'Admin/Concurso/ListarEmpresas';

    $.ajax({
        type: "POST",
        url: url,
        success: function (retorno) {
            $('.lista').html('');
            $('.lista').append(retorno);
            montarTabela();

            if ($('.lista').find('table').attr('summary') == naoEncontrado)
                $('.lista').css('marginTop', 0);

            $('.editarAtivacao').on('click', function () { editarAtivacao($(this), $(this).closest('tr').find('td:first span').html()); });
            $('.editar').on('click', function () { editar($(this).closest('tr').find('td:first span').html()); });

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'listarEmpresas()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function montarTabela()
{
    oTable = $('#tblEmpresas').dataTable(
            {
                "bLengthChange": false,
                "order": [[1, "asc"]],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [0, 3, 4, 5, 6] }],
                "language": {
                    "url": urlDataTable
                }
            }
        );

    $('#tblEmpresas_filter').parent().removeClass('col-sm-6').addClass('col-sm-10');
    $('#tblEmpresas_filter input').focus();
}

function salvar() {
    var id = $('#Id').val() <= 0 ? 0 : $('#Id').val();
    var nome = $('#Nome').val();
    var razaoSocial = $('#RazaoSocial').val();
    var cnpj = removerTodosCaracteresMenosNumeros($('#txtCNPJ').val());
    var banco = $('#ddlBanco').val();
    var convenio = removerTodosCaracteresMenosNumeros($('#Convenio').val());
    var convenio_cobranca = removerTodosCaracteresMenosNumeros($('#ConvenioCobranca').val());
    var agencia = removerTodosCaracteresMenosNumeros($('#Agencia').val());
    var conta = removerTodosCaracteresMenosNumeros($('#ContaCorrente').val());
    var ativo = $('#chkAtivo').is(':checked');

    $.blockUI({ message: mensagemSalvando, css: cssCarregando });
    var url = homePage + 'Admin/Concurso/Salvar_Empresa';

    $.ajax({
        type: "POST",
        url: url,
        data: { id: id, nome: nome, razaoSocial: razaoSocial, cnpj: cnpj, banco: banco, convenio: convenio, convenio_cobranca: convenio_cobranca, agencia: agencia, conta: conta, ativo: ativo },
        success: function (retorno) {
            alert(retorno.Mensagem);

            if (retorno.Sucesso)
                window.location.href = homePage + 'Admin/Concurso/Index_Empresa';

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'salvar()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}