// This class bridges a C++ callback to a .NET Action, which
// can be queued up in a .NET Task.
template<typename TCallback, typename TState>
ref class CallbackToAction
{
public:
  typedef void (CallbackHandler)(TCallback, TState, System::Exception^, System::String^,bool);
 
  // Converts function pointer and parameters to an action.
  static System::Action<System::Exception^,System::String^,bool>^ Convert(CallbackHandler* handler, TCallback callback, TState state)
  {
    CallbackToAction<TCallback,TState>^ callbackToAction = gcnew CallbackToAction<TCallback,TState>();
    callbackToAction->_handler = handler;
    callbackToAction->_state = state;
    callbackToAction->_callback = callback;
    return callbackToAction->ToAction();
  }
 
private:
 
  void DoCallback(System::Exception^ exception, System::String^ result, bool retainCallback)
  {
    _handler(_callback, _state, exception, result, retainCallback);
  }
 
  System::Action<System::Exception^,System::String^,bool>^ ToAction()
  {
    return gcnew System::Action<System::Exception^,System::String^,bool>(this, &CallbackToAction::DoCallback);
  }
 
  CallbackHandler* _handler;
  TCallback _callback;
  TState _state;
};