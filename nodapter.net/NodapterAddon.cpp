#pragma comment(lib, "node")

#include <node.h>
#include <v8.h>
#include <string>

#include "NodapterLibHelper.h"

using namespace node;
using namespace v8;

class NodapterObject: ObjectWrap
{
private:
    NodapterLibHelper* _nodapterHelper;

public:

    static Persistent<FunctionTemplate> s_ct;
    static void NODE_EXTERN Init(Handle<Object> target)
    {
        HandleScope scope;

        // set the constructor function
        Local<FunctionTemplate> t = FunctionTemplate::New(New);

        // set the node.js/v8 class name
        s_ct = Persistent<FunctionTemplate>::New(t);
        s_ct->InstanceTemplate()->SetInternalFieldCount(1);
        s_ct->SetClassName(String::NewSymbol("NodapterObject"));

        // registers a class member functions 
        NODE_SET_PROTOTYPE_METHOD(s_ct, "async", Async);
        NODE_SET_PROTOTYPE_METHOD(s_ct, "beginExecuteAsync", BeginExecuteAsync);
        NODE_SET_PROTOTYPE_METHOD(s_ct, "getNodapterValue", GetNodapterValue);

        target->Set(String::NewSymbol("NodapterObject"),
            s_ct->GetFunction());
    }

    NodapterObject() 
    {
        _nodapterHelper = NodapterLibHelper::New();
    }

    ~NodapterObject()
    {
        delete _nodapterHelper;
    }

    static Handle<Value> New(const Arguments& args)
    {
        HandleScope scope;
        NodapterObject* pm = new NodapterObject();
        pm->Wrap(args.This());
        return args.This();
    }

    struct Baton {
        uv_work_t request;
        NodapterLibHelper* nodapterHelper;
        Persistent<Function> callback;
        // A parameter that will be passed to the .Net library
        std::string command;
        baton_cb continuation;
        // Tracking errors that happened in the worker function. You can use any
        // variables you want. E.g. in some cases, it might be useful to report
        // an error number.
        bool error;
        std::string error_message;
        bool closed;

        // Custom data
        std::string result;
    };

    typedef void (*continuation_cb)(Baton* req);

    static Handle<Value> GetNodapterValue(const Arguments& args)
    {
        HandleScope scope;
        NodapterObject* so = ObjectWrap::Unwrap<NodapterObject>(args.This());
        Local<Integer> result = Integer::New(so->_nodapterHelper->GetNumber());
        return scope.Close(result);
    }

    //
    // Async
    //
    static Handle<Value> Async(const Arguments& args)
    {
        HandleScope scope;

        if (!args[0]->IsString()) {
            return ThrowException(Exception::TypeError(
                String::New("First argument must be a string")));
        }

        if (!args[1]->IsFunction()) {
            return ThrowException(Exception::TypeError(
                String::New("Second argument must be a callback function")));
        }

        Local<String> command = Local<String>::Cast(args[0]);
        // There's no ToFunction(), use a Cast instead.
        Local<Function> callback = Local<Function>::Cast(args[1]);

        NodapterObject* so = ObjectWrap::Unwrap<NodapterObject>(args.This());

        // create a state object
        Baton* baton = new Baton();
        baton->closed = false;
        baton->request.data = baton;
        baton->nodapterHelper = so->_nodapterHelper;
        baton->callback = Persistent<Function>::New(callback);
        baton->command = *v8::String::AsciiValue(command);

        // register a worker thread request
        uv_queue_work(uv_default_loop(), &baton->request,
            StartAsync, AfterAsync);

        return Undefined();
    }



    // this runs on the worker thread and should not callback or interact with node/v8 in any way
    static void StartAsync(uv_work_t* req)
    {
        Baton *baton = static_cast<Baton*>(req->data);
        baton->error = !baton->nodapterHelper->TryExecuteSync(baton->command, baton->error_message, baton->result);
    }

