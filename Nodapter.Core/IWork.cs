using System;

namespace Nodapter.Core
{
   public interface IWorkContext
   {
      CommandInterpretter CommandInterpretter { get; }
      string Command { get; }
   }

   public class WorkContext : IWorkContext
   {
      public CommandInterpretter CommandInterpretter { get; set; }
      public string Command { get; set; }
   }

   public interface IWorkContinuation
   {
      void Continue(object work, object result);
      void Fail(object work, Exception exception);
   }

   public interface ISyncWork
   {
      string Run(IWorkContext context);
   }

   public interface IAsyncWork
   {
      bool StartAndContinueWith(IWorkContext context, Action<Exception, string, bool> callback, out Exception exception);
   }
}
