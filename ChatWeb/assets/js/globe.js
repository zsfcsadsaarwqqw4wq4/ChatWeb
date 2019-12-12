String.prototype.trim = function () {
    return this.replace(/(^\s*)|(\s*$)/g, "");
}
String.prototype.ltrim = function () {
    return this.replace(/(^\s*)/g, "");
}
String.prototype.rtrim = function () {
    return this.replace(/(\s*$)/g, "");
}
String.prototype.startWith = function (str) {
    var reg = new RegExp("^" + str);
    return reg.test(this);
}
String.prototype.endWith = function (str) {
    var reg = new RegExp(str + "$");
    return reg.test(this);
}

Array.prototype.indexOf = function (val) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == val) return i;
    }
    return -1;
};
Array.prototype.addMe = function (val) {
    var index = this.indexOf(val);
    if (index < 0) {
        this.push(val);
    }
};
Array.prototype.removeMe = function (val) {
    var index = this.indexOf(val);
    if (index > -1) {
        this.splice(index, 1);
    }
};
Array.prototype.joinMe = function (val) {
    var res = '';
    for (var i = 0; i < this.length; i++) {
        res += this[i];
        res += ',';
    }
    return res.slice(0, -1);
};

var HtmlEncode = function (str) {
    var s = "";
    if (str.length == 0) return " ";
    for (var i = 0; i < str.length; i++) {
        switch (str.substr(i, 1)) {
            case "<": s += "&lt;"; break;
            case ">": s += "&gt;"; break;
            case "\'": s += "&apos;"; break;
            case "\"": s += "&quot;"; break;
            //case " ": s += "&nbsp;"; break;
            default: s += str.substr(i, 1); break;
        }
    }
    return s;
}
var HtmlDecode = function (str) {
    if (str.length == 0) return " ";
    str.replace(/&lt;/g, '<');
    str.replace(/&gt;/g, '>');
    str.replace(/&apos;/g, '\'');
    str.replace(/&quot;/g, "\"");
    //str.replace(/&nbsp;/g, ' ');
    return str;
}

function scrollToObjGeneral($obj) {
    var mainContainer = $('html');
    mainContainer.getNiceScroll().doScrollPos($obj.offset().top + mainContainer.scrollTop() - 156, 500)
}

function scrollToObj($obj) {
    var mainContainer = $('.admin-content');
    var a = $obj.offset().top;
    var b = mainContainer.offset().top;
    var c = mainContainer.scrollTop();
    var top = a - b + c - 156;
    if (top <= 32)
        top = 0;
    mainContainer.animate({
        scrollTop: top
    }, 500);
}

function hash(_key, _value) {
    this.key = _key;
    this.value = _value;
}

function dictionary() {
    this.items = [];
    this.add = function (_key, _value) {
        this.items[this.items.length] = new hash(_key, _value);
    }
}

function isToday(d) {
    var y1 = d.getFullYear();
    var m1 = d.getMonth() + 1;
    var d1 = d.getDate();

    var n = new Date();
    var y2 = n.getFullYear();
    var m2 = n.getMonth() + 1;
    var d2 = n.getDate();

    var date_str1 = y1 + '-' + m1 + '-' + d1;
    var date_str2 = y2 + '-' + m2 + '-' + d2;
    return date_str1 == date_str2;
}

function dateDiff(dateBegin, dateEnd) {
    var df = dateEnd.getTime() - dateBegin.getTime();//时间差的毫秒数
    var day = Math.floor(df / (24 * 3600 * 1000));//计算出相差天数
    return day;
}

function notifyMsg(title, msg, icon, onClick) {
    title = (title + "").replace(/<[^>]+>/g, "");
    msg = (msg + "").replace(/<[^>]+>/g, "");
    if (title.length > 20) {
        title = title.substr(0, 20) + "...";
    }
    var options = {
        body: msg,
        icon: icon || "/assets/i/favicon.png"
    };

    if (typeof (onClick) == 'undefined') {
        onClick = function () { };
    }

    var Notification = window.Notification || window.mozNotification || window.webkitNotification;
    if (Notification && Notification.permission === "granted") {
        var instance = new Notification(title, options);
        instance.onclick = onClick;
        instance.onerror = function () { };
        instance.onshow = function () {
            setTimeout(function () {
                instance.close();
            }, 5000)
        };
        instance.onclose = function () { };
    } else if (Notification && Notification.permission !== "denied") {
        Notification.requestPermission(function (status) {
            if (Notification.permission !== status) {
                Notification.permission = status;
            }
            if (status === "granted") {
                var instance = new Notification(title, options);
                instance.onclick = onClick;
                instance.onerror = function () { };
                instance.onshow = function () {
                    setTimeout(instance.close, 5000);
                };
                instance.onclose = function () { };
            } else {
                return false
            }
        });
    } else {
        return false;
    }
}

