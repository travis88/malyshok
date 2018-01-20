$(document).ready(function () {
    if ($('.uc_input').length > 0) ucInput_init();

    

    //инициализаация TinyMCE
    $('textarea').each(function () {        
        if ($(this).attr("type") == "editor") {
            var Width = 0;
            var Height = 300;
            InitTinyMCE($(this).attr('id'), Width, Height, "/UserFiles/");
        }
        if ($(this).attr("type") == "liteeditor") {
            var Width = 0;
            var Height = 300;
            InitLiteTinyMCE($(this).attr('id'), Width, 300);
        }
        
    });
    //if ($('.uc_input.editor').length > 0) {
    //    $('.uc_input.editor').each(function () {
    //        var _width = $(this).attr('width');
    //        var _height = $(this).attr('height') ? $(this).attr('height') : 300;

    //        InitTinyMCE($(this).attr('id'), $(this).attr('width'), _height, $(this).attr('data-dir'));
    //    });
    //}
    //if ($('.uc_input.liteeditor').length > 0) {
    //    $('.uc_input.liteeditor').each(function () {
    //        var _width = $(this).attr('width') ? $(this).attr('width') : 600;
    //        var _height = $(this).attr('height') ? $(this).attr('height') : 250;

    //        InitLiteTinyMCE($(this).attr('id'), _width, _height, $(this).attr('data-dir'));
    //    });
    //}

    // Создаем события для блока "Помощь"
    //$('.icon-help-circled').click(function () { AddHelp($(this).parents('.uc_block')); });

    // Инициализация типовых текстовых полей
    //if ($('.uc_block.phone').length > 0) Add_ucPhone();
    //if ($('.uc_block.email').length > 0) Add_ucEmail();
    //if ($('.uc_block.date').length > 0 || $('.uc_block.datetime').length > 0) Add_ucData();
    //if ($('.uc_block.numeric').length > 0) Add_ucNum();
    //if ($('.uc_block.password').length > 0) Add_ucPass();

    // Инициализация селестов
    //if ($('.uc_select').length > 0) ucSelect_init();
    // Инициализация селестов
    //if ($('.uc_selectTree').length > 0) uc_selectTree_init();

    // Инициализация полей для выбора файлов
    if ($('.uc_fileupload').length > 0) ucFileUpload_Init();

    // Инициализация пользовательских сообщений
    //if ($('#msg_block').length > 0) msg_init();
})

// Инициализация текстовых полей
function ucInput_init() {
    $('.uc_input').each(function () {
        var ReadOnly = $(this).attr('readonly');
        var Important = $(this).attr('required');
        var Label = $(this).attr('title');
        var Type = $(this).attr('data-type');
        var Mask = $(this).attr('data-mask');
        var Help = $(this).attr('data-help');
        var TypeElem = $(this).attr('type');

        $(this).addClass('form-control').wrap('<div class="form-group"></div>');

        if (Label) { $(this).before('<label for="' + $(this).attr('id') + '">' + Label + ':</label>'); }

        if (Help && !ReadOnly) {
            $(this).wrap('<div class="input-group"></div>');
            $(this).after('<div class="input-group-addon"><div class="icon-help-circled" data-toggle="popover" data-placement="auto bottom" data-content="' + Help + '"></div></div>');

            $(this).next().find('div').popover();
        }

        if (TypeElem == "checkbox") {
            var $This = $(this);
            var $btnGroup = $("<div/>", { 'class': 'btn-group btn-group-xs', 'data-toggle': 'buttons-radio' });
            var $btnYes = $("<button/>", { 'class': 'btn btn-default' }).append('Да');
            
            $btnYes.click(function () { change = 1; $This.attr('checked', 'true'); $btnNo.removeClass('active'); });

            var $btnNo = $("<button/>", { 'class': 'btn btn-default' }).append('Нет');
            $btnNo.click(function () { change = 1; $This.removeAttr('checked'); $btnYes.removeClass('active'); });

            if ($(this).attr('checked') == 'checked') $btnYes.addClass('active');
            else $btnNo.addClass('active');

            $btnGroup.append($btnYes).append($btnNo);
            $(this).after($btnGroup);

        }
    });
}

