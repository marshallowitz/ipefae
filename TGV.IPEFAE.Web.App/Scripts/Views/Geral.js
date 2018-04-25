var ExibirAlerta = false;

Date.prototype.addDays = function (days)
{
    var dat = new Date(this.valueOf());
    dat.setDate(dat.getDate() + days);
    return dat;
}

var accentMap = {
    "á": "a", "â": "a", "ã": "a",
    "é": "e", "ê": "e",
    "í": "i",
    "ó": "o", "ô": "o", "õ": "o",
    "ú": "u", "ü": "u",
    "ç": "c"
};

function detectarBrowse()
{
    // Opera 8.0+
    var isOpera = (!!window.opr && !!opr.addons) || !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0;

    if (isOpera)
        return "Opera";

    // Firefox 1.0+
    var isFirefox = typeof InstallTrigger !== 'undefined';

    if (isFirefox)
        return "Firefox";

    // Safari 3.0+ "[object HTMLElementConstructor]" 
    var isSafari = /constructor/i.test(window.HTMLElement) || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || (typeof safari !== 'undefined' && safari.pushNotification));

    if (isSafari)
        return "Safari";

    // Internet Explorer 6-11
    var isIE = /*@cc_on!@*/false || !!document.documentMode;

    if (isIE)
        return "IE";

    // Edge 20+
    var isEdge = !isIE && !!window.StyleMedia;

    if (isEdge)
        return "Edge";

    // Chrome 1+
    var isChrome = !!window.chrome && !!window.chrome.webstore;

    if (isChrome)
        return "Chrome";

    // Blink engine detection
    var isBlink = (isChrome || isOpera) && !!window.CSS;

    if (isBlink)
        return "Blink";

    return "Não Detectado";
}

function firstLetterCapitalized(str)
{
    str = str.toLowerCase().replace(/\b[a-z]/g, function (letter)
    {
        return letter.toUpperCase();
    });

    return str;
}

function IsCEP(strCEP, blnVazio)
{
    // Caso o CEP não esteja nesse formato ele é inválido!
    var objER = /^[0-9]{2}[0-9]{3}-[0-9]{3}$/;

    strCEP = $.trim(strCEP)
    if (strCEP.length > 0)
    {
        if (objER.test(strCEP))
            return true;
        else
            return false;
    }
    else
        return blnVazio;
}

function calcularIdade(dateString)
{ // dateString FORMAT: dd/MM/yyyy
    var year = dateString.substring(6, 10);
    var month = dateString.substring(3, 5);
    var day = dateString.substring(0, 2);
    var today = new Date();
    var birthDate = new Date(year + '/' + month + '/' + day);
    var age = today.getFullYear() - birthDate.getFullYear();
    var m = today.getMonth() - birthDate.getMonth();

    if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate()))
    {
        age--;
    }

    return age;
}

function checkInput(type) {
    var input = document.createElement("input");
    input.setAttribute("type", type);
    return input.type == type;
}

function createCookie(name, value, days) {
    var expires;

    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toGMTString();
    } else {
        expires = "";
    }
    document.cookie = escape(name) + "=" + escape(value) + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = escape(name) + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return unescape(c.substring(nameEQ.length, c.length));
    }
    return null;
}