function openWin(url) {
    $('body').append($('<a href="' + url + '" target="_blank" id="openWin"></a>'))
    document.getElementById("openWin").click();
    $('#openWin').remove();
}

function resetModPrompt() {
    if ($('#mod-prompt').length > 0) {
        $('#mod-prompt').remove();
        $('body').append('<div class="am-modal am-modal-prompt" tabindex="-1" id="mod-prompt"><div class="am-modal-dialog"><div class="am-modal-hd"></div><div class="am-modal-bd"></div><div class="am-modal-footer"><span class="am-modal-btn" style="border-radius:0 0 0 5px" data-am-modal-cancel>Cancel</span> <span class="am-modal-btn" style="border-radius:0 0 5px 0" data-am-modal-confirm><i class="am-icon-check"></i>&nbsp;Confirm</span></div></div></div>');
    }
}

function resetModConfirm() {
    if ($('#mod-confirm').length > 0) {
        $('#mod-confirm').remove();
        $('body').append('<div class="am-modal am-modal-confirm" tabindex="-1" id="mod-confirm"><div class="am-modal-dialog"><div class="am-modal-hd"></div><div class="am-modal-bd"></div><div class="am-modal-footer"><span class="am-modal-btn" style="border-radius:0 0 0 5px" data-am-modal-cancel>Cancel</span> <span class="am-modal-btn" style="border-radius:0 0 5px 0" data-am-modal-confirm><i class="am-icon-check"></i>&nbsp;Confirm</span></div></div></div>');
    }
}

function LinkClient(ename, data, key) {
    if (navigator.userAgent == key) {
        data = data.replace(/\\/g, "\\\\");
        data = data.replace(/\"/g, "\\\"");
        eval(ename + '("' + data + '")');
    }
}

