$(document).ready(function () {
    var frameHeight = $(document).height();

    // Инициализация полосы прокрутки
    if ($(".Scrollbar").length > 0) $(".Scrollbar").mCustomScrollbar();
    

    //назначение прав отдельному пользователю
    $('#UserResolutions .CB_Block input[type="checkbox"]').change(function () {
        var ParamsList = $(this).parent();
        var _User = ParamsList.attr('data-user');
        var _Url = ParamsList.attr('data-url');
        var _Action = ParamsList.attr('data-action');
        var _Value = ParamsList.find('input:checked').length;

        var Content = null;
        $.ajax({
            type: "POST",
            async: false,
            url: '/Admin/services/UserResolutions/',
            data: ({ id: _User, url: _Url, action: _Action, val: _Value }),
            error: function () { Content = 'Error' },
            success: function (data) { Content = data; }
        });

        //if (Content == 'Success') return false;
        //alert(Content);
    });

    //Назначение прав группе
    $('#UserGroupResolutions .CB_Block input[type="checkbox"]').change(function () {
        var ParamsList = $(this).parent();
        var _User = ParamsList.attr('data-user');
        var _Url = ParamsList.attr('data-url');
        var _Action = ParamsList.attr('data-action');
        var _Value = ParamsList.find('input:checked').length;

        var Content = null;
        $.ajax({
            type: "POST",
            async: false,
            url: '/Admin/services/GroupResolut/',
            data: ({ user: _User, url: _Url, action: _Action, val: _Value }),
            error: function () { Content = 'Error' },
            success: function (data) { Content = data; }
        });
    });

    
    //удаление доменных имен при помощи AJAX
    if ($('.delete-domain').length > 0) {
        $('.delete-domain').click(function () {
            var elem = $(this);
            var id = $(this).find('span').attr('data-delete');

            $.ajax({
                type: "POST",
                url: '/admin/sites/DeleteDomains/' + id,
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

    //удаление связи сайта с пользователем при помощи AJAX
    if ($('.delete-sitelink').length > 0) {
        $('.delete-sitelink').click(function () {
            var elem = $(this);
            var siteId = $(this).attr('data-delete-site');
            var userId = $(this).attr('data-delete-user');

            $.ajax({
                type: "POST",
                url: '/admin/allusers/DeleteSiteLinks/?siteId=' + siteId + '&userId=' + userId,
                contentType: false,
                processData: false,
                data: false,
                success: function (result) {
                    if (result !== '') alert(result);
                    else elem.parent().parent().remove();
                }
            });
        });
    }
})