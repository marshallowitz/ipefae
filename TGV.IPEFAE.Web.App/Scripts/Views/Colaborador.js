function editar(codigo)
{
    var id = parseInt(codigo);
    window.location.href = homePage + 'Admin/Colaborador/Cadastro/' + id;
}

function exibirSenha()
{
    if ($('#chkVer').is(':checked'))
        $('#txtSenhaAdmin').attr('type', 'text');
    else
        $('#txtSenhaAdmin').attr('type', 'password');
}

function iniciarTelaListaColaboradores()
{
    listarColaboradores();
}

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
    $("input").on('keypress', function (e)
    {
        if (String.fromCharCode(event.which) == 'ç')
        {
            event.preventDefault()
            $(this).val($(this).val() + 'c');
        }
        else if (String.fromCharCode(event.which) == 'Ç')
        {
            event.preventDefault()
            $(this).val($(this).val() + 'C');
        }
        else
        {
            var chr = String.fromCharCode(e.which);
            return (/^[a-zA-Z0-9 @.]+$/.test(chr));
        }
    });

    $("input").on('blur', function (e)
    {
        var valor = $.trim(firstLetterCapitalized($(this).val()));
        $(this).val(valor);
    });
}

function listarColaboradores()
{
    $.blockUI({ message: 'Carregando Colaboradores', css: cssCarregando });
    var url = homePage + 'Admin/Colaborador/ListarColaboradores';

    $.ajax({
        type: "POST",
        url: url,
        success: function (retorno)
        {
            $('.lista').html('');
            $('.lista').append(retorno);
            montarTabela();

            if ($('.lista').find('table').attr('summary') == 'Nenhum colaborador foi encontrado')
                $('.lista').css('marginTop', 0);

            $('.editar').on('click', function () { editar($(this).closest('tr').find('td:first span').html()); });
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'listarColaboradores()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function montarTabela()
{
    oTable = $('#tblColaboradores').dataTable(
            {
                "bLengthChange": false,
                "order": [[1, "asc"]],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [3, 5 ] }],
                "language": {
                    "url": urlDataTable
                }
            }
        );

    $('#tblColaboradores_filter').parent().removeClass('col-sm-6').addClass('col-sm-10');
    $('#tblColaboradores_filter input').focus();
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

            $('.modal-reenviar-senha').modal('hide');
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
    colaboradorController.$inject = ['$scope', '$rootScope', '$http', '$q', '$timeout'];

    function colaboradorController($scope, $rootScope, $http, $q, $timeout)
    {
        var vm = this;
        vm.activate = _activate;

        function carregarCidades(estado_id, ehNaturalidade, cidade_id)
        {
            if (ehNaturalidade)
                $('#ddlNaturalidadeCidade').parent().addClass('whirl');
            else
                $('#ddlCidade').parent().addClass('whirl');

            var url = homePage + 'Colaborador/ListarCidades';

            $.ajax({
                type: "POST",
                url: url,
                data: { estado_id: estado_id },
                success: function (retorno)
                {
                    if (ehNaturalidade)
                    {
                        $scope.listas.naturalidadeCidades = retorno.Cidades;
                        
                        if (cidade_id !== undefined)
                            $scope.colaborador.naturalidade_cidade = findInArray($scope.listas.naturalidadeCidades, 'Id', $scope.colaborador.naturalidade_cidade_id);
                       
                        $timeout(function () { $('#ddlNaturalidadeCidade').parent().removeClass('whirl'); }, 500);
                    }
                    else
                    {
                        $scope.listas.enderecoCidades = retorno.Cidades;

                        if (cidade_id !== undefined)
                            $scope.colaborador.endereco_cidade = findInArray($scope.listas.enderecoCidades, 'Id', $scope.colaborador.endereco_cidade_id);

                        $timeout(function () { $('#ddlCidade').parent().removeClass('whirl'); }, 500);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError)
                {
                    alertaErroJS({ NomeFuncao: 'verificarCPFJaExiste()', ResponseText: xhr.responseText });
                }
            });
        }

        function carregarDados(id)
        {
            var url = homePage + 'Colaborador/ListarDadosTela';

            $.ajax({
                type: "POST",
                url: url,
                success: function (retorno)
                {
                    $scope.listas.bancos = retorno.Bancos;
                    $scope.listas.estados = retorno.Estados;
                    $scope.listas.grausInstrucao = retorno.GrausInstrucao;
                    $scope.listas.racas = retorno.Racas;

                    if (id !== undefined && id > 0)
                    {
                        $('.container.body-content').addClass('whirl');
                        $scope.buscarColaborador(id);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError)
                {
                    alertaErroJS({ NomeFuncao: 'carregarDados()', ResponseText: xhr.responseText });
                }
            });
        }

        function inicializar()
        {
            var id = $('#hdnId').val();
            $scope.isAdmin = $('#hdnIsAdmin').val() === "1";
            $scope.colaborador = {};
            $scope.colaborador.estado_civil = 'S';
            $scope.listas = {};
            $scope.listas.estados = [];
            $scope.listas.grausInstrucao = [];
            $scope.listas.racas = [];

            carregarDados(id);

            $scope.errorList = {};
            $scope.errorList.nome = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.email = { enable: false, ind: -1, validacoes: ['email'] };
            $scope.errorList.cpf = { enable: false, ind: -1, validacoes: ['cpf', 'cpfJaExiste'] };
            $scope.errorList.rg = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.dataNasc = { enable: false, ind: -1, validacoes: ['date'] };
            $scope.errorList.estadocivil = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.sexo = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.grau_instrucao = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.raca = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.telefone = { enable: false, ind: -1, validacoes: ['brPhoneNumber'] };
            $scope.errorList.celular = { enable: false, ind: -1, validacoes: ['brPhoneNumber'] };
            $scope.errorList.pisPasepNet = { enable: false, ind: -1, validacoes: ['minlength'] };
            $scope.errorList.nacionalidade = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.naturalidadeEstado = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.naturalidadeCidade = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.nome_mae = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.logradouro = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.nroEndereco = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.bairro = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.cep = { enable: false, ind: -1, validacoes: ['brCepMask'] };
            $scope.errorList.estado = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.cidade = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.banco = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.agencia = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.agenciaDigito = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.contaCorrente = { enable: false, ind: -1, validacoes: undefined };

            $scope.errorList.senha = { enable: false, ind: -1, validacoes: undefined };
            $scope.errorList.confirmacaoSenha = { enable: false, ind: -1, validacoes: ['senhaIgualConfirmacao'] };
        }

        function _activate()
        {
            inicializar();

            $scope.buscarColaborador = function(id)
            {
                var url = homePage + 'Colaborador/Obter';
                var isAdmin = $scope.isAdmin || false;

                $.ajax({
                    type: "POST",
                    url: url,
                    data: { id: id, isAdmin: isAdmin },
                    success: function (retorno)
                    {
                        if (retorno.Sucesso)
                        {
                            $scope.colaborador = retorno.Colaborador;
                            $scope.id = $scope.colaborador.id;
                            
                            if (isAdmin)
                                $scope.colaborador.senhaDescriptografada = retorno.SD;

                            $scope.carregarColaborador();
                        }
                        else
                            $timeout(function () { $('.container.body-content').removeClass('whirl'); }, 500);
                    },
                    error: function (xhr, ajaxOptions, thrownError) { alertaErroJS({ NomeFuncao: 'buscarColaborador()', ResponseText: xhr.responseText }); }
                });
            }

            $scope.buscarCEP = function ()
            {
                var cep = $scope.colaborador.endereco_cep;
                cep = cep.replace(/\./g, "");
                cep = cep.replace(/\-/g, "");
                var urlWebService = "https://viacep.com.br/ws/" + cep + "/json/";

                $http.get(urlWebService).then(function (response, status)
                {
                    $scope.colaborador.endereco_logradouro = response.data.logradouro;
                    $scope.colaborador.endereco_bairro = response.data.bairro;


                });
            }

            $scope.carregarCidadesEndereco = function(idCidade)
            {
                $scope.listas.enderecoCidades = [];
                $scope.colaborador.endereco_cidade = {};

                var estado = $scope.colaborador.endereco_estado;

                if (estado !== undefined && estado.Id > 0)
                    carregarCidades(estado.Id, false, idCidade);
            }

            $scope.carregarCidadesNaturalidade = function (idCidade)
            {
                $scope.listas.naturalidadeCidades = [];
                $scope.colaborador.naturalidade_cidade = {};

                var estado = $scope.colaborador.naturalidade_estado;

                if (estado !== undefined && estado.Id > 0)
                    carregarCidades(estado.Id, true, idCidade);
            }

            $scope.carregarColaborador = function()
            {
                $scope.colaborador.sexo_masculino = $scope.colaborador.sexo_masculino.toString();
                $scope.colaborador.data_nascimento = new Date(parseInt($scope.colaborador.data_nascimento.replace('/Date(', '').replace(')/', '')));
                $scope.colaborador.banco = findInArray($scope.listas.bancos, 'id', $scope.colaborador.banco_id);
                $scope.colaborador.raca = findInArray($scope.listas.racas, 'id', $scope.colaborador.raca_id);
                $scope.colaborador.grau_instrucao = findInArray($scope.listas.grausInstrucao, 'id', $scope.colaborador.grau_instrucao_id);

                $scope.colaborador.carteira_trabalho_uf = findInArray($scope.listas.estados, 'Id', $scope.colaborador.carteira_trabalho_estado_id);

                $scope.colaborador.endereco_estado = findInArray($scope.listas.estados, 'Id', $scope.colaborador.endereco_estado_id);
                $scope.carregarCidadesEndereco($scope.colaborador.endereco_cidade_id);

                $scope.colaborador.naturalidade_estado = findInArray($scope.listas.estados, 'Id', $scope.colaborador.naturalidade_estado_id);
                $scope.carregarCidadesNaturalidade($scope.colaborador.naturalidade_cidade_id);

                $timeout(function () { $('.container.body-content').removeClass('whirl'); }, 500);
            }

            $scope.checkIfIsTooltipEnable = function(fieldName, outrasValidacoes, formName)
            {
                var isDirty = $scope.checkIsDirty(fieldName, formName);
                var result = !$scope.colaborador[fieldName] && isDirty;
                var ind = 0;

                if (result || !outrasValidacoes || !$.isArray(outrasValidacoes))
                {
                    var r = { enable: result, ind: result ? 0 : -1, validacoes: outrasValidacoes  };
                    $scope.errorList[fieldName] = r;
                    return r;
                }
                
                $.each(outrasValidacoes, function (i, v)
                {
                    if (eval('$scope.' + formName + '.$error.' + v))
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

            $scope.checkIsDirty = function (fieldName, formName)
            {
                var field = $scope.getField(fieldName, formName);

                if (field === undefined)
                    return false;

                return field.$dirty;
            }

            $scope.getErrorMessage = function(fieldName, lista, formName)
            {
                if ($scope.errorList[fieldName] === undefined)
                    return '';

                formName = formName === undefined ? 'colCadastroForm' : formName;

                $scope.checkIfIsTooltipEnable(fieldName, $scope.errorList[fieldName].validacoes, formName);

                var ind = $scope.errorList[fieldName].ind;

                if (!$.isArray(lista) || ind > lista.length - 1)
                    return '';

                return lista[ind];
            }

            $scope.getField = function (fieldName, formName)
            {
                var result = undefined;

                if (formName === 'colCadastroForm')
                {
                    angular.forEach($scope.colCadastroForm.$error.required, function (field)
                    {
                        if (field.$name === fieldName)
                            result = field;
                    });
                }
                else
                {
                    angular.forEach($scope.frmSenha.$error.required, function (field)
                    {
                        if (field.$name === fieldName)
                            result = field;
                    });
                }

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

                        $scope.checkIfIsTooltipEnable(field.$name, item.validacoes, 'colCadastroForm');
                    }
                });

                if (firstErrorField !== undefined)
                    firstErrorField.focus();

                if ($scope.colaborador.id === undefined || $scope.colaborador.id <= 0) // Cadastro
                    $('.modal-senha').modal('show');
                else
                    $scope.salvarComSenha(true);
            }

            $scope.salvarComSenha = function (existente)
            {
                if (!existente)
                {
                    var firstErrorField = undefined;

                    angular.forEach($scope.frmSenha.$$controls, function (field)
                    {
                        field.$dirty = true;
                        
                        var item = $scope.errorList[field.$name];

                        if (item !== undefined)
                        {
                            if (firstErrorField === undefined && field.$invalid)
                                firstErrorField = $('[name="' + field.$name + '"]');

                            $scope.checkIfIsTooltipEnable(field.$name, item.validacoes, 'frmSenha');
                        }
                    });

                    if (firstErrorField !== undefined)
                        firstErrorField.focus();
                }

                $('.container.body-content').addClass('whirl');
                $('.modal-senha').modal('hide');

                $scope.colaborador.email = $scope.colaborador.email.toLowerCase();
                $scope.colaborador.banco_id = $scope.colaborador.banco.id;
                $scope.colaborador.carteira_trabalho_estado_id = $scope.colaborador.carteira_trabalho_uf.Id;
                $scope.colaborador.raca_id = $scope.colaborador.raca.id;
                $scope.colaborador.data_nascimento = formatDateToDDMMYYYY($scope.colaborador.data_nascimento);
                $scope.colaborador.endereco_cidade_id = $scope.colaborador.endereco_cidade.Id;
                $scope.colaborador.grau_instrucao_id = $scope.colaborador.grau_instrucao.id;
                $scope.colaborador.naturalidade_cidade_id = $scope.colaborador.naturalidade_cidade.Id;
                
                var url = homePage + 'Colaborador/Salvar';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: { cM: $scope.colaborador },
                    success: function (retorno)
                    {
                        if (retorno.Sucesso)
                        {
                            $scope.colaborador = retorno.Colaborador;
                            $scope.carregarColaborador();
                            alert('Dados salvos com sucesso');
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError)
                    {
                        alertaErroJS({ NomeFuncao: 'salvar()', ResponseText: xhr.responseText });
                    }
                });
            }

            $scope.verificarCPFJaExiste = function(cpf)
            {
                return $q(function (resolve, reject)
                {
                    if (cpf === undefined || cpf.length < 11)
                        return resolve();

                    var url = homePage + 'Colaborador/VerificarCPFJaExiste';
                    var id = $scope.id || 0;
                    cpf = removerTodosCaracteresMenosNumeros(cpf);

                    $timeout(function ()
                    {
                        $.ajax({
                            type: "POST",
                            url: url,
                            data: { id: id, cpf: cpf },
                            success: function (retorno)
                            {
                                return retorno ? reject() : resolve();
                            },
                            error: function (xhr, ajaxOptions, thrownError)
                            {
                                alertaErroJS({ NomeFuncao: 'verificarCPFJaExiste()', ResponseText: xhr.responseText });
                            }
                        });
                    }, 500);
                });
            }
        }

        vm.activate();
    }
})();