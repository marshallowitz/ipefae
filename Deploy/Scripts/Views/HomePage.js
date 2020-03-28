function iniciarHomePage() {
    $('.page-scroll').attr('href', '#' + $('.body-content').find('section').get(1).id);

    $(window).scroll(function () {
        if ($(".navbar").offset().top > 50) {
            $(".navbar-fixed-top").addClass("top-nav-collapse");
            $('.menu-login').addClass("menu-login-white");
            $('.menu-login-entrar').addClass('menu-login-entrar-white');
            $('.menu-usuario').addClass("menu-usuario-white");
            $('.login-partial').addClass("no-top");
            $('.subtitulo').addClass("white");
        } else {
            $(".navbar-fixed-top").removeClass("top-nav-collapse");
            $('.menu-login').removeClass("menu-login-white");
            $('.menu-login-entrar').removeClass('menu-login-entrar-white');
            $('.menu-usuario').removeClass("menu-usuario-white");
            $('.login-partial').removeClass("no-top");
            $('.subtitulo').removeClass("white");
        }
    });
}

function reenviarSenha(dialogRef, btnConfirmar, btnCancelar) {
    var url = homePage + 'Account/ReenviarSenha';

    $.ajax({
        type: "POST",
        url: url,
        data: { email: $('.reenviar-senha').find('#txtEmail').val() },
        success: function (retorno) {
            if (retorno.Status) {
                btnCancelar.html(botaoFechar);
                btnConfirmar.hide();
                dialogRef.setType(BootstrapDialog.TYPE_SUCCESS);
                dialogRef.setTitle(retorno.Titulo);
                dialogRef.setMessage(retorno.Mensagem);
            }
            else {
                btnCancelar.html(botaoFechar);
                btnConfirmar.hide();
                dialogRef.setType(BootstrapDialog.TYPE_DANGER);
                dialogRef.setTitle(retorno.Titulo);
                dialogRef.setMessage(retorno.Mensagem);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'reenviarSenha()', ResponseText: xhr.responseText });
        }
    });

    return false;
}