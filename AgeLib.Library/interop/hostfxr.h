#pragma once

// Minimal subset of the .NET native hosting "hostfxr.h" contract
// (the handful of exports needed to initialize a runtime config and
// resolve a load_assembly_and_get_function_pointer delegate), declared
// locally so this project does not depend on the .NET SDK headers.

#include <cstdint>
#include "coreclr_delegates.h"

using hostfxr_handle = void*;

struct hostfxr_initialize_parameters
{
	size_t size;
	const char_t* host_path;
	const char_t* dotnet_root;
};

enum hostfxr_delegate_type
{
	hdt_com_activation,
	hdt_load_in_memory_assembly,
	hdt_winrt_activation,
	hdt_com_register,
	hdt_com_unregister,
	hdt_load_assembly_and_get_function_pointer,
	hdt_get_function_pointer,
	hdt_load_assembly,
	hdt_load_assembly_bytes,
};

using hostfxr_initialize_for_runtime_config_fn = int32_t(__stdcall*)(
	const char_t* runtime_config_path,
	const hostfxr_initialize_parameters* parameters,
	/*out*/ hostfxr_handle* host_context_handle);

using hostfxr_get_runtime_delegate_fn = int32_t(__stdcall*)(
	hostfxr_handle host_context_handle,
	hostfxr_delegate_type type,
	/*out*/ void** delegate);

using hostfxr_close_fn = int32_t(__stdcall*)(hostfxr_handle host_context_handle);
