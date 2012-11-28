using System;

namespace Nodapter.Core
{
   public class Route
   {
      public Route(string name, Type type)
      {
         Name = name;
         Type = type;
      }
      public string Name { get; private set; }
      public Type Type { get; private set; }

      public override string ToString()
      {
         return Name + "=" + Type.AssemblyQualifiedName;
      }
   }
   public class CommandRouteInterpretter
   {
      public CommandRouteInterpretter(CommandDef command)
      {
         if (command == null)
            throw new ArgumentNullException("command");
         if (command.Arguments == null)
            throw new ArgumentNullException("command.Arguments");
         this.command = command;
      }

      public Route GetRoute()
      {
         routeString = command.Arguments;
         ParseArguments();
         type = GetType(fullyQualifiedTypeName);
         return new Route(name, type);
      }

      private void ParseArguments()
      {
         equals = routeString.IndexOf('=');
         ValidateString();
         name = GetName();
         fullyQualifiedTypeName = GetFullyQualifiedTypeName();
      }

      void ValidateString()
      {
         if (equals < 0)
            throw new InvalidRouteFormatException("Missing '='.  " +
                                                  InvalidRouteFormatException.ExpectedFormat);
         if (equals == 0)
            throw new InvalidRouteFormatException("Missing name.  " +
                                                  InvalidRouteFormatException.ExpectedFormat);
         if (equals == routeString.Length - 1)
            throw new InvalidRouteFormatException("Missing fully qualified type name.  " +
                                                  InvalidRouteFormatException.ExpectedFormat);
      }

      string GetName()
      {
         return routeString.Substring(0, equals).Trim();
      }

      string GetFullyQualifiedTypeName()
      {
         return routeString.Substring(equals + 1).Trim();
      }

      static Type GetType(string fullyQualifiedTypeName)
      {
         return TypeResolver.GetType(fullyQualifiedTypeName);
      }

      readonly CommandDef command;
      int equals;
      string routeString;
      string name;
      string fullyQualifiedTypeName;
      Type type;

      class InvalidRouteFormatException : Exception
      {
         public const string ExpectedFormat =
            "Expected Format: RouteName=QualifiedAssemblyName,TypeName";
         public InvalidRouteFormatException(string message = null, Exception innerException = null)
            : base (message ?? "Invalid route format string", innerException)
         {
         }
      }
   }
}