#include <Windows.h>
#include <stdint.h>
#include <detours\detours.h>
#include <iostream>
#include <io.h>
#include "AiExpert.h"

inline static int32_t(__thiscall* FuncRunList)(void* aiExpert, int listId, void* statsOutput) = 0;
static int32_t __stdcall DetouredRunList(int listId, void* statsOutput);

#pragma unmanaged
BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
		case DLL_PROCESS_ATTACH:
		{
			DisableThreadLibraryCalls(hModule);

			const int32_t REFERENCE_BASE_ADDR = 0x400000;
			const int32_t ADDR_FUNC_RUN_LIST = 0x5F9C10;
			uintptr_t BASE_ADDR = (uintptr_t)GetModuleHandle(nullptr);
			uintptr_t func_run = BASE_ADDR + (ADDR_FUNC_RUN_LIST - REFERENCE_BASE_ADDR);
			*reinterpret_cast<uintptr_t*>(&FuncRunList) = func_run;

			DetourTransactionBegin();
			DetourUpdateThread(GetCurrentThread());
			DetourAttach(&(PVOID&)FuncRunList, DetouredRunList);
			LONG transactionResult = DetourTransactionCommit();

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
using namespace System::IO;

#pragma managed
static void TestMethod(int id)
{
	String^ fileName = "F:\\textfile.txt";

	StreamWriter^ sw = gcnew StreamWriter(fileName, true);
	sw->WriteLine(DateTime::Now);
	sw->WriteLine("got id {0}", id);
	sw->Close();
}

#pragma unmanaged
static int32_t __stdcall DetouredRunList(int listId, void* statsOutput)
{
	void* expert;
	__asm mov expert, ECX
	TestMethod((int)listId);
	TestMethod((int)statsOutput);

	return FuncRunList(expert, listId, statsOutput);
}