using System;
using System.Threading;

namespace Nodapter.Core.Commands
{
   public class TestWork : ISyncWork, IAsyncWork
   {
      readonly string arguments;

      public TestWork(string arguments)
      {
         this.arguments = arguments;
      }
      string ISyncWork.Run(IWorkContext context)
      {
         var start = DateTime.Now;
         Thread.Sleep(1500);
         return arguments + " " + start.ToLongTimeString () + " sleep " + DateTime.Now.ToLongTimeString ();
      }

      bool IAsyncWork.StartAndContinueWith(IWorkContext context, Action<Exception, string, bool> callback, out Exception exception)
      {
         new Thread(
            () =>
            {
               var start = DateTime.Now;
               Thread.Sleep(2500);
               callback(null, start + " sleep " + DateTime.Now, true);
               Thread.Sleep(2500);
               callback(null, start + " sleep " + DateTime.Now, true);
               Thread.Sleep(2500);
               callback(null, start + " sleep " + DateTime.Now, true);
               Thread.Sleep(2500);
               callback(null, start + " sleep " + DateTime.Now, true);
               Thread.Sleep(2500);
               callback(null, start + " sleep " + DateTime.Now, true);
               Thread.Sleep(2500);
               callback(null, start + " sleep " + DateTime.Now, true);
               Thread.Sleep(2500);
               callback(null, start + " sleep " + DateTime.Now, true);
               Thread.Sleep(2500);
               callback(null, start + " sleep " + DateTime.Now, true);
               Thread.Sleep(2500);
               callback(null, start + " sleep " + DateTime.Now, false);
            }).Start();
         exception = null;
         return true;
      }
   }
}
