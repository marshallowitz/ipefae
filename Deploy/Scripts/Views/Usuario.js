var oTable;

function editar(id)
{
    window.location.href = homePage + 'Admin/Usuario/Cadastro/' + id;
}

function editarAtivacao(divEditarAtivacao, id)
{
    $.blockUI({ message: mensagemDesativando, css: cssCarregando });
    var url = homePage + 'Admin/Usuario/EditarAtivacao';

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

function iniciarTelaListaUsuarios() {
    listarUsuarios();
}

function iniciarTelaCadastroUsuario()
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
            Nome: {
                validators: {
                    notEmpty: { message: nomeObrigatorio }
                }
            }
            , email: {
                validators: {
                    notEmpty: { message: emailObrigatorio },
                    emailAddress: { message: formatoInvalidoEmail }
                }
            }
            , Telefone: {
                validators: {
                    notEmpty: { message: telefoneObrigatorio }
                    //, emailAddress: { message: formatoInvalidoTelefoneContato }
                }
            }
            , Senha: {
                validators: {
                    notEmpty: { message: senhaObrigatorio },
                    identical: {
                        field: 'ConfirmacaoSenha',
                        message: senhaConfirmacaoIguais
                    }
                }
            }
            , ConfirmacaoSenha: {
                validators: {
                    notEmpty: { message: confirmacaoSenhaObrigatorio },
                    identical: {
                        field: 'Senha',
                        message: senhaConfirmacaoIguais
                    }
                }
            }
        }
    })
    .on('success.form.bv', function (e) {
        e.preventDefault(); // Prevent form submission
        salvar();
        return false;
    });

    var options = {};
    options.ui = { container: "#pwd-container", showVerdictsInsideProgressBar: true };
    $('#Senha').pwstrength(options);

    if ($('.cadastro-usuario').find('#Telefone').val() == '0')
        $('.cadastro-usuario').find('#Telefone').val('');

    $('.cadastro-usuario').find('#Telefone').mask("(00) 00009-0000").focusin(function () { focusIn($(this)); }).focusout(function () { ajustarCelularSP($(this)); });

    if ($('#Id').val() <= 0) {
        $('#chkAtivo').prop("checked", true);
        $('#chkAtivo').attr("disabled", true);
    }
    else {
        $('#chkAdmin_Geral').prop("checked", isUAdmin);
        $('#chkAdmin_Concurso').prop("checked", isUConcurso);
        $('#chkAdmin_Estagio').prop("checked", isUEstagio);
        $('#chkAdmin_Vestibular').prop("checked", isUVestibular);
    }

    if (!isAdmin) {
        $('#chkAdmin_Geral').attr("disabled", true);

        if (!isConcurso)
            $('#chkAdmin_Concurso').attr("disabled", true);

        if (!isEstagio)
            $('#chkAdmin_Estagio').attr("disabled", true);

        if (!isVestibular)
            $('#chkAdmin_Vestibular').attr("disabled", true);
    }

    mudarCheckAdmin();
    $('#chkAdmin_Geral').on('change', function () { mudarCheckAdmin(); });
    $('#aTrocarSenha').click(function () { $(this).hide(); $('#liSenha').show(); });
}

function listarUsuarios()
{
    $.blockUI({ message: mensagemCarregando, css: cssCarregando });
    var url = homePage + 'Admin/Usuario/ListarUsuarios';

    $.ajax({
        type: "POST",
        url: url,
        success: function (retorno) {
            $('.lista').html('');
            $('.lista').append(retorno);
            montarTabela();

            if ($('.lista').find('table').attr('summary') == naoEncontrado)
                $('.lista').css('marginTop', 0);

            $('.telefone').each(function () {
                $(this).mask("(00) 00009-0000").focusin(function () { focusIn($(this)); }).focusout(function () { ajustarCelularSP($(this)); });
            });
            $('.editarAtivacao').on('click', function () { editarAtivacao($(this), $(this).closest('tr').find('td:first span').html()); });
            $('.editar').on('click', function () { editar($(this).closest('tr').find('td:first span').html()); });

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'listarUsuarios()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function montarTabela()
{
    oTable = $('#tblUsuarios').dataTable(
            {
                "bLengthChange": false,
                "order": [[1, "asc"]],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [0, 3, 4, 5, 6] }],
                "language": {
                    "url": urlDataTable
                }
            }
        );

    $('#tblUsuarios_filter').parent().removeClass('col-sm-6').addClass('col-sm-10');
    $('#tblUsuarios_filter input').focus();
}

function mudarCheckAdmin()
{
    var chkAdmin_Geral = $('#chkAdmin_Geral');

    if (chkAdmin_Geral.is(':checked')) {
        $('[id^="chkAdmin"]').prop("checked", false);
        $('[id^="chkAdmin"]').attr("disabled", true);

        chkAdmin_Geral.prop("checked", true);
        chkAdmin_Geral.removeAttr("disabled");
    }
    else
        $('[id^="chkAdmin"]').removeAttr("disabled");
}

function salvar() {
    var id = $('#Id').val() <= 0 ? 0 : $('#Id').val();
    var nome = $('#Nome').val();
    var email = $('#Email').val();
    var telefone = removerTodosCaracteresMenosNumeros($('#Telefone').val());
    var senha = $('#liSenha').is(':visible') ? $('#Senha').val() : null;
    var permissao = parseInt($('#chkAdmin_Geral').is(':checked') ? $('#chkAdmin_Geral').attr('valor') : 0) +
                    parseInt($('#chkAdmin_Concurso').is(':checked') ? $('#chkAdmin_Concurso').attr('valor') : 0) +
                    parseInt($('#chkAdmin_Estagio').is(':checked') ? $('#chkAdmin_Estagio').attr('valor') : 0) +
                    parseInt($('#chkAdmin_Vestibular').is(':checked') ? $('#chkAdmin_Vestibular').attr('valor') : 0);
    var ativo = $('#chkAtivo').is(':checked');

    $.blockUI({ message: mensagemSalvando, css: cssCarregando });
    var url = homePage + 'Admin/Usuario/Salvar';

    $.ajax({
        type: "POST",
        url: url,
        data: { id: id, nome: nome, email: email, telefone: telefone, senha: senha, permissao: permissao, ativo: ativo },
        success: function (retorno) {
            alert(retorno.Mensagem);

            if (retorno.Sucesso)
                window.location.href = homePage + 'Admin/Usuario';

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'salvar()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}