function encodeConversion(texto, tipoEntrada, tipoSaida) {
    var chr = ['À', 'Á', 'Â', 'Ã', 'Ä', 'Å', 'Æ', 'Ç', 'È', 'É', 'Ê', 'Ë', 'Ì', 'Í', 'Î', 'Ï', 'Ð', 'Ñ', 'Ò', 'Ó', 'Ô', 'Õ', 'Ö', 'Ø', 'Ù', 'Ú', 'Û', 'Ü', 'Ý', 'Þ', 'ß', 'à', 'á', 'â', 'ã', 'ä', 'å', 'æ', 'ç', 'è', 'é', 'ê', 'ë', 'ì', 'í', 'î', 'ï', 'ð', 'ñ', 'ò', 'ó', 'ô', 'õ', 'ö', 'ø', 'ù', 'ú', 'û', 'ü', 'ý', 'þ', 'ÿ', 'Œ', 'œ', 'Š', 'š', 'Ÿ', 'ƒ', '"'];

    var hex = ['\xC0', '\xC1', '\xC2', '\xC3', '\xC4', '\xC5', '\xC6', '\xC7', '\xC8', '\xC9', '\xCA', '\xCB', '\xCC', '\xCD', '\xCE', '\xCF', '\xD0', '\xD1', '\xD2', '\xD3', '\xD4', '\xD5', '\xD6', '\xD8', '\xD9', '\xDA', '\xDB', '\xDC', '\xDD', '\xDE', '\xDF', '\xE0', '\xE1', '\xE2', '\xE3', '\xE4', '\xE5', '\xE6', '\xE7', '\xE8', '\xE9', '\xEA', '\xEB', '\xEC', '\xED', '\xEE', '\xEF', '\xF0', '\xF1', '\xF2', '\xF3', '\xF4', '\xF5', '\xF6', '\xF8', '\xF9', '\xFA', '\xFB', '\xFC', '\xFD', '\xFE', '\xFF', '\u0152', '\u0153', '\u0160', '\u0161', '\u0178', '\u0192', '\u0022'];

    var num = ['&#192;', '&#193;', '&#194;', '&#195;', '&#196;', '&#197;', '&#198;', '&#199;', '&#200;', '&#201;', '&#202;', '&#203;', '&#204;', '&#205;', '&#206;', '&#207;', '&#208;', '&#209;', '&#210;', '&#211;', '&#212;', '&#213;', '&#214;', '&#216;', '&#217;', '&#218;', '&#219;', '&#220;', '&#221;', '&#222;', '&#223;', '&#224;', '&#225;', '&#226;', '&#227;', '&#228;', '&#229;', '&#230;', '&#231;', '&#232;', '&#233;', '&#234;', '&#235;', '&#236;', '&#237;', '&#238;', '&#239;', '&#240;', '&#241;', '&#242;', '&#243;', '&#244;', '&#245;', '&#246;', '&#248;', '&#249;', '&#250;', '&#251;', '&#252;', '&#253;', '&#254;', '&#255;', '&#338;', '&#339;', '&#352;', '&#353;', '&#376;', '&#402;', '&quot;'];

    var html = ['&Agrave;', '&Aacute;', '&Acirc;', '&Atilde;', '&Auml;', '&Aring;', '&AElig;', '&Ccedil;', '&Egrave;', '&Eacute;', '&Ecirc;', '&Euml;', '&Igrave;', '&Iacute;', '&Icirc;', '&Iuml;', '&ETH;', '&Ntilde;', '&Ograve;', '&Oacute;', '&Ocirc;', '&Otilde;', '&Ouml;', '&Oslash;', '&Ugrave;', '&Uacute;', '&Ucirc;', '&Uuml;', '&Yacute;', '&THORN;', '&szlig;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&ouml;', '&oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&uuml;', '&yacute;', '&thorn;', '&yuml;', '&OElig;', '&oelig;', '&Scaron;', '&scaron;', '&Yuml;', '&fnof;', '&quot;'];

    var esc = ['%C0', '%C1', '%C2', '%C3', '%C4', '%C5', '%C6', '%C7', '%C8', '%C9', '%CA', '%CB', '%CC', '%CD', '%CE', '%CF', '%D0', '%D1', '%D2', '%D3', '%D4', '%D5', '%D6', '%D8', '%D9', '%DA', '%DB', '%DC', '%DD', '%DE', '%DF', '%E0', '%E1', '%E2', '%E3', '%E4', '%E5', '%E6', '%E7', '%E8', '%E9', '%EA', '%EB', '%EC', '%ED', '%EE', '%EF', '%F0', '%F1', '%F2', '%F3', '%F4', '%F5', '%F6', '%F8', '%F9', '%FA', '%FB', '%FC', '%FD', '%FE', '%FF', '%u0152', '%u0153', '%u0160', '%u0161', '%u0178', '%u0192', '%u0022'];

    var uri = ['%C3%80', '%C3%81', '%C3%82', '%C3%83', '%C3%84', '%C3%85', '%C3%86', '%C3%87', '%C3%88', '%C3%89', '%C3%8A', '%C3%8B', '%C3%8C', '%C3%8D', '%C3%8E', '%C3%8F', '%C3%90', '%C3%91', '%C3%92', '%C3%93', '%C3%94', '%C3%95', '%C3%96', '%C3%98', '%C3%99', '%C3%9A', '%C3%9B', '%C3%9C', '%C3%9D', '%C3%9E', '%C3%9F', '%C3%A0', '%C3%A1', '%C3%A2', '%C3%A3', '%C3%A4', '%C3%A5', '%C3%A6', '%C3%A7', '%C3%A8', '%C3%A9', '%C3%AA', '%C3%AB', '%C3%AC', '%C3%AD', '%C3%AE', '%C3%AF', '%C3%B0', '%C3%B1', '%C3%B2', '%C3%B3', '%C3%B4', '%C3%B5', '%C3%B6', '%C3%B8', '%C3%B9', '%C3%BA', '%C3%BB', '%C3%BC', '%C3%BD', '%C3%BE', '%C3%BF', '%C5%92', '%C5%93', '%C5%A0', '%C5%A1', '%C5%B8', '%C6%92', '%22'];

    var arrEntrada = eval(tipoEntrada);
    var arrSaida = eval(tipoSaida);

    if (arrEntrada === undefined || arrSaida === undefined)
        return;

    var retorno = '';
    var retornoParcial = '';

    for (var i = 0; i <= texto.length - 1; i++) {
        var letra = texto.charAt(i);
        var indice = retornoParcial.length;

        retornoParcial += letra;

        var ret = encodeConversionCharExist(arrEntrada, retornoParcial);

        // verifica se a letra está no array de entrada
        if (ret == retornoParcial && i < texto.length - 1)
            continue;
        else if (ret == '') {
            retorno += retornoParcial;
            retornoParcial = '';
        }
        else {
            var ultimoChr = retornoParcial.substring(retornoParcial.length - 1);
            var ret2 = encodeConversionCharExist(arrEntrada, ultimoChr);

            var indiceRet = arrEntrada.indexOf(ret);

            if (ret2 == '') {
                if (i == texto.length - 1)
                    ultimoChr = '';

                retorno += arrSaida[indiceRet] + ultimoChr;
                retornoParcial = '';
            }
            else {
                retorno += arrSaida[indiceRet];
                retornoParcial = ultimoChr;
            }
        }
    }

    return retorno;
}

