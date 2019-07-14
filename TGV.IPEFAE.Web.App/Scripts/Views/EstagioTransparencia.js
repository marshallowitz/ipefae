(function ()
{
    'use strict';

    angular.module('ipefae').controller('estagioTransparenciaController', estagioTransparenciaController);
    estagioTransparenciaController.$inject = ['$scope', '$rootScope', '$http', '$q', '$timeout'];

    function estagioTransparenciaController($scope, $rootScope, $http, $q, $timeout)
    {
        var vm = this;
        vm.activate = _activate;

        function inicializarTela()
        {
            $scope.exibir = false;
            $scope.id_tipo_selecionado = 0;
            $scope.tipo_selecionado = "";
            $scope.arquivos = [];
        }

        function _activate()
        {
            inicializarTela();

            $scope.filtrar = function (item)
            {
                if (!vm.filtro)
                    return true;

                var encontrou = (item.nome.toLowerCase().indexOf(vm.filtro.toLowerCase()) >= 0
                    || item.url.toLowerCase().indexOf(vm.filtro.toLowerCase()) >= 0);

                return encontrou;
            };

            $scope.listar = function (tipo)
            {
                $scope.id_tipo_selecionado = tipo;
                $scope.exibir = true;

                switch (tipo)
                {
                    case 1:
                        $scope.tipo_selecionado = "Atas";
                        break;
                    case 2:
                        $scope.tipo_selecionado = "CNPJ";
                        break;
                    case 3:
                        $scope.tipo_selecionado = "DREs";
                        break;
                    case 4:
                        $scope.tipo_selecionado = "Estatuto";
                        break;
                    case 5:
                        $scope.tipo_selecionado = "Remuneração";
                        break;
                    default:
                        inicializarTela();
                        break;
                }

                $('.tabela').addClass('whirl');

                $.ajax({
                    type: "POST",
                    url: homePage + '/EstagioTransparencia/Listar',
                    data: { tipo: tipo },
                    success: function (retorno)
                    {
                        $scope.arquivos = retorno.Arquivos;
                        $scope.$apply();

                        $('.tabela').removeClass('whirl');
                    },
                    error: function (xhr, status, p3, p4)
                    {
                        alertaErroJS({ NomeFuncao: 'listar()', ResponseText: xhr.responseText });
                        $('.tabela').removeClass('whirl');
                    }
                });
            }
        }

        vm.activate();
    }


    angular.module('ipefae').controller('estagioTransparenciaCadastroController', estagioTransparenciaCadastroController);
    estagioTransparenciaCadastroController.$inject = ['$scope', '$rootScope', '$http', '$q', '$timeout'];

    function estagioTransparenciaCadastroController($scope, $rootScope, $http, $q, $timeout)
    {
        var vm = this;
        vm.activate = _activate;
        vm.tem_arquivo = false;
        $scope.arquivos = [];

        function inicializarTela(salvando, tipo)
        {
            $scope.novoArquivo = {};
            $scope.novoArquivo.tipo = "1";

            $scope.filtro = {};
            
            vm.tem_arquivo = false;

            if (!salvando)
            {
                $scope.exibir_visao_adicionar = false;
                $scope.filtro.tipo = "1";
            }
            else
                $scope.filtro.tipo = tipo ? tipo : "1";

            $scope.listar();
        }

        function _activate()
        {
            $('.tabela').addClass('whirl');

            $scope.apagar = function (indice)
            {
                if (confirm('Deseja realmente apagar esse arquivo?'))
                {
                    var url = $scope.arquivos[indice].url;

                    $.ajax({
                        type: "POST",
                        url: homePage + '/Admin/EstagioTransparencia/Apagar',
                        data: { url: url },
                        success: function (retorno)
                        {
                            $scope.arquivos.splice(indice, 1);
                            $timeout(function () { alert('Arquivo removido com sucesso'); }, 200);

                            $('.tabela').removeClass('whirl');
                        },
                        error: function (xhr, status, p3, p4)
                        {
                            alertaErroJS({ NomeFuncao: 'apagar()', ResponseText: xhr.responseText });
                            $('.tabela').removeClass('whirl');
                        }
                    });
                }
            }

            $scope.cancelar = function ()
            {
                inicializarTela(false);
            }

            $scope.listar = function ()
            {
                $('.tabela').addClass('whirl');

                $.ajax({
                    type: "POST",
                    url: homePage + '/Admin/EstagioTransparencia/Listar',
                    data: { tipo: $scope.filtro.tipo, nome: $scope.filtro.nome },
                    success: function (retorno)
                    {
                        $scope.arquivos = retorno.Arquivos;
                        $scope.$apply();

                        $('.tabela').removeClass('whirl');
                    },
                    error: function (xhr, status, p3, p4)
                    {
                        alertaErroJS({ NomeFuncao: 'listar()', ResponseText: xhr.responseText });
                        $('.tabela').removeClass('whirl');
                    }
                });
            }

            $scope.salvar = function ()
            {
                $('.adicionar').addClass('whirl');
                var tipo = $scope.novoArquivo.tipo;
                var nome = $scope.novoArquivo.nome;

                $.ajax({
                    type: "POST",
                    url: homePage + '/Admin/EstagioTransparencia/Salvar',
                    data: { tipo: tipo, nome: nome },
                    success: function (retorno)
                    {
                        inicializarTela(true, tipo);
                        alert(retorno.Sucesso ? "Documento salvo com sucesso" : "Houve um problema ao gravar o documento");

                        $scope.$apply();
                        $('.adicionar').removeClass('whirl');
                    },
                    error: function (xhr, status, p3, p4)
                    {
                        alertaErroJS({ NomeFuncao: 'salvar()', ResponseText: xhr.responseText });
                        $('.adicionar').removeClass('whirl');
                    }
                });
            }

            $scope.uploadPDF = function(e)
            {
                var files = e.target.files;
                if (files.length > 0)
                {
                    if (window.FormData !== undefined)
                    {
                        var data = new FormData();
                        for (var x = 0; x < files.length; x++)
                        {
                            data.append("file" + x, files[x]);
                        }

                        $('.adicionar').addClass('whirl');

                        $.ajax({
                            type: "POST",
                            url: homePage + '/Admin/EstagioTransparencia/UploadPDF',
                            contentType: false,
                            processData: false,
                            data: data,
                            success: function (retorno)
                            {
                                $('#txtUploadPDF').val('');

                                if (retorno.Sucesso)
                                    vm.tem_arquivo = true;
                                else
                                    alert(retorno.Mensagem);

                                $scope.$apply();
                                $('.adicionar').removeClass('whirl');
                            },
                            error: function (xhr, status, p3, p4)
                            {
                                alertaErroJS({ NomeFuncao: 'uploadPDF()', ResponseText: xhr.responseText });
                                $('.adicionar').removeClass('whirl');
                            }
                        });
                    }
                }
            }

            // Realiza as chamadas
            inicializarTela(false);
            $('#txtUploadPDF').on('change', function (e) { $scope.uploadPDF(e); });
        }

        vm.activate();
    }
})();