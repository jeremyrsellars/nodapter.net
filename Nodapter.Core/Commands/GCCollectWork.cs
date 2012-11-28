using System;

namespace Nodapter.Core.Commands
{
   public class GCCollectWork : ISyncWork
   {
      string ISyncWork.Run(IWorkContext context)
      {
         var cmdParts = context.Command.Split(' ');
         int generation;
         if (cmdParts.Length < 2)
            generation = GC.MaxGeneration;
         else
            generation = int.Parse(cmdParts[1]);
         GC.Collect(generation);

         return "Garbage collected for generation: " + generation;
      }
   }
}