function encodeConversionCharExist(arr, texto) {
    if (texto === '')
        return texto;

    for (var i = 0; i <= arr.length - 1; i++) {
        if (arr[i].indexOf(texto) == 0)
            return texto;
    }

    return encodeConversionCharExist(arr, texto.substring(0, texto.length - 1));
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}

function logout() {
    var url = homePage + 'Account/LogOff';

    $.ajax({
        type: 'POST',
        url: url,
        success: function (logoff) {
            eraseCookie("userCoookie");
        },
        error: function (xhr, ajaxOptions, thrownError) { alertaErroJS({ NomeFuncao: 'logout()', ResponseText: xhr.responseText }); }
    });
}

var normalize = function (term) {
    var ret = "";

    for (var i = 0; i < term.length; i++)
        ret += accentMap[term.charAt(i)] || term.charAt(i);

    return ret;
}

function alertaErroJS(options) {
    var err = options.ResponseText;
    //var err = eval("(" + options.ResponseText + ")");
    //var err = JSON.parse(options.ResponseText);

    if (ExibirAlerta)
        alert("function " + options.NomeFuncao);

    var url = homePage + 'Base/LogarErroClient';

    $.ajax({
        type: 'POST',
        url: url,
        data: { clientMethod: options.NomeFuncao, mensagem: htmlEncode(err) },
        success: function (envio) { },
        error: function (xhr, ajaxOptions, thrownError) { }
    });
}

function ajustarCelularSP(element) {
    var phone;
    element.unmask();
    phone = element.val().replace(/\D/g, '');

    if (phone.length > 10)
        element.mask('(00) 00000-0000');
    else
        element.mask('(00) 0000-0000');
}

function findInArray(array, key, value)
{
    if (!array)
        return null;

    for (var i = 0, len = array.length; i < len; i++) {
        if (eval('array[' + i + '].' + key) === value)
            return array[i];
    }

    return null;
}

function findInArrayIndex(array, key, value)
{
    if (!array)
        return null;

    for (var i = 0, len = array.length; i < len; i++) {
        if (Array.isArray(key)) {
            if (findInArrayValidateMultiple(array[i], key, value))
                return i;
        }
        else if (eval('array[' + i + '].' + key) === value)
            return i;
    }

    return null;
}

function focusIn(ctl) {
    ctl.mask('(00) 00000-0000');
}

function formatDateToDDMMYYYY(data)
{
    var dd = data.getDate();
    var mm = data.getMonth() + 1;
    var yyyy = data.getFullYear();

    if (dd < 10)
        dd = '0' + dd

    if (mm < 10)
        mm = '0' + mm

    var data = dd + '/' + mm + '/' + yyyy;

    return data;
}

