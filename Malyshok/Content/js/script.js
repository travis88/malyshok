$(window).on('load scroll', function () {
    var nowScroll = $(window).scrollTop();

    if (nowScroll < 140) {
        $('.scroll-menu').css('display', 'none');        
    }
    else if ($('.scroll-menu').css('display') == 'none') {
        $('.scroll-menu').css('display', 'block');
    }
});

$(document).ready(function () {

    var SummerSt = $(".validation-summary-valid").find("li").attr("style")
    if (SummerSt == "display:none") $(".validation-summary-valid").hide();

    // Переключатель на форме регистрации
    if ($('form input#UserType').is(":checked")) {
        $('input#Organization')
            .removeAttr('required')
            .closest('.form-group').toggle();
    }
    else {
        $('input#Organization').attr('required', 'required');
    }
    $('form input[type=checkbox]#UserType').bind({
        change: function () {
            $('input#Organization').closest('.form-group').toggle();
            if ($(this).is(":checked")) {
                $('input#Organization').removeAttr('required');
            }
            else {
                $('input#Organization').attr('required', 'required');
            }
        }
    });

    // Переключатель на форме заказа
    if ($('form input#Delivery').is(":checked")) {
        $('input#Address')
            .removeAttr('required')
            .closest('.form-group').toggle();
    }
    else {
        $('input#Address').attr('required', 'required');
    }
    $('form input[type=checkbox]#Delivery').bind({
        change: function () {
            $('input#Address').closest('.form-group').toggle();
            if ($(this).is(":checked")) {
                $('input#Address').removeAttr('required');
            }
            else {
                $('input#Address').attr('required', 'required');
            }
        }
    });
    
    // Проверка уникальности E-Mail
    $('input#Email').bind({
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

    // Инициализация поля "Количество продукции" для работы с корзиной
    ProdInput_init()
    // Инициализация кнопок для работы с корзиной
    ProdButtons_init();
    // Инициализация пейджера
    ProdPager_init();
    // Инициализация кнопки "Сертификаты"
    certificatesBtn_init();
    // Инициализация картинок для предпросмотра
    imagePreview_init();
    // Инициализация фотогалереи
    if ($('.show_original,.swipebox').length > 0) {
        $('.show_original').swipebox();
        $(".swipebox").swipebox();
    }
    $('.prod-img .loop').bind({
        click: function () {
            $(this).parent().find('img').trigger('click');
        }
    });


    // Обрабатываем события изменения сортировки и фильтра
    $('.sort-form select').bind({
        change: function () {
            getProdList(location.href.toLowerCase().replace('catalog', 'prod-list').replace('novelties', 'novelties-list'));
        }
    })
    
    // Новинки
    var bodyHeight = ~~(($('.body_block').height() - $('.prod-catalog').height()) / 280);
    if (bodyHeight < 1) bodyHeight = 1;
    $('.body-right div.new-prod-item:lt(' + bodyHeight + ')').css('display', 'block');


    // Распределение каталога продукции по колонкам
    $('.catalog-list').css('display', 'block');
});

// Актуализирует данные о количестве и стоимости товаров в корзине
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

        $('.menu-basket-info div').empty()
            .append(_count + ' шт./' + _cost + ' р.');

        $('.basket-result').empty()
            .append('<h3>У вас в корзине <span>' + _count + ' ' + _countTitle + '</span> на общую сумму <span>' + _cost + ' руб.</span></h3>');

        $('.bottom-clone').remove();

        if (_count > 3) {
            var $BottomResult = $("<div/>", { "class": "basket-result bottom-clone" });
            $BottomResult.append('<h3>У вас в корзине <span>' + _count + ' ' + _countTitle + '</span> на общую сумму <span>' + _cost + ' руб.</span></h3>');

            $('.order-form').before($BottomResult);
        }
    }
    else {
        $('.basket-counter').empty()
            .append('<div class="basket-empty">Корзина пуста</div>')

        $('.menu-basket-info div')
            .empty()
            .append('Корзина пуста');

        $('.order-form').before('Корзина пуста').remove();

        $('.basket-result').remove();
    }
}

// Меняем количество товаров этой позиции
function sendProdCount(_ID, _count) {
    $.ajax({
        type: "POST",
        async: false,
        url: '/basket/add/' + _ID + '?count=' + _count,
        error: function () {
        },
        success: function (data) {
            changeBasketInfo(data.Count, data.Cost);
        }
    });
}

// Отправляет запрос на получение списка продукции
// Передает параметры сортировки
// Получает список продукции
function getProdList(Link) {
    $('.progress').animate({ width: "40%" });
    $('.prod-list').animate({ opacity: "0.2" });
    $('body,html').animate({ scrollTop: $('.sort-form').offset().top - 60 }, 800);

    $.ajax({
        url: Link,
        type: "POST", //метод отправки
        dataType: "html", //формат данных
        data: $('form.sort-form').serialize(),  // Сеарилизуем объект
        success: function (response) { //Данные отправлены успешно
            setTimeout(function () {
                $('.progress').removeAttr('style');
            }, 1200);

            $('.prod-list').empty().append(response).animate({ opacity: "1" });
            $('.progress').animate({ width: "100%" }).removeAttr('style');

            // Инициализация поля "Количество продукции" для работы с корзиной
            ProdInput_init()
            // Инициализация кнопок для работы с корзиной
            ProdButtons_init();
            // Инициализация пейджера
            ProdPager_init();
            // Инициализация кнопки "Сертификаты"
            certificatesBtn_init();
            // Инициализация картинок для предпросмотра
            imagePreview_init();

            try {
                history.pushState(null, null, $('.info-from').attr('href'));
                return;
            } catch (e) { }
            location.hash = '#' + $('.info-from').attr('href');
        },
        error: function (response) { // Данные не отправлены
            setTimeout(function () { $('.progress').removeAttr('style'); }, 1200);
            $('.prod-list').animate({ opacity: "1" });
            $('.progress').css('background', '#dd201e');
        }
    })
}


