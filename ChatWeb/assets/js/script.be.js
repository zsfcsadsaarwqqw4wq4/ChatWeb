var setFmenus = function () {

    var fmenus = $('.fenu');
    if (fmenus.length > 0) {

        fmenus.find('.fenu-cbg a').off('click').on('click', function () {
            var _this = $(this);
            var _pnt = _this.parents('.fenu');
            _pnt.find('ul.fctn').hide();
            _pnt.find('.fenu-cbg a').removeClass('on');
            var $p = _pnt.find('.p' + _this.data('page'));
            _this.addClass('on');
            $p.show();
        });

        $('.fenu-close').off('click').on('click', function () {
            var $fenu = $(this).parents('.fenu');
            $fenu.removeClass('strong');
            setTimeout(function () {
                $fenu.hide();
            }, 300);
        });

        $('.fctn li.search input[type="text"]').off('keyup').on('keyup', function () {
            var _lis = $(this).parents('ul.fctn').find('li:not(.search)');
            var _val = $(this).val().toLowerCase();
            _lis.each(function () {
                var _this = $(this);
                var _text = _this.text().toLowerCase();
                if (_text.indexOf(_val) >= 0) {
                    _this.show();
                } else {
                    _this.hide();
                }
            });
        });

        $('.fenu>ul>li>a').off('click').on('click', function () {
            var _this = $(this);
            var _chk = _this.parents('.fenu').find('.fenu-check');
            var _tmp = _chk.data('target');
            var _targets;
            if (_tmp.indexOf(',') >= 0) {
                _targets = _tmp.split(',');
            } else {
                _targets = new Array(1);
                _targets[0] = _tmp;
            }

            var isAdd = true;
            for (var i = 0; i < _targets.length; i++) {
                var _target = _targets[i];
                if (_this.hasClass('on')) {
                    if (i == 0) {
                        $('.fctn').find('a[data-' + _target + '="' + _this.data(_target) + '"]').removeClass('on');
                        isAdd = false;
                    }
                } else {
                    if (i == 0) {
                        if (!_this.parents('.fenu').hasClass('multiple')) {
                            $('.fctn').find('a').removeClass('on');
                        }
                        $('.fctn').find('a[data-' + _target + '="' + _this.data(_target) + '"]').addClass('on');
                        isAdd = true;
                    }
                }

                if (isAdd) {
                    if (_this.parents('.fenu').hasClass('multiple')) {
                        var _res = new Array(0);
                        try {
                            _res = _chk.data(_target).split(',');
                            _res.removeMe('');
                        } catch (e) { }
                        var _change = _this.data(_target) + '';
                        if (_change.trim() != '') {
                            _res.push(_change);
                            _chk.data(_target, _res.joinMe(','));
                        }
                    } else {
                        _chk.data(_target, _this.data(_target));
                    }
                } else {
                    if (_this.parents('.fenu').hasClass('multiple')) {
                        var _res = new Array(0);
                        try {
                            _res = _chk.data(_target).split(',');
                        } catch (e) { }
                        var _change = _this.data(_target) + '';
                        _res.removeMe(_change);
                        _chk.data(_target, _res.joinMe(','));
                    } else {
                        _chk.data(_target, '');
                    }
                }
            }
        });
    }
}

$(function () {

    /** 旧菜单 **/
    if (typeof (p) != "undefined") {
        var $mp = $('#m' + p);
        if($mp.length > 0){
            $mp.addClass('hp-active');
            $('ul.admin-sidebar-sub').removeClass('am-in');
            var $pnt = $mp.parents('ul.admin-sidebar-sub');
            if ($pnt.length > 0) {
                $pnt.addClass('am-in')
            }
        }
    }

    /** 旧成员选择器 **/
    var fmenus = $('.fenu');
    if (fmenus.length > 0) {
        if ($(window).width() > 640) {
            $('.fenu>ul').niceScroll({
                cursorcolor: "rgba(0,0,0,0.1)",
                cursorwidth: "3px",
                cursorborder: 'none',
                horizrailenabled: false,
                zindex: 9,
            });
        }
        setFmenus();
    }
});

/**
 * 删除掉此方法的所有调用
 * 然后删除掉此方法
 * */
var reScroll = function () { }

var reScrollObj = function ($obj) {
    if ($obj.getNiceScroll().length > 0) {
        $obj.getNiceScroll().resize();
    }
}

/**
 * 删除掉此方法的所有调用
 * 然后删除掉此方法
 * */
var removeScroll = function () { }

var showfenu = function ($fenu, x, y) {
    $('.fenu-close').not($fenu.find('.fenu-close')).each(function () {
        var $fenu = $(this).parents('.fenu');
        $fenu.removeClass('strong');
        setTimeout(function () {
            $fenu.hide();
        }, 300);
    });
    $('.fenu').removeClass('strong').hide();
    if (x + $fenu.width() + 32 > document.body.clientWidth) {
        x = document.body.clientWidth - $fenu.width() - 64;
    }
    if (y + $fenu.height() + 32 > document.body.clientHeight) {
        y = document.body.clientHeight - $fenu.height() - 64;
    }
    $fenu.removeAttr('style');
    $fenu.css({ 'left': x, 'top': y });
    $fenu.show();
    $fenu.addClass('strong');
    var $feul = $('.fenu>ul');
    if (document.body.clientWidth > 640) {
        setTimeout(function () {
            $feul.getNiceScroll().resize();
        }, 300);
    } else {
        removeScroll();
        if ($feul.getNiceScroll().length > 0) {
            $feul.getNiceScroll().remove();
            $feul.removeAttr('style').show();
        }
    }
}
