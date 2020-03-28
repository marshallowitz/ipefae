﻿function abrirTelaPreviewCurriculo(a) { abrirJanelaPopUpWait("Carregando Preview do Currículo ..."), gerarPreview(a) } function addValidatorIcons(a) { a.parent().find("i.form-control-feedback").hasClass("glyphicon") && ("" == a.val() ? (a.parent().find("i.form-control-feedback").removeClass("glyphicon-ok").addClass("glyphicon-remove"), a.parent().removeClass("has-success").addClass("has-error"), a.parent().find("small.help-block").attr("data-bv-result", "INVALID")) : (a.parent().find("i.form-control-feedback").addClass("glyphicon-ok").removeClass("glyphicon-remove"), a.parent().addClass("has-success").removeClass("has-error"), a.parent().find("small.help-block").attr("data-bv-result", "VALID"))), a.parent().find("i.form-control-feedback").show() } function alterarAdicionarRemover(a, e, o, i, t) { var r = a.closest(".form-group"); r.find(i).hide(); var n = e ? a.closest(o).next() : a.closest(o).prev(); n.find(i).show(); e ? a.closest(o).next() : a.closest(o); e ? (a.closest(o).next().show(), a.closest(o).next().find('input[name="' + t + '"]:first').focus()) : (a.closest(o).hide(), a.closest(o).find('input:not([name="hdnId"])').val(""), a.closest(o).prev().find('input[name="' + t + '"]:first').focus()), $("#btnSalvar").removeAttr("disabled") } function buildComboNumeros(a, e, o) { a.html(""); for (var i = 0; e >= i; i++) 0 == i ? a.append($("<option>", { value: "", text: "" })) : (void 0 == o || i >= o) && a.append($("<option>", { value: i, text: i + "º" })) } function buildComboPeriodo(a) { $("#ddlPeriodo").html(""), $("#ddlPeriodo").append($("<option>", { value: "", text: "" })), $("#ddlPeriodo").append($("<option>", { value: 1, text: optionManha })), $("#ddlPeriodo").append($("<option>", { value: 2, text: optionTarde })), $("#ddlPeriodo").append($("<option>", { value: 3, text: optionNoite })), a && $("#ddlPeriodo").append($("<option>", { value: 4, text: optionIntegral })) } function buscarCidadesComEstagiario() { var a = homePage + "Admin/Estagio/BuscarCidadesCadastradas"; $.ajax({ type: "POST", url: a, success: function (a) { var e = function (a) { for (var e = "", o = 0; o < a.length; o++) e += accentMap[a.charAt(o)] || a.charAt(o); return e }; $("#txtCidade").autocomplete({ source: function (o, i) { var t = new RegExp($.ui.autocomplete.escapeRegex(o.term), "i"); i($.grep(a, function (a) { return a = a.label || a.value || a, t.test(a) || t.test(e(a)) })) }, select: function (a, e) { a.preventDefault(), $("#txtCidade").val(e.item.label) }, focus: function (a, e) { a.preventDefault(), $("#txtCidade").val(e.item.label) } }) }, error: function (a) { alertaErroJS({ NomeFuncao: "buscarCidadesComEstagiario()", ResponseText: a.responseText }) } }) } function carregarCursosCapacitacoes(a) { $("div.cursos-capacitacoes").block({ message: "Carregando cursos e capacitações ...", css: cssCarregando }); var e = homePage + "Estagio/BuscarCursosCapacitacoes"; $.ajax({ type: "POST", url: e, data: { idUsuarioEstagio: a }, success: function (a) { $(".cursos-capacitacoes").html(""), $(".cursos-capacitacoes").html(a.View), $("div.cursos-capacitacoes").unblock() }, error: function (a) { alertaErroJS({ NomeFuncao: "carregarCursosCapacitacoes()", ResponseText: a.responseText }), $("div.cursos-capacitacoes").unblock() } }) } function carregarOutrosConhecimentos(a) { $("div.outros-conhecimentos").block({ message: "Carregando outros conhecimentos ...", css: cssCarregando }); var e = homePage + "Estagio/BuscarOutrosConhecimentos"; $.ajax({ type: "POST", url: e, data: { idUsuarioEstagio: a }, success: function (a) { $(".outros-conhecimentos").html(""), $(".outros-conhecimentos").html(a.View), $("div.outros-conhecimentos").unblock() }, error: function (a) { alertaErroJS({ NomeFuncao: "carregarOutrosConhecimentos()", ResponseText: a.responseText }), $("div.outros-conhecimentos").unblock() } }) } function carregarEstados(a, e, o, i) { var t = homePage + "Inscrito/CarregarEstados"; $.ajax({ type: "POST", url: t, data: { exibirNome: o }, success: function (o) { a.empty(); var t = o.data ? o.data : o; t.length > 0 && (a.addItems({ data: t, valueName: "Value", textName: "Text" }), a.val(e), "ddlEstado" == a.attr("id") && e > 0 && i > 0 && ($("#ddlEstado").trigger("change"), $("#ddlCidade").val(i))) }, error: function (a) { alertaErroJS({ NomeFuncao: "carregarEstados()", ResponseText: a.responseText }), $.unblockUI() } }) } function changeDE01() { if ($("#rdTecnico").is(":checked")) $("#ddlDE01")[0].selectedIndex > 0 ? ($("#lblDE02").html($("#ddlDE01 option:selected").text()), $("#ddlDE02").removeAttr("disabled")) : ($("#lblDE02").html(labelSelecioneET01), $("#ddlDE02")[0].selectedIndex = 0, $("#ddlDE02").attr("disabled", "disabled")); else if ($("#rdEJA").is(":checked")) switch ($("#ddlDE01")[0].selectedIndex) { case 0: $("#ddlDE02").html(""), $("#ddlDE02").attr("disabled", "disabled"); break; case 1: buildComboNumeros($("#ddlDE02"), 3), $("#ddlDE02").removeAttr("disabled"); break; case 2: buildComboNumeros($("#ddlDE02"), 9, 7), $("#ddlDE02").removeAttr("disabled") } addValidatorIcons($("#ddlDE01")) } function definirOrdenacao(a) { a.hasClass("selecionado") || (a.hasClass("nome") ? $(".atualizacao").removeClass("selecionado").addClass("order") : $(".nome").removeClass("selecionado").addClass("order"), a.addClass("selecionado").removeClass("order"), mais(!0)) } function gerarCSVPesquisa() { $.blockUI({ message: mensagemGerarCSV, css: cssCarregando }); var a = homePage + "Admin/Estagio/GerarCSVPesquisa", e = obterJsonFiltros(); $.ajax({ type: "POST", url: a, data: e, success: function (a) { a ? ($("#iframeCSV").attr("src", homePage + "Handlers/DownloadCSVHandler.ashx?tipo=est"), $("#iframeCSV").load(), setTimeout(function () { terminouDownload(homePage + "Admin/Estagio/GerarCSVPesquisaConfirmacao") }, 2e3)) : (alert(mensagemNenhumEncontrado), $.unblockUI()) }, error: function (a) { alertaErroJS({ NomeFuncao: "gerarCSVPesquisa()", ResponseText: a.responseText }), $.unblockUI() } }) } function gerarPDFPesquisa() { $.blockUI({ message: mensagemGerarPDF, css: cssCarregando }); var a = homePage + "Admin/Estagio/GerarPDFPesquisa", e = obterJsonFiltros(); $.ajax({ type: "POST", url: a, data: e, success: function (a) { a ? ($("#iframePDF").attr("src", homePage + "Handlers/DownloadPDFHandler.ashx"), $("#iframePDF").load(), setTimeout(function () { terminouDownload(homePage + "Admin/Estagio/GerarPDFPesquisaConfirmacao") }, 2e3)) : (alert(mensagemNenhumEncontrado), $.unblockUI()) }, error: function (a) { alertaErroJS({ NomeFuncao: "gerarPDFPesquisa()", ResponseText: a.responseText }), $.unblockUI() } }) } function gerarPreview(a) { var e = homePage + "Estagio/PreviewCurriculo"; return $.ajax({ type: "POST", url: e, data: { id: a }, success: function (a) { var e = janelaPopUp; gerarPreviewChamadaFuncao(e, a.View) }, error: function (a) { alertaErroJS({ NomeFuncao: "gerarPreview()", ResponseText: a.responseText }) } }), !1 } function gerarPreviewChamadaFuncao(a, e) { janelaPopUp.getTitulo && "" !== janelaPopUp.getTitulo() ? (a.document.title = ".:: IPEFAE ::.", $(a.document.body).html(e)) : setTimeout(function () { gerarPreviewChamadaFuncao(a, e) }, 1e3) } function incluirFoto(a) { $.blockUI({ message: mensagemRealizandoUpload, css: cssCarregando }); var e = a.target.files; if (e.length > 0 && void 0 !== window.FormData) { for (var o = new FormData, i = 0; i < e.length; i++) o.append("file" + i, e[i]); $.ajax({ type: "POST", url: homePage + "/Estagio/UploadFotoCurriculo", contentType: !1, processData: !1, data: o, success: function (a) { $("#txtUploadFoto").val(""), a.Sucesso && ($("#fotoEstagio").attr("src", a.PathFoto), $("#hdnTemFoto").val("true"), $(".remover-foto").show()), $.unblockUI() }, error: function (a) { alertaErroJS({ NomeFuncao: "incluirFoto()", ResponseText: a.responseText }), $.unblockUI() } }) } } function iniciarTelaAdminListagem() { buscarCidadesComEstagiario(), $("#txtCPF").mask("999.999.999-99"), $("#txtNome").focus(), $("#btnPesquisar").on("click", function () { pesquisar() }), $("#btnCSV").on("click", function () { gerarCSVPesquisa() }), $("#btnPDF").on("click", function () { gerarPDFPesquisa() }), $(".order").on("click", function () { definirOrdenacao($(this)) }) } function iniciarTelaCadastroEstagio(idEstado, idCidade, idEstadoCarteiraTrabalho, possuiDef, possuiExp, ehMasc, tipo_estudo, periodo, anosemestre, ehEad, flgTipo, motivoDesativacao) { inicializarValidationTelaCadastro(), $("#ddlDE01").on("change", function () { changeDE01() }); var id = $("#hdnId").val(); $(".remover-foto").on("click", function () { removerFoto() }), carregarEstados($("#ddlEstado"), idEstado, !0, idCidade), carregarEstados($("#ddlCarteiraTrabalhoUF"), idEstadoCarteiraTrabalho, !1), $("#txtCPF").mask("999.999.999-99"); var SPMaskBehavior = function (a) { return 11 === a.replace(/\D/g, "").length ? "(00) 00000-0000" : "(00) 0000-00009" }, spOptions = { onKeyPress: function (a, e, o, i) { o.mask(SPMaskBehavior.apply({}, arguments), i) } }; $("#txtTelefone").mask(SPMaskBehavior, spOptions), $("#txtCelular").mask(SPMaskBehavior, spOptions), $("#txtDataRGExpedicao").mask("00/00/0000"), $("#txtDataNascimento").mask("00/00/0000"), $("#txtInicioDE").mask("00/0000"), $("#txtTerminoDE").mask("00/0000"), $('input[name="dataInicio"]').mask("00/0000"), $('input[name="dataTermino"]').mask("00/0000"), $("#txtCEP").mask("00000-000"), $("#txtQtdadeFilhos").mask("00"), $("#txtNroEndereco").mask("00000"); var paramsCidade = {}; if (paramsCidade.est_idt_estado = idEstado, $("#ddlEstado").cascade({ url: homePage + "Inscrito/CarregarCidades", params: paramsCidade, paramName: "est_idt_estado", childSelect: $("#ddlCidade"), dadosMinimo: 1, emptyText: "Selecione uma cidade", carregando: "Carregando cidades...", async: !1 }), $("#txtEmail").on("change", function () { verificarEmailExistente() }), $('input[name="deficiencia"]').on("change", function () { $("#rdDefSim").is(":checked") ? $(".tem-deficiencia").slideDown(500) : ($(".tem-deficiencia").slideUp(500), $('input[name="deficiencia_qual"]').prop("checked", !1)) }), $('input[name="tipo_ensino"]').on("change", function () { mudarTipoEnsino(!0, null) }), $("#txtUploadFoto").on("change", function (a) { incluirFoto(a) }), $("#txtDataNascimento").on("change", function () { var a = calcularIdade($(this).val()); $("#idade").html("(" + a + labelIdadeAnos + ")") }), $('input[name="experiencia_profissional"]').on("change", function () { mudarExperienciaProfissional(id) }), 0 >= id) $("#txtDataRGExpedicao").val(""), $("#txtDataNascimento").val(""); else { switch (ehMasc ? $("#ddlSexo").val("M") : $("#ddlSexo").val("F"), $("#txtDataNascimento").trigger("change"), $('input[name="tipo_ensino"][value="' + tipo_estudo + '"]').attr("checked", "checked"), mudarTipoEnsino(!1, flgTipo), $("#ddlPeriodo").val(periodo), $("#ddlPeriodo").val(periodo), tipo_estudo) { case "M": $("#ddlDE01").val(anosemestre); break; case "T": $("#ddlDE02").val(anosemestre); case "E": $("#ddlDE02").val(anosemestre), $("#ddlDE02").removeAttr("disabled"); break; case "S": $("#ddlDE01").val(anosemestre); var idEAD = ehEad ? 2 : 1; $("#ddlDE02").val(idEAD) } if (eval($("#hdnTemFoto").val()) && ($("#fotoEstagio").attr("src", homePage + "Anexos/Estagio/" + id + ".jpg"), $(".remover-foto").show()), $(".area_adm").length > 0) { var ativo = eval($("#hdnAt").val()), consideraPesquisa = eval($("#hdnCP").val()), estaEstagiando = eval($("#hdnEE").val()); ativo ? $("#rdAtiSim").attr("checked", "checked") : $("#rdAtiNao").attr("checked", "checked"), consideraPesquisa ? $("#rdPesSim").attr("checked", "checked") : $("#rdPesNao").attr("checked", "checked"), estaEstagiando ? $("#rdEstSim").attr("checked", "checked") : $("#rdEstNao").attr("checked", "checked"), $("#ddlMotivoDesativacao").val(motivoDesativacao) } } possuiDef ? ($("#rdDefSim").attr("checked", "checked"), $('input[name="deficiencia_qual"][value=' + flgDef + "]").prop("checked", !0), $('input[name="deficiencia"]').trigger("change")) : $("#rdDefNao").attr("checked", "checked"), possuiExp ? ($("#rdExpSim").attr("checked", "checked"), mudarExperienciaProfissional(id)) : $("#rdExpNao").attr("checked", "checked"), carregarCursosCapacitacoes(id), carregarOutrosConhecimentos(id), $(".modal-senha").on("show.bs.modal", function () { $(".form-group").removeClass("has-error").removeClass("has-success"), $(".form-group").find("small.help-block").hide(), $(".form-group").find("i.form-control-feedback").hide(), $(".modal-senha").find("#txtSenha").val(""), $(".modal-senha").find("#txtConfirmacaoSenha").val("") }), $(".modal-senha").on("shown.bs.modal", function () { $(".modal-senha").find("#txtSenha").focus() }); var cursosArray = $.map(cursosScript, function (a, e) { return { value: a, data: e } }), normalize = function (a) { for (var e = "", o = 0; o < a.length; o++) e += accentMap[a.charAt(o)] || a.charAt(o); return e }; $("#txtNomeCurso").autocomplete({ source: function (a, e) { if ($("#rdEnsinoSuperior").is(":checked")) { var o = new RegExp($.ui.autocomplete.escapeRegex(a.term), "i"); e($.grep(cursosArray, function (a) { return a = a.label || a.value || a, o.test(a) || o.test(normalize(a)) })) } }, change: function (a, e) { !e.item && $("#rdEnsinoSuperior").is(":checked") && ($("#txtNomeCurso").val(""), $("#txtNomeCurso").parent("div").removeClass("has-success"), $("#txtNomeCurso").parent("div").addClass("has-error"), $("#txtNomeCurso").parent("div").find("i").removeClass("glyphicon-ok"), $("#txtNomeCurso").parent("div").find("i").addClass("glyphicon-remove"), $("#txtNomeCurso").parent("div").find("small").attr("data-bv-for", "INVALID")) } }), $("#txtNomeCompleto").focus() } function inicializarValidationTelaCadastro() { $(".estCadastroForm").on("init.field.bv", function (a, e) { removerIconeValidator(a, e) }).bootstrapValidator({ container: "tooltip", excluded: [":disabled", ":hidden", ":not(:visible)"], feedbackIcons: { valid: "glyphicon glyphicon-ok", invalid: "glyphicon glyphicon-remove", validating: "glyphicon glyphicon-refresh" }, live: "disabled", fields: { nome: { validators: { notEmpty: { message: nomeObrigatorio } } }, email: { validators: { notEmpty: { message: emailObrigatorio }, emailAddress: { message: formatoInvalidoEmail } } }, cpf: { validators: { notEmpty: { message: cpfObrigatorio } } }, rg: { validators: { notEmpty: { message: rgObrigatorio } } }, dataRGExpedicao: { validators: { notEmpty: { message: dataExpedicaoObrigatoria }, date: { message: dataExpedicaoFormatoInvalido, format: "DD/MM/YYYY" } } }, carteiraTrabalhoNumero: { validators: { notEmpty: { message: carteiraTrabalhoNumeroObrigatorio } } }, carteiraTrabalhoSerie: { validators: { notEmpty: { message: carteiraTrabalhoSerieObrigatorio } } }, carteiraTrabalhoUF: { validators: { notEmpty: { message: carteiraTrabalhoUFObrigatorio } } }, sexo: { validators: { notEmpty: { message: sexoObrigatorio } } }, dataNasc: { validators: { notEmpty: { message: dataNascObrigatorio }, date: { message: dataNascFormatoInvalido, format: "DD/MM/YYYY" } } }, telefone: { validators: { notEmpty: { message: telefoneObrigatorio } } }, endereco: { validators: { notEmpty: { message: enderecoObrigatorio } } }, nroEndereco: { validators: { notEmpty: { message: nroEnderecoObrigatorio } } }, bairro: { validators: { notEmpty: { message: bairroObrigatorio } } }, cep: { validators: { notEmpty: { message: cepObrigatorio } } }, estado: { validators: { notEmpty: { message: estadoObrigatorio } } }, cidade: { validators: { notEmpty: { message: cidadeObrigatorio } } }, qtdadeFilhos: { validators: { notEmpty: { message: qtdadeFilhosObrigatorio } } }, deficiencia_qual: { validators: { notEmpty: { message: deficiencia_qualObrigatorio } } }, tipo_ensino: { validators: { notEmpty: { message: tipo_ensinoObrigatorio } } }, nomeEscola: { validators: { notEmpty: { message: nomeEscolaObrigatorio } } }, de01: { validators: { notEmpty: { message: de01Obrigatorio } } }, de02: { validators: { notEmpty: { message: de02Obrigatorio } } }, periodo: { validators: { notEmpty: { message: periodoObrigatorio } } }, nomeCurso: { validators: { notEmpty: { message: nomeCursoObrigatorio } } }, dataInicioDE: { validators: { notEmpty: { message: dataInicioDEObrigatorio }, callback: { message: dataInicioDEFormatoInvalido, callback: function (a) { var e = a.split("/"); return 2 == e.length && e[0] <= 12 && e[1] > 1999 } } } }, dataTerminoDE: { validators: { notEmpty: { message: dataTerminoDEObrigatorio }, callback: { message: dataTerminoDEFormatoInvalido, callback: function (a) { var e = a.split("/"); if (!(2 == e.length && e[0] <= 12 && e[1] > 1999)) return { valid: !1, message: dataTerminoDEFormatoInvalido }; var o = $("#txtInicioDE").val().split("/"); return 2 == o.length && o[0] <= 12 && o[1] > 1999 && (e[1] < o[1] || e[1] == o[1] && e[0] <= o[0]) ? { valid: !1, message: dataTerminoMaiorDataInicio } : !0 } } } }, nomeEmpresa: { validators: { notEmpty: { message: nomeEmpresaObrigatorio } } }, cargo: { validators: { notEmpty: { message: cargoObrigatorio } } }, atividadesDesenvolvidas: { validators: { notEmpty: { message: atividadesDesenvolvidasObrigatorio } } }, dataInicio: { validators: { notEmpty: { message: dataInicioDEObrigatorio }, callback: { message: dataInicioDEFormatoInvalido, callback: function (a) { var e = a.split("/"); return 2 == e.length && e[0] <= 12 && e[1] > 1999 } } } }, dataTermino: { validators: { callback: { message: dataTerminoDEFormatoInvalido, callback: function (a, e, o) { if (1 == o.attr("tag") && "" == a) return !0; if ("" == a) return { valid: !1, message: dataTerminoDEObrigatorio }; var i = a.split("/"); return 2 == i.length && i[0] <= 12 && i[1] > 1999 } } } }, UploadFile: { validators: { callback: { message: arquivoObrigatorio, callback: function (a) { return "" == a && ("" == $("#fotoEstagio").attr("src") || $("#fotoEstagio").attr("src").indexOf("empty.png") >= 0) ? { valid: !1, message: arquivoObrigatorio } : !0 } }, file: { extension: "jpg,png,bmp", type: "image/*", maxSize: 2097152, message: arquivoFormatoInvalido } } } } }).on("success.form.bv", function (a) { return a.preventDefault(), focoValidacao = "", validateFoto() && validateDadosEscolares(!0) && validateExperienciasProfissionais() ? ($("#btnSalvar").removeAttr("disabled"), void salvar(!1)) : void focoValidacao.focus() }), $(".frmSenha").on("init.field.bv", function (a, e) { removerIconeValidator(a, e) }).bootstrapValidator({ container: "tooltip", feedbackIcons: { valid: "glyphicon glyphicon-ok", invalid: "glyphicon glyphicon-remove", validating: "glyphicon glyphicon-refresh" }, fields: { senha: { validators: { notEmpty: { message: senhaObrigatorio } } }, confirmacaoSenha: { validators: { notEmpty: { message: confirmacaoSenhaObrigatorio }, identical: { field: "senha", message: senhaConfirmacaoDiferente } } } } }).on("success.form.bv", function (a) { a.preventDefault(), salvar(!0) }) } function iniciarTelaInicialEstagio() { $(".modal-reenviar-senha").find("#txtCPF").mask("999.999.999-99"), $(".estLoginForm").on("init.field.bv", function (a, e) { removerIconeValidator(a, e) }).bootstrapValidator({ container: "tooltip", feedbackIcons: { valid: "glyphicon glyphicon-ok", invalid: "glyphicon glyphicon-remove", validating: "glyphicon glyphicon-refresh" }, fields: { email: { validators: { notEmpty: { message: emailObrigatorio }, emailAddress: { message: formatoInvalidoEmail } } }, senha: { validators: { notEmpty: { message: senhaObrigatoria } } } } }).on("success.form.bv", function (a) { a.preventDefault(), realizarLogin() }); $(".cpfForm").on("init.field.bv", function (a, e) { removerIconeValidator(a, e) }).bootstrapValidator({ container: "tooltip", feedbackIcons: { valid: "glyphicon glyphicon-ok", invalid: "glyphicon glyphicon-remove", validating: "glyphicon glyphicon-refresh" }, fields: { cpf: { validators: { notEmpty: { message: cpfObrigatorio } } } } }).on("success.form.bv", function (a) { a.preventDefault(), reenviarSenha() }); $(".modal-reenviar-senha").on("hide.bs.modal", function () { $(".form-group").removeClass("has-error").removeClass("has-success"), $(".form-group").find("small.help-block").hide(), $(".form-group").find("i.form-control-feedback").hide(), $(".modal-reenviar-senha").find("#txtCPF").val("") }), $(".modal-reenviar-senha").on("shown.bs.modal", function () { $(".modal-reenviar-senha").find("#txtCPF").focus() }) } function mais(a) { $(".listagem").block({ message: mensagemPesquisando, css: cssCarregando }), void 0 !== a && a && $("#hdnPagina").val("0"); var e = $("#hdnPagina").val(); e++, $("#hdnPagina").val(e); var o = obterJsonFiltros(), i = homePage + "Admin/Estagio/Pesquisar"; $.ajax({ type: "POST", url: i, data: o, success: function (e) { void 0 !== a && a && $(".scrollTableBody").html(""), e.TotalItens > 0 ? $(".scrollTableBody").append(e.View) : $(".mais").hide(), $(".listagem").unblock() }, error: function (a) { alertaErroJS({ NomeFuncao: "mais()", ResponseText: a.responseText }), $(".listagem").unblock() } }) } function montarListaCursos(a) { for (var e = a ? ".container_curso_capacitacao_" : ".container_outro_conhecimento_", o = "", i = 1; 5 >= i; i++) o += "||", o += "item:" + i + ",", o += "id:" + $(e + i).find('input[name="hdnId"]').val() + ",", o += "nomeCurso:" + $(e + i).find('input[name="nomeCurso"]').val(), a && (o += ",duracao:" + $(e + i).find('input[name="duracao"]').val()); return o } function montarListaExperiencia() { for (var a = "", e = 1; 3 >= e; e++) a += "||", a += "item:" + e + ",", a += "id:" + $(".container_experiencia_" + e).find('input[name="hdnId"]').val() + ",", a += "nomeEmpresa:" + $(".container_experiencia_" + e).find('input[name="nomeEmpresa"]').val() + ",", a += "cargo:" + $(".container_experiencia_" + e).find('input[name="cargo"]').val() + ",", a += "datainicio:" + $(".container_experiencia_" + e).find('input[name="dataInicio"]').val() + ",", a += "datatermino:" + $(".container_experiencia_" + e).find('input[name="dataTermino"]').val() + ",", a += "atividadesDesenvolvidas:" + encodeURIComponent($(".container_experiencia_" + e).find('textarea[name="atividadesDesenvolvidas"]').val()); return a } function mudarExperienciaProfissional() { return $("#rdExpNao").is(":checked") ? $(".tem-experiencia-profissional").fadeOut(500).promise().done(function () { validateExperienciaProfissional(1, !1), validateExperienciaProfissional(2, !1), validateExperienciaProfissional(3, !1), $(".container_experiencia_2").hide(), $(".container_experiencia_3").hide() }) : (validateExperienciaProfissional(1, !0), $(".tem-experiencia-profissional").fadeIn(500), $(".container_experiencia_1").find('input[name="nomeEmpresa"]').focus()), $("#btnSalvar").removeAttr("disabled"), !1 } function mudarTipoEnsino(a, e) { a && ($(".tipo-ensino input").val(""), $("#ddlDE02").html(""), $("#txtNomeCurso").val("")), $(".tipo-ensino").fadeOut(500).promise().done(function () { $(".tipo-ensino").slideDown(1e3) }), $(".terceira-coluna").show(), $("#ddlDE01").off("blur"), $("#ddlDE02").removeAttr("disabled"), $("#txtNomeCurso").removeAttr("readonly"), $("#txtNomeEscola").focus(), $("#rdEnsinoMedio").is(":checked") ? ($("#txtNomeCurso").val(optionEnsinoMedio), $("#txtNomeCurso").attr("readonly", "readonly"), $("#lblDE01").html(labelEM01), buildComboNumeros($("#ddlDE01"), 3), $(".terceira-coluna").hide(), $("#lblPeriodo").html(labelPeriodo), buildComboPeriodo(!1)) : $("#rdTecnico").is(":checked") ? ($("#lblDE02").html(labelSelecioneET01), $("#lblDE01").html(labelET01), $("#ddlDE01").html(""), $("#ddlDE01").append($("<option>", { value: "", text: "" })), $("#ddlDE01").append($("<option>", { value: 1, text: optionTipoModulo })), $("#ddlDE01").append($("<option>", { value: 2, text: optionTipoTermo })), $("#ddlDE01").append($("<option>", { value: 3, text: optionTipoSemestre })), buildComboNumeros($("#ddlDE02"), 10), $("#ddlDE02").attr("disabled", "disabled"), null != e && ($("#ddlDE01").val(e), $("#ddlDE01").trigger("change")), buildComboPeriodo(!1)) : $("#rdEJA").is(":checked") ? ($("#txtNomeCurso").val(optionEJA), $("#txtNomeCurso").attr("readonly", "readonly"), $("#lblDE01").html(labelEJA01), $("#ddlDE01").html(""), $("#ddlDE01").append($("<option>", { value: "", text: "" })), $("#ddlDE01").append($("<option>", { value: 1, text: optionEJAEM })), $("#ddlDE01").append($("<option>", { value: 2, text: optionEJAEF })), $("#lblDE02").html(labelEJA02), $("#ddlDE02").attr("disabled", "disabled"), null != e && ($("#ddlDE01").val(e), $("#ddlDE01").trigger("change")), buildComboPeriodo(!1)) : ($("#lblDE01").html(labelES01), buildComboNumeros($("#ddlDE01"), 10), $("#lblDE02").html(labelES02), $("#ddlDE02").html(""), $("#ddlDE02").append($("<option>", { value: "", text: "" })), $("#ddlDE02").append($("<option>", { value: 1, text: optionModalidadePresencial })), $("#ddlDE02").append($("<option>", { value: 2, text: optionModalidadeEAD })), buildComboPeriodo(!0)), validateDadosEscolares(!0), $("#btnSalvar").removeAttr("disabled") } function obterJsonFiltros() { var nome = $("#txtNome").val(), curso = $("#txtCurso").val(), semAno = "" == $("#ddlAno").val() ? null : $("#ddlAno").val(), estagiando = "" == $("#ddlSituacao").val() ? null : eval($("#ddlSituacao").val()), visualizacao = eval($("#ddlVisualizacao").val()), cpf = removerTodosCaracteresMenosNumeros($("#txtCPF").val()), cidade = $("#txtCidade").val(), ordem = $(".selecionado").hasClass("nome") ? "N" : "A", pagina = $("#hdnPagina").val(); return { nome: nome, curso: curso, semAno: semAno, estagiando: estagiando, cpf: cpf, visualizacao: visualizacao, cidade: cidade, ordem: ordem, pagina: pagina } } function pesquisar() { $.blockUI({ message: mensagemPesquisando, css: cssCarregando }), $(".atualizacao").removeClass("selecionado").addClass("order"), $(".nome").addClass("selecionado").removeClass("order"), $("#hdnPagina").val("1"); var a = obterJsonFiltros(), e = homePage + "Admin/Estagio/Pesquisar"; $.ajax({ type: "POST", url: e, data: a, success: function (a) { $(".scrollTable").fadeOut(500), $(".emptyTable").fadeOut(500), $(".scrollTableBody").html(""), a.TotalItens > 0 ? ($(".scrollTableBody").html(a.View), $(".scrollTable").fadeIn(1e3), $("#btnPDF").show(), $(".mais").show()) : ($(".emptyTable").fadeIn(1e3), $("#btnPDF").hide()), $.unblockUI() }, error: function (a) { alertaErroJS({ NomeFuncao: "pesquisar()", ResponseText: a.responseText }), $.unblockUI() } }) } function realizarLogin() { $.blockUI({ message: mensagemBlockUILogin, css: cssCarregando }); var a = homePage + "Estagio/RealizarLogin", e = $(".estLoginForm").find("#txtEmail").val(), o = $(".estLoginForm").find("#txtSenha").val(); return $.ajax({ type: "POST", url: a, data: { email: e, senha: o }, success: function (a) { a.Sucesso ? window.location.href = homePage + "Estagio/Cadastro/" + a.IdEstagiario : (alert(erroLogin), $("#txtEmail").focus()), $.unblockUI() }, error: function (a) { $.unblockUI(), alertaErroJS({ NomeFuncao: "realizarLogin()", ResponseText: a.responseText }) } }), !1 } function reenviarSenha() { $(".modal-reenviar-senha").find(".modal-content").block({ message: mensagemReenviandoSenha, css: cssCarregando }); var a = homePage + "Estagio/EnviarSenhaPorEmail", e = removerTodosCaracteresMenosNumeros($(".modal-reenviar-senha").find("#txtCPF").val()); return $.ajax({ type: "POST", url: a, data: { cpf: e }, success: function (a) { a ? alert(mensagemEmailEnviadoSucesso) : ($(".modal-reenviar-senha").find(".cpf-nao-encontrado").fadeIn(1e3).fadeOut(3e3), $(".modal-reenviar-senha").find("#txtCPF").focus()), $(".modal-reenviar-senha").find(".modal-content").unblock() }, error: function (a) { $(".modal-reenviar-senha").find(".modal-content").unblock(), alertaErroJS({ NomeFuncao: "reenviarSenha()", ResponseText: a.responseText }) } }), !1 } function removerFoto() { $("#txtUploadFoto").val(""), $("#fotoEstagio").attr("src", homePage + "Content/imagens/empty.png"), $("#hdnTemFoto").val("false"), $(".remover-foto").hide() } function salvar(a) { var e = $("#hdnId").val(); 0 >= e && !a ? $(".modal-senha").modal("show") : salvarUsuarioExistente(e) } function salvarUsuarioExistente(a) { $(".modal-senha").modal("hide"); var e = $("#txtNomeCompleto").val(), o = $("#txtEmail").val(), i = removerTodosCaracteresMenosNumeros($("#txtCPF").val()), t = $("#txtRG").val(), r = $("#txtDataRGExpedicao").val(), n = $("#txtCarteiraTrabalhoNumero").val(), s = $("#txtCarteiraTrabalhoSerie").val(), d = $("#ddlCarteiraTrabalhoUF").val(), l = $("#ddlEstadoCivil").val(), c = $("#txtDataNascimento").val(), m = removerTodosCaracteresMenosNumeros($("#txtTelefone").val()), u = removerTodosCaracteresMenosNumeros($("#txtCelular").val()), p = $("#txtEndereco").val(), v = $("#txtNroEndereco").val(), f = $("#txtComplemento").val(), g = $("#txtBairro").val(), h = removerTodosCaracteresMenosNumeros($("#txtCEP").val()), E = $("#ddlCidade").val(), b = $("#txtQtdadeFilhos").val(), x = $("#rdDefSim").is(":checked"), C = x ? $('input[name="deficiencia_qual"]:checked').val() : null, D = $("#txtObjetivos").val(), k = $("#hdnIdDadosEstagio").val(), P = $('input[name="tipo_ensino"]:checked').val(), T = $("#txtNomeEscola").val(), y = $("#ddlDE01").val(), I = $("#ddlDE02").val(), S = $("#ddlPeriodo").val(), F = $("#txtNomeCurso").val(), N = $("#txtInicioDE").val(), V = $("#txtTerminoDE").val(), O = $("#rdExpSim").is(":checked"), A = montarListaExperiencia(), _ = montarListaCursos(!0), w = montarListaCursos(!1), U = "M" == $("#ddlSexo").val(), M = $("#hdnTemFoto").val(), q = $("#txtSenha").val(), R = $(".area_adm").length > 0, J = !0, L = !1, j = !0, z = null, B = null; R && (J = $("#rdPesSim").is(":checked"), L = $("#rdEstSim").is(":checked"), j = $("#rdAtiSim").is(":checked"), z = $("#ddlMotivoDesativacao").val(), B = $("#txtObsAdmin").val()), $.blockUI({ message: mensagemBlockUISalvar, css: cssCarregando, timeout: 2000 }); var G = homePage + "Estagio/Salvar"; return $.ajax({ type: "POST", url: G, data: { id: a, nome: e, email: o, senha: q, cpf: i, rg: t, dataExpedicao: r, nroCarteira: n, serieCarteira: s, ufCarteira: d, estadoCivil: l, dataNasc: c, telefone01: m, telefone02: u, endereco: p, nroEndereco: v, complemento: f, bairro: g, cep: h, idCidade: E, qtdadeFilhos: b, possuiDef: x, defQual: C, objetivos: D, idDadosEscolares: k, tipo_ensino: P, nomeEscola: T, de01: y, de02: I, periodo: S, nomeCursoEscola: F, dataInicioDE: N, dataTerminoDE: V, possuiExp: O, listaExp: A, listaCursos: _, listaOutros: w, ehMasc: U, temFoto: M, ehAdmin: R, considerar_busca: J, estagiando: L, ativo: j, motivoDesativacao: z, observacoesAdmin: B }, success: function (a) { alert(curriculoSalvoSucesso), $("#hdnId").val(a.Id), $("#hdnIdDadosEstagio").val(a.IdDadosEscolares); var e = $("#btnPreview").attr("href"); $("#btnPreview").attr("href", e.replace("/0", "/" + a.Id)), e = $("#btnPDF").attr("href"), $("#btnPDF").attr("href", e.replace("/0", "/" + a.Id)), $("#btnPreview").show(), $("#btnPDF").show(), $.unblockUI() }, error: function (a) { $.unblockUI(), alertaErroJS({ NomeFuncao: "salvarUsuarioExistente()", ResponseText: a.responseText }) } }), !1 } function validateDadosEscolares(a) { var e = !0, o = $(".tipo-ensino"); if (a) { var i = o.find('input[name="nomeEscola"]'); "" == i.val() && (e = !1, addValidatorIcons(i), "" == focoValidacao && (focoValidacao = i)), i = o.find('select[name="de01"]'), "" == i.val() && (e = !1, addValidatorIcons(i), "" == focoValidacao && (focoValidacao = i)), i = o.find('select[name="de02"]'), i.is(":visible") && "" == i.val() && (e = !1, addValidatorIcons(i), "" == focoValidacao && (focoValidacao = i)), i = o.find('select[name="periodo"]'), "" == i.val() && (e = !1, addValidatorIcons(i), "" == focoValidacao && (focoValidacao = i)), i = o.find('input[name="nomeCurso"]'), addValidatorIcons(i), "" == i.val() && (e = !1, "" == focoValidacao && (focoValidacao = i)), i = o.find('input[name="dataInicioDE"]'), "" == i.val() && (e = !1, addValidatorIcons(i), "" == focoValidacao && (focoValidacao = i)), i = o.find('input[name="dataTerminoDE"]'), "" == i.val() && (e = !1, addValidatorIcons(i), "" == focoValidacao && (focoValidacao = i)) } else o.find(".has-feedback").removeClass("has-success").removeClass("has-error"), o.find("small.help-block").attr("data-bv-result", "NOT_VALIDATED"), o.find("i.form-control-feedback").hide(); return e } function validateExperienciaProfissional(a, e) { var o = !0, i = $(".container_experiencia_" + a); if (e) { var t = i.find('input[name="nomeEmpresa"]'); "" == t.val() && (o = !1, addValidatorIcons(t), "" == focoValidacao && (focoValidacao = t)), t = i.find('input[name="cargo"]'), "" == t.val() && (o = !1, addValidatorIcons(t), "" == focoValidacao && (focoValidacao = t)), t = i.find('input[name="dataInicio"]'), "" == t.val() && (o = !1, addValidatorIcons(t), "" == focoValidacao && (focoValidacao = t)), t = i.find('textarea[name="atividadesDesenvolvidas"]'), "" == t.val() && (o = !1, addValidatorIcons(t), "" == focoValidacao && (focoValidacao = t)) } else i.find('input[name="nomeEmpresa"]').val(""), i.find('input[name="cargo"]').val(""), i.find('input[name="dataInicio"]').val(""), i.find('input[name="dataTermino"]').val(""), i.find('textarea[name="atividadesDesenvolvidas"]').val(""), i.find(".has-feedback").removeClass("has-success").removeClass("has-error"), i.find("small.help-block").attr("data-bv-result", "NOT_VALIDATED"), i.find("i.form-control-feedback").hide(); return o } function validateExperienciasProfissionais() { var a = !0; if ($(".tem-experiencia-profissional").is(":visible")) { if (a = validateExperienciaProfissional(1, !0), !a) return focoValidacao.focus(), a; if ($(".container_experiencia_2").is(":visible")) { if (a = validateExperienciaProfissional(2, !0), !a) return focoValidacao.focus(), a; if ($(".container_experiencia_3").is(":visible") && (a = validateExperienciaProfissional(3, !0), !a)) return focoValidacao.focus(), a } } return a } function validateFoto() { var a = !0, e = $('input[name="UploadFile"]'), o = $("#fotoEstagio"); return "" == e.val() && ("" == o.attr("src") || o.attr("src").indexOf("empty.png") >= 0) && (a = !1, e.parent().parent().find("i.form-control-feedback").hasClass("glyphicon") && ("" == e.val() ? (e.parent().parent().find("i.form-control-feedback").removeClass("glyphicon-ok").addClass("glyphicon-remove"), e.parent().parent().removeClass("has-success").addClass("has-error"), e.parent().parent().find("small.help-block").attr("data-bv-result", "INVALID")) : (e.parent().parent().find("i.form-control-feedback").addClass("glyphicon-ok").removeClass("glyphicon-remove"), e.parent().parent().addClass("has-success").removeClass("has-error"), e.parent().parent().find("small.help-block").attr("data-bv-result", "VALID"))), e.parent().parent().find("i.form-control-feedback").show(), "" == focoValidacao && (focoValidacao = e)), a } function verificarEmailExistente() { $("#txtEmail").block({ message: mensagemValidandoEmail, css: cssCarregando }); var a = homePage + "BaseEstagio/ValidarEmailExistente", e = $("#hdnId").val(), o = $("#txtEmail").val(); return $.ajax({ type: "POST", url: a, data: { id: e, email: o }, success: function (a) { a && ($("#txtEmail").val(""), alert(mensagemEmailExistente), $("#txtEmail").focus()), $("#txtEmail").unblock() }, error: function (a) { $("#txtEmail").unblock(), alertaErroJS({ NomeFuncao: "verificarEmailExistente()", ResponseText: a.responseText }) } }), !1 } var focoValidacao = "";