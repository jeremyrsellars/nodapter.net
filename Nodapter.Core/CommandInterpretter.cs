using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nodapter.Core
{
   public class CommandInterpretter
   {
      public CommandInterpretter()
      {
         syncRoutes = new List<Route>();
         asyncRoutes = new List<Route>();
         AddSyncRoute(new Route("AddSyncRoute", typeof (Commands.AddSyncRouteWork)));
         AddSyncRoute(new Route("AddAsyncRoute", typeof (Commands.AddAsyncRouteWork)));
         AddSyncRoute(new Route("BootStrap", typeof (Commands.BootStrapWork)));
         AddSyncRoute(new Route("GetRoutes", typeof (Commands.GetRoutesWork)));
         AddSyncRoute(new Route("GC.Collect", typeof (Commands.GCCollectWork)));
      }

      public override string ToString()
      {
         return DescribeRoutes(syncRoutes) + DescribeRoutes(asyncRoutes);
      }

      static string DescribeRoutes(IEnumerable<Route> routes)
      {
         return routes.Aggregate(new StringBuilder(), (sb, r) => sb.Append(r).AppendLine()).ToString();
      }

      public void AddSyncRoute(Route route)
      {
         syncRoutes.Add(route);
      }

      public void AddAsyncRoute(Route route)
      {
         asyncRoutes.Add(route);
      }

      public bool TryExecuteSync(string command, out string result, out Exception exception)
      {
         var work = GetSyncWork(command);
         try
         {
            result = work.Run(CreateWorkContext(command));
            exception = null;
            return true;
         }
         catch (Exception e)
         {
            exception = e;
            result = null;
            return false;
         }
      }

      public bool BeginExecuteAsync(string command, Action<Exception,string, bool> callback, out Exception exception)
      {
         var work = GetAsyncWork(command);
         try
         {
            return work.StartAndContinueWith(CreateWorkContext(command), callback, out exception);
         }
         catch (Exception e)
         {
            exception = e;
            return false;
         }
      }

      IWorkContext CreateWorkContext(string command)
      {
         return new WorkContext {CommandInterpretter = this, Command = command};
      }

      public ISyncWork GetSyncWork(string command)
      {
         Console.WriteLine ("Interpretting Sync: " + command);
         var c = ParseCommand(command);
         var route = GetRoute(syncRoutes, c);
         var work = WorkCreator<ISyncWork>.Create(route, c);
         return work;
      }

      public IAsyncWork GetAsyncWork(string command)
      {
         Console.WriteLine("Interpretting Async: " + command);
         var c = ParseCommand(command);
         var route = GetRoute(asyncRoutes, c);
         var work = WorkCreator<IAsyncWork>.Create(route, c);
         return work;
      }

      static CommandDef ParseCommand(string command)
      {
         return new CommandParser(command).Parse();
      }

      internal class WorkCreator<T>
      {
         readonly ConstructorInfo ctor;
         readonly object[] arguments;

         public static T Create(Route route, CommandDef commandDef)
         {
            return new WorkCreator<T>(route, commandDef).Create();
         }

         WorkCreator(Route route, CommandDef commandDef)
         {
            if (!typeof(T).IsAssignableFrom(route.Type))
               throw new ArgumentException(string.Format("Cannot assign {0} to {1}.", route.Type, typeof (T)));
            ctor = route.Type.GetConstructor(new []{typeof(string)}) ?? route.Type.GetConstructor(Type.EmptyTypes);
            arguments = ctor.GetParameters().Length == 0
                               ? new object[0]
                               : new object[] {commandDef.Arguments};
         }

         T Create()
         {
            return (T) ctor.Invoke(arguments);
         }
      }

      static Route GetRoute(IEnumerable<Route> routes, CommandDef command)
      {
         var route = routes.FirstOrDefault(r => r.Name == command.Name);
         if (route == null)
            throw new RouteNotFoundException(command.Name, command.Arguments);
         return route;
      }

      readonly List<Route> syncRoutes;
      readonly List<Route> asyncRoutes;
   }

   public class RouteNotFoundException : Exception
   {
      public string Name { get; private set; }
      public string Arguments { get; private set; }

      public RouteNotFoundException(string name, string arguments)
         : base ("No route found for " + name + ".  Arguments: " + arguments)
      {
         Name = name;
         Arguments = arguments;
      }
   }
}
