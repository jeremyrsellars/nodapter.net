var http = require('http');
var fs = require('fs');

var downloadFile = function(url, localDirectory, localName) {
   if(!fs.existsSync(localDirectory)) fs.mkdirSync(localDirectory);

   var localPath = localDirectory + '\\' + localName;

   var file = fs.createWriteStream(localPath);

   file.on('error', function (err) {
      console.error(err);
   });
   var request = http.get(url, function(response) {
   	  console.log('downloading ' + url + ' to ' + localPath);
      response.pipe(file);
   });
};

console.log(__dirname);

var dist_ver = 'http://nodejs.org/dist/' + process.version + '/'
var nodelib = 'node.lib';
downloadFile(dist_ver + nodelib, __dirname + '\\Win32', nodelib);
downloadFile(dist_ver + 'x64/' + nodelib, __dirname + '\\x64', nodelib);