    // this runs on the main thread and can call back into the JavaScript
    static void AfterAsync(uv_work_t *req)
    {
        Baton* baton = static_cast<Baton*>(req->data);
        HandleScope scope;
        if (baton->error) 
        {
            Local<Value> err = Exception::Error(
                String::New(baton->error_message.c_str()));
            Local<Value> argv[] = { err };

            TryCatch try_catch;
            baton->callback->Call(
                Context::GetCurrent()->Global(), 1, argv);

            if (try_catch.HasCaught()) {
                node::FatalException(try_catch);
            }
        }
        else
        {
            const unsigned argc = 2;
            Local<Value> argv[argc] = {
                Local<Value>::New(Null()),
                Local<Value>::New(String::New(baton->result.c_str()))
            };

            TryCatch try_catch;
            baton->callback->Call(Context::GetCurrent()->Global(), argc, argv);

            if (try_catch.HasCaught()) {
                FatalException(try_catch);
            }
        }

        //baton->nodapterHelper->ReleaseString(baton->error_message);
        //baton->nodapterHelper->ReleaseString(baton->result);
        baton->callback.Dispose();
        baton->closed = true;
        delete baton;
    }

    //
    // beginExecuteAsync
    //
    static Handle<Value> BeginExecuteAsync(const Arguments& args)
    {
        HandleScope scope;

        if (!args[0]->IsString()) {
            return ThrowException(Exception::TypeError(
                String::New("First argument must be a string")));
        }

        if (!args[1]->IsFunction()) {
            return ThrowException(Exception::TypeError(
                String::New("Second argument must be a callback function")));
        }

        Local<String> command = Local<String>::Cast(args[0]);
        // There's no ToFunction(), use a Cast instead.
        Local<Function> callback = Local<Function>::Cast(args[1]);

        NodapterObject* so = ObjectWrap::Unwrap<NodapterObject>(args.This());

        // create a state object
        Baton* baton = new Baton();
        baton->closed = false;
        baton->request.data = baton;
        baton->nodapterHelper = so->_nodapterHelper;
        baton->callback = Persistent<Function>::New(callback);
        baton->command = *v8::String::AsciiValue(command);
        baton->continuation = ContinuationFunction;

        // invoke CommandInterpretter asynchronously
        baton->error = !baton->nodapterHelper->TryBeginExecuteAsync(baton->command, baton->continuation, baton, baton->error_message);

        return Undefined();
    }

   static void ContinuationFunction (void* state, char* err, char* resultData, bool retainCallback)
   {
      Baton* oldBaton = static_cast<Baton*>(state);
      Baton* baton;

      if (oldBaton->closed)
      {
         throw 123;
      }

      if (retainCallback)
      {
         Baton* newBaton = new Baton();
         newBaton->closed = false;
         newBaton->request.data = newBaton;
         newBaton->nodapterHelper = oldBaton->nodapterHelper;
         if (!oldBaton->callback.IsEmpty ())
            newBaton->callback = Persistent<Function>::New (oldBaton->callback);
         newBaton->command = oldBaton->command;
         newBaton->error = oldBaton->error;
         newBaton->continuation = oldBaton->continuation;
         baton = newBaton;
      }
      else
      {
         baton = oldBaton;
      }

      if (err)
      {
         baton->error = true;
         baton->error_message = err;
      }
      baton->result = resultData;

      // register a worker thread request
      uv_queue_work(uv_default_loop(), &baton->request,
         DoNothing, // uv_queue_work must have work to do... unfortunately.
         AfterAsync);  // AfterAsync disposes the callback.
   }

   static void DoNothing(uv_work_t* req)
   {
   }

};

Persistent<FunctionTemplate> NodapterObject::s_ct;

extern "C" {
    void NODE_EXTERN init (Handle<Object> target)
    {
        NodapterObject::Init(target);
        LoadAssembly();
    }
    NODE_MODULE(sharp, init);
}