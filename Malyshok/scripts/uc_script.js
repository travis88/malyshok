$(document).ready(function () {
    //if ($('.uc_input').length > 0) ucInput_init();

    
});


// Инициализация текстовых полей
function ucInput_init() {
    $('.uc_input').each(function () {
        var tagName = $(this).prop("tagName").toLowerCase();
        var ReadOnly = $(this).attr('readonly');
        var Important = $(this).attr('required');
        var Label = $(this).attr('title');
        var Type = $(this).attr('data-type');
        var Mask = $(this).attr('data-mask');
        var Help = $(this).attr('data-help');
        var TypeElem = $(this).attr('type');
        
        $(this).wrap('<div class="form-group"></div>');
        if (Label) { $(this).before('<label for="' + $(this).attr('id') + '">' + Label + ':</label>'); }

        if (Help && !ReadOnly) {
            $(this).wrap('<div class="input-group"></div>');
            $(this).after('<div class="input-group-addon"><div class="icon-help-circled" data-toggle="popover" data-placement="auto bottom" data-content="' + Help + '"></div></div>');

            $(this).next().find('div').popover();
        }


        if (TypeElem == "checkbox") {
            checkboxInit($(this), ReadOnly);
        }
        else if (TypeElem == "file") {
            fileInit($(this), ReadOnly);
        }
        else if (tagName == 'select') {
            selectInit($(this), ReadOnly);
        }
        else {
            $(this).addClass('form-control');
        }

        if (Type == 'date') {
            $(this).attr('value', $(this).attr('value').replace(/(\d+).(\d+).(\d+) (\d+:\d+:\d+)/, '$1.$2.$3'));
            Mask = "99.99.9999";
            $(this).datepicker({ onSelect: function (dateText, inst) { $(this).attr('value', dateText); } });
        }
        

        if (Mask != undefined) $(this).mask(Mask);
    });
}


// Инициализация полей с выпадающим списком
function selectInit(Object, ReadOnly)
{
    var $Select = Object.css('display', 'none');
    // Создаем контейнер для отображения выбранных элементов
    var $Input = $("<input/>", { 'class': 'uc_select form-control', 'type': 'text' });
    var $Btn = $("<div/>", { 'class': 'uc_select-btn' });
    // Создаем лист для представления списка записей
    var $List = $("<div/>", { 'class': 'uc_select-list' });
    var $SelectBlock = $("<div/>", { 'class': 'selsect-group' });

    if (ReadOnly) {
        $Select.find('option:selected').each(function () {
            var key = $(this).text();
            var val = $(this).val();

            $Input.val(key).attr('readonly', 'true');
        });

        // отменяем выделение при клике
        $Input.bind({
            selectstart: function () {
                return false;
            }
        });
        // Добавляем Контейнер на форму
        $SelectBlock.append($Input.addClass('readonly'));
    }
    else {
        // Функция инициализации списка записей
        $Select.find('option').each(function () {
            var key = $(this).text();
            var val = $(this).val();
            var sel = $(this).attr('selected');
            var group = $(this).attr('data-group');

            // Добавляем запись в список
            var $Item = $("<div/>", { 'class': 'uc_list-item', 'data-value': val, 'title': key.toLowerCase() });
            $Item.text(key);
            // Если запись отмечена как выбранная
            if (sel != null) {
                $Item.addClass('sel');
                $Input.val(key);
            }

            $Item.click(function () {
                $Select.find('option').removeAttr('selected');
                $Select.find('option[value="' + $(this).attr('data-value') + '"]').attr('selected', 'selected');

                $List.removeClass('open').slideToggle(100).find('.uc_list-item').removeClass('sel');
                $(this).addClass('sel');

                $Input.removeClass('open').val($(this).text());

                $Select.trigger('change');

                $('body').unbind();

                return false;
            });

            $List.append($Item);
        });

        $Input.bind({
            // для поиска элемента в списке
            input: function () {
                var _word = $(this).val().toLowerCase();

                if (_word != '') {
                    $List.find('.uc_list-item').css('display', 'none');
                    $List.find('.uc_list-item[title*="' + _word + '"]').css('display', '');
                }
                else {
                    $List.find('.uc_list-item').css('display', '');
                }

                return false;
            },
            keyup: function () {
                var _word = $(this).val().toLowerCase();

                if (_word != '') {
                    $List.find('.uc_list-item').css('display', 'none');
                    $List.find('.uc_list-item[title*="' + _word + '"]').css('display', '');
                }
                else {
                    $List.find('.uc_list-item').css('display', '');
                }

                return false;
            },
            // для вызова списка записей
            click: function () {
                $('body').unbind();

                $List.removeClass('open').addClass('open').slideDown(100).find('.uc_list-item').css('display', '');
                $Input.removeClass('open').addClass('open');

                $('body').bind('click', function () {
                    $List.removeClass('open').slideToggle(100);
                    $Input.removeClass('open');
                    $Input.val($Select.find('option:selected').text());

                    $(this).unbind();
                });
                return false;
            }
        });

        $SelectBlock.append($Input).append($Btn).append($List);
    }

    // Добавляем Контейнер и Список на форму
    $Select.before($SelectBlock);
}
// Инициализация полей выбора файлов