// TinyMCE
function InitTinyMCE(id, _width, _height, directory) {
    tinymce.init({
        selector: "textarea#" + id,
        theme: "modern",
        add_unload_trigger: false,
        schema: "html5",
        plugins: [["anchor nonbreaking paste hr searchreplace  textcolor charmap  link autolink image media table visualblocks code fullscreen contextmenu"]],
        toolbar: 'undo redo | styleselect fontsizeselect | bold italic underline superscript subscript | forecolor backcolor | alignleft aligncenter alignright alignjustify | bullist numlist | outdent indent | table | link image media | removeformat code',
        contextmenu: "copy paste | link image",
        extended_valid_elements: "code",
        invalid_elements: "script,!--",
        convert_urls: false,
        relative_urls: false,
        image_advtab: true,
        cleanup: false,
        erify_html: false,
        statusbar: false,
        language: 'ru',
        menubar: false,
        verify_html: false,
        width: _width,
        height: _height,
        file_browser_callback: function (field_name, url, type, win) {
            var cmsURL = "http://" + window.location.hostname + "/FileManager/?cmd=edit&path=" + directory;
            $('div#Canvas').after('<div id="Opacity"></div><iframe id="FileManager" src="' + cmsURL + '">Ваш браузер не поддерживает плавающие фреймы!</iframe>');

            $('iframe#FileManager').bind('load', function () { FileManagerLoad(field_name); });
            if (win.getImageData) win.getImageData();
        }
    });
}


// TinyMCE (Урезанная версия)
function InitLiteTinyMCE(id, _width, _height) {
    tinymce.init({
        selector: "textarea#" + id,
        theme: "modern",
        add_unload_trigger: false,
        schema: "html5",
        plugins: [["textcolor"]],
        toolbar: "fontsizeselect | bold italic underline | forecolor backcolor | alignleft aligncenter alignright alignjustify",
        invalid_elements: "script,!--",
        convert_urls: false,
        relative_urls: false,
        image_advtab: true,
        cleanup: false,
        erify_html: false,
        statusbar: false,
        language: 'ru',
        width: _width,
        height: _height,
        menubar: false
    });
}

// Загружаем дополнительный контент с помощью AJAX
function AddContent(_ContentUrl, Object) {
    var Content = null;
    $.ajax({
        async: false,
        url: _ContentUrl,
        error: function () { Content = '<div>Error! This page is not found!</div>'; },
        success: function (data) {
            if (Object != "") { Content = $(data).find(Object); }
            else { Content = data; }
        }
    });
    return Content;
}

//  ------------------------- Old -----------------------
function Add_ucPhone() {
    $('.uc_block.phone input').keypress(
        function (key) {
            if (key.charCode == 32 || key.charCode == 40 || key.charCode == 41 || (key.charCode > 42 && key.charCode < 46) || (key.charCode > 47 && key.charCode < 60)) { }
            else { return false; }
        });
}
function Add_ucEmail() {
    $('.uc_block.email input').blur(function () {
        var emailMask = /\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/;
        
        var value = $(this).val();
        if (!emailMask.test(value)) { $(this).addClass('error'); }
    });
}
function Add_ucData() {
    // Подключаем календарь
    $.getScript('http://web.it-serv.ru/js/plugins/jquery.ui.datepicker.js', function () {
        if ($('.uc_block.date').length > 0) {
            $('.uc_block.date input').datepicker({ onSelect: function (dateText, inst) { $(this).attr('value', dateText); } });
        }
        if ($('.uc_block.datetime').length > 0) {
            $('.uc_block.datetime input').datepicker({
                onSelect: function (dateText, inst) {
                    var d = new Date();
                    var Hours = (d.getHours() < 10) ? '0' + d.getHours() : d.getHours();
                    var Minutes = (d.getMinutes() < 10) ? '0' + d.getMinutes() : d.getMinutes();

                    $(this).val(dateText + " " + Hours + ":" + Minutes);
                }
            });
        }
    });
    // Подключаем маску
    if ($('.uc_block.date').length > 0) { Add_Mask($('.uc_block.date input'), '99.99.9999'); }
    if ($('.uc_block.datetime').length > 0) { Add_Mask($('.uc_block.datetime input'), '99.99.9999 99:99'); }
};
function Add_ucNum() {
    $('.uc_block.numeric input').keypress(function (key) {
        if (key.charCode < 44 || key.charCode > 57) return false;
    })
}
function Add_ucPass() { }

