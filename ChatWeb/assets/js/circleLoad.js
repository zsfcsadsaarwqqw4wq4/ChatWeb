function loadCircle(id, percent, width, range) {
    if (percent < 1) {
        percent *= 100;
    }
    var c = document.getElementById(id);
    var ctx = c.getContext("2d");
    ctx.clearRect(0, 0, width, width);
    var cx = width / 2;
    var cy = width / 2;
    var lineWidth = 3;
    var cr = range;
    ctx.beginPath();
    ctx.arc(cx, cy, cr, -0.5 * Math.PI, -0.5 * Math.PI + percent / 100 * 2 * Math.PI, false);
    ctx.lineWidth = lineWidth;

    var grad = ctx.createLinearGradient(0, 0, 0, 100);
    grad.addColorStop(0, '#3f95ea');
    grad.addColorStop(1, '#52d3aa');

    ctx.strokeStyle = grad;
    ctx.lineCap = 'round';

    ctx.stroke();
}

var ctim = null;
function loadCircleDynamic(id, width, range, totalSec, inlSec) {
    var sp = 0;
    if (ctim != null) {
        clearInterval(ctim);
    }
    ctim = setInterval(function () {
        loadCircle(id, sp, width, range);
        sp += inlSec / totalSec;
        if (sp >= 1) {
            clearInterval(ctim);
            ctim = null;
        }
    }, inlSec * 1000);
}

function PlayAudio($ptn, _url) {
    var $ados = document.getElementsByTagName('audio');
    var _tmpSg = _url.split('/');
    var _name = _tmpSg[_tmpSg.length - 1].split('.')[0];
    var $ados = $('body').children('audio[name!="' + _name + '"]');
    $ados.each(function () {
        this.pause();
    });
    $('.cado>a').removeClass('am-icon-volume-off am-icon-circle-o-notch am-icon-spin am-icon-volume-up').addClass('am-icon-volume-off');
    var $ado = $('body').children('audio[name="' + _name + '"]')[0];
    if (typeof ($ado) == 'undefined') {
        if ($ptn.hasClass('am-icon-volume-off')) {
            $ptn.removeClass('am-icon-volume-off').addClass('am-icon-circle-o-notch am-icon-spin');

            $ado = document.createElement("audio");
            $ado.src = _url;
            $ado.setAttribute("name", _name);

            $('body').append($ado.outerHTML);

            $ado = $('body').children('audio[name="' + _name + '"]')[0];
            $ado.addEventListener("canplaythrough", function () {
                if ($ptn.hasClass('am-icon-circle-o-notch')) {
                    $ado.play();
                }
                $ptn.removeClass('am-icon-volume-off am-icon-circle-o-notch am-icon-spin').addClass('am-icon-volume-up');
            }, false);

            $ado.addEventListener('ended', function () {
                $ptn.removeClass('am-icon-volume-up am-icon-circle-o-notch am-icon-spin').addClass('am-icon-volume-off');
            });

            $ado.load();
        }
    } else {
        if ($ado.paused) {
            $ado.currentTime = 0.0;
            $ado.play();
            $ptn.removeClass('am-icon-volume-off am-icon-circle-o-notch am-icon-spin').addClass('am-icon-volume-up');
        } else {
            $ado.pause();
            $ptn.removeClass('am-icon-volume-up am-icon-circle-o-notch am-icon-spin').addClass('am-icon-volume-off');
        }
    }
}