#include <windows.h>
#include <shobjidl.h>
#include <shlwapi.h>
#include <strsafe.h>

#include <new>
#include <string>
#include <vector>

namespace
{
    constexpr wchar_t AppUserModelId[] = L"Aptlantis.FilingCabinet_jfrcsngvdwx7g!filingCabinet";

    const CLSID CLSID_CopyToFilingCabinet =
        {0x8266646f, 0x93d6, 0x4d83, {0xb0, 0x68, 0xbb, 0x0d, 0x68, 0x58, 0xbf, 0x83}};
    const CLSID CLSID_MoveToFilingCabinet =
        {0xb61e36e1, 0xbb0b, 0x46a5, {0x95, 0x0b, 0x56, 0x78, 0x92, 0x46, 0x19, 0x8b}};

    long g_moduleRefCount = 0;

    enum class IngestVerb
    {
        Copy,
        Move
    };

    class ModuleRef
    {
    public:
        ModuleRef()
        {
            InterlockedIncrement(&g_moduleRefCount);
        }

        ~ModuleRef()
        {
            InterlockedDecrement(&g_moduleRefCount);
        }
    };

    std::wstring QuoteArgument(const std::wstring& value)
    {
        std::wstring quoted = L"\"";
        size_t backslashCount = 0;

        for (wchar_t ch : value)
        {
            if (ch == L'\\')
            {
                ++backslashCount;
                quoted.push_back(ch);
                continue;
            }

            if (ch == L'"')
            {
                quoted.append(backslashCount, L'\\');
                quoted.push_back(L'\\');
            }

            backslashCount = 0;
            quoted.push_back(ch);
        }

        quoted.append(backslashCount, L'\\');
        quoted.push_back(L'"');
        return quoted;
    }

    HRESULT CollectSelectedPaths(IShellItemArray* selection, std::vector<std::wstring>& paths)
    {
        if (selection == nullptr)
        {
            return E_INVALIDARG;
        }

        DWORD count = 0;
        HRESULT hr = selection->GetCount(&count);
        if (FAILED(hr))
        {
            return hr;
        }

        for (DWORD i = 0; i < count; ++i)
        {
            IShellItem* item = nullptr;
            hr = selection->GetItemAt(i, &item);
            if (FAILED(hr))
            {
                return hr;
            }

            PWSTR path = nullptr;
            hr = item->GetDisplayName(SIGDN_FILESYSPATH, &path);
            item->Release();

            if (SUCCEEDED(hr) && path != nullptr)
            {
                paths.emplace_back(path);
            }

            CoTaskMemFree(path);
        }

        return paths.empty() ? E_FAIL : S_OK;
    }

    std::wstring BuildActivationArguments(IngestVerb verb, const std::vector<std::wstring>& paths)
    {
        std::wstring args = (verb == IngestVerb::Copy) ? L"--copy" : L"--move";

        for (const auto& path : paths)
        {
            args.push_back(L' ');
            args.append(QuoteArgument(path));
        }

        return args;
    }

    HRESULT ActivateFilingCabinet(IngestVerb verb, const std::vector<std::wstring>& paths)
    {
        IApplicationActivationManager* activator = nullptr;
        HRESULT hr = CoCreateInstance(
            CLSID_ApplicationActivationManager,
            nullptr,
            CLSCTX_LOCAL_SERVER,
            IID_PPV_ARGS(&activator));

        if (FAILED(hr))
        {
            return hr;
        }

        DWORD processId = 0;
        std::wstring args = BuildActivationArguments(verb, paths);
        hr = activator->ActivateApplication(AppUserModelId, args.c_str(), AO_NONE, &processId);
        activator->Release();
        return hr;
    }

    class ExplorerCommand final : public IExplorerCommand
    {
    public:
        explicit ExplorerCommand(IngestVerb verb) : _verb(verb)
        {
        }

        IFACEMETHODIMP QueryInterface(REFIID riid, void** object) override
        {
            if (object == nullptr)
            {
                return E_POINTER;
            }

            *object = nullptr;
            if (riid == IID_IUnknown || riid == IID_IExplorerCommand)
            {
                *object = static_cast<IExplorerCommand*>(this);
                AddRef();
                return S_OK;
            }

            return E_NOINTERFACE;
        }

        IFACEMETHODIMP_(ULONG) AddRef() override
        {
            return InterlockedIncrement(&_refCount);
        }

        IFACEMETHODIMP_(ULONG) Release() override
        {
            ULONG count = InterlockedDecrement(&_refCount);
            if (count == 0)
            {
                delete this;
            }

            return count;
        }

        IFACEMETHODIMP GetTitle(IShellItemArray*, PWSTR* name) override
        {
            if (name == nullptr)
            {
                return E_POINTER;
            }

            return SHStrDup((_verb == IngestVerb::Copy) ? L"Copy to Filing Cabinet" : L"Move to Filing Cabinet", name);
        }

