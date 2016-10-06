#! /usr/bin/env node
var Transform = require('stream').Transform || require('readable-stream').Transform;

var template = require('./azuredeploy.json');
var fs = require('fs');
var zlib = require('zlib');
var scriptFile = fs.createReadStream('./server-cloud-init.txt');
var templateFile = fs.createWriteStream('./azuredeploy.json');

var streamToString = function(stream, callback) {
  var str = '';
  stream.on('data', function(chunk) {
    str += chunk;
  });
  stream.on('end', function() {
    callback(str);
  });
}

streamToString(scriptFile, (encodedScript) => {
  template.variables.customData = "[base64(concat('" + encodedScript.replace(/'/g, "ß").replace("PARAMETERS", "',variables('customDataParameters'),'") + "'))]";
  templateFile.write(JSON.stringify(template, null, 4).replace(/ß/g, "\\u0027"));
  templateFile.end();
});