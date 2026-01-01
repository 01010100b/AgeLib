#pragma once
#include <stdint.h>

struct SymbolHashNode
{
	SymbolHashNode* next;
	char* symbolText;
	uint8_t type;
	uint16_t id;
};

