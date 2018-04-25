var oTable;

function editar(codigo)
{
    var id = parseInt(codigo);
    window.location.href = homePage + 'Admin/Concurso/Cadastro/' + id;
}

function iniciarTelaListaConcursos()
{
    listarConcursos();
}

function iniciarTelaCadastroConcurso() { montarTabelaFuncoes(); }

function listarConcursos()
{
    $.blockUI({ message: 'Carregando Concursos', css: cssCarregando });
    var url = homePage + 'Admin/Concurso/ListarConcursos';

    $.ajax({
        type: "POST",
        url: url,
        success: function (retorno)
        {
            $('.lista').html('');
            $('.lista').append(retorno);
            montarTabela();

            if ($('.lista').find('table').attr('summary') === 'Nenhum concurso foi encontrado')
                $('.lista').css('marginTop', 0);

            $('.editar').on('click', function () { editar($(this).closest('tr').find('td:first span').html()); });
            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'listarConcursos()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function montarTabela()
{
    oTable = $('#tblConcursos').dataTable(
            {
                "bLengthChange": false,
                "order": [[1, "asc"]],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [2,3] }],
                "language": {
                    "url": urlDataTable
                }
            }
        );

    $('#tblConcursos_filter').parent().removeClass('col-sm-6').addClass('col-sm-10');
    $('#tblConcursos_filter input').focus();
}

function montarTabelaFuncoes()
{
    oTable = $('#tblFuncoes').dataTable(
            {
                "bLengthChange": false,
                "order": [[1, "asc"]],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [0, 2, 3] }],
                "language": {
                    "url": urlDataTable
                }
            }
        );

    $('#tblFuncoes_filter').parent().removeClass('col-sm-6').addClass('col-sm-10');
}

function montarTabelaLocais()
{
    oTable = $('#tblLocais').dataTable(
            {
                "bLengthChange": false,
                "order": [[1, "asc"]],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [0, 2] }],
                "language": {
                    "url": urlDataTable
                }
            }
        );

    $('#tblLocais_filter').parent().removeClass('col-sm-6').addClass('col-sm-10');
}

