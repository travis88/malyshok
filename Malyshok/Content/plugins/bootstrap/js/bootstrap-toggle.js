/*! ========================================================================
 * Bootstrap Toggle: bootstrap-toggle.js v2.2.0
 * http://www.bootstraptoggle.com
 * ========================================================================
 * Copyright 2014 Min Hur, The New York Times Company
 * Licensed under MIT
 * ======================================================================== */


+function ($) {
    'use strict';

    // TOGGLE PUBLIC CLASS DEFINITION
    // ==============================

    var Toggle = function (element, options) {
        this.$element = $(element)
        this.options = $.extend({}, this.defaults(), options)
        this.render()
    }

    Toggle.VERSION = '2.2.0'

    Toggle.DEFAULTS = {
        title: null,
        on: 'Да',
        off: 'Нет',
        onstyle: 'default',
        offstyle: 'default',
        size: 'small',
        style: '',
        width: null,
        height: null,
        readonly: null,
        help: null
    }

    Toggle.prototype.defaults = function () {
        return {
            title: this.$element.attr('title') || Toggle.DEFAULTS.title,
            on: this.$element.attr('data-on') || Toggle.DEFAULTS.on,
            off: this.$element.attr('data-off') || Toggle.DEFAULTS.off,
            onstyle: this.$element.attr('data-onstyle') || Toggle.DEFAULTS.onstyle,
            offstyle: this.$element.attr('data-offstyle') || Toggle.DEFAULTS.offstyle,
            size: this.$element.attr('data-size') || Toggle.DEFAULTS.size,
            style: this.$element.attr('data-style') || Toggle.DEFAULTS.style,
            width: this.$element.attr('data-width') || Toggle.DEFAULTS.width,
            height: this.$element.attr('data-height') || Toggle.DEFAULTS.height,
            readonly: this.$element.attr('readonly') || Toggle.DEFAULTS.readonly,
            help: this.$element.attr('data-help') || Toggle.DEFAULTS.help,
        }
    }

    Toggle.prototype.render = function () {
        // ----- dislyStyle
        this.$element.wrap('<div class="form-group">');

        if (this.options.title) {
            var $toggleTitle = $('<label for="' + this.$element.attr('id') + '">').html(this.options.title + ':');
            this.$element.before($toggleTitle);
        }
        if (this.options.help) {
            this.$element.wrap('<div class="input-group"></div>');
            this.$element.after('<div class="input-group-addon"><div class="icon-help-circled" data-toggle="popover" data-placement="auto bottom" data-content="' + this.options.help + '"></div></div>');
            this.$element.next().find('div').popover();
        }
        if (this.options.readonly) {
            this.$element.attr('disabled', ' disabled');
        }

        this.$element.wrap('<div class="input-group"></div>');
        // -- // dislyStyle


        this._onstyle = 'btn-' + this.options.onstyle
        this._offstyle = 'btn-' + this.options.offstyle
        var size = this.options.size === 'large' ? 'btn-lg'
			: this.options.size === 'small' ? 'btn-sm'
			: this.options.size === 'mini' ? 'btn-xs'
			: ''
        var $toggleOn = $('<label class="btn">').html(this.options.on)
			.addClass(this._onstyle + ' ' + size)
        var $toggleOff = $('<label class="btn">').html(this.options.off)
			.addClass(this._offstyle + ' ' + size + ' active')
        var $toggleHandle = $('<span class="toggle-handle btn btn-default">')
			.addClass(size)
        var $toggleGroup = $('<div class="toggle-group">')
			.append($toggleOn, $toggleOff, $toggleHandle)
        var $toggle = $('<div class="toggle btn" data-toggle="toggle">')
			.addClass(this.$element.prop('checked') ? this._onstyle : this._offstyle + ' off')
			.addClass(size).addClass(this.options.style)

        this.$element.wrap($toggle)
        $.extend(this, {
            $toggle: this.$element.parent(),
            $toggleOn: $toggleOn,
            $toggleOff: $toggleOff,
            $toggleGroup: $toggleGroup
        })
        this.$toggle.append($toggleGroup)

        var width = this.options.width || Math.max($toggleOn.outerWidth(), $toggleOff.outerWidth()) + ($toggleHandle.outerWidth() / 2)
        var height = this.options.height || Math.max($toggleOn.outerHeight(), $toggleOff.outerHeight())
        $toggleOn.addClass('toggle-on')
        $toggleOff.addClass('toggle-off')
        this.$toggle.css({ width: width, height: height })
        if (this.options.height) {
            $toggleOn.css('line-height', $toggleOn.height() + 'px')
            $toggleOff.css('line-height', $toggleOff.height() + 'px')
        }
        this.update(true)
        this.trigger(true)
    }

    Toggle.prototype.toggle = function () {
        if (this.$element.prop('checked')) this.off()
        else this.on()
    }

    Toggle.prototype.on = function (silent) {
        if (this.$element.prop('disabled')) return false
        this.$toggle.removeClass(this._offstyle + ' off').addClass(this._onstyle)
        this.$element.prop('checked', true)
        if (!silent) this.trigger()
    }

    Toggle.prototype.off = function (silent) {
        if (this.$element.prop('disabled')) return false
        this.$toggle.removeClass(this._onstyle).addClass(this._offstyle + ' off')
        this.$element.prop('checked', false)
        if (!silent) this.trigger()
    }

    Toggle.prototype.enable = function () {
        this.$toggle.removeAttr('disabled')
        this.$element.prop('disabled', false)
    }

    Toggle.prototype.disable = function () {
        this.$toggle.attr('disabled', 'disabled')
        this.$element.prop('disabled', true)
    }

    Toggle.prototype.update = function (silent) {
        if (this.$element.prop('disabled')) this.disable()
        else this.enable()
        if (this.$element.prop('checked')) this.on(silent)
        else this.off(silent)
    }

    Toggle.prototype.trigger = function (silent) {
        this.$element.off('change.bs.toggle')
        if (!silent) this.$element.change()
        this.$element.on('change.bs.toggle', $.proxy(function () {
            this.update()
        }, this))
    }

    Toggle.prototype.destroy = function () {
        this.$element.off('change.bs.toggle')
        this.$toggleGroup.remove()
        this.$element.removeData('bs.toggle')
        this.$element.unwrap()
    }

    // TOGGLE PLUGIN DEFINITION
    // ========================

    function Plugin(option) {
        return this.each(function () {
            var $this = $(this)
            var data = $this.data('bs.toggle')
            var options = typeof option == 'object' && option

            if (!data) $this.data('bs.toggle', (data = new Toggle(this, options)))
            if (typeof option == 'string' && data[option]) data[option]()
        })
    }

    var old = $.fn.bootstrapToggle

    $.fn.bootstrapToggle = Plugin
    $.fn.bootstrapToggle.Constructor = Toggle

    // TOGGLE NO CONFLICT
    // ==================

    $.fn.toggle.noConflict = function () {
        $.fn.bootstrapToggle = old
        return this
    }

    // TOGGLE DATA-API
    // ===============

    $(function () {
        $('input[type=checkbox][data-init!=false]').bootstrapToggle()
    })
    
    $(document).on('click.bs.toggle', 'div[data-toggle^=toggle]', function (e) {
        var $checkbox = $(this).find('input[type=checkbox][data-init!=false]')
        $checkbox.bootstrapToggle('toggle')
        e.preventDefault()
    })

}(jQuery);