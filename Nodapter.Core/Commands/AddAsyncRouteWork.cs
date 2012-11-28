namespace Nodapter.Core.Commands
{
   public class AddSyncRouteWork : ISyncWork
   {
      string ISyncWork.Run(IWorkContext context)
      {
         var command = new CommandParser(context.Command).Parse();
         var route = new CommandRouteInterpretter(command).GetRoute();
         context.CommandInterpretter.AddSyncRoute(route);
         return "";
      }
   }
   public class AddAsyncRouteWork : ISyncWork
   {
      string ISyncWork.Run(IWorkContext context)
      {
         var command = new CommandParser(context.Command).Parse();
         var route = new CommandRouteInterpretter(command).GetRoute();
         context.CommandInterpretter.AddAsyncRoute(route);
         return "";
      }
   }
   public class BootStrapWork : ISyncWork
   {
      string ISyncWork.Run(IWorkContext context)
      {
         var command = new CommandParser(context.Command).Parse();
         var route = new CommandRouteInterpretter(command).GetRoute();
         CommandInterpretter.WorkCreator<ISyncWork>.Create(route, new CommandDef()).Run(context);
         return null;
      }
   }
   public class GetRoutesWork : ISyncWork
   {
      string ISyncWork.Run(IWorkContext context)
      {
         return context.CommandInterpretter.ToString();
      }
   }
}