function getDate(dateString) {
    var parts = dateString.split("/");
    var day = parseInt(parts[0], 10);
    var month = parseInt(parts[1], 10);
    var year = parseInt(parts[2], 10);

    return new Date(year, month, day);
}

function getHomePage(url) {
    var homePageIni = url;

    if (homePageIni.indexOf('Home/Index/') >= 0)
        homePageIni = homePageIni.split('Home/Index/')[0];

    return homePageIni;
}

function getIeVersion() {
    var ie = (function () {
        var undef,
            v = 3,
            div = document.createElement('div'),
            all = div.getElementsByTagName('i');

        while (div.innerHTML = '<!--[if gt IE ' + (++v) + ']><i></i><![endif]-->', all[0])
        { }

        return v > 4 ? v : undef;
    }());

    return ie;
}

function getParametros(url, code) {
    code = code.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + code + "=([^&#]*)"),
        results = regex.exec(url);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function get_time_difference(earlierDate, laterDate) {
    var nTotalDiff = laterDate.getTime() - earlierDate.getTime();
    var oDiff = new Object();

    oDiff.days = Math.floor(nTotalDiff / 1000 / 60 / 60 / 24);
    nTotalDiff -= oDiff.days * 1000 * 60 * 60 * 24;

    oDiff.hours = Math.floor(nTotalDiff / 1000 / 60 / 60);
    nTotalDiff -= oDiff.hours * 1000 * 60 * 60;

    oDiff.minutes = Math.floor(nTotalDiff / 1000 / 60);
    nTotalDiff -= oDiff.minutes * 1000 * 60;

    oDiff.seconds = Math.floor(nTotalDiff / 1000);

    return oDiff;

}

function getTodayDate() {
    var hoje = new Date();
    var hojeSimplificado = hoje.toISOString().split("T")[0].split("-");
    return getDate(hojeSimplificado[2] + "/" + hojeSimplificado[1] + "/" + hojeSimplificado[0]);
}

function getTodayDateDDMMYYYY() {
    return formatDateToDDMMYYYY(new Date());
}

function htmlDecode(value) { return $('<div/>').html(value).text(); }

function htmlEncode(value) { return $('<div/>').text(value).html(); }

function isFunction(name) {
    return eval('(typeof ' + name + '==\'function\');');
}

function isInteger(valor) {
    validChar = '0123456789';
    for (var i = 0; i < valor.length; i++)
        if (validChar.indexOf(valor.substr(i, 1)) < 0)
            return false;
    return true;
}

function isNumeric(valor) {
    validChar = '0123456789.,-';
    for (var i = 0; i < valor.length; i++)
        if (validChar.indexOf(valor.substr(i, 1)) < 0)
            return false;
    return true;
}

function isValidDate(dateString) {
    // First check for the pattern
    if (!/^\d{2}\/\d{2}\/\d{4}$/.test(dateString) && !/^\d{4}\-\d{2}\-\d{2}$/.test(dateString))
        return false;

    if (dateString.indexOf('-') < 0) {
        // Parse the date parts to integers
        var parts = dateString.split("/");
        var day = parseInt(parts[0], 10);
        var month = parseInt(parts[1], 10);
        var year = parseInt(parts[2], 10);
    }
    else {
        // Parse the date parts to integers
        var parts = dateString.split("-");
        var day = parseInt(parts[2], 10);
        var month = parseInt(parts[1], 10);
        var year = parseInt(parts[0], 10);
    }

    // Check the ranges of month and year
    if (year < 1000 || year > 3000 || month == 0 || month > 12)
        return false;

    var monthLength = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

    // Adjust for leap years
    if (year % 400 == 0 || (year % 100 != 0 && year % 4 == 0))
        monthLength[1] = 29;

    // Check the range of the day
    return day > 0 && day <= monthLength[month - 1];
};

function isValidEmailAddress(emailAddress) {
    var pattern = new RegExp(/^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i);
    return pattern.test(emailAddress);
};

// HH:MM
function isValidTime(timeString) {
    return (/^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$/.test(timeString))
}

function maxLengthCheck(object) {
    if (object.value.length > object.maxLength)
        object.value = object.value.slice(0, object.maxLength);
}

