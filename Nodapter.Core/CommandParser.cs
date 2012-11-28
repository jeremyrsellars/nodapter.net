using System;

namespace Nodapter.Core
{
   public class CommandDef
   {
      public string Name { get; set; }
      public string Arguments { get; set; }
   }

   public class CommandParser
   {
      public CommandParser(string command)
      {
         if (command == null)
            throw new ArgumentNullException("command");
         this.command = command;
      }

      public CommandDef Parse()
      {
         int pos = command.IndexOf(' ');
         if (pos < 0)
            return new CommandDef {Name = command, Arguments = ""};
         return new CommandDef {Name = command.Substring(0, pos), Arguments = command.Substring(pos + 1)};
      }

      readonly string command;
   }
}
