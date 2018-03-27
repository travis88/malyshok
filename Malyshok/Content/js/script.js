$(window).on('load scroll', function () {
    var nowScroll = $(window).scrollTop();

    if (nowScroll > 165) {
        $('.scroll-menu').css('display', 'block');        
    }
    else {
        $('.scroll-menu').css('display', 'none');
    }
});

$(document).ready(function () {

    var SummerSt = $(".validation-summary-valid").find("li").attr("style")
    if (SummerSt == "display:none") $(".validation-summary-valid").hide();

    // Переключатель на форме регистрации
    if ($('form input#UserType').is(":checked")) {
        $('input#Organization')
            .attr('required', 'required')
            .closest('.form-group').toggle();
    }
    else {
        $('input#Organization').removeAttr('required');
    }
    $('form input[type=checkbox]#UserType').bind({
        change: function () {
            $('input#Organization').closest('.form-group').toggle();
            if ($(this).is(":checked")) {
                $('input#Organization').attr('required', 'required');
            }
            else {
                $('input#Organization').removeAttr('required');
            }
        }
    });

    // Переключатель на форме заказа
    if ($('form input#Delivery').is(":checked")) {
        $('input#Address')
            .attr('required', 'required')
            .closest('.form-group').toggle();
    }
    else {
        $('input#Address').removeAttr('required');
    }
    $('form input[type=checkbox]#Delivery').bind({
        change: function () {
            $('input#Address').closest('.form-group').toggle();
            if ($(this).is(":checked")) {
                $('input#Address').attr('required', 'required');
            }
            else {
                $('input#Address').removeAttr('required');
            }
        }
    });
    

    // Проверка уникальности E-Mail
    $('input#Mail').bind({
        change: function () {
            var $obj = $('.check-mail').empty();

            $.ajax({
                type: "POST",
                async: false,
                url: "/user/CheckMail/"+$(this).val(),
                error: function () { $obj.append("ошибка в запросе на проверку уникальности E-Mail"); },
                success: function (data) {
                    var _result = data.Result;
                    $obj.append(_result);
                }
            });
        }
    });


    // Кнопка "Добавить в корзину"
    $('.in-basket').bind({
        click: function () {
            var $Btn = $(this);
            var id = $(this).closest('.basket-form').find('input').attr('data-id');
            var count = $(this).closest('.basket-form').find('input').val();
            count = (count > 0) ? count : 1;
            $Btn.closest('.basket-form').find('input')
                .val(count)
                .attr('data-count', count)
                .removeClass('chenge-input');

            //$(this).closest('.basket-form').find('input').focus();
            //$(this).closest('.item_prod').next().find('input').focus();

            $.ajax({
                type: "POST",
                async: false,
                url: '/basket/add/' + id + '?count=' + count,
                error: function () {
                },
                success: function (data) {
                    $Btn.animate({ backgroundColor: "#ffffff" }, 600, function () {
                            $(this).removeClass('btn-blue')
                                .addClass('btn-invers')
                                .removeAttr('style')
                                .empty()
                                .append('В корзине');
                        });

                    changeBasketInfo(data.Count, data.Cost);
                }
            });
        }
    });
    // Кнопка "Удалить из корзину"
    $('.del-basket').bind({
        click: function () {
            var $block = $(this).closest('.basket_item');
            var id = $block.find('input').attr('data-id');

            $.ajax({
                type: "POST",
                async: false,
                url: '/basket/delete/' + id + '/',
                error: function () {
                },
                success: function (data) {
                    $block.slideToggle(800).remove();

                    changeBasketInfo(data.Count, data.Cost);
                }
            });
        }
    });
    // поле "Количество" в разделах "Продукция" и "Корзина"
    $('.basket-form input, .basket_item-counter input').bind({
        focus: function () { 
            var _css = $(this).attr('css')
            if (_css != 'chenge-input')
                $(this).attr('data-count', $(this).val());
        },
        keydown: function (e) {
            //alert(e.keyCode);
            if ((e.keyCode > 47 & e.keyCode < 59) || (e.keyCode > 95 & e.keyCode < 106) || e.keyCode === 37 || e.keyCode === 39 || e.keyCode === 8 || e.keyCode === 46) {
                // Цифры, стрелки "Вправо" и "Влево", Кнопки "Удалить"
            }
            else if (e.keyCode === 13 && $(this).val() >= 0) {
                // Кнопка "Enter"
                var _ID = $(this).attr('data-id');
                var _Value = $(this).val();
                var $Btn = $(this).closest('.basket-form')
                    .find('.in-basket')
                    .removeClass('btn-invers')
                    .addClass('btn-blue');

                $(this).attr('data-count', _Value).removeClass('chenge-input');

                $.ajax({
                    type: "POST",
                    async: false,
                    url: '/basket/add/' + _ID + '?count=' + _Value,
                    error: function () {
                    },
                    success: function (data) {
                        changeBasketInfo(data.Count, data.Cost);

                        $Btn.animate({ backgroundColor: "#ffffff", color:"#786de3" }, 600, function () {
                            $(this).removeClass('btn-blue')
                                .addClass('btn-invers')
                                .removeAttr('style')
                                .empty()
                                .append('В корзине')
                                .focus();
                        });
                    }
                });
            }
            else {
                return false;
            }
        },
        keyup: function (e) {
            if ((e.keyCode > 47 & e.keyCode < 59) || (e.keyCode > 95 & e.keyCode < 106) || e.keyCode === 8 || e.keyCode === 46) {
                // Цифры
                var _nowVal = $(this).val();
                var _oldVal = $(this).attr('data-count');

                if (_nowVal != _oldVal)
                    $(this).addClass('chenge-input');
                else
                    $(this).removeClass('chenge-input');
            } 
        }
    });

    // Распределение каталога продукции по колонкам
    var CatalogLength = $('.catalog-item').length;
    var Ost = CatalogLength % 3;
    var RowLength = (Ost != 0) ? Math.floor(CatalogLength / 3) + 1 : Math.floor(CatalogLength / 3);
    
    var $Left = $("<div/>", { "class": "col-left" });
    $Left.append($('.catalog-item').slice(0, RowLength).clone());

    var $Center = $("<div/>", { "class": "col-center" });
    $Center.append($('.catalog-item').slice(RowLength, RowLength * 2).clone());

    var $Right = $("<div/>", { "class": "col-right" });
    $Right.append($('.catalog-item').slice(RowLength * 2, CatalogLength).clone());

    $('.catalog-block .row').empty().append($Left).append($Right).append($Center);

    //original photo
    if ($('.show_original,.swipebox').length > 0) {
        $('.show_original').swipebox();
        $(".swipebox").swipebox();
    }
});

function changeBasketInfo(_count, _cost) {
    if (_count > 0) {
        var _countTitle = "";
        var _remainder = _count.substring(_count.length - 1);

        if (_remainder == "1" && _count != 11) _countTitle += " товар";
        else if ((_remainder == "2" || _remainder == "3" || _remainder == "4") && _count != 12 && _count != 13 && _count != 14) _countTitle += " товара";
        else _countTitle += " товаров";

        $('.basket-counter').empty()
            .append('<div class="goods"><span>' + _count + '</span> ' + _countTitle + '</div>')
            .append(' <div class="cost"><span>' + _cost + '</span> руб.</div>');

        $('.menu-basket-info div')
            .empty()
            .append(_count + ' шт./' + _cost + ' р.');
    }
    else {
        $('.basket-counter').empty()
            .append('<div class="basket-empty">Корзина пуста</div>')

        $('.menu-basket-info div')
            .empty()
            .append('Корзина пуста');

        $('.order-form').before('Корзина пуста').remove();
    }
}