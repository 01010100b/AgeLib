#pragma once

#include <string>

// Loads hostfxr.dll, initializes a CoreCLR runtime for a given
// runtimeconfig.json, and resolves a native function pointer to an
// [UnmanagedCallersOnly] managed method.
class NetHost
{
public:
	// Locates and loads hostfxr, starts the runtime described by
	// runtime_config_path, and resolves method_name on type_name
	// (assembly-qualified, e.g. L"AgeLib.Engine.Receiver, AgeLib.Engine")
	// inside assembly_path. Returns false and leaves GetFunctionPointer()
	// null on any failure.
	bool Load(const std::wstring& runtime_config_path, const std::wstring& assembly_path,
		const std::wstring& type_name, const std::wstring& method_name);

	void* GetFunctionPointer() const { return function_pointer_; }

private:
	void* function_pointer_ = nullptr;
};
