//'use strict';
//var http = require('http');
var port = process.env.PORT || 3000;
var express = require('express');           // For web server
var bodyParser = require('body-parser');    // Receive JSON format

var app = express();
app.use(bodyParser.json());
app.use(express.static(__dirname + '/wwwroot'));

// This is for web server to start listening to port 3000
app.set('port', port);
var server = app.listen(app.get('port'), function () {
    console.log('Server listening on port ' + server.address().port);
});

//http.createServer(function (req, res) {
//    //res.writeHead(200, { 'Content-Type': 'text/plain' });
//    //res.end('Hello World\n');
//    fs.readFile(__dirname + '/www/' + 'index.html', function (err, data) {
//        if (err) {
//            res.writeHead(404);
//            res.end(JSON.stringify(err));
//            return;
//        }
//        res.writeHeader(200, { "Content-Type": "text/html" });
//        res.end(data);
//    });
//}).listen(port);