        IFACEMETHODIMP GetIcon(IShellItemArray*, PWSTR* icon) override
        {
            if (icon == nullptr)
            {
                return E_POINTER;
            }

            *icon = nullptr;
            return E_NOTIMPL;
        }

        IFACEMETHODIMP GetToolTip(IShellItemArray*, PWSTR* tooltip) override
        {
            if (tooltip == nullptr)
            {
                return E_POINTER;
            }

            *tooltip = nullptr;
            return E_NOTIMPL;
        }

        IFACEMETHODIMP GetCanonicalName(GUID* guidCommandName) override
        {
            if (guidCommandName == nullptr)
            {
                return E_POINTER;
            }

            *guidCommandName = (_verb == IngestVerb::Copy) ? CLSID_CopyToFilingCabinet : CLSID_MoveToFilingCabinet;
            return S_OK;
        }

        IFACEMETHODIMP GetState(IShellItemArray*, BOOL, EXPCMDSTATE* commandState) override
        {
            if (commandState == nullptr)
            {
                return E_POINTER;
            }

            *commandState = ECS_ENABLED;
            return S_OK;
        }

        IFACEMETHODIMP Invoke(IShellItemArray* selection, IBindCtx*) override
        {
            std::vector<std::wstring> paths;
            HRESULT hr = CollectSelectedPaths(selection, paths);
            if (FAILED(hr))
            {
                return hr;
            }

            return ActivateFilingCabinet(_verb, paths);
        }

        IFACEMETHODIMP GetFlags(EXPCMDFLAGS* flags) override
        {
            if (flags == nullptr)
            {
                return E_POINTER;
            }

            *flags = ECF_DEFAULT;
            return S_OK;
        }

        IFACEMETHODIMP EnumSubCommands(IEnumExplorerCommand** enumCommands) override
        {
            if (enumCommands == nullptr)
            {
                return E_POINTER;
            }

            *enumCommands = nullptr;
            return E_NOTIMPL;
        }

    private:
        ~ExplorerCommand() = default;

        ModuleRef _moduleRef;
        long _refCount = 1;
        IngestVerb _verb;
    };

    class ClassFactory final : public IClassFactory
    {
    public:
        explicit ClassFactory(IngestVerb verb) : _verb(verb)
        {
        }

        IFACEMETHODIMP QueryInterface(REFIID riid, void** object) override
        {
            if (object == nullptr)
            {
                return E_POINTER;
            }

            *object = nullptr;
            if (riid == IID_IUnknown || riid == IID_IClassFactory)
            {
                *object = static_cast<IClassFactory*>(this);
                AddRef();
                return S_OK;
            }

            return E_NOINTERFACE;
        }

        IFACEMETHODIMP_(ULONG) AddRef() override
        {
            return InterlockedIncrement(&_refCount);
        }

        IFACEMETHODIMP_(ULONG) Release() override
        {
            ULONG count = InterlockedDecrement(&_refCount);
            if (count == 0)
            {
                delete this;
            }

            return count;
        }

        IFACEMETHODIMP CreateInstance(IUnknown* outer, REFIID riid, void** object) override
        {
            if (outer != nullptr)
            {
                return CLASS_E_NOAGGREGATION;
            }

            ExplorerCommand* command = new (std::nothrow) ExplorerCommand(_verb);
            if (command == nullptr)
            {
                return E_OUTOFMEMORY;
            }

            HRESULT hr = command->QueryInterface(riid, object);
            command->Release();
            return hr;
        }

        IFACEMETHODIMP LockServer(BOOL lock) override
        {
            if (lock)
            {
                InterlockedIncrement(&g_moduleRefCount);
            }
            else
            {
                InterlockedDecrement(&g_moduleRefCount);
            }

            return S_OK;
        }

    private:
        ~ClassFactory() = default;

        ModuleRef _moduleRef;
        long _refCount = 1;
        IngestVerb _verb;
    };
}

STDAPI DllGetClassObject(REFCLSID clsid, REFIID riid, void** object)
{
    if (object == nullptr)
    {
        return E_POINTER;
    }

    *object = nullptr;

    IngestVerb verb;
    if (IsEqualCLSID(clsid, CLSID_CopyToFilingCabinet))
    {
        verb = IngestVerb::Copy;
    }
    else if (IsEqualCLSID(clsid, CLSID_MoveToFilingCabinet))
    {
        verb = IngestVerb::Move;
    }
    else
    {
        return CLASS_E_CLASSNOTAVAILABLE;
    }

    ClassFactory* factory = new (std::nothrow) ClassFactory(verb);
    if (factory == nullptr)
    {
        return E_OUTOFMEMORY;
    }

    HRESULT hr = factory->QueryInterface(riid, object);
    factory->Release();
    return hr;
}

STDAPI DllCanUnloadNow()
{
    return g_moduleRefCount == 0 ? S_OK : S_FALSE;
}
