var oTable;

function editar(id)
{
    window.location.href = homePage + 'Admin/Recurso/Cadastro/' + id;
}

function enviarRecurso(idConcurso, idInscrito)
{
    $.blockUI({ message: mensagemEnviandoRecurso, css: cssCarregando });

    var url = homePage + 'Concurso/EnviarRecurso';
    var mensagem = '';

    if ($('.recurso-isencao ').find('#txtNomeCompleto').length > 0)
    {
        mensagem += labelNome + ': ' + $('.recurso-isencao ').find('#txtNomeCompleto').val() + '<br>';
        mensagem += labelCPF + ': ' + $('.recurso-isencao ').find('.cpfRecurso').html() + '<br>';
        mensagem += labelIdentSocial + ': ' + $('.recurso-isencao ').find('#txtIdentSocial').val() + '<br>';
        mensagem += labelDataNasc + ': ' + $('.recurso-isencao ').find('#txtDataNascimento').val() + '<br>';
        mensagem += labelSexo + ': ' + ($('.recurso-isencao ').find('#ddlSexo').val() == "F" ? optionFeminino : optionMasculino) + '<br>';
        mensagem += labelRG + ': ' + $('.recurso-isencao ').find('#txtRG').val() + '<br>';
        mensagem += labelDataExpedicaoRG + ': ' + $('.recurso-isencao ').find('#txtDataExpedicaoRG').val() + '<br>';
        mensagem += labelOrgaoExpedidorRG + ': ' + $('.recurso-isencao ').find('#txtOrgaoExpedidorRG').val() + '<br>';
        mensagem += labelNomeMae + ': ' + $('.recurso-isencao ').find('#txtNomeMae').val();
    }
    else
        mensagem = $('#txtMensagem').val();

    $.ajax({
        type: "POST",
        url: url,
        data: { idConcurso: idConcurso, idInscrito: idInscrito, mensagem: encodeURIComponent(mensagem) },
        success: function (idRecurso)
        {
            if (idRecurso > 0)
            {
                var url = homePage + 'Concurso/EnviarRecursoEmail';

                $.ajax({
                    type: "POST",
                    url: url,
                    success: function (retorno)
                    {
                        $('.recursoForm').data('bootstrapValidator').resetForm();

                        if ($('.recurso-isencao ').find('#txtNomeCompleto').length > 0)
                        {
                            $('.recurso-isencao ').find('input[type=text]').attr('readonly', 'readonly');
                            $('.recurso-isencao ').find('select').attr('readonly', 'readonly');

                            $('.declaracao').hide();
                        }
                        else
                        {
                            $('.mensagem-aviso').hide();
                            $('.upload-file').parent().hide();
                            $('#txtMensagem').attr('readonly', 'readonly');
                        }
                        
                        $('#btnSalvar').hide();
                        $('#btnVoltarOK').show();
                        
                        $('.mensagem-protocolo').html(mensagemRecursoAbertoSucesso.replace('{0}', retorno.Protocolo));
                        $('.mensagem-protocolo').show();

                        $.unblockUI();
                    },
                    error: function (xhr, ajaxOptions, thrownError)
                    {
                        alertaErroJS({ NomeFuncao: 'enviarRecursoEmail()', ResponseText: xhr.responseText });
                        $.unblockUI();
                    }
                });
            }
            else
                $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'enviarRecurso()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function iniciarFormularioRecurso(params)
{
    var idConcurso = params.idConcurso;
    var idInscrito = params.idInscrito;
    var inscricaoRequerente = params.inscricaoRequerente;
    var nomeRequerente = params.nomeRequerente;
    var cargoRequerente = params.cargoRequerente;

    if ($('.recurso-isencao').find('#txtNomeCompleto').length > 0)
    {
        $('.declaracao').show();

        $('.recurso-isencao').find('#txtDataNascimento').mask('00/00/0000');
        $('.recurso-isencao').find('#txtDataExpedicaoRG').mask('00/00/0000');

        $('.recurso-isencao ').find('input[type=text]').removeAttr('readonly');
        $('.recurso-isencao ').find('select').removeAttr('readonly');

        $('.recurso-isencao').find('input[type=text]').val('');
        $('.recurso-isencao').find('input[type=text]').parent().removeClass('has-success').removeClass('has-error');
        $('.recurso-isencao').find('input[type=text]').parent().find('i').hide();
        $('.recurso-isencao').find('input[type=text]').parent().find('.help-block').hide();
        $('.recurso-isencao').find('select').val('');
        $('.recurso-isencao').find('select').parent().removeClass('has-success').removeClass('has-error');
        $('.recurso-isencao').find('select').parent().find('i').hide();
        $('.recurso-isencao').find('select').parent().find('.help-block').hide();
    }
    else
    {
        $('#txtMensagem').removeAttr('readonly');
        $('#txtMensagem').val('');
        $('#txtMensagem').parent().removeClass('has-success').removeClass('has-error');
        $('#txtMensagem').parent().find('i').hide();
        $('#txtMensagem').parent().find('.help-block').hide();

        $('.upload-file').parent().removeClass('has-success').removeClass('has-error');
        $('.upload-file').parent().find('i').hide();
        $('.upload-file').parent().find('.help-block').hide();
        $('.upload-file').parent().show();

        $('#txtUploadFile').val('');
        $('#txtUploadFile').on('change', function (e) { uploadFileAnexoRecurso(e); });

        $('.inscricao-requerente').html(inscricaoRequerente);
        $('.nome-requerente').html(nomeRequerente);
        $('.cargo-requerente').html(cargoRequerente);

        $('.mensagem-aviso').show();
        $('.mensagem-protocolo').hide();
    }

    $('.cpf-requerente-cargos').html('');

    $('#btnVoltarOK').hide();
    $("#btnVoltarOK").on('click', function () { $('a[data-content="publicacoes"]').trigger('click'); });
    $('#btnSalvar').show();

    $('#hdnIdInscrito').val(idInscrito);
    $('.cpf-requerente').fadeOut(500).promise().done(function ()
    {
        $('.recursoForm').fadeIn(500).promise().done(function ()
        {
            tabsContentResize(150 - $('.recursoForm').height());

            if ($('.recurso-isencao ').find('#txtNomeCompleto').length > 0)
            {
                $('.recurso-isencao').find('.cpfRecurso').html($('.recurso-cadastro-usuario').find('#txtCPF').val());
                $('.recursoForm').find('#txtNomeCompleto').focus();
            }
            else
                $('.recursoForm').find('#txtMensagem').focus();
        });
    });
}

function iniciarFormularioRecursoBootstrap()
{
    $('#txtCPF').mask('999.999.999-99');

    $('.recursoForm')
    .on('init.field.bv', function (e, data) { removerIconeValidator(e, data); })
    .bootstrapValidator({
        container: 'tooltip',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        }
        , fields: {
            nome: { validators: { notEmpty: { message: nomeObrigatorio } } }
            , cpfRecurso: { validators: { notEmpty: { message: cpfObrigatorio } } }
            , identSocial: { validators: { notEmpty: { message: identSocialObrigatorio } } }
            , sexo: { validators: { notEmpty: { message: sexoObrigatorio } } }
            , rg: { validators: { notEmpty: { message: rgObrigatorio } } }
            , orgaoExpedidorRG: { validators: { notEmpty: { message: orgaoExpedidorRGObrigatorio } } }
            , nomeMae: { validators: { notEmpty: { message: nomeMaeObrigatorio } } }
            , dataNasc: { validators: { notEmpty: { message: dataNascObrigatorio }, date: { message: dataNascFormatoInvalido, format: 'DD/MM/YYYY', min: '01/01/1920' } } }
            , dataDataExpedicaoRG: { validators: { notEmpty: { message: dataExpedicaoRGObrigatorio }, date: { message: dataExpedicaoRGFormatoInvalido, format: 'DD/MM/YYYY', min: '01/01/1920' } } }

            , mensagem: { validators: { notEmpty: { message: mensagemObrigatoria } } }
            , UploadFile: {
                validators: {
                    file: {
                        extension: 'pdf,doc,docx',
                        type: 'application/pdf,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document',
                        maxSize: 2097152,   // 2048 * 1024
                        message: arquivoFormatoInvalido
                    }
                }
            }
        }
    }).on('success.form.bv', function (e)
    {
        e.preventDefault();
        var idConcurso = $('#hdnIdConcurso').val();
        var idInscrito = $('.aba-recurso').find('#hdnIdInscrito').val();
        enviarRecurso(idConcurso, idInscrito);
    });
}

function iniciarTelaCadastroRecurso(idConcurso, idRecurso, pathRequerente, pathAtendente, novo) {
    listarAnexosRequerente(pathRequerente, pathAtendente, novo);
    $('#txtUploadFile').on('change', function (e) { uploadFileAnexos(e, idRecurso, pathAtendente, novo); });

    $('.recursoForm')
        .on('init.field.bv', function (e, data) { removerIconeValidator(e, data); })
        .bootstrapValidator({
            container: 'tooltip',
            feedbackIcons: {
                valid: 'glyphicon glyphicon-ok',
                invalid: 'glyphicon glyphicon-remove',
                validating: 'glyphicon glyphicon-refresh'
            },
            fields: {
                resposta: {
                    validators: {
                        notEmpty: { message: respostaObrigatorio },
                    }
                },
                txtComentario: {
                    validators: {
                        notEmpty: { message: comentarioObrigatorio }
                    }
                }
            }
        }).on('success.form.bv', function (e) {
            e.preventDefault(); // Prevent form submission
            salvar(idConcurso, idRecurso);
        });

    if (!novo)
        $('#btnSalvar').hide();
}

function iniciarTelaListaRecursos() {
    listarRecursos();
}

function listarAnexosAtendente(block, pathAtendente, novo) {
    if (block)
        $.blockUI({ message: mensagemCarregandoAnexos, css: cssCarregando });

    var url = homePage + 'Base/ListarAnexos';

    $.ajax({
        type: "POST",
        url: url,
        data: { pathAnexos: pathAtendente },
        success: function (retorno) {
            $('.anexo-atendente').html('');
            $('.anexo-atendente').append(retorno.View);

            if (novo) {
                $('.anexo-atendente').find('.remover-anexo').show();
                $('.anexo-atendente').find('.remover-anexo').on('click', function () { removerAnexo($(this).attr("path"), function () { listarAnexosAtendente(block, pathAtendente, novo); }); });
            }

            if ($('.anexo-atendente').find('.col-md-4').length >= 3)
                $('.upload-file').hide();
            else
                $('.upload-file').show();

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'listarAnexosAtendente()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function listarAnexosRequerente(pathRequerente, pathAtendente, novo) {
    $.blockUI({ message: mensagemCarregandoAnexos, css: cssCarregando });
    var url = homePage + 'Base/ListarAnexos';

    $.ajax({
        type: "POST",
        url: url,
        data: { pathAnexos: pathRequerente },
        success: function (retorno) {
            $('.anexo-requerente').html('');
            $('.anexo-requerente').append(retorno.View);

            listarAnexosAtendente(false, pathAtendente, novo);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'listarAnexosRequerente()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function listarRecursos()
{
    $.blockUI({ message: mensagemCarregando, css: cssCarregando });
    var url = homePage + 'Admin/Recurso/ListarRecursos';

    $.ajax({
        type: "POST",
        url: url,
        success: function (retorno) {
            $('.lista').html('');
            $('.lista').append(retorno);
            montarTabela();

            if ($('.lista').find('table').attr('summary') == naoEncontrado)
                $('.lista').css('marginTop', 0);

            $('.redirecionar').on('click', function () { editar($(this).closest('tr').find('td:first span').html()); });

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'listarRecursos()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function montarTabela()
{
    $.fn.dataTable.moment('DD/MM/YYYY');

    oTable = $('#tblRecursos').dataTable(
    {
        "bLengthChange": false,
        "order": [[1, "desc"]],
        "aoColumnDefs": [{ "bSortable": false, "aTargets": [7] }],
        "language": {
            "url": urlDataTable
        },
        "fnDrawCallback": function (oSettings) {
            $('#tblRecursos_filter input').focus();
        },
        //"fnInitComplete": function (oSettings, json) {
        //    $('#tblRecursos_filter input').val("Novo");
        //    oTable.fnFilter('Novo');
        //}
    });
}

function salvar(idConcurso, idRecurso) {
    var comentario = $('#txtComentario').val();
    var deferido = $('#rDeferido').is(':checked');

    $.blockUI({ message: mensagemEnviando, css: cssCarregando });
    var url = homePage + 'Admin/Recurso/Enviar';

    $.ajax({
        type: "POST",
        url: url,
        data: { idRecurso: idRecurso, comentario: comentario, deferido: deferido },
        success: function (retorno) {
            alert(retorno.Mensagem);

            if (retorno.Sucesso)
                window.location.href = homePage + 'Admin/Recurso/Index/' + idConcurso;

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'salvar()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function uploadFileAnexos(e, idRecurso, pathAtendente, novo)
{
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
                url: homePage + '/Base/UploadFiles?idRecurso=' + idRecurso + '&atendente=true',
                contentType: false,
                processData: false,
                data: data,
                success: function (retorno) {
                    $('#txtUploadFile').val('');

                    if (retorno.Sucesso) {
                        listarAnexosAtendente(false, pathAtendente, novo);
                        $('.sucesso').fadeIn(1000).fadeOut(3000);
                    }
                    else
                    {
                        $('.erro').fadeIn(1000).fadeOut(3000);
                        $.unblockUI();
                    }
                },
                error: function (xhr, status, p3, p4) {
                    alertaErroJS({ NomeFuncao: 'uploadFileAnexos()', ResponseText: xhr.responseText });
                    $.unblockUI();
                }
            });
        }
    }
}