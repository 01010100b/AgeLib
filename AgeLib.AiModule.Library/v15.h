#pragma once
#include <Windows.h>
#include <stdint.h>
#include <detours\detours.h>
#include "passthrough.h"

namespace V15
{
	struct Config
	{
		intptr_t expert_ptr = 0;
		intptr_t game_ptr = 0;
		intptr_t custom_string_ptr = 0;
	};

	static Config config;
	inline static char custom_string[256] = { };

	static const int CUSTOM_STRING_ID = 89733;
	static const uintptr_t GAME_ADDR = 0x7912A0;
	static const uintptr_t FUNC_RUN_LIST_ADDR = 0x5F9C10;
	static const uintptr_t FUNC_GET_STRING_ADDR = 0x5F9950;

#pragma unmanaged
	static uintptr_t Translate(uintptr_t addr)
	{
		static const uintptr_t BASE_ADDR = (uintptr_t)GetModuleHandle(nullptr);
		static const uintptr_t REFERENCE_BASE_ADDR = 0x400000;

		return BASE_ADDR + (addr - REFERENCE_BASE_ADDR);
	}

	inline static int32_t(__thiscall* FuncRunList)(void* ai_expert, int list_id, void* stats_output) = 0;
	static int32_t __stdcall DetouredRunList(int list_id, void* stats_output);

	inline static char* (__thiscall* FuncGetString)(void* ai_expert_engine, int string_id) = 0;
	static char* __stdcall DetouredGetString(int stringId);

#pragma unmanaged
	static void Attach()
	{
		config.game_ptr = Translate(GAME_ADDR);
		config.custom_string_ptr = (intptr_t)&custom_string;
		*reinterpret_cast<uintptr_t*>(&FuncRunList) = Translate(FUNC_RUN_LIST_ADDR);
		*reinterpret_cast<uintptr_t*>(&FuncGetString) = Translate(FUNC_GET_STRING_ADDR);

		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(PVOID&)FuncRunList, DetouredRunList);
		DetourAttach(&(PVOID&)FuncGetString, DetouredGetString);
		LONG transaction_result = DetourTransactionCommit();
	}

#pragma unmanaged
	static void Detach()
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(&(PVOID&)FuncRunList, DetouredRunList);
		DetourDetach(&(PVOID&)FuncGetString, DetouredGetString);
		DetourTransactionCommit();
	}

#pragma unmanaged
	static int32_t __stdcall DetouredRunList(int list_id, void* stats_output)
	{
		void* expert = nullptr;
		__asm mov expert, ECX

		config.expert_ptr = (intptr_t)expert;
		Passthrough(15, (intptr_t)&config);

		return FuncRunList(expert, list_id, stats_output);
	}

#pragma unmanaged
	char* __stdcall DetouredGetString(int string_id)
	{
		void* expert_engine = nullptr;
		__asm mov expert_engine, ECX

		if (string_id == CUSTOM_STRING_ID)
		{
			return custom_string;
		}
		else
		{
			return FuncGetString(expert_engine, string_id);
		}
	}
}