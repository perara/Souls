/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

var sClient = (function() {
    // var socket = new WebSocket('ws://tux.persoft.no:8140');
    var socket = new WebSocket('ws://hybel.keel.no:8140/chat');

    console.log("WebSocket client");

    // Handle any errors that occur.
    function onError(callback)
    {
        socket.onerror = function(error) {
            console.log('WebSocket Error: ' + error);
            callback(error);
        };
    }

    // Show a connected message when the WebSocket is opened.
    function onOpen(callback)
    {
        socket.onopen = function(event) {
            //console.log("Connection OPEN");
            callback(event);
        };
    }

    // Handle messages sent by the server.
    function receive(callback)
    {
        socket.onmessage = function(event) {
            callback(event);
        };
    }

    // Show a disconnected message when the WebSocket is closed.
    function onClose(callback)
    {
        socket.onclose = function(event) {
            //console.log("Closed WebSocket");
        };
    }

    function send(data)
    {
        if (socket.readyState === 1)
        {
            //console.log("Sent following: " + data);
            socket.send(data);
        } else {
            //console.log("Could not send, ReadyState=" + socket.readyState);
            setTimeout(function() {
                send(data)
            }, 50);
        }
    }

    // Public function setters
    return {
        send: send,
        receive: receive,
        onOpen: onOpen,
        onError: onError,
        onClose: onClose,
    };
})();


