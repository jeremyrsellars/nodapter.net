var setExports = function setExports(requiredModule) {
   try
   {
      module.exports = require(requiredModule);
   }
   catch(e)
   {
      throw new Error(
      	'Nodapter.net could not load "' + requiredModule + '". \r\n' +
      	'Perhaps it is not built against the executing version of node.js (' + process.version + ')? \r\n' +
      	'Try installing the package with "npm install".\r\n\r\n' + e);
   }
};

switch(process.arch)
{
   case 'x64':
      setExports('./nodapter.net.x64.node');
      break;
   case 'ia32':
      setExports('./nodapter.net.Win32.node');
      break;
   default:
   	  throw new Error('Unsupported process.arch: ' + process.arch);
}
