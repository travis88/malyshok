$(document).ready(function () {
    // Если система администрирования загружена в frame открываем её в родительском окне
    if (top != self) { top.location.href = location.href; }

    // показываем preloader при клике на кнопку
    $('input[type=submit], .button').bind({
        click: function () {
            var $load_bg = $("<div/>", { "class": "load_page" });
            $load_bg.bind({
                mousedown: function () { return false; },
                selectstart: function () { return false; }
            });

            $('body').append($load_bg);
        }
    });

    // Показываем страницу, убираем preloader
    load_page();
});

// Показываем страницу, убираем preloader
function load_page() {
    var $load_bg = $('div.load_page');
    setTimeout(function () {
        $load_bg.css('opacity', '1');

        var interhellopreloader = setInterval(function () {
            $load_bg.css('opacity', $load_bg.css('opacity') - 0.05);
            if ($load_bg.css('opacity') <= 0.05) { clearInterval(interhellopreloader); $load_bg.remove(); }
        }, 6);
    }, 1000);
};
