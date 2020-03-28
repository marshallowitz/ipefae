(function ($) {
    $.fn.cascade = function (options) {
        var defaults = {};
        var opts = $.extend(defaults, options);

        return this.each(function () { $(this).change(function () { $(this).changeCombo(opts); }); });
    };

    $.fn.changeCombo = function (opts) {
        var url = opts.url ? opts.url : '/' + opts.controllerMethod + '/' + opts.listMethod;
        var valueName = opts.valueName ? opts.valueName : "Value";
        var textName = opts.textName ? opts.textName : "Text";
        var appendEmpty = opts.appendEmpty ? opts.appendEmpty : false;
        var emptyText = opts.emptyText ? opts.emptyText : '';
        var dadosMinimo = opts.dadosMinimo ? opts.dadosMinimo : 0;
        var params = (opts.params != undefined ? opts.params : {});
        var async = (opts.async != undefined ? opts.async : true);
        params[opts.paramName] = $(this).val() == null ? opts.childVal : $(this).val();

        if (opts.carregando)
            opts.childSelect.append($('<option/>').attr('value', '0').text(opts.carregando));

        $.ajax({
            type: "POST",
            url: url,
            data: params,
            async: async,
            success: function (items) {
                opts.childSelect.empty();
                var data = items.data ? items.data : items;

                if (data.length > dadosMinimo) {
                    opts.childSelect.removeAttr('disabled');

                    if (appendEmpty)
                        opts.childSelect.append($('<option/>').attr('value', '0').text(''));
                }
                else {
                    opts.childSelect.attr('disabled', 'disabled');
                    data[0].Text = params[opts.paramName] == "0" ? emptyText : data[0].Text;
                }

                if (data.length > 0)
                    opts.childSelect.addItems({ data: data, valueName: valueName, textName: textName, childVal: opts.childVal });

                if (opts.func)
                    opts.func();
            },
            error: function (req, status, error) {
                alert(error);
            }
        });
    }
})(jQuery);