$(document).ready(function () {
    var change = 0;

    $("#ActualBl").find(".modal").modal({ show: true });

    // Если система администрирования загружена в frame открываем её в родительском окне
    if (top != self) { top.location.href = location.href; }
    // Полоса прокрутки главного меню
    $("#MenuBlock").mCustomScrollbar();

    // Панель авторизации пользователя
    $('.AdminInfo').click(function () { $('.AdminSetting').toggle(); });
    // Раскрывает карту сайта
    $('.treeIcon.folder').bind('click', function () { SM_Branch($(this)) });
    

    //Показывает в модальном окне фрейм
    $('a.PopUpFrame').click(function () {
        PopUpFrame($(this));
        return false;
    });

    // Перехват нажатия клавиш клавиатуры
    $(window).keydown(function (e) {
        //alert(e.keyCode);
        if (e.keyCode == 112) { $('div#HelpIcons a.HelpIcon').trigger('click'); return false; }
        if ((e.ctrlKey) && (e.keyCode == 83)) { $('input[id$="BtnCreate"]').trigger('click'); $('input[id$="BtnSave"]').trigger('click'); return false; }
        if ((e.ctrlKey) && (e.keyCode == 68)) { $('input[id$="BtnDelete"]').trigger('click'); return false; }
        if ((e.ctrlKey) && (e.keyCode == 73)) { $('input[id$="BtnNew"]').trigger('click'); return false; }
        if (e.keyCode == 27) { $('input[id$="BtnCancel"]').trigger('click'); return false; }
        if ((e.ctrlKey) && (e.keyCode == 13)) { FindNext(window.getSelection().toString()); }
    });

    //
    $('input, select').change(function () {
        change = 1;
    })

    // Показываем диалог при попытке выйти без сохранения данных
    $('#Buttons button.BtnCancel').mousedown(function () {
        if (change != 0)
            Confirm('Уведомление', 'Выйти без изменений?', $(this));
        else
        {
            $('form input[required]').removeAttr('required');
            $(this).trigger('click');
        }
        return false;
    });

    // Показываем диалог при удалении записи
    $('#Buttons button.BtnDelete').mousedown(function () {
        Confirm('Уведомление', 'Вы хотите удалить эту запись?', $(this));
        return false;
    });



    //Скрываем или раскрываем блоки
    if ($('div.GroupBlock').length > 0) GroupBlock_init();

    //Гео поинт
    if ($('div.GeoPoint').length > 0) GeoPoint_init();

    // Изменение приоритета
    if ($(".Sortable").length > 0) $('.Sortable').each(function () { Sorting_init($(this)); });


     // Инициализация ckeEditor
    if ($('#editor1').length > 0) {
        CKEDITOR.replace('editor1', {
            language: 'ru',
            htmlEncodeOutput: true,
            disableObjectResizing: false
            //allowedContent: 'table thead tbody th tr td img a[!href]'
        });
        //CKEDITOR.config.autoGrow_onStartup = true;
        
     }

    // Растягиваем #DopInfo на высоту окна
    if ($('#DopInfo').children().length > 0) {
        var scrollHeight = Math.max(
          document.body.scrollHeight, document.documentElement.scrollHeight,
          document.body.offsetHeight, document.documentElement.offsetHeight,
          document.body.clientHeight, document.documentElement.clientHeight
        );
        var Login = parseInt($('#LogIn').height() + parseInt($('#LogIn').css('padding-top').replace('px', '')) + parseInt($('#LogIn').css('padding-bottom').replace('px', '')));
        var Buttons = $('#Buttons').height();
        $('#DopInfo').css('height', scrollHeight - Login - Buttons - 1);
    }
    else $('#DopInfo').css('display', 'none');


    // Переход по ссылке при выборе домена из списка
    $('#DomainSelect').change(function () {
        window.location.href = this.options[this.selectedIndex].value
    });
});

function PopUpFrame(Object) {
    var FrameTitle = Object.attr("title");
    var FrameUrl = Object.attr("href");

    var $ModalHeader = $('#UpFrame').find('.modal-title').empty().append(FrameTitle);
    var $ModalBody = $('iframe').attr("src", FrameUrl);
    $('#Confirm').find('iframe').attr("src", FrameUrl);

    var $BtnNo = $('#Confirm').find('#modal-btn-no');

    $BtnNo.unbind('click').click(function () {
        $('#Confirm').modal('toggle');
    });    

    $('#UpFrame').modal('toggle');

    $('#UpFrame').on('hidden.bs.modal', function (e) {
        e.preventDefault();
        History.pushState(null, document.title, location.href);
        loadPage(location.href);

        function loadPage(url) {
            $('.filtr-block').load(url + ' .filtr-block > *', function () {
                $('.filtr-block a.PopUpFrame').click(function () {
                    PopUpFrame($(this));
                    return false;
                });
            });
        }

        History.Adapter.bind(window, 'statechange', function (e) {
            var State = History.getState();
            loadPage(State.url);
        });
        //location.reload();
    })
}


