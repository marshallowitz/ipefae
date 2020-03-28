﻿function editar(a) { window.location.href = homePage + "Admin/Usuario/Cadastro/" + a } function editarAtivacao(a, e) { $.blockUI({ message: mensagemDesativando, css: cssCarregando }); var o = homePage + "Admin/Usuario/EditarAtivacao"; $.ajax({ type: "POST", url: o, data: { id: e }, success: function (e) { e.Sucesso ? (a.removeClass("checked"), a.removeClass("unchecked"), a.addClass(e.Ativo ? "checked" : "unchecked"), alert(operacaoMensagemSucesso)) : alert(operacaoMensagemErro), $.unblockUI() }, error: function (a, e, o) { alertaErroJS({ NomeFuncao: "editarAtivacao()", ResponseText: a.responseText }), $.unblockUI() } }) } function iniciarTelaListaUsuarios() { listarUsuarios() } function iniciarTelaCadastroUsuario() { $(".userForm").on("init.field.bv", function (a, e) { removerIconeValidator(a, e) }).bootstrapValidator({ container: "tooltip", feedbackIcons: { valid: "glyphicon glyphicon-ok", invalid: "glyphicon glyphicon-remove", validating: "glyphicon glyphicon-refresh" }, fields: { Nome: { validators: { notEmpty: { message: nomeObrigatorio } } }, email: { validators: { notEmpty: { message: emailObrigatorio }, emailAddress: { message: formatoInvalidoEmail } } }, Telefone: { validators: { notEmpty: { message: telefoneObrigatorio } } }, Senha: { validators: { notEmpty: { message: senhaObrigatorio }, identical: { field: "ConfirmacaoSenha", message: senhaConfirmacaoIguais } } }, ConfirmacaoSenha: { validators: { notEmpty: { message: confirmacaoSenhaObrigatorio }, identical: { field: "Senha", message: senhaConfirmacaoIguais } } } } }).on("success.form.bv", function (a) { return a.preventDefault(), salvar(), !1 }); var a = {}; a.ui = { container: "#pwd-container", showVerdictsInsideProgressBar: !0 }, $("#Senha").pwstrength(a), "0" == $(".cadastro-usuario").find("#Telefone").val() && $(".cadastro-usuario").find("#Telefone").val(""), $(".cadastro-usuario").find("#Telefone").mask("(00) 00009-0000").focusin(function () { focusIn($(this)) }).focusout(function () { ajustarCelularSP($(this)) }), $("#Id").val() <= 0 ? ($("#chkAtivo").prop("checked", !0), $("#chkAtivo").attr("disabled", !0)) : ($("#chkAdmin_Geral").prop("checked", isUAdmin), $("#chkAdmin_Concurso").prop("checked", isUConcurso), $("#chkAdmin_Estagio").prop("checked", isUEstagio), $("#chkAdmin_Vestibular").prop("checked", isUVestibular)), isAdmin || ($("#chkAdmin_Geral").attr("disabled", !0), isConcurso || $("#chkAdmin_Concurso").attr("disabled", !0), isEstagio || $("#chkAdmin_Estagio").attr("disabled", !0), isVestibular || $("#chkAdmin_Vestibular").attr("disabled", !0)), mudarCheckAdmin(), $("#chkAdmin_Geral").on("change", function () { mudarCheckAdmin() }), $("#aTrocarSenha").click(function () { $(this).hide(), $("#liSenha").show() }) } function listarUsuarios() { $.blockUI({ message: mensagemCarregando, css: cssCarregando }); var a = homePage + "Admin/Usuario/ListarUsuarios"; $.ajax({ type: "POST", url: a, success: function (a) { $(".lista").html(""), $(".lista").append(a), montarTabela(), $(".lista").find("table").attr("summary") == naoEncontrado && $(".lista").css("marginTop", 0), $(".telefone").each(function () { $(this).mask("(00) 00009-0000").focusin(function () { focusIn($(this)) }).focusout(function () { ajustarCelularSP($(this)) }) }), $(".editarAtivacao").on("click", function () { editarAtivacao($(this), $(this).closest("tr").find("td:first span").html()) }), $(".editar").on("click", function () { editar($(this).closest("tr").find("td:first span").html()) }), $.unblockUI() }, error: function (a, e, o) { alertaErroJS({ NomeFuncao: "listarUsuarios()", ResponseText: a.responseText }), $.unblockUI() } }) } function montarTabela() { oTable = $("#tblUsuarios").dataTable({ bLengthChange: !1, order: [[1, "asc"]], aoColumnDefs: [{ bSortable: !1, aTargets: [0, 3, 4, 5, 6] }], language: { url: urlDataTable } }), $("#tblUsuarios_filter").parent().removeClass("col-sm-6").addClass("col-sm-10"), $("#tblUsuarios_filter input").focus() } function mudarCheckAdmin() { var a = $("#chkAdmin_Geral"); a.is(":checked") ? ($('[id^="chkAdmin"]').prop("checked", !1), $('[id^="chkAdmin"]').attr("disabled", !0), a.prop("checked", !0), a.removeAttr("disabled")) : $('[id^="chkAdmin"]').removeAttr("disabled") } function salvar() { var a = $("#Id").val() <= 0 ? 0 : $("#Id").val(), e = $("#Nome").val(), o = $("#Email").val(), i = removerTodosCaracteresMenosNumeros($("#Telefone").val()), s = $("#liSenha").is(":visible") ? $("#Senha").val() : null, n = parseInt($("#chkAdmin_Geral").is(":checked") ? $("#chkAdmin_Geral").attr("valor") : 0) + parseInt($("#chkAdmin_Concurso").is(":checked") ? $("#chkAdmin_Concurso").attr("valor") : 0) + parseInt($("#chkAdmin_Estagio").is(":checked") ? $("#chkAdmin_Estagio").attr("valor") : 0) + parseInt($("#chkAdmin_Vestibular").is(":checked") ? $("#chkAdmin_Vestibular").attr("valor") : 0), r = $("#chkAtivo").is(":checked"); $.blockUI({ message: mensagemSalvando, css: cssCarregando }); var t = homePage + "Admin/Usuario/Salvar"; $.ajax({ type: "POST", url: t, data: { id: a, nome: e, email: o, telefone: i, senha: s, permissao: n, ativo: r }, success: function (a) { alert(a.Mensagem), a.Sucesso && (window.location.href = homePage + "Admin/Usuario"), $.unblockUI() }, error: function (a, e, o) { alertaErroJS({ NomeFuncao: "salvar()", ResponseText: a.responseText }), $.unblockUI() } }) } var oTable;