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

function iniciarTelaCadastroConcurso() { }

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

            $scope.funcao_adicionar = function()
            {
                $scope.funcao_opened = true;
                $scope.local_opened = false;

                var nova_funcao = { id: 0, funcao: '', valor_liquido: 0 };
                $scope.concurso.funcoes.push(nova_funcao);
                $scope.funcao_editar(nova_funcao, true);

                $timeout(function ()
                {
                    $.each($('.ipt-funcao'), function (i, el)
                    {
                        if ($(el).scope().funcao.id === 0)
                            $(el).focus();
                    });
                }, 500);
            }

            $scope.funcao_changeIfInEdicao = function (inEdicao)
            {
                $.each($scope.concurso.funcoes, function (i, funcao)
                {
                    funcao.modoEdicao = inEdicao;
                });
            }

            $scope.funcao_checkIfInEdicao = function()
            {
                var inEdicao = false;
                
                $.each($scope.concurso.funcoes, function (i, funcao)
                {
                    if(funcao.modoEdicao)
                    {
                        inEdicao = true;
                        return false;
                    }
                });

                return inEdicao;
            }

            $scope.funcao_editar = function (funcao, editar)
            {
                if (editar)
                    funcao.old_values = { id: funcao.id, funcao: funcao.funcao, valor_liquido: funcao.valor_liquido, valor_liquido_formatado: funcao.valor_liquido_formatado };
                else
                {
                    var index = findInArrayIndex($scope.concurso.funcoes, 'id', funcao.old_values.id);

                    if (funcao.old_values.id === 0)
                    {
                        $scope.concurso.funcoes.splice(index, 1);
                    }
                    else
                    {
                        funcao.id = funcao.old_values.id;
                        funcao.funcao = funcao.old_values.funcao;
                        funcao.valor_liquido = funcao.old_values.valor_liquido;
                        funcao.valor_liquido_formatado = funcao.old_values.valor_liquido_formatado;
                    }

                    $scope.funcao_limparDirty('ipt-funcao', index, funcao);
                }

                funcao.modoEdicao = !funcao.modoEdicao;
            }

            $scope.funcao_excluir = function (idFuncao)
            {
                var url = homePage + 'Admin/Concurso/Funcao_Excluir';
                var index = findInArrayIndex($scope.concurso.funcoes, 'id', idFuncao);

                if (index < 0)
                    return;

                $scope.funcao_changeIfInEdicao(false);

                $('.lista-funcoes').addClass('whirl');

                $.ajax({
                    type: "POST",
                    url: url,
                    data: { idFuncao: idFuncao },
                    success: function (retorno)
                    {
                        if (retorno.Sucesso)
                        {
                            $timeout(function () { $scope.$apply(function () { $scope.concurso.funcoes.splice(index, 1); }); });

                            alert('Função removida com sucesso');
                        }

                        $('.lista-funcoes').removeClass('whirl');
                    },
                    error: function (xhr, ajaxOptions, thrownError) { alertaErroJS({ NomeFuncao: 'funcao_excluir()', ResponseText: xhr.responseText }); }
                });
            }

            $scope.funcao_limparDirty = function(nomeCampo, index, funcao)
            {
                $('.' + nomeCampo + ':eq(' + index + ')').parent().removeClass('has-error');
                funcao.funcao_enabled = false;
            }

            $scope.funcao_salvar = function (funcao)
            {
                var firstErrorField = undefined;
                var index = findInArrayIndex($scope.concurso.funcoes, 'id', funcao.id);

                if (funcao.funcao === undefined || $.trim(funcao.funcao) === '')
                {
                    $('.ipt-funcao:eq(' + index + ')').parent().addClass('has-error');
                    funcao.funcao_enabled = true;
                    firstErrorField = $('.ipt-funcao:eq(' + index + ')');
                }
                else
                    $scope.funcao_limparDirty('ipt-funcao', index, funcao);


                if (firstErrorField !== undefined) {
                    firstErrorField.focus();
                    return;
                }

                var idConcurso = $scope.concurso.id;

                $('.lista-funcoes').addClass('whirl');
                var url = homePage + 'Admin/Concurso/Funcao_Salvar';

                $.ajax({
                    type: "POST",
                    url: url,
                    data: { idConcurso: idConcurso, cfM: funcao },
                    success: function (retorno)
                    {
                        if (retorno.Sucesso)
                        {
                            $timeout(function () { $scope.$apply(function () { funcao.modoEdicao = false; funcao.id = retorno.Funcao.id }); });
 
                            alert('Dados da função gravados com sucesso');
                        }

                        $('.lista-funcoes').removeClass('whirl');
                    },
                    error: function (xhr, ajaxOptions, thrownError) { alertaErroJS({ NomeFuncao: 'funcao_salvar()', ResponseText: xhr.responseText }); }
                });
            }

            $scope.funcao_valor_liquido = function(funcao)
            {
                if (!funcao.valor_liquido)
                    funcao.valor_liquido = 0;
            }


            $scope.getUrl = function(url)
            {
                return homePage + url;
            }
            

            $scope.local_adicionar = function ()
            {
                $scope.funcao_opened = false;
                $scope.local_opened = true;

                var novo_local = { id: 0, local: '', Colaboradores: [] };
                $scope.concurso.locais.push(novo_local);
                $scope.local_editar(novo_local, true);

                $timeout(function ()
                {
                    $.each($('.ipt-local'), function (i, el)
                    {
                        if ($(el).scope().local.id === 0)
                            $(el).focus();
                    });
                }, 500);
            }

            $scope.local_changeIfInEdicao = function (inEdicao)
            {
                $.each($scope.concurso.locais, function (i, local)
                {
                    local.modoEdicao = inEdicao;
                });
            }

            $scope.local_checkIfInEdicao = function ()
            {
                var inEdicao = false;

                $.each($scope.concurso.locais, function (i, local)
                {
                    if (local.modoEdicao) {
                        inEdicao = true;
                        return false;
                    }
                });

                return inEdicao;
            }

            $scope.local_editar = function (local, editar)
            {
                if (editar)
                    local.old_values = { id: local.id, local: local.local, colaboradores: local.colaboradores };
                else {
                    var index = findInArrayIndex($scope.concurso.locais, 'id', local.old_values.id);

                    if (local.old_values.id === 0) {
                        $scope.concurso.locais.splice(index, 1);
                    }
                    else {
                        local.id = local.old_values.id;
                        local.funcao = local.old_values.local;
                        local.colaboradores = local.old_values.colaboradores;
                    }

                    $scope.local_limparDirty('ipt-local', index, local);
                }

                local.modoEdicao = !local.modoEdicao;
            }

            $scope.local_excluir = function (idLocal)
            {
                if (!confirm('Deseja realmente remover este local de prova?'))
                    return;

                var url = homePage + 'Admin/Concurso/Local_Excluir';
                var index = findInArrayIndex($scope.concurso.locais, 'id', idLocal);

                if (index < 0)
                    return;

                $scope.local_changeIfInEdicao(false);

                $('.lista-locais').addClass('whirl');

                $.ajax({
                    type: "POST",
                    url: url,
                    data: { idLocal: idLocal },
                    success: function (retorno)
                    {
                        if (retorno.Sucesso) {
                            $timeout(function () { $scope.$apply(function () { $scope.concurso.locais.splice(index, 1); }); });

                            alert('Local de Prova removido com sucesso');
                        }

                        $('.lista-locais').removeClass('whirl');
                    },
                    error: function (xhr, ajaxOptions, thrownError) { alertaErroJS({ NomeFuncao: 'local_excluir()', ResponseText: xhr.responseText }); }
                });
            }

            $scope.local_limparDirty = function (nomeCampo, index, local)
            {
                $('.' + nomeCampo + ':eq(' + index + ')').parent().removeClass('has-error');
                local.local_enabled = false;
            }

            $scope.local_salvar = function (local)
            {
                var firstErrorField = undefined;
                var index = findInArrayIndex($scope.concurso.locais, 'id', local.id);

                if (local.local === undefined || $.trim(local.local) === '')
                {
                    $('.ipt-local:eq(' + index + ')').parent().addClass('has-error');
                    local.local_enabled = true;
                    firstErrorField = $('.ipt-local:eq(' + index + ')');
                }
                else
                    $scope.local_limparDirty('ipt-local', index, local);


                if (firstErrorField !== undefined)
                {
                    firstErrorField.focus();
                    return;
                }

                var idConcurso = $scope.concurso.id;

                $('.lista-locais').addClass('whirl');
                var url = homePage + 'Admin/Concurso/Local_Salvar';

                $.ajax({
                    type: "POST",
                    url: url,
                    data: { idConcurso: idConcurso, clM: local },
                    success: function (retorno)
                    {
                        if (retorno.Sucesso) {
                            $timeout(function () { $scope.$apply(function () { local.modoEdicao = false; local.id = retorno.Local.id }); });

                            alert('Dados do local da prova gravados com sucesso');
                        }

                        $('.lista-locais').removeClass('whirl');
                    },
                    error: function (xhr, ajaxOptions, thrownError) { alertaErroJS({ NomeFuncao: 'local_salvar()', ResponseText: xhr.responseText }); }
                });
            }


            $scope.local_colaborador_adicionar = function (local)
            {
                var novo_colaborador = { id: 0, nome: '', valor_liquido: 0 };
                local.Colaboradores.push(novo_colaborador);
                $scope.local_colaborador_editar(local, novo_colaborador, true);

                $timeout(function ()
                {
                    $.each($('.ipt-local_colaborador'), function (i, el)
                    {
                        if ($(el).scope().local.id === 0)
                            $(el).focus();
                    });
                }, 500);
            }

            $scope.local_colaborador_checkIfInEdicao = function (local)
            {
                if (!$scope.local_checkIfInEdicao())
                    return false;

                var inEdicao = false;

                $.each(local.Colaboradores, function (i, colaborador)
                {
                    if (colaborador.modoEdicao) {
                        inEdicao = true;
                        return false;
                    }
                });

                return inEdicao;
            }

            $scope.local_colaborador_editar = function (local, colaborador, editar)
            {
                if (editar)
                    colaborador.old_values = { id: colaborador.id, local: local, nome: colaborador.nome, valor_liquido: colaborador.valor_liquido };
                else {
                    var index = findInArrayIndex(local.Colaboradores, 'id', colaborador.old_values.id);

                    if (local.old_values.id === 0) {
                        $scope.concurso.locais.splice(index, 1);
                    }
                    else {
                        local.id = local.old_values.id;
                        local.funcao = local.old_values.local;
                        local.colaboradores = local.old_values.colaboradores;
                    }

                    $scope.local_limparDirty('ipt-local', index, local);
                }

                local.modoEdicao = !local.modoEdicao;
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
                $scope.concurso.locais = [];
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