var oTable;

function editar(id)
{
    window.location.href = homePage + 'Admin/Concurso/Cadastro/' + id;
}

function excluir(id)
{
    if (!confirm('Deseja realmente remover este concurso?'))
        return;

    $.blockUI({ message: 'Excluindo o Concurso', css: cssCarregando });
    var url = homePage + 'Admin/Concurso/Excluir';

    $.ajax({
        type: "POST",
        url: url,
        data: { id: id },
        success: function (retorno)
        {
            $('.lista').html('');
            $('.lista').append(retorno);
            montarTabela();

            if ($('.lista').find('table').attr('summary') === 'Nenhum concurso foi encontrado')
                $('.lista').css('marginTop', 0);

            alert('Concurso excluído com sucesso');

            $.unblockUI();
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'excluir()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function gerarListaColaboradores(id)
{
    $.blockUI({ message: 'Gerando Lista de Colaboradores', css: cssCarregando });
    var url = homePage + 'Admin/Concurso/GerarListaColaboradores';

    $.ajax({
        type: "POST",
        url: url,
        data: { id: id },
        success: function (result)
        {
            if (result) {
                $('#iframeCSV').attr('src', homePage + 'Handlers/DownloadCSVHandler.ashx?tipo=con&id=' + id);
                $('#iframeCSV').load();

                setTimeout(function () { terminouDownload(homePage + 'Admin/Concurso/GerarCSVListaColaboradoresConfirmacao'); }, 2000);
            }
            else {
                alert("Nenhum colaborador foi encontrado para este concurso");
                $.unblockUI();
            }
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'gerarListaColaboradores()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function gerarRPA(id)
{
    window.location.href = homePage + 'Admin/Concurso/RPA/' + id;
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
    concursoController.$inject = ['$scope', '$rootScope', '$filter', '$http', '$q', '$timeout'];

    function concursoController($scope, $rootScope, $filter, $http, $q, $timeout)
    {
        var vm = this;
        vm.activate = _activate;

        function _activate()
        {
            $scope.buscarConcurso = function(id)
            {
                $('.conCadastroForm').addClass('whirl');
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

                            $scope.listas.colaboradores = retorno.Colaboradores;
                            
                            $scope.carregarConcurso();
                        }
                        else
                            $timeout(function () { $('.conCadastroForm').removeClass('whirl'); }, 500);
                    },
                    error: function (xhr, ajaxOptions, thrownError) { alertaErroJS({ NomeFuncao: 'buscarConcurso()', ResponseText: xhr.responseText }); }
                });
            }

            $scope.carregarConcurso = function ()
            {
                $scope.concurso.data = new Date(parseInt($scope.concurso.data.replace('/Date(', '').replace(')/', '')));

                //$scope.colaborador.endereco_estado = findInArray($scope.listas.estados, 'Id', $scope.colaborador.endereco_estado_id);
                //$scope.carregarCidadesEndereco($scope.colaborador.endereco_cidade_id);

                $timeout(function () { $('.conCadastroForm').removeClass('whirl'); }, 500);
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

            $scope.funcao_atualizar_tem_associacao = function()
            {
                var idConcurso = $scope.concurso.id;

                var url = homePage + 'Admin/Concurso/Funcao_Listar';

                $.ajax({
                    type: "POST",
                    url: url,
                    data: { idConcurso: idConcurso },
                    success: function (retorno)
                    {
                        if (retorno.Sucesso)
                        {
                            var nova_lista = retorno.Funcoes;

                            $scope.$apply(function ()
                            {
                                $.each(nova_lista, function (i, f)
                                {
                                    var ind = findInArrayIndex($scope.concurso.funcoes, 'id', f.id);

                                    if (ind !== undefined && ind != null && ind >= 0) {
                                        $scope.concurso.funcoes[ind].temAssociacao = f.temAssociacao;
                                    }
                                });
                            });
                        }

                        $('.lista-locais').removeClass('whirl');
                    },
                    error: function (xhr, ajaxOptions, thrownError) { console.log(xhr.responseText); alertaErroJS({ NomeFuncao: 'local_colaborador_salvar()', ResponseText: xhr.responseText }); }
                });

                
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
                {
                    $.each(local.Colaboradores, function (ind, colaborador)
                    {
                        colaborador.colaborador = findInArray($scope.listas.colaboradores, 'id', colaborador.colaborador_id);
                        colaborador.funcao = findInArray($scope.concurso.funcoes, 'id', colaborador.funcao_id);
                    });

                    local.old_values = { id: local.id, local: local.local, Colaboradores: local.Colaboradores };
                }
                else {
                    var index = findInArrayIndex($scope.concurso.locais, 'id', local.old_values.id);

                    if (local.old_values.id === 0) {
                        $scope.concurso.locais.splice(index, 1);
                    }
                    else {
                        local.id = local.old_values.id;
                        local.local = local.old_values.local;
                        local.Colaboradores = local.old_values.Colaboradores;
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
                            $timeout(function () { $scope.$apply(function () { $scope.concurso.locais.splice(index, 1); }); $scope.funcao_atualizar_tem_associacao(); });

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
                var colaboradores = $filter('notInLocais')($scope.listas.colaboradores, $scope.concurso.locais, 0);

                var funcao = $filter('orderBy')($scope.concurso.funcoes, 'funcao')[0];
                var colaborador = $filter('orderBy')(colaboradores, 'nome')[0];
                var novo_colaborador = { id: 0, colaborador: undefined, funcao: funcao, valor: 0, tem_empresa: false, modoEdicao: false };

                $scope.local_colaborador_mudar_funcao(novo_colaborador, funcao);

                local.Colaboradores.push(novo_colaborador);
                $scope.local_colaborador_editar(0, local, novo_colaborador, true);
            }

            $scope.local_colaborador_changeIfInEdicao = function (colaborador, inEdicao)
            {
                colaborador.modoEdicao = inEdicao;
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

            $scope.local_colaborador_editar = function (i, local, colaborador, editar)
            {
                if (editar)
                    colaborador.old_values = { id: colaborador.id, colaborador: colaborador.colaborador, funcao: colaborador.funcao, valor: colaborador.valor, tem_empresa: colaborador.tem_empresa };
                else {
                    var index = findInArrayIndex(local.Colaboradores, 'id', colaborador.old_values.id);

                    if (colaborador.old_values.id === 0)
                    {
                        local.Colaboradores.splice(index, 1);
                    }
                    else {
                        colaborador.id = colaborador.old_values.id;
                        
                        var old_colaborador = colaborador.old_values.colaborador.originalObject || colaborador.old_values.colaborador;
                        $scope.$broadcast('angucomplete-alt:changeInput', 'ddlColaborador' + i, old_colaborador);

                        colaborador.funcao = colaborador.old_values.funcao;
                        colaborador.valor = colaborador.old_values.valor;
                        colaborador.tem_empresa = colaborador.old_values.tem_empresa;
                    }
                }

                colaborador.modoEdicao = !colaborador.modoEdicao;
            }

            $scope.local_colaborador_excluir = function (local, idColaborador)
            {
                var url = homePage + 'Admin/Concurso/Local_Colaborador_Excluir';
                var index = findInArrayIndex(local.Colaboradores, 'id', idColaborador);
                var colaborador = findInArray(local.Colaboradores, 'id', idColaborador);
                
                if (colaborador === undefined || colaborador === null)
                    return;

                $scope.local_colaborador_changeIfInEdicao(colaborador, false);

                $('.lista-colaboradores').addClass('whirl');

                $.ajax({
                    type: "POST",
                    url: url,
                    data: { idColaborador: idColaborador },
                    success: function (retorno)
                    {
                        if (retorno.Sucesso)
                        {
                            $timeout(function () { $scope.$apply(function () { local.Colaboradores.splice(index, 1); }); $scope.funcao_atualizar_tem_associacao(); });

                            alert('O Colaborador foi removido com sucesso do Local de Prova');
                        }

                        $('.lista-colaboradores').removeClass('whirl');
                    },
                    error: function (xhr, ajaxOptions, thrownError) { alertaErroJS({ NomeFuncao: 'local_colaborador_excluir()', ResponseText: xhr.responseText }); }
                });
            }

            $scope.local_colaborador_filtrar = function (str, colaboradores, colaboradorId)
            {
                var matches = [];
                var listaColaboradores = $filter('notInLocais')(colaboradores, $scope.concurso.locais, colaboradorId);

                listaColaboradores.forEach(function (colaborador)
                {
                    if ((colaborador.nome.toLowerCase().indexOf(str.toString().toLowerCase()) >= 0) ||
                        (colaborador.cpf.toLowerCase().indexOf(str.replace('.', '').replace('.', '').replace('-', '').toString().toLowerCase()) >= 0)) {
                        matches.push(colaborador);
                    }
                });

                return matches;
            };

            $scope.local_colaborador_mudar_funcao = function(colaborador, funcao)
            {
                funcao = funcao || colaborador.funcao;

                if (funcao !== undefined)
                    colaborador.valor = funcao.valor_liquido;
            }

            $scope.local_colaborador_salvar = function (index, local, colaborador)
            {
                var firstErrorField = undefined;
                
                if (colaborador.colaborador === undefined) {
                    $('.ipt-local_colaborador:eq(' + index + ')').parent().addClass('has-error');
                    firstErrorField = $('.ipt-local_colaborador:eq(' + index + ') input');
                    colaborador.colaborador_enabled = true;
                }
                else
                {
                    $('.ipt-local_colaborador:eq(' + index + ')').parent().removeClass('has-error');
                    colaborador.colaborador_enabled = false;
                }

                if (firstErrorField !== undefined) {
                    firstErrorField.focus();
                    return;
                }

                colaborador.colaborador = colaborador.colaborador.originalObject || colaborador.colaborador;

                var idConcursoLocal = local.id;
                colaborador.colaborador_id = colaborador.colaborador.id;
                colaborador.funcao_id = colaborador.funcao.id;


                $('.lista-locais').addClass('whirl');
                var url = homePage + 'Admin/Concurso/Local_Colaborador_Salvar';

                $.ajax({
                    type: "POST",
                    url: url,
                    data: { idConcursoLocal: idConcursoLocal, clcM: colaborador },
                    success: function (retorno)
                    {
                        if (retorno.Sucesso) {
                            $timeout(function () { $scope.$apply(function () { colaborador.modoEdicao = false; colaborador.id = retorno.Colaborador.id; $scope.funcao_atualizar_tem_associacao(); }); });

                            alert('Associação do colaborador com o local de prova efetuada com sucesso');
                        }

                        $('.lista-locais').removeClass('whirl');
                    },
                    error: function (xhr, ajaxOptions, thrownError) { console.log(xhr.responseText); alertaErroJS({ NomeFuncao: 'local_colaborador_salvar()', ResponseText: xhr.responseText }); }
                });
            }

            $scope.local_colaborador_valor_liquido = function (colaborador)
            {
                if (!colaborador.valor_liquido)
                    colaborador.valor_liquido = 0;
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

                $('.conCadastroForm').addClass('whirl');
                var url = homePage + 'Concurso/Salvar';
                $.ajax({
                    type: "POST",
                    url: url,
                    data: { cM: $scope.concurso },
                    success: function (retorno)
                    {
                        if (retorno.Sucesso)
                        {
                            //$scope.concurso = retorno.Concurso;
                            //$scope.carregarConcurso();
                            alert('Dados salvos com sucesso');

                            window.location.href = homePage + 'Admin/Concurso/Cadastro/' + retorno.Concurso.id;
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


    angular.module('ipefae').filter('notInLocais', function ()
    {
        return function (list, locais, current_id)
        {
            if (locais)
            {
                // Obtem a lista de colaboradores selecionados
                var colaboradores_ids = [];

                $.each(locais, function (ind, local)
                {
                    $.each(local.Colaboradores, function (i, colaborador)
                    {
                        colaboradores_ids.push(colaborador.colaborador_id);
                    });
                });

                // Se não houver nenhum selecionado, retorna a propria lista original
                if (colaboradores_ids === undefined || colaboradores_ids.length <= 0)
                    return list;

                var result = [];

                // Percorre a lista original
                $.each(list, function (i, c)
                {
                    // Se o colaborador não estiver na lista de selecionados, adiciona
                    if (c.id == current_id || colaboradores_ids.indexOf(c.id) < 0)
                        result.push(c);
                });

                return result;
            }
        };
    });
})();

(function ()
{
    'use strict';

    angular.module('ipefae').controller("pdfController", pdfController);
    pdfController.$inject = ['$scope', '$q', '$timeout'];

    function pdfController($scope, $q, $timeout)
    {
        $scope.colaboradores = [];
        $scope.PAGE_HEIGHT = 842;
        $scope.PAGE_WIDTH = 595;
        $scope.docDefinition = {};

        $('.rpa-container').addClass('whirl');

        $scope.carregarDados = function ()
        {
            var url = homePage + 'Admin/Concurso/ListarColaboradores';

            $.ajax({
                type: "POST",
                url: url,
                data: { idConcurso: idConcurso },
                success: function (result)
                {
                    $scope.$apply(function ()
                    {
                        $scope.colaboradores = result.Colaboradores;
                    });

                    $('.rpa-container').removeClass('whirl');
                },
                error: function (xhr, ajaxOptions, thrownError)
                {
                    alertaErroJS({ NomeFuncao: 'carregarDados()', ResponseText: xhr.responseText });
                    $.unblockUI();
                }
            });
        }

        $timeout(function () { $scope.carregarDados(); }, 500);

        $scope.export = function ()
        {
            var exportthis = document.getElementById('exportthis');
            var imageWidth = exportthis.clientWidth;

            $('.lista-colaboradores').addClass('whirl');

            html2canvas(exportthis, {
                useCORS: true,
                onrendered: function (canvas)
                {
                    var data = canvas.toDataURL();
                    addImage(data, imageWidth);
                }
            });
        }

        function getPngDimensions (base64)
        {
            const header = atob(base64.slice(22, 70)).slice(16, 24);
            const uint8 = Uint8Array.from(header, c => c.charCodeAt(0));
            const dataView = new DataView(uint8.buffer);

            return {
                width: dataView.getInt32(0),
                height: dataView.getInt32(4)
            };
        }

        const splitImage = (img, imageWidth, callback) => () =>
        {
            var content = [];
            const canvas = document.createElement('canvas');
            const ctx    = canvas.getContext('2d');
            const printHeight = img.height * $scope.PAGE_WIDTH / img.width;

            canvas.width = 1140;
            canvas.height = 1525;

            for (let pages = 0; printHeight > pages * $scope.PAGE_HEIGHT; pages++)
            {
                ctx.fillStyle = "#FFFFFF";
                ctx.fillRect(0, 0, canvas.width, canvas.height);
                ctx.drawImage(img, 0, pages * 1540, 1140, 1525, 0, 0, 1000, 1338);
                content.push({ image: canvas.toDataURL(), width: $scope.PAGE_WIDTH });
            }

            $scope.docDefinition.content = content;

            if (typeof callback === 'function')
                callback();
        };

        function next()
        {
            pdfMake.createPdf($scope.docDefinition).download("RPA.pdf", function () { $('.lista-colaboradores').removeClass('whirl'); });
        }

        function addImage(image, imageWidth)
        {
            var dimensoes = getPngDimensions(image);
            var printHeight = dimensoes.height * $scope.PAGE_WIDTH / dimensoes.width;

            if (printHeight > $scope.PAGE_HEIGHT)
            {
                var img = new Image();
                img.onload = splitImage(img, imageWidth, next);
                img.src = image;
                return;
            }

            var content = [];
            content.push({ image, margin: [0, 5], width: $scope.PAGE_WIDTH });

            $scope.docDefinition.content = content;
            next();
        }
    }
})();