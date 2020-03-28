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
        { id: 11, codigo: '318', nome: 'Banco BMG S.A', ordem: 1 },
        { id: 4, codigo: '237', nome: 'Banco Bradesco S.A.', ordem: 2 },
        { id: 5, codigo: '745', nome: 'Banco Citibank S.A.', ordem: 3 },
        { id: 13, codigo: '756', nome: 'Banco Cooperativo do Brasil S.A. (Bancoob - Sicoob)', ordem: 4 },
        { id: 12, codigo: '748', nome: 'Banco Cooperativo Sicred', ordem: 5 },
        { id: 1, codigo: '001', nome: 'Banco do Brasil S.A.', ordem: 6 },
        { id: 2, codigo: '341', nome: 'Banco Itaú S.A.', ordem: 7 },
        { id: 8, codigo: '389', nome: 'Banco Mercantil do Brasil S.A.', ordem: 8 },
        { id: 9, codigo: '453', nome: 'Banco Rural S.A.', ordem: 9 },
        { id: 10, codigo: '422', nome: 'Banco Safra S.A.', ordem: 10 },
        { id: 3, codigo: '033', nome: 'Banco Santander (Brasil) S.A.', ordem: 11 },
        { id: 7, codigo: '104', nome: 'Caixa Econômica Federal', ordem: 12 },
        { id: 6, codigo: '399', nome: 'HSBC Bank Brasil S.A.', ordem: 13 },
        { id: 14, codigo: '260', nome: 'Nubank', ordem: 14 },
        { id: 1000, codigo: '1000', nome: '------------', ordem: 15 },
        { id: 15, codigo: '356', nome: 'Abn Amro S.A', ordem: 15 },
        { id: 16, codigo: '332', nome: 'Acesso Soluções De Pagamento S.A.', ordem: 16 },
        { id: 17, codigo: '654', nome: 'Banco A. J. Renner S.A', ordem: 17 },
        { id: 18, codigo: '246', nome: 'Banco Abc Roma S.A', ordem: 18 },
        { id: 19, codigo: '222', nome: 'Banco Agf Brasil S. A.', ordem: 19 },
        { id: 20, codigo: '483', nome: 'Banco Agrimisa S.A.', ordem: 20 },
        { id: 21, codigo: '217', nome: 'Banco Agroinvest S.A', ordem: 21 },
        { id: 22, codigo: '025', nome: 'Banco Alfa S/A', ordem: 22 },
        { id: 23, codigo: '215', nome: 'Banco America Do Sul S.A', ordem: 23 },
        { id: 24, codigo: '621', nome: 'Banco Aplicap S.A.', ordem: 24 },
        { id: 25, codigo: '625', nome: 'Banco Araucaria S.A', ordem: 25 },
        { id: 26, codigo: '213', nome: 'Banco Arbi S.A', ordem: 26 },
        { id: 27, codigo: '648', nome: 'Banco Atlantis S.A', ordem: 27 },
        { id: 28, codigo: '201', nome: 'Banco Augusta Industria E Comercial S.A', ordem: 28 },
        { id: 29, codigo: '394', nome: 'Banco B.M.C. S.A', ordem: 29 },
        { id: 30, codigo: '116', nome: 'Banco B.N.L Do Brasil S.A', ordem: 30 },
        { id: 31, codigo: '239', nome: 'Banco Bancred S.A', ordem: 31 },
        { id: 32, codigo: '230', nome: 'Banco Bandeirantes S.A', ordem: 32 },
        { id: 33, codigo: '752', nome: 'Banco Banque Nationale De Paris Brasil S', ordem: 33 },
        { id: 34, codigo: '107', nome: 'Banco Bbm S.A', ordem: 34 },
        { id: 35, codigo: '267', nome: 'Banco Bbm-Com.C.Imob.Cfi S.A.', ordem: 35 },
        { id: 36, codigo: '231', nome: 'Banco Boavista S.A', ordem: 36 },
        { id: 37, codigo: '262', nome: 'Banco Boreal S.A', ordem: 37 },
        { id: 38, codigo: '351', nome: 'Banco Bozano Simonsen S.A', ordem: 38 },
        { id: 39, codigo: '225', nome: 'Banco Brascan S.A', ordem: 39 },
        { id: 40, codigo: '282', nome: 'Banco Brasileiro Comercial', ordem: 40 },
        { id: 41, codigo: '501', nome: 'Banco Brasileiro Iraquiano S.A.', ordem: 41 },
        { id: 42, codigo: '168', nome: 'Banco C.C.F. Brasil S.A', ordem: 42 },
        { id: 43, codigo: '336', nome: 'Banco C6 S.A.', ordem: 43 },
        { id: 44, codigo: '263', nome: 'Banco Cacique', ordem: 44 },
        { id: 45, codigo: '236', nome: 'Banco Cambial S.A', ordem: 45 },
        { id: 46, codigo: '266', nome: 'Banco Cedula S.A', ordem: 46 },
        { id: 47, codigo: '002', nome: 'Banco Central Do Brasil', ordem: 47 },
        { id: 48, codigo: '244', nome: 'Banco Cidade S.A', ordem: 48 },
        { id: 49, codigo: '713', nome: 'Banco Cindam S.A', ordem: 49 },
        { id: 50, codigo: '241', nome: 'Banco Classico S.A', ordem: 50 },
        { id: 51, codigo: '308', nome: 'Banco Comercial Bancesa S.A.', ordem: 51 },
        { id: 52, codigo: '730', nome: 'Banco Comercial Paraguayo S.A', ordem: 52 },
        { id: 53, codigo: '753', nome: 'Banco Comercial Uruguai S.A.', ordem: 53 },
        { id: 54, codigo: '109', nome: 'Banco Credibanco S.A', ordem: 54 },
        { id: 55, codigo: '721', nome: 'Banco Credibel S.A', ordem: 55 },
        { id: 56, codigo: '295', nome: 'Banco Crediplan S.A', ordem: 56 },
        { id: 57, codigo: '220', nome: 'Banco Crefisul', ordem: 57 },
        { id: 58, codigo: '628', nome: 'Banco Criterium S.A', ordem: 58 },
        { id: 59, codigo: '229', nome: 'Banco Cruzeiro S.A', ordem: 59 },
        { id: 60, codigo: '003', nome: 'Banco Da Amazonia S.A', ordem: 60 },
        { id: 61, codigo: '707', nome: 'Banco Daycoval S.A', ordem: 61 },
        { id: 62, codigo: '479', nome: 'Banco De Boston S.A', ordem: 62 },
        { id: 63, codigo: '219', nome: 'Banco De Credito De Sao Paulo S.A', ordem: 63 },
        { id: 64, codigo: '640', nome: 'Banco De Credito Metropolitano S.A', ordem: 64 },
        { id: 65, codigo: '291', nome: 'Banco De Credito Nacional S.A', ordem: 65 },
        { id: 66, codigo: '240', nome: 'Banco De Credito Real De Minas Gerais S.', ordem: 66 },
        { id: 67, codigo: '022', nome: 'Banco De Credito Real De Minas Gerais Sa', ordem: 67 },
        { id: 68, codigo: '300', nome: 'Banco De La Nacion Argentina S.A', ordem: 68 },
        { id: 69, codigo: '627', nome: 'Banco Destak S.A', ordem: 69 },
        { id: 70, codigo: '214', nome: 'Banco Dibens S.A', ordem: 70 },
        { id: 71, codigo: '649', nome: 'Banco Dimensao S.A', ordem: 71 },
        { id: 72, codigo: '034', nome: 'Banco Do Esado Do Amazonas S.A', ordem: 72 },
        { id: 73, codigo: '028', nome: 'Banco Do Estado Da Bahia S.A', ordem: 73 },
        { id: 74, codigo: '030', nome: 'Banco Do Estado Da Paraiba S.A', ordem: 74 },
        { id: 75, codigo: '020', nome: 'Banco Do Estado De Alagoas S.A', ordem: 75 },
        { id: 76, codigo: '031', nome: 'Banco Do Estado De Goias S.A', ordem: 76 },
        { id: 77, codigo: '048', nome: 'Banco Do Estado De Minas Gerais S.A', ordem: 77 },
        { id: 78, codigo: '024', nome: 'Banco Do Estado De Pernambuco', ordem: 78 },
        { id: 79, codigo: '059', nome: 'Banco Do Estado De Rondonia S.A', ordem: 79 },
        { id: 80, codigo: '645', nome: 'Banco Do Estado De Roraima S.A', ordem: 80 },
        { id: 81, codigo: '027', nome: 'Banco Do Estado De Santa Catarina S.A', ordem: 81 },
        { id: 82, codigo: '047', nome: 'Banco Do Estado De Sergipe S.A', ordem: 82 },
        { id: 83, codigo: '026', nome: 'Banco Do Estado Do Acre S.A', ordem: 83 },
        { id: 84, codigo: '635', nome: 'Banco Do Estado Do Amapa S.A', ordem: 84 },
        { id: 85, codigo: '035', nome: 'Banco Do Estado Do Ceara S.A', ordem: 85 },
        { id: 86, codigo: '021', nome: 'Banco Do Estado Do Espirito Santo S.A', ordem: 86 },
        { id: 87, codigo: '036', nome: 'Banco Do Estado Do Maranhao S.A', ordem: 87 },
        { id: 88, codigo: '032', nome: 'Banco Do Estado Do Mato Grosso S.A.', ordem: 88 },
        { id: 89, codigo: '037', nome: 'Banco Do Estado Do Para S.A', ordem: 89 },
        { id: 90, codigo: '038', nome: 'Banco Do Estado Do Parana S.A', ordem: 90 },
        { id: 91, codigo: '039', nome: 'Banco Do Estado Do Piaui S.A', ordem: 91 },
        { id: 92, codigo: '029', nome: 'Banco Do Estado Do Rio De Janeiro S.A', ordem: 92 },
        { id: 93, codigo: '041', nome: 'Banco Do Estado Do Rio Grande Do Sul S.A', ordem: 93 },
        { id: 94, codigo: '004', nome: 'Banco Do Nordeste Do Brasil S.A', ordem: 94 },
        { id: 95, codigo: '302', nome: 'Banco Do Progresso S.A', ordem: 95 },
        { id: 96, codigo: '622', nome: 'Banco Dracma S.A', ordem: 96 },
        { id: 97, codigo: '743', nome: 'Banco Emblema S.A', ordem: 97 },
        { id: 98, codigo: '245', nome: 'Banco Empresarial S.A', ordem: 98 },
        { id: 99, codigo: '242', nome: 'Banco Euroinvest S.A', ordem: 99 },
        { id: 100, codigo: '641', nome: 'Banco Excel Economico S/A', ordem: 100 },
        { id: 101, codigo: '496', nome: 'Banco Exterior De Espana S.A', ordem: 101 },
        { id: 102, codigo: '265', nome: 'Banco Fator S.A', ordem: 102 },
        { id: 103, codigo: '375', nome: 'Banco Fenicia S.A', ordem: 103 },
        { id: 104, codigo: '224', nome: 'Banco Fibra S.A', ordem: 104 },
        { id: 105, codigo: '626', nome: 'Banco Ficsa S.A', ordem: 105 },
        { id: 106, codigo: '725', nome: 'Banco Finabanco S.A', ordem: 106 },
        { id: 107, codigo: '199', nome: 'Banco Financial Portugues', ordem: 107 },
        { id: 108, codigo: '473', nome: 'Banco Financial Portugues S.A', ordem: 108 },
        { id: 109, codigo: '252', nome: 'Banco Fininvest S.A', ordem: 109 },
        { id: 110, codigo: '728', nome: 'Banco Fital S.A', ordem: 110 },
        { id: 111, codigo: '729', nome: 'Banco Fonte S.A', ordem: 111 },
        { id: 112, codigo: '434', nome: 'Banco Fortaleza S.A', ordem: 112 },
        { id: 113, codigo: '346', nome: 'Banco Frances E Brasileiro S.A', ordem: 113 },
        { id: 114, codigo: '652', nome: 'Banco Frances E Brasileiro Sa', ordem: 114 },
        { id: 115, codigo: '200', nome: 'Banco Fricrisa Axelrud S.A', ordem: 115 },
        { id: 116, codigo: '505', nome: 'Banco Garantia S.A', ordem: 116 },
        { id: 117, codigo: '624', nome: 'Banco General Motors S.A', ordem: 117 },
        { id: 118, codigo: '353', nome: 'Banco Geral Do Comercio S.A', ordem: 118 },
        { id: 119, codigo: '734', nome: 'Banco Gerdau S.A', ordem: 119 },
        { id: 120, codigo: '731', nome: 'Banco Gnpp S.A.', ordem: 120 },
        { id: 121, codigo: '221', nome: 'Banco Graphus S.A', ordem: 121 },
        { id: 122, codigo: '612', nome: 'Banco Guanabara S.A', ordem: 122 },
        { id: 123, codigo: '256', nome: 'Banco Gulvinvest S.A', ordem: 123 },
        { id: 124, codigo: '303', nome: 'Banco Hnf S.A.', ordem: 124 },
        { id: 125, codigo: '228', nome: 'Banco Icatu S.A', ordem: 125 },
        { id: 126, codigo: '258', nome: 'Banco Induscred S.A', ordem: 126 },
        { id: 127, codigo: '604', nome: 'Banco Industrial Do Brasil S.A', ordem: 127 },
        { id: 128, codigo: '320', nome: 'Banco Industrial E Comercial', ordem: 128 },
        { id: 129, codigo: '653', nome: 'Banco Indusval S.A', ordem: 129 },
        { id: 130, codigo: '166', nome: 'Banco Inter-Atlantico S.A', ordem: 130 },
        { id: 131, codigo: '630', nome: 'Banco Intercap S.A', ordem: 131 },
        { id: 132, codigo: '722', nome: 'Banco Interior De Sao Paulo S.A', ordem: 132 },
        { id: 133, codigo: '077', nome: 'Banco Intermedium', ordem: 133 },
        { id: 134, codigo: '616', nome: 'Banco Interpacifico S.A', ordem: 134 },
        { id: 135, codigo: '232', nome: 'Banco Interpart S.A', ordem: 135 },
        { id: 136, codigo: '223', nome: 'Banco Interunion S.A', ordem: 136 },
        { id: 137, codigo: '705', nome: 'Banco Investcorp S.A.', ordem: 137 },
        { id: 138, codigo: '249', nome: 'Banco Investcred S.A', ordem: 138 },
        { id: 139, codigo: '617', nome: 'Banco Investor S.A.', ordem: 139 },
        { id: 140, codigo: '499', nome: 'Banco Iochpe S.A', ordem: 140 },
        { id: 141, codigo: '106', nome: 'Banco Itabanco S.A.', ordem: 141 },
        { id: 142, codigo: '372', nome: 'Banco Itamarati S.A', ordem: 142 },
        { id: 143, codigo: '488', nome: 'Banco J. P. Morgan S.A', ordem: 143 },
        { id: 144, codigo: '757', nome: 'Banco Keb Do Brasil S.A.', ordem: 144 },
        { id: 145, codigo: '495', nome: 'Banco La Provincia De Buenos Aires', ordem: 145 },
        { id: 146, codigo: '494', nome: 'Banco La Rep. Oriental Del Uruguay', ordem: 146 },
        { id: 147, codigo: '234', nome: 'Banco Lavra S.A.', ordem: 147 },
        { id: 148, codigo: '235', nome: 'Banco Liberal S.A', ordem: 148 },
        { id: 149, codigo: '600', nome: 'Banco Luso Brasileiro S.A', ordem: 149 },
        { id: 150, codigo: '233', nome: 'Banco Mappin S.A', ordem: 150 },
        { id: 151, codigo: '647', nome: 'Banco Marka S.A', ordem: 151 },
        { id: 152, codigo: '206', nome: 'Banco Martinelli S.A', ordem: 152 },
        { id: 153, codigo: '656', nome: 'Banco Matrix S.A', ordem: 153 },
        { id: 154, codigo: '243', nome: 'Banco Máxima S.A', ordem: 154 },
        { id: 155, codigo: '720', nome: 'Banco Maxinvest S.A', ordem: 155 },
        { id: 156, codigo: '008', nome: 'Banco Meridional Do Brasil', ordem: 156 },
        { id: 157, codigo: '755', nome: 'Banco Merrill Lynch S.A', ordem: 157 },
        { id: 158, codigo: '466', nome: 'Banco Mitsubishi Brasileiro S.A', ordem: 158 },
        { id: 159, codigo: '504', nome: 'Banco Multiplic S.A', ordem: 159 },
        { id: 160, codigo: '007', nome: 'Banco Nac Desenv. Eco. Social S.A', ordem: 160 },
        { id: 161, codigo: '412', nome: 'Banco Nacional Da Bahia S.A', ordem: 161 },
        { id: 162, codigo: '420', nome: 'Banco Nacional Do Norte S.A', ordem: 162 },
        { id: 163, codigo: '415', nome: 'Banco Nacional S.A', ordem: 163 },
        { id: 164, codigo: '733', nome: 'Banco Nacoes S.A.', ordem: 164 },
        { id: 165, codigo: '735', nome: 'Banco Neon', ordem: 165 },
        { id: 166, codigo: '165', nome: 'Banco Norchem S.A', ordem: 166 },
        { id: 167, codigo: '424', nome: 'Banco Noroeste S.A', ordem: 167 },
        { id: 168, codigo: '247', nome: 'Banco Omega S.A', ordem: 168 },
        { id: 169, codigo: '608', nome: 'Banco Open S.A', ordem: 169 },
        { id: 170, codigo: '718', nome: 'Banco Operador S.A', ordem: 170 },
        { id: 171, codigo: '212', nome: 'Banco Original', ordem: 171 },
        { id: 172, codigo: '208', nome: 'Banco Pactual S.A', ordem: 172 },
        { id: 173, codigo: '623', nome: 'Banco Panamericano S.A', ordem: 173 },
        { id: 174, codigo: '254', nome: 'Banco Parana Banco S.A', ordem: 174 },
        { id: 175, codigo: '602', nome: 'Banco Patente S.A', ordem: 175 },
        { id: 176, codigo: '611', nome: 'Banco Paulista S.A', ordem: 176 },
        { id: 177, codigo: '650', nome: 'Banco Pebb S.A', ordem: 177 },
        { id: 178, codigo: '613', nome: 'Banco Pecunia S.A', ordem: 178 },
        { id: 179, codigo: '264', nome: 'Banco Performance S.A', ordem: 179 },
        { id: 180, codigo: '277', nome: 'Banco Planibanc S.A', ordem: 180 },
        { id: 181, codigo: '304', nome: 'Banco Pontual S.A', ordem: 181 },
        { id: 182, codigo: '658', nome: 'Banco Porto Real S.A', ordem: 182 },
        { id: 183, codigo: '724', nome: 'Banco Porto Seguro S.A', ordem: 183 },
        { id: 184, codigo: '480', nome: 'Banco Portugues Do Atlantico-Brasil S.A', ordem: 184 },
        { id: 185, codigo: '732', nome: 'Banco Premier S.A.', ordem: 185 },
        { id: 186, codigo: '719', nome: 'Banco Primus S.A', ordem: 186 },
        { id: 187, codigo: '638', nome: 'Banco Prosper S.A', ordem: 187 },
        { id: 188, codigo: '275', nome: 'Banco Real S.A', ordem: 188 },
        { id: 189, codigo: '633', nome: 'Banco Redimento S.A', ordem: 189 },
        { id: 190, codigo: '216', nome: 'Banco Regional Malcon S.A', ordem: 190 },
        { id: 191, codigo: '750', nome: 'Banco Republic National Of New York (Bra', ordem: 191 },
        { id: 192, codigo: '204', nome: 'Banco S.R.L S.A', ordem: 192 },
        { id: 193, codigo: '502', nome: 'Banco Santander S.A', ordem: 193 },
        { id: 194, codigo: '607', nome: 'Banco Santos Neves S.A', ordem: 194 },
        { id: 195, codigo: '702', nome: 'Banco Santos S.A', ordem: 195 },
        { id: 196, codigo: '251', nome: 'Banco Sao Jorge S.A.', ordem: 196 },
        { id: 197, codigo: '250', nome: 'Banco Schahin Cury S.A', ordem: 197 },
        { id: 198, codigo: '643', nome: 'Banco Segmento S.A', ordem: 198 },
        { id: 199, codigo: '211', nome: 'Banco Sistema S.A', ordem: 199 },
        { id: 200, codigo: '637', nome: 'Banco Sofisa S.A', ordem: 200 },
        { id: 201, codigo: '366', nome: 'Banco Sogeral S.A', ordem: 201 },
        { id: 202, codigo: '347', nome: 'Banco Sudameris Brasil S.A', ordem: 202 },
        { id: 203, codigo: '205', nome: 'Banco Sul America S.A', ordem: 203 },
        { id: 204, codigo: '464', nome: 'Banco Sumitomo Brasileiro S.A', ordem: 204 },
        { id: 205, codigo: '657', nome: 'Banco Tecnicorp S.A', ordem: 205 },
        { id: 206, codigo: '618', nome: 'Banco Tendencia S.A', ordem: 206 },
        { id: 207, codigo: '456', nome: 'Banco Tokio S.A', ordem: 207 },
        { id: 208, codigo: '634', nome: 'Banco Triangulo S.A', ordem: 208 },
        { id: 209, codigo: '493', nome: 'Banco Union S.A.C.A', ordem: 209 },
        { id: 210, codigo: '736', nome: 'Banco United S.A', ordem: 210 },
        { id: 211, codigo: '726', nome: 'Banco Universal S.A', ordem: 211 },
        { id: 212, codigo: '610', nome: 'Banco V.R. S.A', ordem: 212 },
        { id: 213, codigo: '261', nome: 'Banco Varig S.A', ordem: 213 },
        { id: 214, codigo: '715', nome: 'Banco Vega S.A', ordem: 214 },
        { id: 215, codigo: '711', nome: 'Banco Vetor S.A.', ordem: 215 },
        { id: 216, codigo: '655', nome: 'Banco Votorantim S.A.', ordem: 216 },
        { id: 217, codigo: '629', nome: 'Bancorp Banco Coml. E. De Investmento', ordem: 217 },
        { id: 218, codigo: '489', nome: 'Banesto Banco Urugauay S.A', ordem: 218 },
        { id: 219, codigo: '184', nome: 'Bba - Creditanstalt S.A', ordem: 219 },
        { id: 220, codigo: '740', nome: 'Bcn Barclays', ordem: 220 },
        { id: 221, codigo: '294', nome: 'Bcr - Banco De Credito Real S.A', ordem: 221 },
        { id: 222, codigo: '370', nome: 'Beal - Banco Europeu Para America Latina', ordem: 222 },
        { id: 223, codigo: '601', nome: 'Bfc Banco S.A.', ordem: 223 },
        { id: 224, codigo: '739', nome: 'Bgn', ordem: 224 },
        { id: 225, codigo: '639', nome: 'Big S.A. - Banco Irmaos Guimaraes', ordem: 225 },
        { id: 226, codigo: '070', nome: 'Brb - Banco De Brasília', ordem: 226 },
        { id: 227, codigo: '749', nome: 'Brmsantil Sa', ordem: 227 },
        { id: 228, codigo: '741', nome: 'Brp', ordem: 228 },
        { id: 229, codigo: '218', nome: 'Bs2', ordem: 229 },
        { id: 230, codigo: '151', nome: 'Caixa Economica Do Estado De Sao Paulo', ordem: 230 },
        { id: 231, codigo: '153', nome: 'Caixa Economica Do Estado Do R.G.Sul', ordem: 231 },
        { id: 232, codigo: '498', nome: 'Centro Hispano Banco', ordem: 232 },
        { id: 233, codigo: '376', nome: 'Chase Manhattan Bank S.A', ordem: 233 },
        { id: 234, codigo: '175', nome: 'Continental Banco S.A', ordem: 234 },
        { id: 235, codigo: '085', nome: 'Cooperativa Central De Credito - Ailos', ordem: 235 },
        { id: 236, codigo: '210', nome: 'Deutsch Sudamerikaniche Bank Ag', ordem: 236 },
        { id: 237, codigo: '487', nome: 'Deutsche Bank S.A - Banco Alemao', ordem: 237 },
        { id: 238, codigo: '751', nome: 'Dresdner Bank Lateinamerika-Brasil S/A', ordem: 238 },
        { id: 239, codigo: '742', nome: 'Equatorial', ordem: 239 },
        { id: 240, codigo: '492', nome: 'Internationale Nederlanden Bank N.V.', ordem: 240 },
        { id: 241, codigo: '472', nome: 'Lloyds Bank Plc', ordem: 241 },
        { id: 242, codigo: '738', nome: 'Marada', ordem: 242 },
        { id: 243, codigo: '323', nome: 'Mercadopago.Com Representações Ltda.', ordem: 243 },
        { id: 244, codigo: '255', nome: 'Milbanco S.A.', ordem: 244 },
        { id: 245, codigo: '746', nome: 'Modal S\A', ordem: 245 },
        { id: 246, codigo: '148', nome: 'Multi Banco S.A', ordem: 246 },
        { id: 247, codigo: '325', nome: 'Órama Dtvm S.A.', ordem: 247 },
        { id: 248, codigo: '290', nome: 'Pagseguro Internet S.A.', ordem: 248 },
        { id: 249, codigo: '125', nome: 'Plural S.A. Banco Multiplo', ordem: 249 },
        { id: 250, codigo: '369', nome: 'Pontual', ordem: 250 },
        { id: 251, codigo: '747', nome: 'Raibobank Do Brasil', ordem: 251 },
        { id: 252, codigo: '744', nome: 'The First National Bank Of Boston', ordem: 252 },
        { id: 253, codigo: '737', nome: 'Theca', ordem: 253 },
        { id: 254, codigo: '409', nome: 'Unibanco - Uniao Dos Bancos Brasileiros', ordem: 254 },
        { id: 255, codigo: '102', nome: 'Xp Investimentos', ordem: 255 }
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