function Confirm(Title, Body, Object) {
    var $ModalHeader = $('#Confirm').find('.modal-title').empty().append(Title);
    var $ModalBody = $('#Confirm').find('.modal-body').empty().append('<p>' + Body + '</p>');

    var $BtnOk = $('#Confirm').find('#modal-btn-ok');
    var $BtnNo = $('#Confirm').find('#modal-btn-no');

    $BtnOk.unbind('click').click(function () {
        $('form input[required]').removeAttr('required');
        Object.trigger('click');
    })

    $BtnNo.unbind('click').click(function () {
        $('#Confirm').modal('toggle');
    });

    $('#Confirm').modal('toggle');
}


function Sorting_init(Object) {
    Object.sortable({
        axis: "y",
        start: function () { $(this).addClass('Active'); },
        stop: function (event, ui) {
            $(this).removeClass('Active');

            var _ServiceUrl = $(this).attr('data-service');
            var _SortableItem = ui.item;
            var _Id = _SortableItem.attr('data-id');
            var _Num = $(this).find('.ui-sortable-handle').index(_SortableItem) + 1;

            _ServiceUrl = _ServiceUrl;

            $.ajax({
                type: "POST",
                async: false,
                url: _ServiceUrl,
                data: {id:_Id, permit:_Num},
                error: function () {Content = '<div>Error!</div>';},
                success: function (data) { }
            });
        }
    }).disableSelection();
}

function GeoPoint_init() {
    $('div.GeoPoint').click(function () {
        $(this).css('background-color', '#054A77');
       alert("Нажмите на точку на карте где должна стоять метка.")
    });

}

//размер iframe
function IframeSize(size) {
    var HeightFr = 250;
    if (size == "big") HeightFr = 770;
    else if (size == "average") HeightFr = 580;
    else HeightFr = size;

    if (HeightFr < 250) HeightFr = 250;

    $("iframe").css("height",HeightFr);
}

// Всплывающее окно
function SM_Branch(Object) {
    Object.toggleClass('open');
    var $Block = Object.parent().parent();
    var _link = Object.attr('title');
    var _BranchCount = $Block.children('div.TreeBranch').length;
    if (_BranchCount > 0) {
        if ($Block.children('div.TreeBranch').css('display') == 'block') $Block.children('div.TreeBranch').css('display', 'none');
        else $Block.children('div.TreeBranch').css('display', 'block');
    }
    else {
        var NewBranch = AddContent(_link, "div.TreeBranch");
        //Sorting_init(NewBranch);
        NewBranch.find('.treeIcon.folder').click(function () { SM_Branch($(this)) });
        $(NewBranch).find('.treeBtn.PopUp').click(function () {
            var $IframeClass = $(this).attr('id');
            var $Iframe = $("<iframe/>", { "id": "DopContent", "class": "FeebBackTypes", "src": $(this).attr('href') });
            $Iframe.append('Ваш браузер не поддерживает плавающие фреймы!');
            var BtnName = 'Закрыть'
            PopUp($Iframe, BtnName);
            return false;
        });
        $Block.append(NewBranch);
    }
}

// 
function old_SM_Branch(Object) {
    Object.toggleClass('open');
    var $Block = Object.parent().parent();
    var Elem;
    var _link = Object.attr('data-path');
    var _BranchCount = $Block.children('div.TreeBranch').length;
    if (_BranchCount > 0) {
        if ($Block.children('div.TreeBranch').css('display') == 'block') $Block.children('div.TreeBranch').css('display', 'none');
        else $Block.children('div.TreeBranch').css('display', 'block');
    }
    else {
        var NewBranch;
        $.ajax({
            async: false,
            url: "/Admin/SiteMap/OpenFolder?path=" + _link,
            data: ({ path: _link}),
            success: function (data) {                                
                NewBranch = data;
                }
            });
        $Block.append(NewBranch);
        NewBranch.find('.treeIcon.folder').click(function () { SM_Branch($(this)) });

        //var NewBranch = AddContent(_link, 'div.TreeBranch');
        //Sorting_init(NewBranch);
        //NewBranch.find('.treeIcon.folder').click(function () { SM_Branch($(this)) });
        //NewBranch.find('.treeBtn.PopUp').click(function () {
        //    var $IframeClass = $(this).attr('id');
        //    var $Iframe = $("<iframe/>", { "id": "DopContent", "class": "FeebBackTypes", "src": $(this).attr('href') });
        //    $Iframe.append('Ваш браузер не поддерживает плавающие фреймы!');
        //    var BtnName = 'Закрыть'
        //    PopUp($Iframe, BtnName);
        //    return false;
        //});

        //$Block.append(Elem);
    }
}


// Поиск текста на странице
function FindNext(findText) {
    if (findText == "") { alert("Please enter some text to search!"); return; }

    if (window.find) {        // Firefox, Google Chrome, Safari
        var found = window.find(findText);
        if (!found) {
            alert("<div>The following text was not found:\n" + findText + "</div>");
        }
    }
    else {
        alert("<div>Your browser does not support this example!</div>");
    }
};

