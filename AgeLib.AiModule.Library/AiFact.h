#pragma once
#include <stdint.h>

struct AiFact
{
	int32_t type;
	int32_t touched;
	int32_t lastResult;
	uint8_t argc;
	void* factFn;
	uint8_t arg1Type;
	uint8_t arg2Type;
	uint8_t arg3Type;
	uint8_t arg4Type;
};

