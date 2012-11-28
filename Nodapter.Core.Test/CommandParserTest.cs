using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nodapter.Core
{
   [TestClass]
   public abstract class CommandParserBase
   {
      protected CommandParser Parser;
      protected string Command;
      protected string ExpectedName;
      protected string ExpectedArguments;
      protected CommandDef CommandDef;

      [TestInitialize]
      public void TestInitialize()
      {
         Parser = new CommandParser(Command);
         CommandDef = Parser.Parse();
      }

      [TestMethod]
      public void ParseResultIsNamedCorrectly()
      {
         Assert.AreEqual(ExpectedName, CommandDef.Name);
      }

      [TestMethod]
      public void ParseResultHasCorrectArguments()
      {
         Assert.AreEqual(ExpectedArguments, CommandDef.Arguments);
      }
   }
   [TestClass]
   public class CommandParserDoNothing : CommandParserBase
   {
      public CommandParserDoNothing()
      {
         Command = "DoNothing";
         ExpectedName = "DoNothing";
         ExpectedArguments = "";
      }
   }
   [TestClass]
   public class CommandParserAddRoute : CommandParserBase
   {
      public CommandParserAddRoute()
      {
         Command = "AddRoute AddRoute=Nodapter.Core,Nodapter.Core.Commands.AddRouteWork";
         ExpectedName = "AddRoute";
         ExpectedArguments = "AddRoute=Nodapter.Core,Nodapter.Core.Commands.AddRouteWork";
      }
   }
}
