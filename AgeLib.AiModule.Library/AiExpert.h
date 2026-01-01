#pragma once
#include <stdint.h>
#include "AiAction.h"
#include "AiFact.h"
#include "SymbolHashNode.h"

struct AiExpert
{
	void* vfptr;
	int16_t maxStrings;
	int16_t numStrings;
	char** string;
	int16_t maxFacts;
	int16_t numFacts;
	AiFact* fact;
	int16_t maxActions;
	int16_t numActions;
	AiAction* action;
	int16_t maxLists;
	void* listInfo;
	void* groupTable;
	void* currentRule;
	void* currentList;
	void* currentGroupTable;
	int globalSymbolTableSize;
	SymbolHashNode** bucket;
};

