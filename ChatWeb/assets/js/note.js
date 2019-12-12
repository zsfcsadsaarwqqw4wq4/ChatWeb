(function ($) {
    $.fn.editor = function (props) {
        return new FroalaEditor($(this).selector, props)
    }
})(jQuery)

//所需对象参数
var props = {
    language: 'zh_cn',
    quickInsertEnabled: false,
    events: {
        'focus': function () {
            this.toolbar.show()
            this.$wp.find('.fr-view').css('background', '#F8F8F8');
            //this.html.set(this.myText)
        },
        'blur': function () {
            this.toolbar.hide()
            this.$wp.find('.fr-view').css('background', '');
            this.myText = getFroalaContent(this);
            var id = this.$box.parent().attr("data-idx");
            var nid = $("#sub-note").find("div[class*='selected']").attr("data-note-id");
            $.ajax({
                type: 'post',
                cache: false,
                url: "/Ajax/NoteEdit",
                data: { 'type': '1', 'Nid': nid, 'vid': id, 'val': this.myText },
                dataType: 'text',
                async: true,
                success: function (data) {

                }
            });

            //SaveNotes("Note" + id, this.myText);
        },
        'initialized': function () {
            this.$second_tb.remove();
            this.$wp.attr('style', 'border:0; border-bottom-color:#eee')
            this.$tb.css('border-radius', '0')
            this.toolbar.hide()
        },
        'keyup': function () {
        }
    },
    imageUploadURL: 'https://work.damaiking.com/handler/FroalaHandler.ashx',
}

//FroalaEditor对象集合
var listEditor = []

var num = 0
$(function () {
    var $modal = $('#mod-confirm');
    var $task = $('#mod-task');
    var $alert = $('#mod-alert');
    var $note = $("#mod-add-note");
    //添加笔记
    $('.wp-add-content').click(function () {
        var $dom = $(`<div class="wp-editors" data-idx="${num}"></div>`);
        $dom.append(
            `<div id="editorDiv${num}"></div>` +
            `<div class=wp-btn>` +
                `<div class=wp-task><a class=wp-textbox href=javascript:;><i class="iconfont icon-task-new"></i><p class=wp-text>创建任务</p></a></div>` +
                `<div class=wp-delete><a class=wp-textbox href=javascript:;><i class="iconfont icon-trash"></i><p class=wp-text>删除笔记</p></a></div>` +
            `</div>`
        );
        $('#main').append($dom);
        listEditor.push($('#editorDiv' + num++).editor(props));

        //打开任务的按钮
        //图标是"icon-jump"
        //名字为"打开任务"
    })

    $('.wp-add-note').click(function () {

        $note.wModal({
            targetEle: this,
            onConfirm: function (target) {
                var name = $($note).find("input[class='note-ctn']").val();
                $.ajax({
                    type: 'post',
                    cache: false,
                    url: "/Ajax/NoteEdit",
                    data: { 'type': '0', 'name': name },
                    dataType: 'text',
                    async: true,
                    success: function (data) {
                        if (data == "" || data == null) {
                            $alert.wModal({
                                title: "添加笔记失败",
                                content: '点击返回界面',
                                targetEle: target,
                                onConfirm: function (target) {

                                }
                            });
                        } else {
                            $("#sub-note").prepend(data);
                            NoteEvent();

                        }
                    }
                });

            }
        });

        //$.ajax({
        //    type: 'post',
        //    cache: false,
        //    url: "/Ajax/NoteEdit",
        //    data: { 'type': '0', 'name': "new测试表" },
        //    dataType: 'text',
        //    async: true,
        //    success: function (data) {
        //        $("#sub-note").append(data);
        //        NoteEvent();
        //    }
        //});

        //var $dom = $(`<div class="wp-editors" data-idx="${num}"></div>`);
        //$dom.append(
        //    `<div id="editorDiv${num}"></div>` +
        //    `<div class=wp-btn>` +
        //    `<div class=wp-task><a class=wp-textbox href=javascript:;><i class="iconfont icon-task-new"></i><p class=wp-text>创建任务</p></a></div>` +
        //    `<div class=wp-delete><a class=wp-textbox href=javascript:;><i class="iconfont icon-trash"></i><p class=wp-text>删除笔记</p></a></div>` +
        //    `</div>`
        //);
        //$('#main').append($dom);
        //listEditor.push($('#editorDiv' + num++).editor(props));

        //打开任务的按钮
        //图标是"icon-jump"
        //名字为"打开任务"
    })

    //删除笔记内容
    $('#main').on('click', '.wp-delete a', function () {
        $modal.wModal({
            targetEle: this,
            onConfirm: function (target) {
                var id = $(target).parents(".wp-editors").attr("data-idx");
                $.ajax({
                    type: 'post',
                    cache: false,
                    url: "/Ajax/NoteEdit",
                    data: { 'type': '2', 'Nid': $("#sub-note").children("class*='selected'").attr("data-note-id"), 'vid': id },
                    dataType: 'json',
                    async: true,
                    success: function (data) {
                        if (data.res == "1") {
                            $alert.wModal({
                                title: '删除成功',
                                content: '点击返回界面',
                                targetEle: target,
                                onConfirm: function (target) {
                                    var $target = $(target);
                                    var $eBox = $target.parents('.wp-editors');
                                    var _idx = $eBox.data('idx');
                                    $eBox.remove();
                                    listEditor.splice(_idx, 1);
                                    reSetIndex();
                                }
                            });
                        }
                    }
                });

            }
        });
    });

    //点击任务按钮
    $('#main').on('click', '.wp-task a', function () {
        var $this = $(this);
        var $eBox = $this.parents('.wp-editors');
        var _idx = $eBox.data('idx');
        var $editor = listEditor[_idx];
        //$task.wModal({
        //    targetEle: this,
        //    onConfirm: function (target) {
        //        //var $target = $(target);
        //        //var $eBox = $target.parents('.wp-editors');
        //        //var _idx = $eBox.data('idx');
        //        //$eBox.remove();
        //        //listEditor.splice(_idx, 1);
        //        //reSetIndex();
        //        var data = $(target).find("input").val();
        //    }
        //});
        //当前行的文本内容
        console.log($editor.myText);
    });

    var reSetIndex = function () {
        var _idx = 0;
        $('.wp-editors').each(function () {
            $(this).data('idx', _idx++);
        })
    }
})

