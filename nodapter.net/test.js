var NodapterObject = require('./Nodapter.net').NodapterObject;
var so = new NodapterObject();
console.log(so.getNodapterValue());

setTimeout(function () { console.log("done"); }, 10000);


setTimeout(function () {
   so.async("AddSyncRoute Sync=Nodapter.Core,Nodapter.Core.Commands.TestWork", function (err, blah) {
      so.async("Sync 1wingbat", function (err, data) {
         if (err) {
            console.log("Error:", err);
            return;
         }

         console.log("Data:", data);
      });
      so.async("Sync 2sayanara", function (err, data) {
         if (err) {
            console.log("Error:", err);
            return;
         }

         console.log("Data:", data);
      });
      so.async("Sync 3wingbat sayanara", function (err, data) {
         if (err) {
            console.log("Error:", err);
            return;
         }

         console.log("Data:", data);
      });
   });
}, 5000);