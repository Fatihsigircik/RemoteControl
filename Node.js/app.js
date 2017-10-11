var app = require('express')();
var http = require('http').Server(app);
var io = require('socket.io')(http);
app.get('/', function(req, res) {
    res.sendfile("index.html");
});

app.get('/index.js', function(req, res) {
    res.sendfile("index.js");
});

app.get('/pako.js', function(req, res) {
    res.sendfile("pako.js");
});

app.get('/jsxcompressor.min.js', function(req, res) {
    res.sendfile("jsxcompressor.min.js");
});
var s = "";

io.on('connection', function(socket) {
    console.log('a user connected');
    socket.emit("connection", "ok");
    socket.on("sendMessage", function(msg) {
        socket.broadcast.emit("SentMessage", msg);
    });
    socket.on('disconnect', function() {
        console.log('user disconnected');
    });
    socket.on('sever', function(data) {
        s = socket.id;
    });
    socket.on('mouseChange', function(data) {

        io.to(s).emit('mouseChange', data);
    });

    socket.on('mouseDown', function(data) {

        io.to(s).emit('mouseDown', data);
    });

    socket.on('mouseUp', function(data) {

        io.to(s).emit('mouseUp', data);
    });

    socket.on('keyPress', function(data) {
        console.log(data);
        io.to(s).emit('keyPress', data);
    });


});
http.listen(3000, function() {
    console.log('listening on *:3000');
});