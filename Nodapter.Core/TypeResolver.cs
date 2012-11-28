using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Nodapter.Core
{
   public static class TypeResolver
   {
      public static Type GetType(string name)
      {
         var tn = new TypeNameParser(name);
         var a = LoadAssembly(tn.AssemblyNameOrPath);
         var t = a.GetType(tn.TypeName, true);
         return t;
      }

      static Assembly LoadAssembly(string assemblyNameOrPath)
      {
         if (ShouldLoadFromSafe(assemblyNameOrPath))
            return Assembly.LoadFrom(assemblyNameOrPath);
         return Assembly.Load(assemblyNameOrPath);
      }

      static bool ShouldLoadFromSafe(string assemblyNameOrPath)
      {
         try
         {
            return ShouldLoadFrom(assemblyNameOrPath);
         }
         catch(Exception e)
         {
            Console.Error.WriteLine(
               "Failure determining if an assembly path was specified to load from." + Environment.NewLine + e);
            return false;
         }
      }

      static bool ShouldLoadFrom(string assemblyNameOrPath)
      {
         return (HasFileExtension(assemblyNameOrPath) || LooksLikePath(assemblyNameOrPath))
                   && System.IO.File.Exists(assemblyNameOrPath);
      }

      static bool LooksLikePath(string assemblyNameOrPath)
      {
         return assemblyNameOrPath.IndexOfAny(Slashes) >= 0;
      }

      static readonly char[] Slashes = new[] {'/', '\\'};

      static bool HasFileExtension(string assemblyNameOrPath)
      {
         return assemblyNameOrPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                || assemblyNameOrPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase);
      }

      class TypeNameParser
      {
         readonly string name;

         public TypeNameParser(string name)
         {
            this.name = name;
            Parse();
         }

         private void Parse()
         {
            AssemblyNameOrPath = Regex.Match(name, "^[^,]+").Value;
            TypeName = Regex.Match(name, "[^,]+$").Value;
         }

         public string AssemblyNameOrPath
         {
            get;
            private set;
         }
         public string TypeName
         {
            get;
            private set;
         }
      }
   }
}