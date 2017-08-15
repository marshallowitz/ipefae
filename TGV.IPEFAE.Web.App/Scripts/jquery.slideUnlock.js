jQuery.slideUnlock = {
    build: function (options) {
        var defaults = {
            contentClass: 'sliderContent',
            spanInitialValue: 'slide to unlock',
            spanEndValue: 'unlocked',
            urlSessionCaptcha: 'Home/Captcha'
        };

        if (this.length > 0)
            return jQuery(this).each(function (i) {
                /** Vars **/
                var
                    opts = $.extend(defaults, options),
                    $this = $(this),
                    $sliderContainter = $this.closest("." + opts['contentClass']),
                    //$fimSlide = $sliderContainter.width() - $this.width() - 10,
                    $span = $sliderContainter.find('span');
                var curX;

                $span.html(opts['spanInitialValue']);

                $this.draggable({
                    axis: 'x',
                    containment: 'parent',
                    drag: function (event, ui) {
                        var $fimSlide = $sliderContainter.width() - $this.width() - 10;

                        if (ui.position.left > $fimSlide) {
                            ui.position.left = $fimSlide;
                            execute();
                        } else {
                            // Apparently Safari isn't allowing partial opacity on text with background clip? Not sure.
                            // $("h2 span").css("opacity", 100 - (ui.position.left / 5))
                        }
                    },
                    stop: function (event, ui) {
                        var $fimSlide = $sliderContainter.width() - $this.width() - 10;

                        if (ui.position.left < $fimSlide) {
                            $(this).animate({
                                left: 0
                            })
                        }
                    }
                });

                $this[0].addEventListener('touchmove', function (event) {
                    event.preventDefault();

                    var orig = event;
                    var x = orig.changedTouches[0].pageX - $this.width() - 10;
                    $this.css({ left: x });

                    if (x == 0)
                        return;

                    var $fimSlide = $sliderContainter.width() - $this.width() - 10;

                    if (x > $fimSlide) {
                        $this.css({ left: $fimSlide });
                        execute();
                    }
                }, false);

                $this[0].addEventListener('touchend', function (event) {
                    event.preventDefault();

                    var orig = event;
                    var x = orig.changedTouches[0].pageX - $this.width() - 10;
                    var notCompleted = $sliderContainter.find('input[type=hidden]').val() === '';
                    var $fimSlide = $sliderContainter.width() - $this.width() - 10;

                    if (!notCompleted || x > $fimSlide) {
                        $this.css({ left: $fimSlide });
                        return;
                    }

                    if (notCompleted || x <= 0) {
                        $this.css({ left: 0 });
                        return;
                    }
                }, false);

                function generatePass(nb) {
                    var chars = 'azertyupqsdfghjkmwxcvbn23456789AZERTYUPQSDFGHJKMWXCVBN';
                    var pass = '';
                    for (i = 0; i < nb; i++) {
                        var wpos = Math.round(Math.random() * chars.length);
                        pass += chars.substring(wpos, wpos + 1);
                    }
                    return pass;
                }

                function execute() {
                    $this.draggable('disable').css('cursor', 'default');
                    $span.html(opts['spanEndValue']);
                    $sliderContainter.find('.sliderTooltip').hide();

                    var sessionName = generatePass(7);
                    $sliderContainter.find('input[type=hidden]').val(sessionName);

                    var url = opts['urlSessionCaptcha'];
                    var data = { sessionName: sessionName };

                    $.ajax({
                        type: 'POST',
                        url: url,
                        data: data,
                        success: function () {
                            $this.closest('form').find('input[type=submit]').removeAttr('disabled');
                        },
                        error: function (xhr, ajaxOptions, thrownError) { alertaErroJS({ NomeFuncao: 'captcha()', ResponseText: xhr.responseText }); }
                    });
                }
            });
    },

    restart: function () {
        $(this)
    }
}; jQuery.fn.slideUnlock = jQuery.slideUnlock.build;