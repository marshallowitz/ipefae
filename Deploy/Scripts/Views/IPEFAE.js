var janelaPopUp;

function abrirJanelaPopUpWait(mensagem)
{
    var url = homePage + 'Home/ViewCarregando';
    janelaPopUp = window.open(url);
    janelaPopUp.document.title = mensagem;
    abrirJanelaPopUpWaitChamadaFuncao(janelaPopUp, mensagem);
}

function abrirJanelaPopUpWaitChamadaFuncao(janelaPopUp, mensagem)
{
    if (!janelaPopUp.executarWait)
        setTimeout(function () { abrirJanelaPopUpWaitChamadaFuncao(janelaPopUp, mensagem); }, 1000);
    else
        janelaPopUp.executarWait(mensagem);
}

function checarCaptcha(container) {
    if (container.length == 0)
        return true;

    var retorno = false;
    var sessionName = container.find('input[type=hidden]').val();

    var url = homePage + 'Base/CaptchaCheck';
    var data = { sessionName: sessionName };

    $.ajax({
        type: 'POST',
        url: url,
        data: data,
        async: false,
        success: function (ok) {
            retorno = ok;
        },
        error: function (xhr, ajaxOptions, thrownError) { alertaErroJS({ NomeFuncao: 'captcha()', ResponseText: xhr.responseText }); }
    });

    return retorno;
}

