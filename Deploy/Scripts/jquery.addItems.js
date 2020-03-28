(function ($) {
    $.fn.addItems = function (options) {
        var data = options.data;
        var valueName = options.valueName ? options.valueName : "Value";
        var textName = options.textName ? options.textName : "Text";
        var childVal = options.childVal;

        return this.each(function () {
            var list = this;

            $.each(data, function (index, itemData) {
                var text = eval("itemData." + textName);
                var value = eval("itemData." + valueName);
                var option = new Option(text, value);

                if (childVal != undefined && childVal == value)
                    option.selected = true;

                list.add(option);
            });
        });
    };

    $.fn.addItemsReturnOptions = function (options) {
        var data = options.data;
        var valueName = options.valueName ? options.valueName : "Value";
        var textName = options.textName ? options.textName : "Text";
        var childVal = options.childVal;
        var retorno = [];

        this.each(function () {
            $.each(data, function (index, itemData) {
                var text = eval("itemData." + textName);
                var value = eval("itemData." + valueName);

                //var selected = (childVal != undefined && childVal == value) ? " selected" : "";
                //var optValue = "<option value='" + text + "' " + selected + " id='" + value + "'></option>";
                var optValue = { "value": text, "id": value, "label": text };

                retorno.push(optValue);
            });
        });

        return retorno;
    };
})(jQuery);