function getGuid() {
    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    return (S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
}

var beforeChangeSwitch = function (_this, _val) { return true; }
var afterChangeSwitch = function (_this, _val) { }
var toggleSwtich = function (_this) {
    var _hid = _this.children('input[type="hidden"]');
    if (_this.hasClass('on')) {
        _this.removeClass('on');
        _hid.val('0');
    } else {
        _this.addClass('on');
        _hid.val('1');
    }
}

$(function () {
    SetHpSwitch();
    SetHpTreeSel();
})

function SetHpSwitch() {
    $switchs = $('.hp-switch');
    if ($switchs.length > 0) {
        $switchs.each(function () {
            var _this = $(this);
            var _name = _this.data('name');
            var _val = _this.hasClass('on') ? "1" : "0";
            _this.html('<i></i><input type="hidden" name="' + _name + '" value="' + _val + '">');
        });
        $switchs.on('click', function () {
            var _this = $(this);
            var _hid = _this.children('input[type="hidden"]');
            if (!beforeChangeSwitch(_this, _hid.val())) {
                return;
            }
            toggleSwtich(_this);
            afterChangeSwitch(_this, _hid.val());
        });
    }
}

function SetHpTreeSel() {
    $trees = $('.hp-treeSel');
    if ($trees.length > 0) {
        $trees.each(function () {
            var _this = $(this);
            var _name = _this.data('name');
            var _mul = _this.data('multiple');
            var _val = _this.data('value');
            if (!!!_val) {
                _val = _this.children('li').eq(0).data('value');
            }
            var _iptHtml = '<input type="hidden" name="' + _name + '" value="' + _val + '">';
            _this.after(_iptHtml);
            var $input = _this.next();
            var $lis = _this.find('li');
            var $divs = $lis.children('div');
            var $clear = $lis.filter('[data-clear]');
            $divs.on('click', function () {
                var _this = $(this);
                var _pnt = _this.parent();
                if (_mul == true && !!!_pnt.data('clear')) {
                    var _isOn = _pnt.hasClass('on');
                    var _selVal = _pnt.data('value')
                    _pnt.toggleClass('on');
                    if ($input.val() == '' || $input.val() == $clear.data('value')) {
                        $input.val(',');
                    }
                    if (_isOn) {
                        $input.val($input.val().replace(',' + _selVal + ',', ','));
                    } else {
                        $input.val($input.val().replace(',' + _selVal + ',', ',') + _selVal + ',');
                    }
                    $clear.removeClass('on');
                } else {
                    $lis.removeClass('on');
                    _pnt.addClass('on');
                    $input.val(_pnt.data('value'));
                }
            });
            var _vals = (_val + '').split(',');
            $divs.each(function () {
                var _this = $(this)
                var $li = _this.parent();
                var _crtVal = $li.data('value');
                if (_vals.indexOf(_crtVal) >= 0) {
                    $li.addClass('on');
                }
                if (_this.next('ul').length > 0) {
                    _this.prepend('<i class="am-icon-minus-square-o" title="打开/收缩"></i>');
                }
            });
            $divs.children('i').on('click', function () {
                event.stopPropagation();
                var _this = $(this);
                var _ul = _this.parent().next();
                _this.toggleClass('am-icon-minus-square-o');
                _this.toggleClass('am-icon-plus-square-o');
                if (_ul.hasClass('shrink')) {
                    _ul.height(_ul.data('height'));
                } else {
                    _ul.data('height', _ul.height());
                    if (!!!_ul.attr('style')) {
                        _ul.height(_ul.height());
                    }
                    _ul.height(0);
                }
                _ul.toggleClass('shrink');
                _ul.parents('ul').filter('.hp-treeSel ul').removeAttr('style');
            });
        });
    }
}

$.fn.hpTreeSetVal = function (_val) {
    var _vals = (_val + '').split(',');
    var $lis = this.find('li[data-value]');
    $lis.removeClass('on');
    $lis.each(function () {
        var $li = $(this);
        if (_vals.indexOf($li.data('value')) >= 0) {
            $li.addClass('on');
        }
    });
    this.next().val(',' + _val + ',');
}

var Base64 = {
    _keyStr: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",
    encode: function (input) {
        var output = "";
        var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
        var i = 0;

        input = Base64._utf8_encode(input);

        while (i < input.length) {

            chr1 = input.charCodeAt(i++);
            chr2 = input.charCodeAt(i++);
            chr3 = input.charCodeAt(i++);

            enc1 = chr1 >> 2;
            enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
            enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
            enc4 = chr3 & 63;

            if (isNaN(chr2)) {
                enc3 = enc4 = 64;
            } else if (isNaN(chr3)) {
                enc4 = 64;
            }

            output = output + this._keyStr.charAt(enc1) + this._keyStr.charAt(enc2) + this._keyStr.charAt(enc3) + this._keyStr.charAt(enc4);

        }

        return output;
    },
    decode: function (input) {
        var output = "";
        var chr1, chr2, chr3;
        var enc1, enc2, enc3, enc4;
        var i = 0;

        input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

        while (i < input.length) {

            enc1 = this._keyStr.indexOf(input.charAt(i++));
            enc2 = this._keyStr.indexOf(input.charAt(i++));
            enc3 = this._keyStr.indexOf(input.charAt(i++));
            enc4 = this._keyStr.indexOf(input.charAt(i++));

            chr1 = (enc1 << 2) | (enc2 >> 4);
            chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
            chr3 = ((enc3 & 3) << 6) | enc4;

            output = output + String.fromCharCode(chr1);

            if (enc3 != 64) {
                output = output + String.fromCharCode(chr2);
            }
            if (enc4 != 64) {
                output = output + String.fromCharCode(chr3);
            }

        }

        output = Base64._utf8_decode(output);

        return output;

    },
    _utf8_encode: function (string) {
        string = string.replace(/\r\n/g, "\n");
        var utftext = "";

        for (var n = 0; n < string.length; n++) {

            var c = string.charCodeAt(n);

            if (c < 128) {
                utftext += String.fromCharCode(c);
            } else if ((c > 127) && (c < 2048)) {
                utftext += String.fromCharCode((c >> 6) | 192);
                utftext += String.fromCharCode((c & 63) | 128);
            } else {
                utftext += String.fromCharCode((c >> 12) | 224);
                utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                utftext += String.fromCharCode((c & 63) | 128);
            }

        }

        return utftext;
    },
    _utf8_decode: function (utftext) {
        var string = "";
        var i = 0;
        var c = c1 = c2 = 0;

        while (i < utftext.length) {

            c = utftext.charCodeAt(i);

            if (c < 128) {
                string += String.fromCharCode(c);
                i++;
            } else if ((c > 191) && (c < 224)) {
                c2 = utftext.charCodeAt(i + 1);
                string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
                i += 2;
            } else {
                c2 = utftext.charCodeAt(i + 1);
                c3 = utftext.charCodeAt(i + 2);
                string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
                i += 3;
            }
        }

        return string;
    }
}
