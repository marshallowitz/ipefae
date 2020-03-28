var enviandoContato = false;

function iniciarTelaContato(opts) {
    var spanInitialValue = opts.spanInitialValue;
    var spanEndValue = opts.spanEndValue;
    var nomeContatoObrigatorio = opts.nomeContatoObrigatorio;
    var emailContatoObrigatorio = opts.emailContatoObrigatorio;
    var formatoInvalidoEmailContato = opts.formatoInvalidoEmailContato;
    var telefoneContatoObrigatorio = opts.telefoneContatoObrigatorio;
    var formatoInvalidoTelefoneContato = opts.formatoInvalidoTelefoneContato;
    var mensagemContatoObrigatoria = opts.mensagemContatoObrigatoria;

    $('section .contactForm input[type=submit]').attr('disabled', 'disabled');
    $('.contactForm').find('#txtTelefone').mask("(00) 00009-0000").focusin(function () { focusIn($(this)); }).focusout(function () { ajustarCelularSP($(this)); });
    $('.slider').slideUnlock({ spanInitialValue: spanInitialValue, spanEndValue: spanEndValue, urlSessionCaptcha: homePage + 'Base/Captcha' });

    $('.contactForm')
        .on('init.field.bv', function (e, data) { removerIconeValidator(e, data); })
        .bootstrapValidator({
            container: 'tooltip',
            feedbackIcons: {
                valid: 'glyphicon glyphicon-ok',
                invalid: 'glyphicon glyphicon-remove',
                validating: 'glyphicon glyphicon-refresh'
            }
            , fields: {
                nomeContato: {
                    validators: {
                        notEmpty: { message: nomeContatoObrigatorio }
                    }
                }
                , emailContato: {
                    validators: {
                        notEmpty: { message: emailContatoObrigatorio },
                        emailAddress: { message: formatoInvalidoEmailContato }
                    }
                }
                , telefoneContato: {
                    validators: {
                        notEmpty: { message: telefoneContatoObrigatorio }
                        //, emailAddress: { message: formatoInvalidoTelefoneContato }
                    }
                }
                , mensagemContato: {
                    validators: {
                        notEmpty: { message: mensagemContatoObrigatoria }
                    }
                }
            }
        })
        .on('success.form.bv', function (e) {
            e.preventDefault(); // Prevent form submission

            if (enviandoContato)
                return false;

            if (checarCaptcha($('section#contato .sliderContent'))) {
                $('section#contato .sliderContent').find('.sliderTooltip').hide();
                enviarEmail();
            }
            else
                $('section#contato .sliderContent').find('.sliderTooltip').show();

            return false;
        });

    $('.modal-contato').on('hidden.bs.modal', function () {
        $('.contactForm').bootstrapValidator("resetForm", true);
    });

    $('.modal-contato').on('shown.bs.modal', function () {
        $('.modal-contato').find('#txtNome').focus();
    });
}

function enviarEmail() {
    enviandoContato = true;
    var txtNome = $('section#contato').find('#txtNome').length == 0 ? $('.modal-contato').find('#txtNome') : $('section#contato').find('#txtNome');
    var txtEmail = $('section#contato').find('#txtEmail').length == 0 ? $('.modal-contato').find('#txtEmail') : $('section#contato').find('#txtEmail');
    var txtTelefone = $('section#contato').find('#txtTelefone').length == 0 ? $('.modal-contato').find('#txtTelefone') : $('section#contato').find('#txtTelefone');
    var txtMensagem = $('section#contato').find('#txtMensagem').length == 0 ? $('.modal-contato').find('#txtMensagem') : $('section#contato').find('#txtMensagem');
    var nome = txtNome.val();
    var email = txtEmail.val();
    var telefone = txtTelefone.val();
    var mensagem = txtMensagem.val();

    var url = homePage + 'Base/EnviarContato';
    var data = { nome: nome, email: email, telefone: telefone, mensagem: mensagem };

    $.ajax({
        type: 'POST',
        url: url,
        data: data,
        async: false,
        success: function (retorno) {
            txtNome.val('');
            txtEmail.val('');
            txtTelefone.val('');
            txtMensagem.val('');
            $('.contactForm').bootstrapValidator("resetForm", true);
            enviandoContato = false;

            if ($('section#contato .sliderContent').length > 0)
                $('section#contato .sliderContent').fadeOut().promise().done(function () { $('section#contato .mensagemEnviada').fadeIn(); });
            else
                $('.modal-contato .mensagemEnviada').fadeIn();

            setTimeout(reiniciarCaptcha, 2000);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            enviandoContato = false;
            alertaErroJS({ NomeFuncao: 'enviarEmail()', ResponseText: xhr.responseText });
        }
    });
}

function reiniciarCaptcha() {
    if ($('section#contato .mensagemEnviada').length > 0)
        $('section#contato .mensagemEnviada').fadeOut().promise().done(function () {
            $('section#contato .sliderContent').fadeIn();
            $('section#contato .sliderContent .slider').animate({ left: "0px" });
            $('section#contato .sliderContent .slider').draggable('enable');
            $('section#contato .sliderContent').find('span').html(spanInitialValue);
            $('section#contato .sliderContent').find('input[type=hidden]').val('');
        });
    else {
        $('.modal-contato .mensagemEnviada').fadeOut();
        $('.modal-contato').modal('hide');
    }
}