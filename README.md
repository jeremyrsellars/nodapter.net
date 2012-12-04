Nodapter.net
==========================

Run .net code from node.js.

For when you don't want to learn C++/CLI, but want to run .net code in NodeJS.

## Why add .net to NodeJS?

Let's say you have an awesome library that is tried, trusted, and true written on .NET.  Now, let's say you want to write a web site on Node.JS.  Why rewrite your library for NodeJS?


```C#

    public class ChecksumWork : Nodapter.Core.ISyncWork
    {
      readonly string argument;

      public ChecksumWork(string argument)
      {
         this.argument = argument;
      }

      string ISyncWork.Run(IWorkContext context)
      {
         System.Threading.Thread.Sleep(1500);
         int sum = 0;
         foreach(int c in argument)
            sum ^= c;
         return sum.ToString();
      }
    }

```

```javascript

var NodapterObject = require('nodapter.net').NodapterObject;
var so = new NodapterObject();

// Add a route named "Checksum", which, when called, 
// instantiates the ChecksumWork class in the Nodapter.Core.dll assembly.
so.async("AddSyncRoute Checksum=Nodapter.Core,Nodapter.Core.Commands.ChecksumWork",
   function (err, blah) {
      // Begin calling our Checksum route, expecting a callback with the answer.
      so.async("Checksum ABC", function (err, checksumString) {
         if (err) {
            console.log("Error:", err);
            return;
         }

         var expected = 64;
         if(expected == parseInt(checksumString))
            console.log("correct checksum calculated: ");
         else
            console.log("wrong checksum calculated: ");
         console.log(checksumString);
      });
});

var intervalCookie = setInterval(function () {
   var dt = new Date();
   console.log('The nodejs message loop is responsive.  ' + dt.getHours() + ':' + dt.getMinutes() + ':' + dt.getSeconds() + '.' + ('00' + dt.getMilliseconds()).slice(-3));
}, 500);

setTimeout(function() {
   clearInterval(intervalCookie);
   console.log("done");
}, 3000);

```

### Nodapter.net.node
A C++/CLI dll interoping between V8 and .NET.  It lets you access .NET "work" classes that implement [Nodapter.Core.ISyncWork](nodapter.net/blob/master/Nodapter.Core/IWork.cs)



Installation
----------------------------

From a __Visual Studio Command Prompt__ (That's right, I suggest Visual C++ should be installed because it has header & lib files that you'll need), run:

```
SET NODE_HOME=c:\node\v0.8.15
npm install

```

Then, build your own .net classes which implement ISyncWork and put them in the 
current directory.  Add a route to them, then execute the work, as shown in the example.

__Note:__ At the moment, this works on Windows x86, so run a 32-bit version of node.