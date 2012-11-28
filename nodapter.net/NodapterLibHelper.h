#include <node.h>
#include <v8.h>
#include <string>

using namespace v8;

void LoadAssembly();

typedef void(*baton_cb)(void* baton, char* err, char* resultData, bool retainCallback);

// this is a shim class which mediates between managed and unmanaged code
class NodapterLibHelper
{
protected:
    NodapterLibHelper() {};
public:
    virtual bool TryExecuteSync(std::string& command, std::string& err, std::string& result) = 0;
    virtual bool TryBeginExecuteAsync(std::string& command, baton_cb callback, void* baton, std::string& result) = 0;
    virtual int GetNumber() = 0;

    static NodapterLibHelper* New();
};