function gerarBoletoBancario(idConcurso, idInscrito, apagarSessao, reload) {
    var url = homePage + 'Concurso/GerarBoletoBancario';

    if (reload === undefined)
        reload = true;

    $.ajax({
        type: "POST",
        url: url,
        data: { idConcurso: idConcurso, idInscrito: idInscrito, apagarSessao: apagarSessao },
        success: function (retorno) {
            var w = janelaPopUp;
            //console.log(retorno);
            gerarBoletoBancarioChamadaFuncao(w, retorno.Boleto, reload);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'gerarBoletoBancario()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

function gerarBoletoBancarioChamadaFuncao(w, boleto, reload)
{
    if (!janelaPopUp.getTitulo || janelaPopUp.getTitulo() === '')
        setTimeout(function () { gerarBoletoBancarioChamadaFuncao(w, boleto, reload); }, 1000);
    else
    {
        w.document.title = ".:: IPEFAE ::.";

        try
        {
            w.document.write(boleto);
            w.document.close();
        } catch (e)
        {
            //alert(e);
            
            if (reload) window.location.reload();
        }
    }
}

function gerarComprovanteInscricao(nomeConcurso, idInscrito)
{
    var url = homePage + 'Concurso/GerarComprovanteInscricao';

    $.ajax({
        type: "POST",
        url: url,
        data: { nomeConcurso: nomeConcurso, idInscrito: idInscrito },
        success: function (retorno) {
            var w = janelaPopUp;
            gerarComprovanteInscricaoChamadaFuncao(w, retorno.Comprovante);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'gerarComprovanteInscricao()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

function gerarComprovanteInscricaoChamadaFuncao(w, comprovante) {
    if (!janelaPopUp.getTitulo || janelaPopUp.getTitulo() === '')
        setTimeout(function () { gerarComprovanteInscricaoChamadaFuncao(w, comprovante); }, 1000);
    else {
        w.document.title = ".:: IPEFAE ::.";
        $(w.document.body).html(comprovante);
    }
}

function gerarPDFColaboradores(idConcurso, reload)
{
    var url = homePage + 'Admin/Concurso/RPA';

    $.ajax({
        type: "POST",
        url: url,
        data: { id: idConcurso },
        success: function (html)
        {
            var w = janelaPopUp;
            //console.log(retorno);
            gerarPDFColaboradoresChamadaFuncao(w, html, reload);
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'gerarPDFColaboradores()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

function gerarPDFColaboradoresChamadaFuncao(w, html, reload)
{
    if (!janelaPopUp.getTitulo || janelaPopUp.getTitulo() === '')
        setTimeout(function () { gerarPDFColaboradoresChamadaFuncao(w, html, reload); }, 1000);
    else {
        w.document.title = ".:: IPEFAE ::.";

        try {
            w.document.write(html);
            w.document.close();
        } catch (e) {
            //alert(e);
            if (reload) window.location.reload();
        }
    }
}

function obterClassificacao(nomeConcurso, idInscrito)
{
    var url = homePage + 'Concurso/GerarClassificacao';

    $.ajax({
        type: "POST",
        url: url,
        data: { nomeConcurso: nomeConcurso, idInscrito: idInscrito },
        success: function (retorno)
        {
            var w = janelaPopUp;
            obterClassificacaoChamadaFuncao(w, retorno.Comprovante);
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'obterClassificacao()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

function obterClassificacaoChamadaFuncao(w, comprovante)
{
    if (!janelaPopUp.getTitulo || janelaPopUp.getTitulo() === '')
        setTimeout(function () { obterClassificacaoChamadaFuncao(w, comprovante); }, 1000);
    else
    {
        w.document.title = ".:: IPEFAE ::.";
        $(w.document.body).html(comprovante);
    }
}

function obterStatusPagamento(nomeConcurso, idConcurso, idInscrito, reload)
{
    var url = homePage + 'Concurso/GerarStatusPagamento';

    if (reload === undefined)
        reload = true;

    $.ajax({
        type: "POST",
        url: url,
        data: { nomeConcurso: nomeConcurso, idInscrito: idInscrito },
        success: function (retorno)
        {
            var w = janelaPopUp;
            obterStatusPagamentoChamadaFuncao(w, retorno.Panel, reload);
        },
        error: function (xhr, ajaxOptions, thrownError)
        {
            alertaErroJS({ NomeFuncao: 'obterStatusPagamento()', ResponseText: xhr.responseText });
        }
    });

    return false;
}

function obterStatusPagamentoChamadaFuncao(w, panel, reload)
{
    if (!janelaPopUp.getTitulo || janelaPopUp.getTitulo() === '')
        setTimeout(function () { obterStatusPagamentoChamadaFuncao(w, panel, reload); }, 1000);
    else
    {
        w.document.title = ".:: IPEFAE ::.";
        try { $(w.document.body).html(panel); if (reload) { window.location.reload(); } } catch (e) { window.location.reload(); }
    }
}

function imprimirBoletoInscrito(idConcurso, idInscrito)
{
    var janelaPopUp;
    var mensagemGerandoBoletoBancario = "Gerando Boleto Bancário";
    abrirJanelaPopUpWait(mensagemGerandoBoletoBancario);
    gerarBoletoBancario(idConcurso, idInscrito, false, false);
}

function imprimirComprovanteInscricao(idConcurso, idInscrito) {
    abrirJanelaPopUpWait(mensagemGerandoComprovanteInscricao);
    gerarComprovanteInscricao(idConcurso, idInscrito);
}

function removerAnexo(pathArquivo, callbackFunction)
{
    var url = homePage + 'Base/RemoverArquivo';

    $.ajax({
        type: "POST",
        url: url,
        data: { pathArquivo: pathArquivo },
        success: function (retorno) {
            if (retorno && (typeof callbackFunction == 'function'))
                callbackFunction();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'removerAnexo()', ResponseText: xhr.responseText });
        }
    });
}

function setTabsContentHeight(func, ctl, cnt)
{
    if (cnt > 10)
        return;

    if (ctl)
        if (ctl.children().length > 0)
            func();
        else
            setTimeout(function () { setTabsContentHeight(func, ctl, cnt++); }, 500);
}

var DadosGerais = {};

function carregarDadosGerais()
{
    var bancos = [
        { id: 1, codigo: '001', nome: 'Banco do Brasil S.A.', ordem: 1 },
        { id: 2, codigo: '341', nome: 'Banco Itaú S.A.', ordem: 2 },
        { id: 3, codigo: '033', nome: 'Banco Santander(Brasil) S.A.', ordem: 3 },
        { id: 4, codigo: '237', nome: 'Banco Bradesco S.A.', ordem: 4 },
        { id: 5, codigo: '745', nome: 'Banco Citibank S.A.', ordem: 5 },
        { id: 6, codigo: '399', nome: 'HSBC Bank Brasil S.A.', ordem: 6 },
        { id: 7, codigo: '104', nome: 'Caixa Econômica Federal', ordem: 7 },
        { id: 8, codigo: '389', nome: 'Banco Mercantil do Brasil S.A.', ordem: 8 },
        { id: 9, codigo: '453', nome: 'Banco Rural S.A.', ordem: 9 },
        { id: 10, codigo: '422', nome: 'Banco Safra S.A.', ordem: 10 },
        { id: 11, codigo: '318', nome: 'Banco BMG S.A', ordem: 11 },
        { id: 12, codigo: '748', nome: 'Banco Cooperativo Sicred', ordem: 12 },
        { id: 13, codigo: '756', nome: 'Banco Cooperativo do Brasil S.A. (Bancoob – Sicoob)', ordem: 13 },
        { id: 14, codigo: '260', nome: 'Nubank', ordem: 14 }
    ];

    var estados = [
        { Id: 1, Sigla: 'AC', Nome: 'Acre' },
        { Id: 2, Sigla: 'AL', Nome: 'Alagoas' },
        { Id: 3, Sigla: 'AM', Nome: 'Amazonas' },
        { Id: 4, Sigla: 'AP', Nome: 'Amapá' },
        { Id: 5, Sigla: 'BA', Nome: 'Bahia' },
        { Id: 6, Sigla: 'CE', Nome: 'Ceará' },
        { Id: 7, Sigla: 'DF', Nome: 'Distrito Federal' },
        { Id: 8, Sigla: 'ES', Nome: 'Espírito Santo' },
        { Id: 9, Sigla: 'GO', Nome: 'Goiás' },
        { Id: 10, Sigla: 'MA', Nome: 'Maranhão' },
        { Id: 11, Sigla: 'MG', Nome: 'Minas Gerais' },
        { Id: 12, Sigla: 'MS', Nome: 'Mato Grosso do Sul' },
        { Id: 13, Sigla: 'MT', Nome: 'Mato Grosso' },
        { Id: 14, Sigla: 'PA', Nome: 'Pará' },
        { Id: 15, Sigla: 'PB', Nome: 'Paraíba' },
        { Id: 16, Sigla: 'PE', Nome: 'Pernambuco' },
        { Id: 17, Sigla: 'PI', Nome: 'Piauí' },
        { Id: 18, Sigla: 'PR', Nome: 'Paraná' },
        { Id: 19, Sigla: 'RJ', Nome: 'Rio de Janeiro' },
        { Id: 20, Sigla: 'RN', Nome: 'Rio Grande do Norte' },
        { Id: 21, Sigla: 'RO', Nome: 'Rondônia' },
        { Id: 22, Sigla: 'RR', Nome: 'Roraima' },
        { Id: 23, Sigla: 'RS', Nome: 'Rio Grande do Sul' },
        { Id: 24, Sigla: 'SC', Nome: 'Santa Catarina' },
        { Id: 25, Sigla: 'SE', Nome: 'Sergipe' },
        { Id: 26, Sigla: 'SP', Nome: 'São Paulo' },
        { Id: 27, Sigla: 'TO', Nome: 'Tocantins' }
    ];

    var grausInstrucao = [
        { id: 1, nome: 'Analfabeto' },
        { id: 2, nome: 'Primário Incompleto(até quarta série)' },
        { id: 3, nome: 'Primário Completo(quarta série completa)' },
        { id: 4, nome: 'Primeiro Grau(ginásio) incompleto' },
        { id: 5, nome: 'Primeiro Grau(ginásio) completo' },
        { id: 6, nome: 'Segundo Grau(colegial) incompleto' },
        { id: 7, nome: 'Segundo Grau(colegial) completo' },
        { id: 8, nome: 'Superior incompleto' },
        { id: 9, nome: 'Superior completo' },
        { id: 10, nome: 'Mestrado completo' },
        { id: 11, nome: 'Doutorado completo' },
        { id: 12, nome: 'Pós graduação / Especialização completo' },
        { id: 13, nome: 'Pós Doutorado completo' }
    ];

    var racas = [
        { id: 1, nome: 'Amarela' },
        { id: 2, nome: 'Branca' },
        { id: 3, nome: 'Indígena' },
        { id: 4, nome: 'Parda' },
        { id: 5, nome: 'Negra' },
        { id: 6, nome: 'Não Informado' }
    ];

    DadosGerais.Bancos = bancos;
    DadosGerais.Estados = estados;
    DadosGerais.GrausInstrucao = grausInstrucao;
    DadosGerais.Racas = racas;
}

(function ()
{
    'use strict';

    angular.module('ipefae', ["angucomplete-alt", 'ngAnimate', 'ngRoute', 'ui.bootstrap', 'ui.utils.masks', 'ui.validate']);

    angular.module('ipefae').controller('ipefaeController', ipefaeController);
    ipefaeController.$inject = ['$scope', '$rootScope'];

    function ipefaeController($scope, $rootScope)
    {
        function ativar()
        {
            carregarDadosGerais();
        }

        $rootScope.hasError = function (e, d)
        {
            if (angular.isDefined(e))
                return e && d;

            return false;
        };

        $rootScope.hasError2 = function (element, errorName)
        {
            return (!angular.equals({}, element.$error) && element.$error[errorName] && element.$dirty);
        };

        $rootScope.hasErrorMultiple = function (errors, d)
        {
            if (errors === undefined)
                return { hasError: false, message: '' };

            for (var i = 0; i <= errors.length - 1; i++)
            {
                var e = errors[i].error;
                var message = errors[i].message;

                if (angular.isDefined(e) && e && d)
                {
                    return { hasError: true, message: message };
                }
            }

            return { hasError: false, message: '' };
        };

        ativar();
    }
})();