function padLeft(texto, totalWidth, paddingChar) {
    if (totalWidth < texto.length)
        return texto;
    else
        return Array(totalWidth - texto.length + 1).join(paddingChar || " ") + texto;
}

function removerIconeValidator(e, data) {
    var $parent = data.element.parents('.form-group'),
        $icon = $parent.find('.form-control-feedback[data-bv-icon-for="' + data.field + '"]');

    $icon.on('click.clearing', function () {
        // Check if the field is valid or not via the icon class
        if ($icon.hasClass('glyphicon-remove')) {
            // Clear the field
            data.bv.resetField(data.element);
            data.element.focus();
        }
    });
}

function removerTodosCaracteresMenosNumeros(texto) { return texto.replace(/[^\w\s]/gi, '').replace(/[a-zA-Z ]/g, ''); }

function resizePage(ctl, diferenca) { ctl.height($(window).height() - diferenca); }

function setCaretPosition(elemId, caretPos) {
    var elem = document.getElementById(elemId);

    if (elem != null) {
        if (elem.createTextRange) {
            var range = elem.createTextRange();
            range.move('character', caretPos);
            range.select();
        }
        else {
            if (elem.selectionStart) {
                elem.focus();
                elem.setSelectionRange(caretPos, caretPos);
            }
            else
                elem.focus();
        }
    }
}

function sleep(delay) {
    var start = new Date().getTime();
    while (new Date().getTime() < start + delay) { }
}

function somenteData(e, txt) {
    // Allow only backspace and delete and tab and left and right
    if (e.keyCode == 46 || e.keyCode == 8 || e.keyCode == 9 || e.keyCode == 37 || e.keyCode == 39) { }
    else {
        // Ensure that it is a number and stop the keypress
        if ((e.keyCode >= 48 && e.keyCode <= 57) || (e.keyCode >= 96 && e.keyCode <= 105)) {
            var digitado = String.fromCharCode(e.which);
            var dt = txt.val();
            var retorno;
            var da = dt.replace('/', '').replace('/', '').split('');

            if (da.length == 0 && digitado > 3) { e.preventDefault(); return; }
            if (da.length == 1 && (da[0] == 3 && digitado > 1)) { e.preventDefault(); return; }
            if (da.length == 2 && digitado > 1) { e.preventDefault(); return; }
            if (da.length == 3) {
                if (da[2] == 1 && digitado > 2) { e.preventDefault(); return; }

                if (da[2] == 0) {
                    switch (digitado) {
                        case 4:
                        case 6:
                        case 9:
                        case 11:
                            if (parseInt(da[0] + da[1]) > 30) { e.preventDefault(); return; }
                            break;
                        case 2:
                            if (parseInt(da[0] + da[1]) > 29) { e.preventDefault(); return; }
                            break;
                    }
                }
            }

            if (da.length == 4 && digitado != 2) { e.preventDefault(); return; }
            if (da.length == 5 && digitado != 0) { e.preventDefault(); return; }
            if (da.length == 6 && digitado != 1) { e.preventDefault(); return; }

            if (da.length == 7) {
                var dia = parseInt(da[0] + da[1]);
                var mes = parseInt(da[2] + da[3]);
                var ano = parseInt(da[4] + da[5] + da[6] + da[7]);
                if (dia > 28 && mes == 2 && (ano == 2012 || ano == 2016)) { e.preventDefault(); return; } // ano bissexto
            }

            if (dt.length == 2 || dt.length == 5) dt += '/';

            dt += digitado;
            e.preventDefault();
            txt.val(dt);
        }
        else { e.preventDefault(); }
    }
}

function somenteNumeros(e) {
    var theEvent = e || window.event;
    var key = theEvent.keyCode || theEvent.which;
    key = String.fromCharCode(key);
    var regex = /[0-9]|\./;
    if (!regex.test(key)) {
        theEvent.returnValue = false;
        if (theEvent.preventDefault) theEvent.preventDefault();
    }
}

function somenteLetrasNumerosUnderHifen(e) {
    var digitado = String.fromCharCode(e.which);

    if (!(/^[a-z0-9_-]$/.test(digitado)))
        e.preventDefault();
}

function SortByName(a, b) {
    var aName = a.name == undefined ? a : a.name.toLowerCase();
    var bName = b.name == undefined ? b : b.name.toLowerCase();
    return ((aName < bName) ? -1 : ((aName > bName) ? 1 : 0));
}

