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

function iniciarTelaCadastroColaborador()
{

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

(function ()
{
    'use strict';

    angular.module('ipefae').controller('colaboradorController', colaboradorController);
    colaboradorController.$inject = ['$scope', '$rootScope', '$q', '$timeout'];

    function colaboradorController($scope, $rootScope, $q, $timeout)
    {
        var vm = this;
        vm.activate = _activate;

        function inicializar()
        {
            $scope.errorList = {};
            $scope.errorList.nome = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.email = { enable: false, ind: -1, validacoes: ['email'] };
            $scope.errorList.cpf = { enable: false, ind: -1, validacoes: ['cpf', 'cpfJaExiste'] };
            $scope.errorList.rg = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.dataNasc = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.sexo = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.grau_instrucao = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.raca = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.telefone = { enable: false, ind: -1, validacoes: ['email'] };
        }

        function _activate()
        {
            inicializar();

            $scope.checkIfIsTooltipEnable = function(fieldName, outrasValidacoes)
            {
                var isDirty = $scope.checkIsDirty(fieldName);
                var result = !$scope[fieldName] && isDirty;
                var ind = 0;

                if (result || !outrasValidacoes || !$.isArray(outrasValidacoes))
                {
                    var r = { enable: result, ind: result ? 0 : -1, validacoes: outrasValidacoes  };
                    $scope.errorList[fieldName] = r;
                    return r;
                }

                $.each(outrasValidacoes, function (i, v)
                {
                    if (eval('$scope.colCadastroForm.$error.' + v))
                    {
                        result = true;
                        ind = i + 1;
                        return false;
                    }
                });

                var r = { enable: result, ind: result ? ind : -1, validacoes: outrasValidacoes };
                $scope.errorList[fieldName] = r;
                return r;
            }

            $scope.checkIsDirty = function (fieldName)
            {
                var field = $scope.getField(fieldName);

                if (field === undefined)
                    return false;

                return field.$dirty;
            }

            $scope.getErrorMessage = function(fieldName, lista)
            {
                if ($scope.errorList[fieldName] === undefined)
                    return '';

                $scope.checkIfIsTooltipEnable(fieldName, $scope.errorList[fieldName].validacoes);

                var ind = $scope.errorList[fieldName].ind;

                if (!$.isArray(lista) || ind > lista.length - 1)
                    return '';

                return lista[ind];
            }

            $scope.getField = function (fieldName)
            {
                var result = undefined;

                angular.forEach($scope.colCadastroForm.$error.required, function (field)
                {
                    if (field.$name === fieldName)
                        result = field;
                });

                return result;
            }

            $scope.salvar = function()
            {
                var firstErrorField = undefined;

                angular.forEach($scope.colCadastroForm.$$controls, function (field)
                {
                    field.$dirty = true;
                    var item = $scope.errorList[field.$name];

                    if (item !== undefined)
                    {
                        if (firstErrorField === undefined && field.$invalid)
                            firstErrorField = $('[name="' + field.$name + '"]');

                        $scope.checkIfIsTooltipEnable(field.$name, item.validacoes);
                    }
                });

                if (firstErrorField !== undefined)
                    firstErrorField.focus();
            }

            $scope.verificarCPFJaExiste = function(cpf)
            {
                var url = homePage + 'Colaborador/VerificarCPFJaExiste';
                var id = $scope.id || 0;
                cpf = removerTodosCaracteresMenosNumeros(cpf);

                $.ajax({
                    type: "POST",
                    url: url,
                    data: { id: id, cpf: cpf },
                    success: function (retorno)
                    {
                        console.log(retorno);
                        return retorno;
                    },
                    error: function (xhr, ajaxOptions, thrownError)
                    {
                        alertaErroJS({ NomeFuncao: 'removerAnexo()', ResponseText: xhr.responseText });
                    }
                });
            }
        }

        vm.activate();
    }
})();