/*
** Onload
*/
$(function () {
    $(window).resize(function() {
        if ($(window).width() < 800) {
            $('#banner').hide();
        } else {
            $('#banner').show();
        }
    });

    // Navigation - Data menu
    DataMenu.Init();

    // Twitter
    Twitter.Init();
});

/*
** Twitter
*/
var Twitter = {
    _current: 0,
    _total: 0,
    _duration: 4000,
    Init: function () {
        this._total = $('div.tweet').length;
        if (this._total > 0) {
            setInterval(Twitter.Next, this._duration);
        }
    },
    Next: function () {
        var previous = $('div.tweet')[Twitter._current];
        Twitter._current = (Twitter._current == Twitter._total - 1 ? 0 : Twitter._current + 1);
        var next = $('div.tweet')[Twitter._current];

        $(previous).hide('drop', {direction: 'up'}, 400, function () {
            $(next).show('drop', { direction: 'down' }, 400, null);
        });
    }
};

/*
** Data navigation menu
*/
var DataMenu = {
    _hoverButton: false,
    _hoverMenu: false,
    Hide: function() {
        if (!DataMenu._hoverButton && !DataMenu._hoverMenu) {
            $('#dataMenu').slideUp('fast');
        }
    },
    Init: function () {
        $('#dataButton').mouseover(function () {
            DataMenu._hoverButton = true;
            $('#dataMenu').slideDown('fast');
            $('#dataMenu').css('min-width', ($('#dataButton').outerWidth() + 2) + 'px');
            $('#dataMenu').css('top', $(this).offset().top + $(this).parents('li').height() + 'px');
            $('#dataMenu').css('left', ($(this).offset().left - 1) + 'px');
        });

        $('#dataButton').mouseout(function () {
            DataMenu._hoverButton = false;
            setTimeout(DataMenu.Hide, 200);
        });

        $('#dataMenu').mouseover(function () {
            DataMenu._hoverMenu = true;
        });

        $('#dataMenu').mouseout(function () {
            DataMenu._hoverMenu = false;
            setTimeout(DataMenu.Hide, 200);
        });

        $('#dataMenu div').click(function () {
            if ($(this).html() == '...') {
                window.location = '/Data';
            } else {
                window.location = '/Data?categoryFilter=' + $(this).html();
            }
        });
    }
};
