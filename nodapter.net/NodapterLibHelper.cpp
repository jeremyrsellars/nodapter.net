#include "NodapterLibHelper.h"
#include "CallbackToAction.h"
#using <mscorlib.dll>
#using <Nodapter.Core.dll>
#include <gcroot.h>

using namespace System::Runtime::InteropServices;
using namespace System::Reflection;
using namespace Nodapter::Core;

// we need to look for the assembly in the current directory
// node will search for it next to the node.exe binary
System::Reflection::Assembly ^OnAssemblyResolve(System::Object ^obj, System::ResolveEventArgs ^args)
{
   System::String ^path = System::Environment::CurrentDirectory;
   array<System::String^>^ assemblies =
      System::IO::Directory::GetFiles(
         System::IO::Path::GetDirectoryName(System::Reflection::Assembly::GetExecutingAssembly()->Location),
         "*.dll");
   for (long ii = 0; ii < assemblies->Length; ii++) {
      AssemblyName ^name = AssemblyName::GetAssemblyName(assemblies[ii]);
      if (AssemblyName::ReferenceMatchesDefinition(gcnew AssemblyName(args->Name), name)) {
         return Assembly::Load(name);
      }
   }
   return nullptr;
}

// register a custom assembly load handler
void LoadAssembly()
{
    System::AppDomain::CurrentDomain->AssemblyResolve +=
        gcnew System::ResolveEventHandler(OnAssemblyResolve);
}

typedef CallbackToAction<baton_cb,void*> Continuation;

class NodapterLibWrapper : public NodapterLibHelper {
private:
    gcroot<CommandInterpretter^> _commandInterpretter;

public:
   NodapterLibWrapper()
   {
      _commandInterpretter = gcnew CommandInterpretter();
   }

   virtual bool TryExecuteSync(std::string& command, std::string& err, std::string& result)
   {
      try
      {
         System::Exception^ exception = nullptr;
         System::String^ resultData = nullptr;
         bool ok =  _commandInterpretter->TryExecuteSync(gcnew System::String(command.c_str()), resultData, exception);

         if (resultData)
         {
            System::IntPtr p = Marshal::StringToHGlobalAnsi(resultData);
            result = static_cast<char*>(p.ToPointer());
            Marshal::FreeHGlobal(p);
         }

         if (exception)
         {
            System::IntPtr p = Marshal::StringToHGlobalAnsi(exception->ToString());
            err = static_cast<char*>(p.ToPointer());
            Marshal::FreeHGlobal(p);
         }
         //if (exception != nullptr)
         //{
         //   System::IntPtr p = Marshal::StringToHGlobalAnsi(exception->Message);
         //   err = static_cast<char*>(p.ToPointer());
         //   Marshal::FreeHGlobal(p);
         //}
         //else
         //   err = nullptr;

         return ok;
      }
      catch(System::Exception^ e)
      {
         System::IntPtr p = Marshal::StringToHGlobalAnsi(e->ToString());
         err = static_cast<char*>(p.ToPointer());
         Marshal::FreeHGlobal(p);
         return false;
      }
   }
   
   virtual bool TryBeginExecuteAsync(std::string& command, baton_cb callback, void* state, std::string& err)
   {
      try
      {
         System::Exception^ exception;
         bool ok =  _commandInterpretter->BeginExecuteAsync(
            gcnew System::String(command.c_str()), 
            Continuation::Convert(&CallbackFunction, callback, state), 
            exception);

         if (exception)
         {
            System::IntPtr p = Marshal::StringToHGlobalAnsi(exception->ToString());
            err = static_cast<char*>(p.ToPointer());
            Marshal::FreeHGlobal(p);
         }

         return ok;
      }
      catch(System::Exception^ e)
      {
         System::IntPtr p = Marshal::StringToHGlobalAnsi(e->ToString());
         err = static_cast<char*>(p.ToPointer());
         Marshal::FreeHGlobal(p);
         return false;
      }
   }

   static void CallbackFunction(baton_cb callback, void* state, System::Exception^ exception, System::String^ resultData, bool retainCallback)
   {
      char* cErr;
      char* cResult;

      System::IntPtr pResult = Marshal::StringToHGlobalAnsi(resultData);
      cResult = static_cast<char*>(pResult.ToPointer());

      System::IntPtr pErr;
      if (exception)
      {
         pErr = Marshal::StringToHGlobalAnsi(exception->ToString());
         cErr = static_cast<char*>(pErr.ToPointer());
      }

      if (callback)
         callback (state, cErr, cResult, retainCallback);
      Marshal::FreeHGlobal(pResult);
      if (exception)
         Marshal::FreeHGlobal(pErr);
   }

   virtual int GetNumber()
   {
      return 0;
   }
};

NodapterLibHelper* NodapterLibHelper::New()
{
    return new NodapterLibWrapper();
}

