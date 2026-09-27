#include "NetHost.h"
#include "interop/hostfxr.h"

#include <Windows.h>
#include <algorithm>
#include <sstream>
#include <vector>

namespace
{
	std::wstring GetEnvVar(const wchar_t* name)
	{
		wchar_t buffer[MAX_PATH];
		DWORD length = GetEnvironmentVariableW(name, buffer, MAX_PATH);

		return (length > 0 && length < MAX_PATH) ? std::wstring(buffer, length) : std::wstring();
	}

	std::wstring GetInstallLocationFromRegistry()
	{
		HKEY key = nullptr;

		if (RegOpenKeyExW(HKEY_LOCAL_MACHINE, L"SOFTWARE\\dotnet\\Setup\\InstalledVersions\\x86", 0, KEY_READ, &key) != ERROR_SUCCESS)
		{
			return std::wstring();
		}

		wchar_t buffer[MAX_PATH] = { };
		DWORD size = sizeof(buffer);
		DWORD type = 0;
		LONG result = RegQueryValueExW(key, L"InstallLocation", nullptr, &type, reinterpret_cast<LPBYTE>(buffer), &size);
		RegCloseKey(key);

		return (result == ERROR_SUCCESS && type == REG_SZ) ? std::wstring(buffer) : std::wstring();
	}

	// Finds the .NET install root (the folder containing "host\fxr\<version>\hostfxr.dll").
	std::wstring FindDotNetRoot()
	{
		std::wstring root = GetEnvVar(L"DOTNET_ROOT(x86)");

		if (!root.empty())
		{
			return root;
		}

		root = GetEnvVar(L"DOTNET_ROOT");

		if (!root.empty())
		{
			return root;
		}

		root = GetInstallLocationFromRegistry();

		if (!root.empty())
		{
			return root;
		}

		return L"C:\\Program Files (x86)\\dotnet";
	}

	std::vector<int> ParseVersion(const std::wstring& version)
	{
		std::vector<int> parts;
		std::wstringstream stream(version);
		std::wstring segment;

		while (std::getline(stream, segment, L'.'))
		{
			parts.push_back(_wtoi(segment.c_str()));
		}

		return parts;
	}

	bool IsVersionLess(const std::wstring& a, const std::wstring& b)
	{
		return ParseVersion(a) < ParseVersion(b);
	}

	// Picks the highest-versioned "host\fxr\<version>" folder under the .NET install root.
	std::wstring FindHostFxrPath(const std::wstring& dotnet_root)
	{
		std::wstring fxr_dir = dotnet_root + L"\\host\\fxr";
		WIN32_FIND_DATAW find_data = { };
		HANDLE find_handle = FindFirstFileW((fxr_dir + L"\\*").c_str(), &find_data);

		if (find_handle == INVALID_HANDLE_VALUE)
		{
			return std::wstring();
		}

		std::wstring best_version;

		do
		{
			if ((find_data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) == 0)
			{
				continue;
			}

			std::wstring name = find_data.cFileName;

			if (name == L"." || name == L"..")
			{
				continue;
			}

			if (best_version.empty() || IsVersionLess(best_version, name))
			{
				best_version = name;
			}
		} while (FindNextFileW(find_handle, &find_data));

		FindClose(find_handle);

		if (best_version.empty())
		{
			return std::wstring();
		}

		return fxr_dir + L"\\" + best_version + L"\\hostfxr.dll";
	}
}

bool NetHost::Load(const std::wstring& runtime_config_path, const std::wstring& assembly_path,
	const std::wstring& type_name, const std::wstring& method_name)
{
	std::wstring hostfxr_path = FindHostFxrPath(FindDotNetRoot());

	if (hostfxr_path.empty())
	{
		return false;
	}

	HMODULE hostfxr = LoadLibraryW(hostfxr_path.c_str());

	if (hostfxr == nullptr)
	{
		return false;
	}

	auto init_fn = reinterpret_cast<hostfxr_initialize_for_runtime_config_fn>(GetProcAddress(hostfxr, "hostfxr_initialize_for_runtime_config"));
	auto get_delegate_fn = reinterpret_cast<hostfxr_get_runtime_delegate_fn>(GetProcAddress(hostfxr, "hostfxr_get_runtime_delegate"));
	auto close_fn = reinterpret_cast<hostfxr_close_fn>(GetProcAddress(hostfxr, "hostfxr_close"));

	if (init_fn == nullptr || get_delegate_fn == nullptr || close_fn == nullptr)
	{
		return false;
	}

	hostfxr_handle host_context = nullptr;
	int32_t result = init_fn(runtime_config_path.c_str(), nullptr, &host_context);

	if (result < 0 || host_context == nullptr)
	{
		return false;
	}

	load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = nullptr;
	result = get_delegate_fn(host_context, hdt_load_assembly_and_get_function_pointer, reinterpret_cast<void**>(&load_assembly_and_get_function_pointer));
	close_fn(host_context);

	if (result < 0 || load_assembly_and_get_function_pointer == nullptr)
	{
		return false;
	}

	result = load_assembly_and_get_function_pointer(
		assembly_path.c_str(),
		type_name.c_str(),
		method_name.c_str(),
		UNMANAGEDCALLERSONLY_METHOD,
		nullptr,
		&function_pointer_);

	return result >= 0 && function_pointer_ != nullptr;
}
