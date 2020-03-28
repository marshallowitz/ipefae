function abrirModalLogin()
{
    var isMobile = ($(window).width() < 800);
    var url = homePage + 'Account/AbrirModalLogin';

    $.ajax({
        type: "POST",
        url: url,
        success: function (retorno) {
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            $('section#modal-login').html('');
            $('section#modal-login').html(retorno.View);

            iniciarTelaModalLogin();

            if ($('section#modal-login').parent().context.URL.indexOf('Concurso') > 0 && !isMobile)
                $('section#modal-login').find('.confirmar').css('marginTop', '-20px');

            $('.modal-login').modal('show');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'abrirModalLogin()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

function definirTelaAposLogin(mensagem, admin, url) {
    $('.ulLogado').show();
    $('.ulDeslogado').hide();

    var urlNova = homePage + url;

    if (admin) {
        $('.menu-admin').show();
    }

    $('.modal-login').modal('hide');
    window.location.href = urlNova;
}

function iniciarTelaLogin()
{
    //console.log('oi');
    verificarUsuarioLogado();
    $('.menu-login-li').on('mouseover', function () { $('.menu-login-entrar').stop().animate({ width: 'toggle' }, "slow"); });
    $('.menu-login-li').on('mouseout', function () { $('.menu-login-entrar').stop().animate({ width: 'toggle' }, "slow"); });

    $('section#modal-login').on('shown.bs.modal', function () {
        $('section#modal-login').find('#txtEmail').focus();
    });

    $('section#modal-login').on('hidden.bs.modal', function () {
        $('section#modal-login').find('#txtEmail').val('');
        $('section#modal-login').find('#txtSenha').val('');
    });
}

function iniciarTelaModalLogin()
{
    $('.modal-login').on('show.bs.modal', function () {
        $('.frmLogin')
            .on('init.field.bv', function (e, data) { removerIconeValidator(e, data); })
            .bootstrapValidator({
                container: 'tooltip',
                feedbackIcons: {
                    valid: 'glyphicon glyphicon-ok',
                    invalid: 'glyphicon glyphicon-remove',
                    validating: 'glyphicon glyphicon-refresh'
                },
                fields: {
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
                var email = $(this).find('#txtEmail').val();
                var senha = $(this).find('#txtSenha').val();
                $(this).find('#txtEmail').val('');
                $(this).find('#txtSenha').val('');

                realizarLogin(email, senha);
            });

        var $txtEmailModal = $('.modal-login').find('#txtEmail');
        $txtEmailModal.val('');
        $txtEmailModal.parents('.form-group').find('.form-control-feedback[data-bv-icon-for="email"]').trigger('click.clearing');

        var $txtSenhaModal = $('.modal-login').find('#txtSenha');
        $txtSenhaModal.val('');
        $txtSenhaModal.parents('.form-group').find('.form-control-feedback[data-bv-icon-for="senha"]').trigger('click.clearing');
    });

    $('.modal-login').on('shown.bs.modal', function () {
        if ($('.modal-login').find('#txtEmail').val() === '')
            $('.modal-login').find('#txtEmail').focus();
        else
            $('.modal-login').find('#txtSenha').focus();
    });

    $('.modal-login').on('hidden.bs.modal', function () {
        var $txtEmail = $('#txtEmail')
        $txtEmail.val('');
        $txtEmail.parents('.form-group').find('.form-control-feedback[data-bv-icon-for="email"]').trigger('click.clearing');
        $txtEmail.focus();
    });
}

function realizarLogin(email, senha) {
    $.blockUI({ message: mensagemBlockUILogin, css: cssCarregando });
    $('.frmLogin').closest(".modal-dialog").hide();
    var url = homePage + 'Account/RealizarLogin';

    $.ajax({
        type: "POST",
        url: url,
        data: { email: email, senha: senha },
        success: function (retorno) {
            $('.frmLogin').bootstrapValidator("resetForm", true);

            if (retorno.status.toLowerCase() == 'ok') {
                definirTelaAposLogin(retorno.Mensagem, retorno.Admin, retorno.Url);
            }
            else if (retorno.status.toLowerCase() == 'senhaerrada') {
                $.unblockUI();
                alert(retorno.Mensagem);
                $('.frmLogin').closest(".modal-dialog").show();
                $('.frmLogin').find('#txtEmail').focus();
                return;
            }
            else {
                alert(erroLogin);
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

function realizarLogOff()
{
    $.blockUI({ message: mensagemBlockUILogoff, css: cssCarregando });
    var url = homePage + 'Account/LogOff';

    $.ajax({
        type: "POST",
        url: url,
        success: function (retorno) {
            window.location.href = homePage;
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $.unblockUI();
            alertaErroJS({ NomeFuncao: 'realizarLogOff()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

function verificarUsuarioLogado() {
    $('.ulLogado').hide();
    $('.ulDeslogado').hide();
    $('.menu-admin').hide();

    var url = homePage + 'Account/VerificarUsuarioLogado';

    $.ajax({
        type: "POST",
        url: url,
        success: function (retorno) {
            if (retorno.Logado) {
                $('.spanOla').html(retorno.Ola);
                $('.ulLogado').show();

                if (retorno.Admin)
                    $('.menu-admin').show();
            }
            else
                $('.ulDeslogado').show();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'verificarUsuarioLogado()', ResponseText: xhr.responseText });
            realizarLogOff();
        }
    });

    return false;
}