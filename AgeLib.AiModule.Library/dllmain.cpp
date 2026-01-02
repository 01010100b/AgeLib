#include <Windows.h>
#include <stdint.h>
#include <detours\detours.h>

inline static int32_t(__thiscall* FuncRunList)(void* ai_expert, int list_id, void* stats_output) = 0;
static int32_t __stdcall DetouredRunList(int list_id, void* stats_output);
static intptr_t GamePtr;

#pragma unmanaged
BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
		case DLL_PROCESS_ATTACH:
		{
			DisableThreadLibraryCalls(hModule);

			uintptr_t BASE_ADDR = (uintptr_t)GetModuleHandle(nullptr);
			const int32_t REFERENCE_BASE_ADDR = 0x400000;
			GamePtr = BASE_ADDR + (0x7912A0 - REFERENCE_BASE_ADDR);
			const int32_t ADDR_FUNC_RUN_LIST = 0x5F9C10;
			uintptr_t func_run = BASE_ADDR + (ADDR_FUNC_RUN_LIST - REFERENCE_BASE_ADDR);
			*reinterpret_cast<uintptr_t*>(&FuncRunList) = func_run;

			DetourTransactionBegin();
			DetourUpdateThread(GetCurrentThread());
			DetourAttach(&(PVOID&)FuncRunList, DetouredRunList);
			LONG transaction_result = DetourTransactionCommit();

			break;
		}
		case DLL_PROCESS_DETACH:
		{
			DetourTransactionBegin();
			DetourUpdateThread(GetCurrentThread());
			DetourDetach(&(PVOID&)FuncRunList, DetouredRunList);
			DetourTransactionCommit();
			
			break;
		}
	}

	return TRUE;
}

using namespace System;
using namespace AgeLib::AiModule::Engine;

#pragma managed
static void Passthrough(intptr_t id)
{
	Receiver::Receive((IntPtr)id, (IntPtr)GamePtr);
}

#pragma unmanaged
static int32_t __stdcall DetouredRunList(int list_id, void* stats_output)
{
	void* expert = nullptr;
	__asm mov expert, ECX

	int32_t result = FuncRunList(expert, list_id, stats_output);
	Passthrough((intptr_t)expert);

	return result;
}