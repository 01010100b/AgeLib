#include <Windows.h>
#include "v15.h"

#pragma unmanaged
BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
		case DLL_PROCESS_ATTACH:
		{
			DisableThreadLibraryCalls(hModule);
			V15::Attach();

			break;
		}
		case DLL_PROCESS_DETACH:
		{
			V15::Detach();

			break;
		}
	}

	return TRUE;
}