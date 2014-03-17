define("socket", [], function (Messages) {


    function Socket(url) {
        this.msg = new Array();
        this.con;
        this.url = url;

    }
    Socket.constructor = Socket;

    Socket.prototype.connect = function (url) {
        //this.con = new WebSocket("ws://hybel.keel.no:8140");
        this.con = new WebSocket(this.url);
    }


    // Handle any errors that occur.
    Socket.prototype.onError = function (callback) {
        this.con.onerror = function (error) {
            console.log('WebSocket Error: ' + error);
            callback(error);
        };
    }

    // Show a connected message when the WebSocket is opened.
    Socket.prototype.onOpen = function (callback) {
        this.con.onopen = function (event) {
            //console.log("Connection OPEN");
            callback(event);
        };
    }

    // Handle messages sent by the server.
    Socket.prototype.onMessage = function (callback) {
            this.con.onmessage = function (event) {
                callback(event);
            };
    }

    // Show a disconnected message when the WebSocket is closed.
    Socket.prototype.onClose = function (callback) {
        this.con.onclose = function (event) {
            //console.log("Closed WebSocket");
        };
    }

    Socket.prototype.send = function (data) {
        this.msg.push(data);

        var that = this;
        setInterval(function () {
            if (that.con.bufferedAmount == 0 && that.msg.length > 0 && that.con.readyState === 1) {
                var tmpData = that.msg.shift();

                console.log("Sent: "+tmpData.Type);
                that.con.send(JSON.stringify(tmpData));


            }
        }, 50);
    }

    Socket.prototype.Callbacks = new Array();
    Socket.prototype.RegisterCallback = function (responses, callback) {

        for (var response in responses) {

            if (typeof Socket.prototype.Callbacks[responses[response]] == 'undefined') {
                Socket.prototype.Callbacks[responses[response]] = new Array();
    
            }
            Socket.prototype.Callbacks[responses[response]].push(callback)

        }
    }





    return Socket;
});