function terminouDownload(url) {
    $.ajax(
    {
        type: "POST",
        url: url,
        success: function (result) {
            if (result)
                $.unblockUI();
            else
                setTimeout(function () { terminouDownload(url); }, 2000);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alertaErroJS({ NomeFuncao: 'terminouDownload()', ResponseText: xhr.responseText });
            $.unblockUI();
        }
    });
}

function validarCpf(cpf) {
    cpf = cpf.replace('.', '').replace('.', '').replace('-', '');
    var digitos_iguais = 1;

    if (cpf.length < 11)
        return false;

    for (var i = 0; i < cpf.length - 1; i++) {
        if (cpf.charAt(i) != cpf.charAt(i + 1)) {
            digitos_iguais = 0;
            break;
        }
    }

    if (!digitos_iguais) {
        var numeros = cpf.substring(0, 9);
        var digitos = cpf.substring(9);
        var soma = 0;

        for (var i = 10; i > 1; i--)
            soma += numeros.charAt(10 - i) * i;

        var resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;

        if (resultado != digitos.charAt(0))
            return false;

        numeros = cpf.substring(0, 10);
        soma = 0;

        for (i = 11; i > 1; i--)
            soma += numeros.charAt(11 - i) * i;

        resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;

        return (resultado == digitos.charAt(1));
    }

    return false;
}

function validarCNPJ(cnpj) {
    var numeros, digitos, soma, i, resultado, pos, tamanho, digitos_iguais;
    if (cnpj.length == 0) {
        return false;
    }

    cnpj = cnpj.replace(/\D+/g, '');
    digitos_iguais = 1;

    for (i = 0; i < cnpj.length - 1; i++)
        if (cnpj.charAt(i) != cnpj.charAt(i + 1)) {
            digitos_iguais = 0;
            break;
        }
    if (digitos_iguais)
        return false;

    tamanho = cnpj.length - 2;
    numeros = cnpj.substring(0, tamanho);
    digitos = cnpj.substring(tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2)
            pos = 9;
    }
    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado != digitos.charAt(0)) {
        return false;
    }
    tamanho = tamanho + 1;
    numeros = cnpj.substring(0, tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2)
            pos = 9;
    }

    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;

    return (resultado == digitos.charAt(1));
}

function validarExtensaoImagem(filename) {
    // Use a regular expression to trim everything before final dot
    var extension = filename.replace(/^.*\./, '');

    // Iff there is no dot anywhere in filename, we would have extension == filename,
    // so we account for this possibility now
    if (extension == filename)
        extension = '';
    else
        // if there is an extension, we convert to lower case
        // (N.B. this conversion will not effect the value of the extension
        // on the file upload.)
        extension = extension.toLowerCase();

    switch (extension) {
        case 'jpg':
        case 'jpeg':
        case 'png':
            return true;
        default:
            return false;
    }
}

jQuery.fn.extend({
    slideLeft: function (show, speed, callback) {
        if (show)
            $(this).show('slide', { direction: 'left' }, speed);
        else
            $(this).hide('slide', { direction: 'left' }, speed);

        if (typeof callback == 'function') {
            callback();
        }
        return false;
    },
    slideRight: function (show, speed, callback) {
        if (show)
            $(this).show('slide', { direction: 'right' }, speed);
        else
            $(this).hide('slide', { direction: 'right' }, speed);

        if (typeof callback == 'function') {
            callback();
        }

        return;
    }
});

Date.prototype.dateAdd = function (size, value) {
    value = parseInt(value);
    var incr = 0;
    switch (size) {
        case 'day':
            incr = value * 24;
            this.dateAdd('hour', incr);
            break;
        case 'hour':
            incr = value * 60;
            this.dateAdd('minute', incr);
            break;
        case 'week':
            incr = value * 7;
            this.dateAdd('day', incr);
            break;
        case 'minute':
            incr = value * 60;
            this.dateAdd('second', incr);
            break;
        case 'second':
            incr = value * 1000;
            this.dateAdd('millisecond', incr);
            break;
        case 'month':
            value = value + this.getUTCMonth();
            if (value / 12 > 0) {
                this.dateAdd('year', value / 12);
                value = value % 12;
            }
            this.setUTCMonth(value);
            break;
        case 'millisecond':
            this.setTime(this.getTime() + value);
            break;
        case 'year':
            this.setFullYear(this.getUTCFullYear() + value);
            break;
        default:
            throw new Error('Invalid date increment passed');
            break;
    }
}