function Add_Mask(object, val) {
    $.getScript('', function () { object.mask(val) })
}

// Блок "Помощь"
function AddHelp(Object) {
    var OpenHelp = $('.icon-help-circled.Active').parents('.uc_block');
    // закрываем все открытые блоки
    if (OpenHelp.length > 0 && Object.get(0) != OpenHelp.get(0)) {
        OpenHelp.find('.icon-help-circled').toggleClass("Active");
        OpenHelp.find('.uc_help').toggle(300);
    }

    // Открываем выбранный блок
    Object.find('.uc_help').toggle(300);
    // Помечаем выбранный блок как активный
    Object.find('.icon-help-circled').toggleClass("Active");
};

// Инициализация селестов
function ucSelect_init() {
    $('.uc_select').each(function () {
        var ReadOnly = ($(this).parent().attr('class').indexOf('readonly') > -1);
        var Important = ($(this).parent().attr('class').indexOf('importent') > -1);

        var $Select = $(this).find('select').css('display', 'none');
        // Создаем контейнер для отображения выбранных элементов
        var $Input = $("<div/>", { 'class': 'uc_Result' });
        // Создаем лист для представления списка записей
        var $List = $("<div/>", { 'class': 'uc_List' });
        // Определяем тип элемента
        var type = $Select.attr('multiple');
        if (type == 'multiple') { }
        else if ($Select.find('option:selected').length == 0) $Select.find('option:first').attr('selected', 'selected');

        var groups = {};
        $Select.find('option[data-group]').each(function () { groups[$.trim($(this).attr('option[data-group]'))] = true; });
        $.each(groups, function (c) {
            var $GroupItem = $("<div/>", { 'class': 'uc_ListGroup', 'data-group': c });
            $GroupItem.append(c);
            $List.append($GroupItem);
        });

        // Функция создает представление выбранного элемента в MltiSelect
        function addSelItem(title, value) {
            var $ResultItem = $("<div/>", { 'class': 'uc_ResultItem' });
            var $RItemClose = $("<div/>", { 'class': 'uc_ResultItemClose' });

            $ResultItem.click(function () { return false; });

            if (!ReadOnly) {
                $RItemClose.click(function () {
                    $Select.find('option[value="' + value + '"]').removeAttr('selected');
                    $List.find('div[data-value="' + value + '"]').removeClass('Sel');
                    $ResultItem.remove();
                    return false;
                });
                $ResultItem.append($RItemClose);
            }

            $ResultItem.append(title);
            $Input.append($ResultItem);
        }

        if (ReadOnly) {
            $Select.find('option:selected').each(function () {
                var key = $(this).text();
                var val = $(this).val();

                if (type == 'multiple') { addSelItem(key, val); }
                else { $Input.text(key); }
            });
            $Select.after($Input);
        }
        else {
            // Функция инициализации списка записей
            $Select.find('option').each(function () {
                var key = $(this).text();
                var val = $(this).val();
                var sel = $(this).attr('selected');
                var group = $(this).attr('data-group');

                // Добавляем запись в список
                var $Item = $("<div/>", { 'class': 'uc_ListItem', 'data-value': val });
                $Item.text(key);
                // Если запись отмечена как выбранная
                if (sel != null) {
                    $Item.addClass('Sel');
                    if (type == 'multiple') { addSelItem(key, val); }
                    else { $Input.text(key); }
                }

                $Item.click(function () {
                    if (type == 'multiple') { addSelItem($(this).text(), $(this).attr('data-value')); }
                    else {
                        $Select.find('option').removeAttr('selected');
                        $List.find('.uc_ListItem').removeClass('Sel');
                        $Input.empty().append($(this).text());
                    }
                    $Select.find('option[value="' + $(this).attr('data-value') + '"]').attr('selected', 'selected');

                    $Item.addClass('Sel');
                });

                if (group == null) $List.append($Item);
                else $List.find('div[data-group=' + group + ']').append($Item);
            });

            //  для вызова списка записей
            $Input.click(function () {
                $('body').unbind();

                $('.uc_List.Open').removeClass('Open').slideToggle(100).prev().removeClass('Open');
                $List.addClass('Open').slideToggle(100);
                $Input.addClass('Open');

                $('body').bind('click', function () { $List.removeClass('Open').slideToggle(100); $Input.removeClass('Open'); $(this).unbind() });
                return false;
            });
            // Добавляем Контейнер и Список на форму
            $Select.after($List).after($Input);
        }
    });
}
function uc_selectTree_init() {
    $('.uc_selectTree').each(function () {
        var $Tree = $(this).find('.uc_Tree');//.mCustomScrollbar().css('display', 'none');
        var $Input = $(this).find('.uc_Result');

        // Функция для визуального оформления выбранной записи
        function AddSelectItem() {
            $('.uc_selectTree .TreeLink').removeClass('Now');

            var val = $Input.prev().val();
            if (val != '') {
                var $SelectItem = $Tree.find('.treeUrl[href=' + val + ']');
                $SelectItem.parent().addClass('Now');
                $SelectItem.parents('.TreeBranch').prev().addClass('Now');

                $Input.empty().append($SelectItem.text());
            }
        }

        $Input.click(function () {
            var $BtnBlock = $("<div/>", { "id": "PopUpBtn" });
            var $Bg = $("<div/>", { "id": "Opacity" }).click(function () { $BtnBlock.remove(); $Bg.remove(); $Tree.toggle(); $('body').css('overflow-y', 'scroll'); });
            // Создаем кнопку "Выход без изменений"
            var $Back = $("<div/>", { "id": "uc_SaveBtn" }).append("Выйти без изменений").click(function () {
                AddSelectItem();

                $BtnBlock.remove();
                $Bg.remove();
                $Tree.toggle();
                $('body').css('overflow-y', 'scroll');
            });
            // Создаем кнопку "Выбрать"
            var $Save = $("<div/>", { "id": "PopUpClose" }).append("Выбрать").click(function () {
                var key = $Tree.find('.TreeLink.Now:last .treeUrl').text();
                var val = $Tree.find('.TreeLink.Now:last .treeUrl').attr('href');

                $(this).parents('.uc_selectTree').find('input').val(val);
                $Input.empty().append(key);

                $BtnBlock.remove();
                $Bg.remove();
                $Tree.toggle();
                $('body').css('overflow-y', 'scroll');
            });

            $('body').prepend($Bg).css('overflow-y','hidden');
            $BtnBlock.append($Save).append($Back);
            $Tree.append($BtnBlock);
            
            $Tree.toggle();
        });

        $Tree.find('.treeUrl').click(function () {
            var $Parent = $(this).parent().parent().parent();
            var key = $(this).text();
            var val = $(this).attr('href');

            if ($Parent.attr('class') == 'TreeBranch') $Parent.find('.TreeLink').removeClass('Now');
            else $('.uc_selectTree .TreeLink').removeClass('Now');

            $(this).parent().addClass('Now');

            return false;
        });
        $Tree.find('.treeIcon').unbind();

        AddSelectItem();
    })
}

