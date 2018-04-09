function iniciarTelaInicialColaborador()
{
    $('.modal-reenviar-senha').find('#txtCPF').mask('999.999.999-99');

    $('.colLoginForm')
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
                    notEmpty: { message: 'E-mail é obrigatório' },
                    emailAddress: { message: 'Formato inválido de e-mail' }
                }
            },
            senha: {
                validators: {
                    notEmpty: { message: 'Senha obrigatória' }
                }
            }
        }
    }).on('success.form.bv', function (e)
    {
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
        , fields: { cpf: { validators: { notEmpty: { message: 'CPF obrigatório' } } } }
    }).on('success.form.bv', function (e)
    {
        e.preventDefault(); // Prevent form submission
        reenviarSenha();
    });

    $('.modal-reenviar-senha').on('hide.bs.modal', function ()
    {
        $('.form-group').removeClass('has-error').removeClass('has-success');
        $('.form-group').find('small.help-block').hide();
        $('.form-group').find('i.form-control-feedback').hide();
        $('.modal-reenviar-senha').find('#txtCPF').val('');
    });
    $('.modal-reenviar-senha').on('shown.bs.modal', function () { $('.modal-reenviar-senha').find('#txtCPF').focus(); });
}

function realizarLogin()
{
    $.blockUI({ message: 'Realizando Login...', css: cssCarregando });
    var url = homePage + 'Colaborador/RealizarLogin';
    var email = $('.colLoginForm').find('#txtEmail').val();
    var senha = $('.colLoginForm').find('#txtSenha').val();

    $.ajax({
        type: "POST",
        url: url,
        data: { email: email, senha: senha },
        success: function (retorno)
        {
            if (retorno.Sucesso)
                window.location.href = homePage + 'Colaborador/Cadastro/' + retorno.IdColaborador;
            else {
                alert('E-mail ou senha não encontrado');
                $('#txtEmail').focus();
            }

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            $.unblockUI();
            alertaErroJS({ NomeFuncao: 'realizarLogin()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

function reenviarSenha()
{
    $('.modal-reenviar-senha').find('.modal-content').block({ message: 'Reenviando senha por e-mail', css: cssCarregando });
    var url = homePage + 'Colaborador/EnviarSenhaPorEmail';
    var cpf = removerTodosCaracteresMenosNumeros($('.modal-reenviar-senha').find('#txtCPF').val());

    $.ajax({
        type: "POST",
        url: url,
        data: { cpf: cpf },
        success: function (retorno)
        {
            if (retorno)
                alert('Senha enviada para o e-mail com sucesso');
            else {
                $('.modal-reenviar-senha').find('.cpf-nao-encontrado').fadeIn(1000).fadeOut(3000);
                $('.modal-reenviar-senha').find('#txtCPF').focus();
            }

            $('.modal-reenviar-senha').find('.modal-content').unblock();
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            $('.modal-reenviar-senha').find('.modal-content').unblock();
            alertaErroJS({ NomeFuncao: 'reenviarSenha()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

