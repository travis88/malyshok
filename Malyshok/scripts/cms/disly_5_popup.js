$(document).ready(function () {
    var iframe = $('.modal_frame', parent.document.body);
    iframe.height($(document).height() + 5);

    // Инициализация полосы прокрутки
    if ($(".scrollbar").length > 0) $(".scrollbar").mCustomScrollbar();

    // устанавливаем курсор
    setCursor();


    //удаление документов при помощи AJAX
    if ($('.delete-document').length > 0) {
        $('.delete-document').click(function () {
            var elem = $(this);
            var id = $(this).find('span').attr('data-delete');

            $.ajax({
                type: "POST",
                url: '/admin/Documents/Delete/' + id,
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

    //Назначение прав группе
    $("#modal-userGroupResolutions-table input[type='checkbox']").on('ifChanged', function () {
        var targetUrl = "/Admin/Services/UpdateGroupClaims";
        var _group = $(this).data("group");
        var _url = $(this).data("url");
        var _action = $(this).data("action");
        var _checked = $(this).is(':checked');

        var el = $(this);
        var elTooltip = $(this).closest(".groupClaim-item").find(".groupClaim-item-tooltip").first();

        try
        {
            var params = {
                GroupAlias: _group,
                ContentId:_url,
                Claim: _action,
                Checked: _checked
            };

            var _data = JSON.stringify(params);

            //ShowPreloader(content);
            
            $.ajax({
                url: targetUrl,
                method: "POST",
                cache: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(params)
            })
                .done(function (response) {
                    elTooltip.tooltip('show');
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                    elTooltip.attr("title", "Ошибка сохранения");
                })
                .always(function (response) {
                    setTimeout(function () {
                        //content.fadeOut("slow");
                        elTooltip.tooltip('hide');
                    }, 1000);
                });
        }
        catch(ex){
            console.log(ex);
        }
    });

    //Привязка пользователей к сайтам 
    $("#modal-userSite-table .userSite-item-chkbx").on('ifToggled', function () {
        var targetUrl = "/Admin/Services/UpdateUserLinkToSite";
        var _objctId = $(this).data("objectId");
        var _objectType = $(this).data("objectType");
        var _linkId = $(this).data("linkId");
        var _linkType = $(this).data("linkType");
        var _checked = $(this).is(':checked');

        var el = $(this);
        var elTooltip = $(this).closest(".userSite-item-row").find(".userSite-item-tooltip").first();

        try {
            var params = {
                ObjctId: _objctId,
                ObjctType: _objectType,
                LinkId: _linkId,
                LinkType: _linkType,
                Checked: _checked
            };

            var _data = JSON.stringify(params);

            //ShowPreloader(content);

            $.ajax({
                url: targetUrl,
                method: "POST",
                async: true,
                cache: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(params)
            })
                .done(function (response) {
                    elTooltip.attr("title", "Сохранено");
                    elTooltip.tooltip('show');
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                    elTooltip.attr("title", "Ошибка сохранения");
                    elTooltip.tooltip('show');
                })
                .always(function (response) {
                    setTimeout(function () {
                        elTooltip.tooltip('hide');
                    }, 1000);
                    //location.reload();
                });
        }
        catch (ex) {
            console.log(ex);
        }
    });

    //Привязка к организациям
    $(".org-byType-tree .orgType-item-chkbx").on('ifToggled', function () {
        var targetUrl = "/Admin/Orgs/UpdateLinkToOrg";
        var _objctId = $(this).data("objectId");
        var _objectType = $(this).data("objectType");
        var _linkId =  $(this).data("linkId");
        var _linkType = $(this).data("linkType");
        var _checked = $(this).is(':checked');

        var el = $(this);
        var elTooltip = $(this).closest(".orgType-item").find(".orgType-item-tooltip").first();
        var _chkbxlabel = $(this).closest(".orgType-item").find("label .orgType-item-html").first().html();

        var listBlock = $("#model-linksToOrgs-ul", top.document);

        try {
            var params = {
                ObjctId: _objctId,
                ObjctType: _objectType,
                LinkId: _linkId,
                LinkType: _linkType,
                Checked: _checked
            };

            var _data = JSON.stringify(params);

            //ShowPreloader(content);

            $.ajax({
                url: targetUrl,
                method: "POST",
                async: true,
                cache: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(params)
            })
                .done(function (response) {
                    elTooltip.attr("title", "Сохранено");
                    elTooltip.tooltip('show');
                    if (_checked) {
                        if (listBlock.find("org_" + _linkId).length === 0)
                        {
                            listBlock.append($("<li id='org_" + _linkId + "' class='icon-location-5'/>").html(_chkbxlabel));
                        }
                    }
                    else {
                        listBlock.find("#org_" + _linkId).remove();
                    }
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                    elTooltip.attr("title", "Ошибка сохранения");
                    elTooltip.tooltip('show');
                })
                .always(function (response) {
                    setTimeout(function () {
                        //content.fadeOut("slow");
                        elTooltip.tooltip('hide');
                    }, 1000);
                    location.reload();
                });
        }
        catch (ex) {
            console.log(ex);
        }
    });

    //Привязка к событиям
    $("#modal-event-table .event-item-chkbx").on('ifToggled', function () {
        var targetUrl = "/Admin/Events/UpdateLinkToEvent";
        var _objctId = $(this).data("objectId");
        var _objectType = $(this).data("objectType");
        var _linkId =  $(this).data("linkId");
        var _linkType = $(this).data("linkType");
        var _checked = $(this).is(':checked');

        var el = $(this);
        var elTooltip = $(this).closest(".event-item-row").find(".event-item-tooltip").first();
        var _chkbxEvent = $(this).closest(".event-item-row").find(".event-item-html").first().html();
        var _dateEvent = $(this).closest(".event-item-row").find(".event-item-date").first().html();

        var listBlock = $("#model-linksToEvent-ul", top.document);

        try {
            var params = {
                ObjctId: _objctId,
                ObjctType: _objectType,
                LinkId: _linkId,
                LinkType: _linkType,
                Checked: _checked
            };

            var _data = JSON.stringify(params);

            //ShowPreloader(content);

            $.ajax({
                url: targetUrl,
                method: "POST",
                async: true,
                cache: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(params)
            })
                .done(function (response) {
                    elTooltip.attr("title", "Сохранено");
                    elTooltip.tooltip('show');
                    if (_checked) {
                        if (listBlock.find("evnt_" + _linkId).length === 0)
                        {
                            listBlock.append($("<li id='evnt_" + _linkId + "' class='icon-calendar'/>").html(_dateEvent + _chkbxEvent));
                        }
                    }
                    else {
                        listBlock.find("#evnt_" + _linkId).remove();
                    }
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                    elTooltip.attr("title", "Ошибка сохранения");
                    elTooltip.tooltip('show');
                })
                .always(function (response) {
                    setTimeout(function () {
                        elTooltip.tooltip('hide');
                    }, 1000);
                    //location.reload();
                });
        }
        catch (ex) {
            console.log(ex);
        }
    });

    //Привязка баннеров к сайтам 
    $("#modal-site-table .site-item-chkbx").on('ifToggled', function () {
        var targetUrl = "/Admin/Sites/UpdateLinkToSite";
        var _objctId = $(this).data("objectId");
        var _objectType = $(this).data("objectType");
        var _linkId = $(this).data("linkId");
        var _linkType = $(this).data("linkType");
        var _checked = $(this).is(':checked');

        var el = $(this);
        var elTooltip = $(this).closest(".site-item-row").find(".site-item-tooltip").first();
        var _chkbxHtml = $(this).closest(".site-item-row").find(".site-item-html").first().html();

        var listBlock = $("#model-linksToSite-ul", top.document);

        try {
            var params = {
                ObjctId: _objctId,
                ObjctType: _objectType,
                LinkId: _linkId,
                LinkType: _linkType,
                Checked: _checked
            };

            var _data = JSON.stringify(params);

            //ShowPreloader(content);

            $.ajax({
                url: targetUrl,
                method: "POST",
                async: true,
                cache: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(params)
            })
                .done(function (response) {
                    elTooltip.attr("title", "Сохранено");
                    elTooltip.tooltip('show');
                    if (_checked) {
                        if (listBlock.find("site_" + _linkId).length === 0) {
                            listBlock.append($("<li id='site_" + _linkId + "' class='icon-location-5'/>").html(_chkbxHtml));
                        }
                    }
                    else {
                        listBlock.find("#site_" + _linkId).remove();
                    }
                })
                .fail(function (jqXHR, status) {
                    console.log("Ошибка" + " " + status + " " + jqXHR);
                    elTooltip.attr("title", "Ошибка сохранения");
                    elTooltip.tooltip('show');
                })
                .always(function (response) {
                    setTimeout(function () {
                        elTooltip.tooltip('hide');
                    }, 1000);
                    //location.reload();
                });
        }
        catch (ex) {
            console.log(ex);
        }
    });

})


// устанавливаем курсор
function setCursor() {
    if ($('input.input-validation-error').length > 0) $('input.input-validation-error:first').focus();
    else if ($('input[required]').val() === '') $('input[required]:first').focus();
    else if ($(' input').length > 0) $('input:first').focus();
}