// Инициализация полей для выбора файлов
function ucFileUpload_Init() {
    $('.uc_fileupload').each(function () {
        var $BlockTitle = $(this).find('.uc_title');
        var $ControlBlock = $(this).find('.uc_control');
        var $BlockPreview = $(this).find('.uc_preview');
        var $ResultInput = $(this).find('input[type=hidden]');

        // Разворачивает и сворачивает список файлов
        $BlockTitle.click(function () {
            var Class = $(this).parent().attr('class');

            $ControlBlock.slideToggle(
                function () {
                    if (Class.indexOf('Open') == -1) $(this).parent().addClass('Open');
                    else $(this).parent().removeClass('Open');
                });

            return false;
        });

        // Добавить файл при помощи кнопки
        $ControlBlock.find('input:file').change(function (e) {
            $(this).addClass('UnLook');
            $BlockPreview.empty();

            var File = $(this)[0].files[0];
            var FileName = File.name;
            var FileType = File.type;
            var FileSize = File.size;

            $ResultInput.val(FileName);

            var $PreviewBlock = $("<div/>", { 'class': 'uc_preview_item' });
            var $ImgBlock = $("<div/>", { 'class': 'uc_preview_img' });
            // если картинка
            if (FileType.indexOf('image') > -1) {
                var img = $("<img/>");
                img.attr('src', window.URL.createObjectURL(File));
                $ImgBlock.append(img);
            }
            else {
                $ImgBlock.append(File.substring(FileName.lastIndexOf(".") + 1));
            }
            $PreviewBlock.append($ImgBlock);

            var $InfoBlock = $("<div/>", { 'class': 'uc_preview_info' });
            $InfoBlock.append('<div class="uc_preview_name">' + FileName + '</div>');
            $InfoBlock.append('<div class="uc_preview_size">' + FileSize + '</div>');

            // Кнопка Удалить
            var $DelPreview = $("<div/>", { 'class': 'uc_preview_btn' }).append('удалить');
            $DelPreview.click(function () {
                $ControlBlock.find('input:file').removeClass('UnLook');
                $ResultInput.val('');
                $BlockPreview.empty();
            });

            $InfoBlock.append($DelPreview);
            $PreviewBlock.append($InfoBlock);
            $BlockPreview.append($PreviewBlock);
        });

        // Кнопка "Удалить"
        $(this).find('.uc_preview_Del').click(function () {
            $ControlBlock.find('input:file').removeClass('UnLook');
            $ResultInput.val('');
            $BlockPreview.empty();

            return false;
        });
    });
}

