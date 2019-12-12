(function ($) {
    $.fn.ScaleBar = function (options) {
        this.each(function () {
            var obj = this;
            var $this = $(obj);
            let minPercent = options.minPercent;
            let customName = options.customName;
            let callback_click = options.onClick;
            let data = options.data;
            $this.append(`<div class="br-box"></div>`);
            for (item of data) {
                let percent = item.num + '%';
                if (item.num / 100 < minPercent / 100) {
                    $this.find('.br-box').append(`<div class="br-changebig br-small" data-name="${item.name}" title="${item.name}: ${percent}" style="width:${percent};"><span><em style="width:${percent};">${item.name}<br>${percent}</em></span><b></b></div>`)
                } else {
                    $this.find('.br-box').append(`<div class="br-changebig" data-name="${item.name}" title="${item.name}: ${percent}" style="width:${percent};"><span><em style="width:${percent};">${item.name}<br>${percent}</em></span><b></b></div>`)
                }
            }
            $this.on('click', '.br-changebig', function () {
                var $e = $(this);
                callback_click($e.index(), customName, $e.data('name'));
            });
            return;
        });
    };
})(jQuery);

