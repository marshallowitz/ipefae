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
    else {
        w.document.title = ".:: IPEFAE ::.";
        try { $(w.document.body).html(boleto); } catch (e) { if (reload) window.location.reload(); }
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