// Подгружаем значения в Select
function UC_SelectChenge(Object, ServiceUrl, Value) {
    $.getJSON(ServiceUrl + Value, function (Data) {
        $.each(Data, function (key, val) {
            var $Item = $("<div/>", { "data-value": val }).append(key);
            Object.append($Item);
        });
    })
        .done(function (data) { alert(ServiceUrl + Value); })
        .fail(function (err1, err2, err3) { alert('error') })
        .always(function () { });
}

// Инициализация пользовательских сообщений
function msg_init() {
    var $Massege = $('#msg_block');

    
    var $Msg = $("<div/>", { "id": "PopUpMassege", "class": $Massege.attr('class') });
    var $Bg = $("<div/>", { "id": "Opacity" });
    if ($Msg.find('#msg_close').attr("href") == "undefined") {
        $Bg.click(function () { $Bg.remove(); $Msg.remove(); })
    }
    else {
        
        $Bg.click(function () { top.location.href = $Msg.find('#msg_close').attr("href"); })
    }
    $('body').prepend($Bg);

    var msgTitle = $("<div/>", { "id": "msgTitle" }).append($Massege.attr('title'));
    var msgBody = $("<div/>", { "id": "msgBody" }).append($Massege.find('.msgText').text());
    var msgBtn = $("<div/>", { "id": "msgBtn" }).append($Massege.find('.msg_BtnBlock').html());

    msgBtn.find('#msg_close').addClass('ok').click(function () { $Bg.remove(); $Msg.remove(); });

    $Msg.append(msgTitle).append(msgBody).append(msgBtn);
    $Bg.after($Msg);
}

// Объявление событий для Файлового менеджера
function FileManagerLoad(getId) {
    var iframContent; //DOM содержащий содержимое iframe
    iframContent = $('iframe#FileManager').contents();

    // Создаем обработчик для кнопки "Выбрать файл"
    iframContent.find('#FM_BtnOk').bind('click', function () {
        $('#' + getId).val(iframContent.find('#DirNavigation').text().replace("~/", "/") + iframContent.find('#BackUrl').val());
        $('iframe#FileManager').remove();
        $('div#Opacity').remove();
    });

    // Создаем обработчик для кнопки "Отмена"
    iframContent.find('#FM_BtnCancel').bind('click', function () {
        $('iframe#FileManager').remove();
        $('div#Opacity').remove();
    });
};
