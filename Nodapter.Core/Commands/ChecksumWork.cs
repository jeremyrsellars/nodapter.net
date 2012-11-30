namespace Nodapter.Core.Commands
{
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
}