function DelTask(num) {
    //删除一个任务
    var id = $(num).parent().attr("data-idx");
    DelNote("Note" + parseInt(id));
}

function AddTask(obj, value) {
    var $dom = $(`<div class="wp-editors" data-idx="${obj}"></div>`);
    $dom.append(
        `<div id="editorDiv${obj}"></div>` +
        `<div class=wp-btn>` +
        `<div class=wp-task><a class=wp-textbox href=javascript:;><i class="iconfont icon-task-new"></i><p class=wp-text>创建任务</p></a></div>` +
        `<div class=wp-delete><a class=wp-textbox href=javascript:;><i class="iconfont icon-trash"></i><p class=wp-text>删除笔记</p></a></div>` +
        `</div>`
    );
    $('#main').append($dom);
    var newprops = {
        language: 'zh_cn',
        quickInsertEnabled: false,
        events: {
            'focus': function () {
                this.toolbar.show()
                this.$wp.find('.fr-view').css('background', '#F8F8F8');
                //this.html.set(this.myText)
            },
            'blur': function () {
                this.toolbar.hide()
                this.$wp.find('.fr-view').css('background', '');
                this.myText = getFroalaContent(this);
                var id = this.$box.parent().attr("data-idx");
                var nid = $("#sub-note").children("class*='selected'").attr("data-note-id");
                $.ajax({
                    type: 'post',
                    cache: false,
                    url: "/Ajax/NoteEdit",
                    data: { 'type': '1', 'Nid': nid, 'vid': id, 'val': this.myText },
                    dataType: 'text',
                    async: true,
                    success: function (data) {

                    }
                });
            },
            'initialized': function () {
                this.$second_tb.remove();
                this.$wp.attr('style', 'border:0; border-bottom-color:#eee')
                this.$tb.css('border-radius', '0')
                this.toolbar.hide()
                this.html.set(value)
            },
            'keyup': function () {
            }
        },
        imageUploadURL: 'https://work.damaiking.com/handler/FroalaHandler.ashx',
    }
    listEditor.push($('#editorDiv' + obj).editor(newprops));
    //, function () { this.html.set(value); this.toolbar.hide() }
}

function Bind(obj) {
    listEditor.push($('#editorDiv' + obj).editor(props));
}