// Инициализация поля "Количество продукции" для работы с корзиной
function ProdInput_init() {
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
            else if (e.keyCode === 13 && $(this).val() > 0) {
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

                        $Btn.animate({ backgroundColor: "#ffffff", color: "#786de3" }, 600, function () {
                            $(this).removeClass('btn-blue')
                                .addClass('btn-invers')
                                .removeAttr('style')
                                .empty()
                                .append('В корзине')
                                .focus();
                        });

                        $.each($('.chenge-input ~ button'), function () {
                            $(this).trigger('click');
                        })
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
        },
        blur: function () {
            var isBasket = $(this).closest('.basket_item-counter').length;
            if (isBasket > 0) {
                // Кнопка "Enter"
                var _ID = $(this).attr('data-id');
                var _Value = $(this).val();

                $(this).removeClass('chenge-input');

                sendProdCount(_ID, _Value);
            }
        }
    });
}

// Инициализация кнопок для работы с корзиной
function ProdButtons_init() {
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

            $.ajax({
                type: "POST",
                async: false,
                url: '/basket/add/' + id + '?count=' + count,
                error: function () {
                },
                success: function (data) {
                    changeBasketInfo(data.Count, data.Cost);

                    $Btn.animate({ backgroundColor: "#ffffff" }, 600, function () {
                        $(this).removeClass('btn-blue')
                            .addClass('btn-invers')
                            .removeAttr('style')
                            .empty()
                            .append('В корзине');
                    });

                    $.each($('.chenge-input ~ button'), function () {
                        $(this).trigger('click');
                    });
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

    $('.count-more').bind({
        click: function () {
            var $Input = $(this).closest('.basket_item-counter').find('input');
            var _ID = $Input.attr('data-id');
            var _count = parseInt($Input.val()) + 1;

            $Input.val(_count).removeClass('chenge-input');
            sendProdCount(_ID, _count);
        }
    })
    $('.count-less').bind({
        click: function () {
            var $Input = $(this).closest('.basket_item-counter').find('input');
            var _ID = $Input.attr('data-id');
            var _count = parseInt($Input.val()) - 1;

            if (_count > 0) {
                $Input.val(_count).removeClass('chenge-input');
                sendProdCount(_ID, _count);
            }
        }
    })
}

// Инициализация кнопки "Сертификаты"
function certificatesBtn_init() {
    $('.prod-cert').bind({
        click: function () {
            $('.cert-panel').remove();
            var _Id = $(this).parent().find('input').attr('data-id');

            var $Panel = $("<div/>", { "class": "cert-panel" });
            var $PanelBody = $("<div/>", { "class": "cert-panel_body" });
            var $btnClose = $("<div/>", { "class": "btn-close" });
            $btnClose.bind({
                click: function () {
                    $('.cert-panel').remove();
                }
            });
            $PanelBody.append($btnClose);

            $.ajax({
                type: "POST",
                async: false,
                url: '/certificates/' + _Id + '/',
                error: function () {
                    $PanelBody.append('<div>Нет данных о сертификатах</div>');
                },
                success: function (data) {
                    for (var i = 0; i < data.Certificates.length; i = i + 1) {
                        var item = data.Certificates[i];
                        $PanelBody.append('<div class="cert-link" data-link="' + item.value + '">' + item.text + '</div>');
                    }

                    if (data.Certificates.length == 0) {
                        $PanelBody.append('<div>Нет данных о сертификатах</div>');
                    }
                }
            });

            $Panel.append($PanelBody);
            $(this).closest('.item_prod').append($Panel);
        }
    });
}

// Инициализация событий Пейджера на странице "Каталог продукции"
function ProdPager_init() {
    $('.prod-list .pagination a').bind({
        click: function () {
            var _Link = $(this).attr('href');
            if (location.href.indexOf('?') > 0)
                _Link = location.href.substring(0, location.href.indexOf('?')) + _Link;
            else
                _Link = location.href + _Link;

            _Link = _Link.replace('catalog', 'prod-list');

            getProdList(_Link);
            return false;
        }
    });
}

// 
function imagePreview_init() {
    $('.image-gallery').bind({
        click: function () {
            var $Panel = $("<div/>", { "class": "popup-bg" });
            var $GaleryContent = $("<div/>", { "class": "gallery-content" });
            var $Img = $("<img/>", { "class": "gallery-img", "src": $(this).attr('href') });
            $GaleryContent.append($Img);

            $Panel
                .append($GaleryContent)
                .bind({
                    click: function () {
                        $Panel.remove();
                        $GaleryContent.remove();
                    }
                });

            $('body').append($Panel);

            return false;
        }
    });
}