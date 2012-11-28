using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nodapter.Core
{
   [TestClass]
   public abstract class CommandRouteInterpretterTest
   {
      protected CommandRouteInterpretter Interpretter;
      protected Route Route;
      protected Type ExpectedType;
      protected string ExpectedName;

      protected void TestInitialize(CommandDef commandDef)
      {
         Interpretter = new CommandRouteInterpretter(commandDef);
         Route = Interpretter.GetRoute();
      }

      [TestMethod]
      public void TypeIsCorrect()
      {
         Assert.AreEqual(ExpectedType, Route.Type);
      }

      [TestMethod]
      public void NameIsCorrect()
      {
         Assert.AreEqual(ExpectedName, Route.Name);
      }
   }
   [TestClass]
   public class CommandRouteInterpretterTestWork : CommandRouteInterpretterTest
   {
      [TestInitialize]
      public void TestInitialize()
      {
         ExpectedType = typeof (Commands.TestWork);
         ExpectedName = "AddRoute";
         TestInitialize(new CommandDef{Name="AddRouteXXX", Arguments = "AddRoute=Nodapter.Core,Nodapter.Core.Commands.TestWork"});
      }
   }
   [TestClass]
   public class CommandRouteInterpretterNestedTestWork : CommandRouteInterpretterTest
   {
      [TestInitialize]
      public void TestInitialize()
      {
         ExpectedType = typeof (NestedSyncTestWork);
         ExpectedName = "AddSyncRoute";
         TestInitialize(new CommandDef{Name="AddRouteXXX", Arguments = "AddSyncRoute=Nodapter.Core.Test,Nodapter.Core.CommandRouteInterpretterNestedTestWork+NestedSyncTestWork"});
      }
      public class NestedSyncTestWork : ISyncWork
      {
         public string Run(IWorkContext context)
         {
            throw new NotImplementedException();
         }
      }
   }
}
