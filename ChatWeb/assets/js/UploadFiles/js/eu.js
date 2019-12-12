; (function ($, window, document, undefined) {
    $.fn.easyUpload = function (options) {
        if (typeof options == 'string') {
            var otherArgs = Array.prototype.slice.call(arguments, 1);
            var fn = tooltip[options];
            if ($.isFunction(fn)) {
                var otherArgs = Object.values(otherArgs);
                otherArgs.unshift(this);
                return fn.apply(this, otherArgs);
            } else {
                $.error('Method ' + options + ' does not exist on jQuery.EasyUpload');
            }
        }

        var ec = function (ele, opt) {
            this.$e = ele,
            this.defaults = {
                url: "", //文件上传路径
                fileSize: 1024*1024*20, //文件上传大小
                multiple: true, //是否允许多个文件上传
                uploadNumber:"10",//最大上传文件个数
                fileType: [], //文件上传格式
                hideSelBtn: false, //是否隐藏控件的上传按钮
                specified: "",  //自定义上传按钮
                onCode: function () { }, //点击自定义按钮时触发的事件
            },
            this.options = $.extend({}, this.defaults, opt),

            // 将文件的单位由bytes转换为KB或MB，若第二个参数指定为true，则永远转换为KB
            this.formatFileSize = function (size, justKB) {
                if (size > 1024 * 1024 && !justKB) {
                    size = (Math.round(size * 100 / (1024 * 1024)) / 100).toString() + 'MB';
                } else {
                    size = (Math.round(size * 100 / 1024) / 100).toString() + 'KB';
                }
                return size;
            },
            // 将输入的文件类型字符串转化为数组,原格式为*.jpg;*.png
            this.getFileTypes = function (str) {
                var result = [];
                var arr = str.split(";");
                for (var i = 0, len = arr.length; i < len; i++) {
                    result.push(arr[i].split(".").pop());
                }
                return result;
            },
            //将指定文件类型添加到file标签中
            this.acceptType = function (tp) {
                var apn = ["jpeg", "jpg", "jpe", "tif", "tiff", "gif", "jp2", "png", "css", "csv", "htm", "html", "javascript", "txt", "xml", "3gpp", "ac3", "au", "asf", "doc", "dot", "dtd", "pot", "ppt", "rtf", "xlc", "xlm", "xlw", "xlsx", "zip", "json", "mpp", "ogg", "pdf", "wps", "xhtml", "xls", "xlt", "xlw", "rar", "z", "arj", "exe"];
                if (apn.indexOf(tp)) {
                    return "." + tp;
                }
                return "";
            }
        }
        var eo = new ec(this, options);
        this.each(function (index, element) {
            var lastIndex;
            var a = 1;
            var upAjax;
            var uploadSize = 0;
            var totalSize = 0;
            var selectedFiles = [];//定义变量存储未上传文件
            var uploadFiles = [];//定义变量存储已上传文件（不管上传是否成功）
            var opt = eo.options;

            // 实例化相关变量
            var $e = $(element);
            //将指定文件类型全部统一为小写
            for (var i = 0; i < opt.fileType.length; i++) {
                opt.fileType[i] = opt.fileType[i].toLowerCase();
            }
            var _html = '';
            _html = '<div class="eu-container">';
            _html += '<div class="eu-head"> ';
            _html += '<input type="file" class="fileInput" data-count="0" style="display: none;" ';
            _html += opt.multiple ? "multiple" : "";
            if (opt.fileType.length > 0) {
                _html += ' accept="';
                for (var i = 0; i < opt.fileType.length; i++) {

                    _html += eo.acceptType(opt.fileType[i]);
                    if (i < opt.fileType.length - 1) {
                        _html += ",";
                    };
                }
                _html += '" ';
            }
            _html += '/>';
            _html += '<div>';
            if (!opt.hideSelBtn) {
                _html += '<span class="eu_select noselect">选择文件</span> ';
                _html += '<span class="eu_fc"><span class="s1">0</span><span>/</span><span class="s2">0</span></span>';
            }
            _html += '</div>';
            _html += '<p class="eu_progress"> ';
            _html += '<span class="eu_bar"></span></p> ';
            if (opt.hideSelBtn) {
                _html += '<p class="eu_percent">0%<span class="eu_fc eu_fc_left"><span class="s1">0</span><span>/</span><span class="s2">0</span></span></p>';
            } else {
                _html += '<p class="eu_percent">0%</p>';
            }
            _html += '</div> ';
            _html += '<ul class="queue"></ul> ';
            _html += '</div> ';
            _html += '<input type="hidden" class="file_urls" name="file" value="" />';
            _html += '<input type="hidden" class="file_names" name="fileNames" value="" /> ';
            $e.html(_html);

            //初始化按钮
            $e.find(".eu_select").on("click", function () {
                if ($e.find(".fileInput").data('sta') != "0") {
                    opt.onCode();
                    $e.find('.fileInput').trigger("click");
                }
            });
            //自定义的上传按钮
            var $ub = $(opt.specified);
            if ($ub.length > 0) {
                $ub.on("click", function () {
                    if ($e.find(".fileInput").data('sta') != "0") {
                        opt.onCode();
                        $e.find('.fileInput').trigger("click");
                    }
                });
            }
            $e.find('.fileInput').off('change').on('change', function () {
                if ($e.find(".queue").find("li").length == 0) {
                    a = 1;
                    uploadSize = 0;
                    totalSize = 0;
                    selectedFiles = [];
                    uploadFiles = [];
                }
                if (opt.multiple) {
                    var count = Number($(this).attr('data-count'));
                    var fileArr = [];
                    var files = this.files;
                    var filenumber = 0;
                    if (parseInt(opt.uploadNumber) > 0) {
                        filenumber = parseInt(opt.uploadNumber) - parseInt($e.find(".s2").text());
                        if (filenumber >= files.length)
                            filenumber = files.length;
                        else alert("文件个数限制为"+opt.uploadNumber+"个，已超出上限");
                    } else {
                        filenumber = files.length;
                    }
                    for (var i = 0; i < filenumber; i++) {
                        if (files[i].size <= opt.fileSize) {
                            totalSize += files[i].size;
                            //对上传文件进行类型的判断
                            if (opt.fileType.length > 0) {
                                if (opt.fileType.indexOf(files[i].name.split('.').pop().toLowerCase()) != -1) {
                                    var obj = {};
                                    obj.index = count;
                                    obj.file = files[i];
                                    fileArr.push(obj);
                                    // 用对象将所有选择文件存储起来
                                    selectedFiles[Object.keys(selectedFiles).length + 1] = obj;
                                    render(obj.file, i);
                                }
                            } else {
                                var obj = {};
                                obj.index = count;
                                obj.file = files[i];
                                fileArr.push(obj);
                                // 用对象将所有选择文件存储起来
                                selectedFiles[Object.keys(selectedFiles).length + 1] = obj;
                                render(obj.file, i);
                            }
                        } else {
                            alert(files[i].name + "文件超出上限");
                        }
                    }
                    var arr = Object.keys(selectedFiles);
                    var arr1 = Object.keys(uploadFiles);
                    //显示所选文件个数
                    $e.find(".s2").text(parseInt(arr.length) + parseInt($e.find(".s2").text()));
                    if (Object.keys(selectedFiles).length > 0) {
                        fileSubmit();
                    }
                } else {
                    if ($e.find(".s2").text() < 1) {
                        var count = Number($(this).attr('data-count'));
                        var fileArr = [];
                        var files = this.files;
                        for (var i = 0; i < files.length; i++) {
                            if (files[i].size <= opt.fileSize) {
                                if (opt.fileType.length > 0) {
                                    totalSize += files[i].size;
                                    //对上传文件进行类型的判断
                                    if (opt.fileType.indexOf(files[i].name.split('.').pop()) != -1) {
                                        var obj = {};
                                        obj.index = count;
                                        obj.file = files[i];
                                        fileArr.push(obj);
                                        // 用对象将所有选择文件存储起来
                                        selectedFiles[Object.keys(selectedFiles).length + 1] = obj;
                                        render(obj.file, i);
                                    }
                                } else {
                                    var obj = {};
                                    obj.index = count;
                                    obj.file = files[i];
                                    fileArr.push(obj);
                                    // 用对象将所有选择文件存储起来
                                    selectedFiles[Object.keys(selectedFiles).length + 1] = obj;
                                    render(obj.file, i);
                                }
                            }
                            else {
                                alert(files[i].name + "文件超出上限");
                            }
                        }
                        var arr = Object.keys(selectedFiles);
                        var arr1 = Object.keys(uploadFiles);
                        //显示所选文件个数
                        $e.find(".s2").text(parseInt(arr.length) + parseInt($e.find(".s2").text()));
                        if (Object.keys(selectedFiles).length > 0) {
                            fileSubmit();
                        }
                    }
                }
            });

            function render(file, i) {
                var _ul = $e.find(".queue");
                var _li = _ul.find("li").last();
                if (_li.length != 0) i = parseInt(_li.attr("data-index")) + 1;
                var preview;
                var fileType = file.name.split('.').pop();
                if (fileType == 'bmp' || fileType == 'jpg' || fileType == 'jpeg' || fileType == 'png' || fileType == 'gif') {
                    var imgSrc = URL.createObjectURL(file);
                    preview = '<div class="eu_img" style="background-image:url(\'' + imgSrc + '\');"></div>';
                } else if (fileType == 'rar' || fileType == 'zip' || fileType == 'arj' || fileType == 'z') {
                    preview = '<div class="eu_tbg" style="background-image:url(\'/assets/i/filetype/zip.png\');"></div>';
                } else {
                    preview = '<div class="eu_tbg" style="background-image:url(\'/assets/i/filetype/others.png\');"></div>';
                }
                var _html = '';
                _html += '<li class="queue_item" data-index="' + i + '">';
                _html += '<div>' + preview;
                _html += '<div class="eu_file">';
                _html += '<p class="eu_filename">' + file.name + '</p>';
                _html += '</p>';
                _html += '</div>';
                _html += '<div class="eu_delete_file">';
                _html += '<i class="am-icon-close"></i><span class="eu_delete_span noselect">删除</span>';
                _html += '</div>';
                _html += '<div class="eu_success_file"><i class="am-icon-check"></i></div>';
                _html += '<div class="eu_error_file"><i class="am-icon-close"></i></div>';
                _html += '</div>';
                _html += '</li>';
                _ul.append(_html).show();

                $e.find('.eu_delete_file').off('click').on('click', function () {
                    var _this = $(this)
                    var _url = _this.data("url");
                    if (Object.keys(selectedFiles).length == 0) {
                        if ($e.find(".s2").text() > 0)
                            $e.find(".s2").text(parseInt($e.find(".s2").text()) - 1);
                        if ($e.find(".s1").text() > 0)
                            $e.find(".s1").text(parseInt($e.find(".s1").text()) - 1);
                        if ($e.find(".s1").text() == 0) {
                            $e.find(".eu_percent").text('0%');
                            $e.find(".eu_bar").css("width", "0%");
                        }
                        var name = _this.data("name");
                        var url = _this.data("url");
                        var _fileName = $(".file_names");
                        if (!!name) {
                            if (_fileName.val().length > name.length) {
                                if (_fileName.val().indexOf(name) == _fileName.val().length - name.length)
                                    name = "|&|" + name;
                                else
                                    name = name + "|&|";
                            }
                            var _fileUrl = $(".file_urls");
                            if (_fileUrl.val().length > url.length) {
                                if (_fileUrl.val().indexOf(url) == _fileUrl.val().length - url.length)
                                    url = "|&|" + url;
                                else
                                    url = url + "|&|";
                            }
                            _fileName.val(_fileName.val().replace(name, ""));
                            _fileUrl.val(_fileUrl.val().replace(url, ""));
                            _this.parents("li").remove();
                            lastIndex = $e.find(".queue>li:last").data("index");

                            $.ajax({
                                url: opt.url + "?del=1&&url=" + _url,
                                type: "post",
                                datatype: "json",
                                cache: false,
                                processData: false,
                                contentType: false,
                                async: true,
                                success: function (res) {
                                    console.log('Delete File: ' + res);
                                }
                            });
                        }
                    }
                });
            };
            //实时上传文件
            function fileSubmit() {
                if (a < 1) a = 1;
                if (a > Object.keys(selectedFiles).length) {
                    a = 1;
                    uploadSize = 0;
                    totalSize = 0;
                    selectedFiles = [];
                    uploadFiles = [];
                    $e.find('.fileInput:eq(0)').val("");
                    var _lis = $e.find(".queue>li");
                    if (_lis.length > 0) {
                        lastIndex = $e.find(".queue>li:last").data("index");
                    } else {
                        lastIndex = 0;
                    }
                    return;
                }
                var count;
                if (!!lastIndex)
                    count = lastIndex + a;
                else
                    count = a - 1;
                var fileArr = [];
                var obj = {};
                obj.index = count;
                var vb = selectedFiles[a];
                obj.file = selectedFiles[a];
                fileArr.push(obj);
                uploadFiles[Object.keys(uploadFiles).length + 1] = obj;
                bb(selectedFiles[a], count, getGuid());
                a++;
            }
            //上传文件方法
            function bb(file, count, guid) {
                if (file != null) {
                    var sindex = 0;
                    var filesize = file != null ? file.file.size : 0;
                    var filename = file != null ? file.file.name : "";
                    var tmp = filename.split('.');
                    var fd = new FormData();
                    fd.append("filename",  guid + '.' + tmp[tmp.length - 1]);
                    fd.append("file", file.file, filename);
                  
                    upAjax = $.ajax({
                        url: opt.url,
                        type: "post",
                        datatype: "json",
                        data: fd,
                        cache: false,
                        processData: false,
                        contentType: false,
                        async: true,
                        success: function (res) {
                            var res = JSON.parse(res);
                            if (res.mo) {
                                uploadSize += filesize;
                                var _div = $e.find("li[data-index=" + count + "]>div");
                                _div.find(".eu_delete_file").data("url", res.url);
                                _div.find(".eu_delete_file").data("name", filename);

                                //将文件路径添加进用于上传的隐藏控件中（|&|分割）
                                if ($e.find(".file_urls").val() != "") {
                                    $e.find(".file_urls").val($e.find(".file_urls").val() + "|&|");
                                }
                                $e.find(".file_urls").val($e.find(".file_urls").val() + res.url);
                                //将原本的文件名添加进隐藏控件中（|&|分割）
                                if ($e.find(".file_names").val() != "") {
                                    $e.find(".file_names").val($e.find(".file_names").val() + "|&|");
                                }
                                $e.find(".file_names").val($e.find(".file_names").val() + filename);

                                $e.find(".s1").text(parseInt($e.find(".s1").text()) + 1);
                                _div.find('.eu_success_file').show();
                                fileSubmit();
                            } else {
                                var _div = $e.find("li[data-index=" + count + "]>div");
                                _div.find('.eu_error_file').show();
                                selectedFiles.splice(parseInt(count), 1);
                                a--;
                                upAjax.abort();
                                fileSubmit();
                            }
                        },
                        error: function () {
                            var _div = $e.find("li[data-index=" + count + "]>div");
                            _div.find('.eu_error_file').show();
                            selectedFiles.splice(parseInt(count), 1);
                            a--;
                            upAjax.abort();
                            fileSubmit();
                        },
                        xhr: function (evt) {
                            var xhr = $.ajaxSettings.xhr();
                            if (onprogress && xhr.upload) {
                                xhr.upload.addEventListener("progress", onprogress, false);
                                return xhr;
                            }
                        }
                    });
                }
            }
            //获取已经上传的进度
            function onprogress(evt) {
                var loaded = evt.loaded;
                var total = 0;
                var percent = 0;
                if (Object.keys(selectedFiles).length > 1) {
                    total = totalSize;
                } else {
                    total = evt.total;
                }
                if (loaded > total) {
                    loaded = total;
                }
                var uu = uploadSize + loaded;
                if (uu > total) {
                    uu = total;
                }
                percent = Math.floor(100 * (uploadSize + loaded) / total);
                $e.find(".eu_percent").text(percent + '%');
                $e.find(".eu_bar").css("width", percent + "%");
            };
        });
    }

    var tooltip = {
        reset: function ($e) {
            $e.find(".s1").text("0");
            $e.find(".s2").text("0");
            $e.find(".eu_percent").text('0%');
            $e.find(".eu_bar").removeAttr("style");
            $e.find(".queue").find("li").remove();
            $e.find(".fileInput").val("");
            $e.find(".file_urls").val("");
            $e.find(".file_names").val("");
            return true;
        },
        setSta: function ($e, _sta) {
            var flag = _sta == 'true';
            if (!flag) {
                flag = _sta == '1';
            }
            $e.find('.fileInput').data("sta", flag ? 1 : 0);
        },
        isUploadFinished: function ($e) {
            if ($e.find(".fileInput").length > 0)
                return $e.find(".fileInput")[0].files.length == 0;
            else
                return false;
        },
        upload: function ($e, url, file, dragTab, params, backcall, errcall) {
            if (file != null) {
                var guid = getGuid();
                var sindex = 0;
                var filesize = file != null ? file.size : 0;
                var filename = file != null ? file.name : "";
                var tmp = filename.split('.');
                var fd = new FormData();
                fd.append("filename", guid + '.' + tmp[tmp.length - 1]);
                fd.append("file", file, filename);

                var fileunit = 1;
                var tmpsize = filesize;
                while (tmpsize > 1024) {
                    tmpsize = (tmpsize / 1024).toFixed(1);
                    fileunit++;
                }
                if (fileunit == 1) {
                    fileunit = 'B';
                } else if (fileunit == 2) {
                    fileunit = 'KB';
                } else if (fileunit == 3) {
                    fileunit = 'MB';
                } else if (fileunit == 4) {
                    fileunit = 'GB';
                } else if (fileunit == 5) {
                    fileunit = 'TB';
                }
                if (!dragTab.hasClass('on')) {
                    dragTab.addClass('on top');
                }
                dragTab.append('<div class="' + guid + '"><div><span class="eu_name">' + filename + ' </span><small>（' + tmpsize + fileunit
                    + '）<span class="eu_percent">0%</span></small></div><p class="eu_progress"> <span class="eu_bar"></span></p></div>');
                var $ps = dragTab.find('.' + guid);

                $.ajax({
                    url: url,
                    type: "post",
                    datatype: "json",
                    data: fd,
                    cache: false,
                    processData: false,
                    contentType: false,
                    async: true,
                    success: function (res) {
                        setTimeout(function () {
                            $ps.addClass('close');
                            setTimeout(function () {
                                $ps.remove();
                                if (dragTab.children('div').length == 0) {
                                    dragTab.removeClass('on');
                                    setTimeout(function () {
                                        dragTab.removeClass('top');
                                    }, 300);
                                }
                            }, 300);
                        }, 1000);
                        backcall(JSON.parse(res), filename, params);
                    },
                    error: function (msg) {
                        if (!!errcall) {
                            errcall(msg)
                        }
                    },
                    xhr: function (evt) {
                        var xhr = $.ajaxSettings.xhr();
                        if (onprogress && xhr.upload) {
                            xhr.upload.addEventListener("progress", onprogress, false);
                            return xhr;
                        }
                    }
                });

                //获取已经上传的进度
                function onprogress(evt) {
                    var loaded = evt.loaded;
                    var total = evt.total;
                    var percent = 0;
                    if (loaded > total) {
                        loaded = total;
                    }
                    percent = Math.floor(100 * loaded / total);
                    $ps.find(".eu_percent").text(percent + '%');
                    $ps.find(".eu_bar").css("width", percent + "%");
                };
            }

        }
    }
}(jQuery));