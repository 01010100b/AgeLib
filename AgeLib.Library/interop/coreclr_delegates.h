#pragma once

// Minimal subset of the .NET native hosting "coreclr_delegates.h" contract
// (function pointer typedefs for the delegates returned by hostfxr),
// declared locally so this project does not depend on the .NET SDK headers.

#include <cstdint>

using char_t = wchar_t;

using load_assembly_and_get_function_pointer_fn = int32_t(__stdcall*)(
	const char_t* assembly_path,
	const char_t* type_name,
	const char_t* method_name,
	const char_t* delegate_type_name,
	void* reserved,
	/*out*/ void** delegate);

// Sentinel passed as delegate_type_name to bind directly to an
// [UnmanagedCallersOnly] method instead of a managed delegate type.
static const char_t* const UNMANAGEDCALLERSONLY_METHOD = reinterpret_cast<const char_t*>(-1);
