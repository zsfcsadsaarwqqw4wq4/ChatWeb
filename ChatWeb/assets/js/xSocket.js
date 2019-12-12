function xSocket(url) {
    this.connectURL = url || "";
    this.time = 6000;
    this.heartMsg = "HeartCheck";
    this.timeoutObj = null;
    this.serverTimeoutObj = null;
    this.isDestroy = false;
    this.onopen = function (event) { };
    this.onmessage = function (event) { };
    this.onerror = function (event) { };
    this.onclose = function (event) { };
    this.webSocketObj = new WebSocket(url);
}

xSocket.fn = xSocket.prototype = {
    create: function (obj) {
        if (obj) {
            $.extend(true, this, obj);
        }
        var websocket = this.webSocketObj;
        var currentThis = this;
        websocket.onopen = function (evnt) {
            currentThis.onopen(evnt);
        };
        websocket.onmessage = function (evnt) {
            currentThis.onmessage(evnt);
        };
        websocket.onerror = function (evnt) {
            currentThis.onerror(evnt);
        };
        websocket.onclose = function (evnt) {
            currentThis.onclose(evnt);
            currentThis.reconnect();
            currentThis.afterReconnect();
        };
    },
    destroy: function () {
        clearTimeout(this.timeoutObj);
        clearTimeout(this.serverTimeoutObj);
        this.isDestroy = true;
        this.webSocketObj.close();
    },
    heartStart: function (time) {
        if (this.webSocketObj.readyState != 1) {
            return false;
        }
        if (time) {
            this.time = time;
        }
        var self = this;
        this.timeoutObj = setTimeout(function () {
            self.webSocketObj.send(self.heartMsg);
        }, this.time);
        this.serverTimeoutObj = self.serverTimeoutObj;
    },
    heartReset: function () {
        clearTimeout(this.timeoutObj);
        clearTimeout(this.serverTimeoutObj);
        var _time = this.time;
        this.heartStart(_time);
    },
    reconnect: function () {
        if (this.timeoutObj) {
            clearTimeout(this.timeoutObj);
            clearTimeout(this.serverTimeoutObj);
        }
        var wsurl = this.connectURL;
        this.webSocketObj = new WebSocket(wsurl);
        this.isDestroy = false;
        this.create();
    },
    afterReconnect: function () {

    }
}