(function ()
{
    'use strict';

    angular.module('ipefae').controller('concursoController', concursoController);
    concursoController.$inject = ['$scope', '$rootScope', '$http', '$q', '$timeout'];

    function concursoController($scope, $rootScope, $http, $q, $timeout)
    {
        var vm = this;
        vm.activate = _activate;

        function _activate()
        {
            $scope.buscarConcurso = function(id)
            {
                var url = homePage + 'Admin/Concurso/Obter';

                $.ajax({
                    type: "POST",
                    url: url,
                    data: { id: id },
                    success: function (retorno)
                    {
                        if (retorno.Sucesso)
                        {
                            $scope.concurso = retorno.Concurso;
                            $scope.id = $scope.concurso.id;
                            
                            $scope.carregarConcurso();
                        }
                        else
                            $timeout(function () { $('.container.body-content').removeClass('whirl'); }, 500);
                    },
                    error: function (xhr, ajaxOptions, thrownError) { alertaErroJS({ NomeFuncao: 'buscarConcurso()', ResponseText: xhr.responseText }); }
                });
            }

            $scope.carregarConcurso = function ()
            {
                $scope.concurso.data = new Date(parseInt($scope.concurso.data.replace('/Date(', '').replace(')/', '')));

                //$scope.colaborador.endereco_estado = findInArray($scope.listas.estados, 'Id', $scope.colaborador.endereco_estado_id);
                //$scope.carregarCidadesEndereco($scope.colaborador.endereco_cidade_id);

                $timeout(function () { $('.container.body-content').removeClass('whirl'); }, 500);
            }

            $scope.checkIfIsTooltipEnable = function(fieldName, outrasValidacoes, formName)
            {
                var isDirty = $scope.checkIsDirty(fieldName, formName);
                var result = !$scope.concurso[fieldName] && isDirty;
                var ind = 0;
                var tipo = $scope.errorListConcurso[fieldName].tipo;
                
                if (result || !outrasValidacoes || !$.isArray(outrasValidacoes))
                {
                    var r = { enable: result, ind: result ? 0 : -1, tipo: tipo, validacoes: outrasValidacoes  };
                    $scope.errorListConcurso[fieldName] = r;
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

                var rx = { enable: result, ind: result ? ind : -1, tipo: tipo, validacoes: outrasValidacoes };
                $scope.errorListConcurso[fieldName] = rx;
                return rx;
            }

            $scope.checkIsDirty = function (fieldName, formName)
            {
                var field = $scope.getField(fieldName, formName);

                if (field === undefined)
                    return false;

                return field.$dirty;
            }

            $scope.funcao_excluir = function (idFuncao)
            {
                var url = homePage + 'Admin/Concurso/Funcao_Excluir';
                var index = findInArrayIndex($scope.concurso.funcoes, 'id', idFuncao);

                if (index < 0)
                    return;

                $.ajax({
                    type: "POST",
                    url: url,
                    data: { idFuncao: idFuncao },
                    success: function (retorno)
                    {
                        if (retorno.Sucesso) {
                            $scope.concurso.funcoes.splice(index, 1);
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) { alertaErroJS({ NomeFuncao: 'funcao_excluir()', ResponseText: xhr.responseText }); }
                });
            }

            $scope.funcao_salvar = function (idFuncao)
            {
                idFuncao = idFuncao || 0;

                var firstErrorField = undefined;

                angular.forEach($scope.conCadastroForm.$$controls, function (field)
                {
                    field.$dirty = true;
                    var item = $scope.errorListConcurso[field.$name];

                    if (item !== undefined && item.tipo === 'funcao')
                    {
                        if (firstErrorField === undefined && field.$invalid)
                            firstErrorField = $('[name="' + field.$name + '"]');

                        var r = $scope.checkIfIsTooltipEnable(field.$name, item.validacoes, 'conCadastroForm');

                        if (r.enable)
                            $('[name="' + field.$name + '"]').parent().addClass('has-error');
                        else
                            $('[name="' + field.$name + '"]').parent().removeClass('has-error');
                    }
                });

                if (firstErrorField !== undefined) {
                    firstErrorField.focus();
                    return;
                }

                var idConcurso = $scope.concurso.id;
                var funcao = $scope.funcao;
                var valor_liquido = $scope.valor_liquido;
                var funcaoModel = { id: idFuncao, funcao: funcao, valor_liquido: valor_liquido };

                $('.lista-funcoes').addClass('whirl');
                var url = homePage + 'Admin/Concurso/Funcao_Salvar';

                $.ajax({
                    type: "POST",
                    url: url,
                    data: { idConcurso: idConcurso, cfM: funcaoModel },
                    success: function (retorno)
                    {
                        if (retorno.Sucesso)
                        {
                            if (idFuncao === 0)
                            {
                                $scope.concurso.funcoes.push({ id: retorno.IdFuncao, funcao: funcao, valor_liquido: valor_liquido });
                            }

                            $scope.modoEdicaoFuncao = false;
                            $scope.funcao = '';
                            $scope.valor_liquido = '';
                        }

                        $('.lista-funcoes').removeClass('whirl');
                    },
                    error: function (xhr, ajaxOptions, thrownError) { alertaErroJS({ NomeFuncao: 'funcao_adicionar()', ResponseText: xhr.responseText }); }
                });
            }

            $scope.getErrorMessage = function (fieldName, lista, formName)
            {
                if ($scope.errorListConcurso[fieldName] === undefined)
                    return '';

                formName = formName === undefined ? 'conCadastroForm' : formName;

                $scope.checkIfIsTooltipEnable(fieldName, $scope.errorListConcurso[fieldName].validacoes, formName);

                var ind = $scope.errorListConcurso[fieldName].ind;

                if (!$.isArray(lista) || ind > lista.length - 1)
                    return '';

                return lista[ind];
            }

            $scope.getField = function (fieldName, formName)
            {
                var result = undefined;

                angular.forEach($scope.conCadastroForm.$error.required, function (field)
                {
                    if (field.$name === fieldName)
                        result = field;
                });

                return result;
            }

            $scope.inicializar = function ()
            {
                var id = $('#hdnId').val();
                $scope.isAdmin = $('#hdnIsAdmin').val() === "1";
                $scope.modoEdicaoFuncao = false;
                $scope.modoEdicaoLocal = false;

                $scope.concurso = {};
                $scope.concurso.id = 0;
                $scope.concurso.funcoes = [];
                $scope.listas = {};

                $scope.buscarConcurso(id);

                $scope.errorListConcurso = {};
                $scope.errorListConcurso.nome = { enable: false, ind: -1, tipo: 'salvar', validacoes: undefined };
                $scope.errorListConcurso.data = { enable: false, ind: -1, tipo: 'salvar', validacoes: ['date'] };

                $scope.errorListConcurso.funcao = { enable: false, ind: -1, tipo: 'funcao', validacoes: undefined };
                $scope.errorListConcurso.valor_liquido = { enable: false, ind: -1, tipo: 'funcao', validacoes: undefined };
            }

            $scope.salvar = function()
            {
                var firstErrorField = undefined;

                angular.forEach($scope.conCadastroForm.$$controls, function (field)
                {
                    field.$dirty = true;
                    var item = $scope.errorListConcurso[field.$name];

                    if (item !== undefined && item.tipo === 'salvar')
                    {
                        if (firstErrorField === undefined && field.$invalid)
                            firstErrorField = $('[name="' + field.$name + '"]');

                        $scope.checkIfIsTooltipEnable(field.$name, item.validacoes, 'conCadastroForm');
                    }
                });

                if (firstErrorField !== undefined)
                {
                    firstErrorField.focus();
                    return;
                }

                $scope.concurso.data = formatDateToDDMMYYYY($scope.concurso.data);

                $('.container.body-content').addClass('whirl');
                var url = homePage + 'Concurso/Salvar';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: { cM: $scope.concurso },
                    success: function (retorno)
                    {
                        if (retorno.Sucesso) {
                            $scope.concurso = retorno.Concurso;
                            $scope.carregarConcurso();
                            alert('Dados salvos com sucesso');
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError)
                    {
                        alertaErroJS({ NomeFuncao: 'salvar()', ResponseText: xhr.responseText });
                    }
                });
            }

            $scope.inicializar();
        }

        vm.activate();
    }
})();