$(document).ready(function () {

    $('#original_version').on('click', function (event) {
        event.preventDefault();
        $.removeCookie("spec_version");
        location.reload();
    });

    //Кнопка наверх 
    $(window).scroll(function () {
        if ($(this).scrollTop() > 0) {
            $("#scroller").fadeIn();
        } else {
            $("#scroller").fadeOut();
        }
    });
    $("#scroller").click(function () {
        $("body, html").animate({ scrollTop: 0 }, 400);
        return false;
    });

    // определение размера шрифта из инфы в куки
    var FontSize = $.cookie('FontSize');
    if (FontSize == undefined) FontSize = "average";
    var ImgStyle = $.cookie('ImgStyle');
    if (ImgStyle == undefined) ImgStyle = "f01";
    var BgStyle = $.cookie('BgStyle');
    if (BgStyle == undefined) BgStyle = "cl01";
    if ((FontSize != undefined && FontSize != "") || (ImgStyle != undefined && ImgStyle != "") || (BgStyle != undefined && BgStyle != "")) {
        $("#Cecutient").hide();
        $("#Cecutient").next().show();
        usingCookie();
    }

    //Использование параметров существующих в куках
    function usingCookie() {
        switch (FontSize) {
            case 'small': {
                var $SizeClass = $('.decreaseFont');
                $(".decreaseFont").addClass("current");
                $(".resetFont").removeClass("current");
                $(".increaseFont").removeClass("current");
                FontSizeSmall($SizeClass);
            }; break;
            case 'average': {
                var $SizeClass = $('.resetFont');
                $(".decreaseFont").removeClass("current");
                $(".resetFont").addClass("current");
                $(".increaseFont").removeClass("current");
                FontSizeAverage($SizeClass);
            }; break;
            case 'big': {
                var $SizeClass = $('.increaseFont');
                $(".decreaseFont").removeClass("current");
                $(".resetFont").removeClass("current");
                $(".increaseFont").addClass("current");
                FontSizeBig($SizeClass);
            }; break;
        };

        switch (BgStyle) {
            case "cl01": {
                $(".cl01").addClass('current').siblings().removeClass('current');
                $('body').addClass('color01');
                $('body').removeClass('color02');
                $('body').removeClass('color03');
                $.cookie('BgStyle', "cl01");
            }; break;
            case "cl02": {
                $(".cl02").addClass('current').siblings().removeClass('current');
                $('body').addClass('color02');
                $('body').removeClass('color01');
                $('body').removeClass('color03');
                $.cookie('BgStyle', "cl02");
            }; break;
            case "cl03": {
                $(".cl03").addClass('current').siblings().removeClass('current');
                $('body').addClass('color03');
                $('body').removeClass('color01');
                $.cookie('BgStyle', "cl03");
            }; break;
        };

        switch (ImgStyle) {
            case "f01": {
                $(".f01").addClass('current').siblings().removeClass('current');
                $('body').addClass('foto01');
                $('body').removeClass('foto02');
                $('body').removeClass('foto03');
            }; break;
            case "f02": {
                $(".f02").addClass('current').siblings().removeClass('current');
                $('body').addClass('foto02');
                $('body').removeClass('foto01');
                $('body').removeClass('foto03');
            }; break;
            case "f03": {
                $(".f03").addClass('current').siblings().removeClass('current');
                $('body').addClass('foto03');
                $('body').removeClass('foto01');
                $('body').removeClass('foto02');
            }; break;
        };
    }
    //вернуться к нормальной версии
    $(".btn_orginal").click(function () {
        $.cookie('FontSize', "");
        $.cookie('ImgStyle', "");
        $.cookie('BgStyle', "");
        location.reload();
    });

    $(".decreaseFont").click(function (e) {
        e.preventDefault;
        $.cookie('FontSize', "small");
        $(this).addClass("current");
        $(".resetFont").removeClass("current");
        $(".increaseFont").removeClass("current");
        FontSizeSmall($(this));
        return false;
    });

    $('.resetFont').click(function (e) {
        e.PreventDefault;
        $.cookie('FontSize', "average");
        $(this).addClass("current");
        $(".decreaseFont").removeClass("current");
        $(".increaseFont").removeClass("current");
        FontSizeAverage($(this));
    });

    $(".increaseFont").click(function (e) {
        e.PreventDefault;
        $.cookie('FontSize', "big");
        $(this).addClass("current");
        $(".decreaseFont").removeClass("current");
        $(".resetFont").removeClass("current");
        FontSizeBig($(this));
    });

    function FontSizeSmall($elem) {
        var currentFontSize = $('body').css('font-size');
        var currentFontSizeNum = parseFloat(currentFontSize, 10);
        var newFontSize = '14pt';
        $('.special_block').css('font-size', newFontSize);
        $('.special_block *').css('font-size', newFontSize);
        $(this).parent().find('current').removeClass('current');
        $(this).addClass('current');

    }

    function FontSizeAverage($elem) {
        var newFontSize = '16pt';
        $('.special_block').css('font-size', newFontSize);
        $('.special_block *').css('font-size', newFontSize);
        $(this).parent().find('current').removeClass('current');
        $(this).addClass('current');
    }

    function FontSizeBig($elem) {
        var currentFontSize = $('body').css('font-size');
        var currentFontSizeNum = parseFloat(currentFontSize, 10);
        var newFontSize = '20pt';
        $('.special_block').css('font-size', newFontSize);
        $('.special_block *').css('font-size', newFontSize);
        $(this).parent().find('current').removeClass('current');
        $(this).addClass('current');
    }

    $(".f01").click(function () {
        $(this).addClass('current').siblings().removeClass('current');
        $('body').addClass('foto01');
        $('body').removeClass('foto02');
        $('body').removeClass('foto03');
        $.cookie('ImgStyle', "f01");
        return false;
    });

    $(".f02").click(function () {
        $(this).addClass('current').siblings().removeClass('current');
        $('body').addClass('foto02');
        $('body').removeClass('foto01');
        $('body').removeClass('foto03');
        $.cookie('ImgStyle', "f02");
        return false;
    });

    $(".f03").click(function () {
        $(this).addClass('current').siblings().removeClass('current');
        $('body').addClass('foto03');
        $('body').removeClass('foto01');
        $('body').removeClass('foto02');
        $.cookie('ImgStyle', "f03");
        return false;
    });

    $(".cl01").click(function () {
        $(this).addClass('current').siblings().removeClass('current');
        $('body').addClass('color01');
        $('body').removeClass('color02');
        $('body').removeClass('color03');
        $.cookie('BgStyle', "cl01");
        return false;
    });

    $(".cl02").click(function () {
        $(this).addClass('current').siblings().removeClass('current');
        $('body').addClass('color02');
        $('body').removeClass('color01');
        $('body').removeClass('color03');
        $.cookie('BgStyle', "cl02");
        return false;
    });

    $(".cl03").click(function () {
        $(this).addClass('current').siblings().removeClass('current');
        $('body').addClass('color03');
        $('body').removeClass('color01');
        $('body').removeClass('color02');
        $.cookie('BgStyle', "cl03");
        return false;
    });

    $("#Cecutient").click(function () {
        $(this).next().slideToggle();
        $(this).addClass("optiopen");
        FontSizeBig($(".increaseFont"));
        $.cookie('FontSize', "big");
        $(".cl01").addClass('current').siblings().removeClass('current');
        $('body').addClass('color01');
        $('body').removeClass('color02');
        $('body').removeClass('color03');
        $.cookie('BgStyle', "cl01");
    });
});

function Coords(x, y, title, desc, zoom, height) {
    ymaps.ready(function () {
        if (title == '') { title = "Название организации"; }
        if (desc == '') { desc = "Описание организации"; }

        var ContactMap = new ymaps.Map("map", { center: [y, x], zoom: zoom });
        ContactMap.controls.add('zoomControl', { top: 5 });

        myPlacemark = new ymaps.Placemark([y, x], {
            balloonContentHeader: title,
            balloonContentBody: desc,
            hintContent: title
        }, { hasBalloon: false });

        ContactMap.geoObjects.add(myPlacemark);
        $('ymaps.ymaps-map').css({ 'height': height + 'px', "width": "inherit" });
    });
}