// Кнопка «Наверх»
function TopButton() {
    var $InTop = $("<div/>", { "class": "ScrollTop" });
    $('body').prepend($InTop);
    $(document).bind('scroll', function () {
        // Показать кнопку
        if ($(window).scrollTop() > 50 && $InTop.css('display') == 'none') $InTop.slideDown(200, function () { $(this).css('display', 'block'); });
            // Скрыть кнопку
        else if ($(window).scrollTop() <= 50 && $InTop.css('display') == 'block') $InTop.slideUp(200, function () { $(this).css('display', 'none'); });
    });
    // Прокрутить страницу наверх
    $InTop.bind('click', function () { $(window).scrollTop('0'); });
};

function CoordPoint(xMap, yMap) {
    $(document).ready(function () {        
        var pointX = document.getElementById('MapX_input');
        pointX.value = xMap;
        var pointY = document.getElementById('MapY_input');
        pointY.value = yMap;
    });
}
function Coords(x, y, title, desc, zoom) {
    $(document).ready(function () {
        var JQ_Ymaps = $('script[src$="//api-maps.yandex.ru/2.0/?load=package.full&lang=ru-RU"]').length;
        x = x.replace(',', '.');
        y = y.replace(',', '.');
        if (JQ_Ymaps == 0) {

            $.getScript("//api-maps.yandex.ru/2.0/?load=package.full&lang=ru-RU", function () {
                var script = document.createElement('script');
                script.type = 'text/javascript';
                script.src = "//api-maps.yandex.ru/2.0/?load=package.full&lang=ru-RU";
                document.head.appendChild(script);

                ymaps.ready(function () {
                    if (title == '') { title = "Название организации"; }
                    if (desc == '') { desc = "Описание организации"; }
                    var myMap = new ymaps.Map("map", { center: [y, x], zoom: zoom }),
                myPlacemark = new ymaps.Placemark([y, x], {
                    // Чтобы балун и хинт открывались на метке, необходимо задать ей определенные свойства.
                    balloonContentHeader: title,
                    balloonContentBody: desc,
                    // balloonContentFooter: "Подвал",
                    hintContent: "Хинт метки"
                }, { draggable: true });
                    

                    //Перемещение метки 
                    myPlacemark.events.add("dragend", function () {                        
                        var coordinates = this.geometry.getCoordinates();
                        var x = this.geometry.getCoordinates(6)[0];
                        var y = this.geometry.getCoordinates(6)[1];
                        CoordPoint(y, x);
                        return false;

                    }, myPlacemark);
                    //Перемещение метки по клику
                    myMap.events.add('click', function (e) {                        
                        var coords = e.get('coordPosition');
                        myPlacemark.geometry.setCoordinates(coords);  //Перемещает метку на место клика                        
                        var xMap = coords[1].toPrecision(8)
                        var yMap = coords[0].toPrecision(8)                        
                        CoordPoint(xMap, yMap);
                        return false;
                        
                    });
                    

                    // Для добавления элемента управления на карту
                    // используется поле map.controls.
                    // Это поле содержит ссылку на экземпляр класса map.control.Manager.

                    // Добавление элемента в коллекцию производится с помощью метода add.

                    // В метод add можно передать строковый идентификатор
                    // элемента управления и его параметры.
                    myMap.controls
                    // Кнопка изменения масштаба.
                .add('zoomControl', { left: 5, top: 5 })
                    // Список типов карты
                .add('typeSelector')
                    // Стандартный набор кнопок
                .add('mapTools', { left: 35, top: 5 });

                    myMap.geoObjects.add(myPlacemark);

                });
            });
        }
    });
    

    myMap.events.add('click', function (e) {
        
        var coords = e.get('coordPosition');

        var xMap = coords[0].toPrecision(6)
        var yMap = coords[1].toPrecision(6)
        

    });
}

function GroupBlock_init() {
    $('div.GroupBlock').each(function () {
        $(this).wrapInner('<div class="GroupBlockInfo"></div>');
        var $BlockTitle = $("<div/>", { "class": "GroupBlockTitle" }).append($(this).attr('title'));
        var $BlockInfo = $(this).find('.GroupBlockInfo');

        $BlockTitle.click(function () {
            var Class = $(this).parent().attr('class');
            
            $BlockInfo.slideToggle(
                function () {
                    if (Class.indexOf('Open') == -1) $(this).parent().addClass('Open');
                    else $(this).parent().removeClass('Open');
                });

            return false;
        });

        $(this).prepend($BlockTitle);
    });
}


window.onload = function () {

    //Фотоальбомы
    if ($('.photoalbum').length > 0) { 
        // Центрирование фотографий по вертикали в фотоальбоме
        var photoImg = $('.photoalbum').find('img');
        photoImg.each(function () {
            var margin = ($(this).parent().height() - $(this).height()) / 2;
            $(this).css('margin', margin + 'px 0');
        });

        // Удаление фотографий из альбома
        $('.delPhoto').click(function () {
            var elem = $(this);
            var id = $(this).attr('data-id');

            $.ajax({
                type: "POST",
                url: '/admin/photoalbums/DeletePhoto/' + id,
                contentType: false,
                processData: false,
                data: false,
                success: function (result) {
                    if (result !== '') alert(result);
                    else elem.parent().remove();
                }
            });
        });
    }
};