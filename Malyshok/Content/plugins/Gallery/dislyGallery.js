/*! ========================================================================
 * DislyGallery: dislyGallery.js
  * ========================================================================
 * Copyright 2018 it-serv, D-Boriskin.
 * ======================================================================== */
+function ($) {

    // ucFile PUBLIC CLASS DEFINITION
    // ==============================

    var Gallery = function (element, options) {
        this.$element = $(element)
        this.options = $.extend({}, this.defaults(), options)
        this.render()
    }

    Gallery.DEFAULTS = {
        title: null,
        link: null
    }

    Gallery.prototype.defaults = function () {
        return {
            title: this.$element.attr('title') || Gallery.DEFAULTS.title,
            link: this.$element.attr('data-link') || Gallery.DEFAULTS.link
        }
    }

    Gallery.prototype.render = function () {
        this.$element.wrap('<div class="form-group">');

        if (this.options.title) {
            var $toggleTitle = $('<label for="' + this.$element.attr('id') + '">').html(this.options.title + ':');
            this.$element.before($toggleTitle);
        }

        if (this.options.type == 'date' || this.options.type == 'datetime' || this.options.help) {
            this.$element.wrap('<div class="input-group"></div>');
        }

        if (this.options.help) {
            this.$element.after('<div class="input-group-addon"><div class="icon-help-circled" data-toggle="popover" data-placement="auto bottom" data-content="' + this.options.help + '"></div></div>');
            this.$element.next().find('div').popover();
        }


        if (this.options.type == 'date' || this.options.type == 'datetime') {
            var $InputTime = $('<input style="width:70px;" class="form-control" placeholder="00:00">');
            var timeValh = this.$element.attr('value').replace(/(\d+).(\d+).(\d+) (\d+):(\d+):(\d+)/, '$4');
            var timeValm = this.$element.attr('value').replace(/(\d+).(\d+).(\d+) (\d+):(\d+):(\d+)/, '$5');

            if (timeValh.length < 2) {
                timeValh = "0" + timeValh;
            }
            if (timeValm.length < 2) {
                timeValm = "0" + timeValm;
            }

            $InputTime.attr('value', timeValh + ":" + timeValm);
            $InputTime.mask('Hh:Mm', {
                'translation': {
                    H: { pattern: /[0-2]/ },
                    h: { pattern: /[0-9]/ },
                    M: { pattern: /[0-5]/ },
                    m: { pattern: /[0-9]/ }
                },
                'placeholder:': '00:00'
            });

            //$InputTime.attr('data-mask', '99:99');

            var $InputDate = $('<input data-type="date" class="form-control" value="">');
            $InputDate.attr('value', this.$element.attr('value').replace(/(\d+).(\d+).(\d+) (\d+:\d+:\d+)/, '$1.$2.$3'));
            $InputDate.attr('data-mask', '99.99.9999');


            if (this.$element.attr('required') == 'required') {
                $InputDate.attr('required', 'required');
            }


            this.$element.hide();
            this.$element.after($InputTime);
            this.$element.after($InputDate);

            if (this.options.type == 'date') {
                $InputTime.hide();
            }
            else {
                $InputTime.wrap('<div class="input-group-addon"><div></div></div>');
            }
            var $TargetInput = this.$element;

            $InputDate.keyup(function () {
                SpotDate();
            });
            $InputTime.keyup(function () {
                SpotDate();
            });
            $InputDate.datepicker()
                .on("input change", function (e) {
                    SpotDate();
                });
            function SpotDate() {
                var time = $InputTime.val();
                var Length = time.length;
                switch (Length) {
                    case (5): time += ':00'; break;
                    case (4): time += '0:00'; break;
                    case (3): time += '00:00'; break;
                    case (2): time += ':00:00'; break;
                    case (1): time += '0:00:00'; break;
                    default: time += '00:00:00'; break;
                }
                $TargetInput.attr('value', $InputDate.val() + ' ' + time);
            }
        }
        this.$element.addClass('form-control');
    }

    //Gallery.prototype.destroy = function () {
    //    this.$element.removeData('bs.Gallery')
    //    this.$element.unwrap()
    //}

    // Gallery PLUGIN DEFINITION
    // ========================

    function Plugin(option) {
        return this.each(function () {
            var $this = $(this)
            var data = $this.data('bs.Gallery')
            var options = typeof option == 'object' && option

            if (!data) $this.data('bs.Gallery', (data = new Gallery(this, options)))
            if (typeof option == 'string' && data[option]) data[option]()
        })
    }

    var old = $.fn.GalleryInit

    $.fn.GalleryInit = Plugin
    $.fn.GalleryInit.Constructor = Gallery

    // TOGGLE NO CONFLICT
    // ==================

    $.fn.toggle.noConflict = function () {
        $.fn.GalleryInit = old
        return this
    }

    // DislyFile DATA-API
    // ==================

    $(function () {
        $('.disly-gallery').GalleryInit()
    })

}(jQuery);
