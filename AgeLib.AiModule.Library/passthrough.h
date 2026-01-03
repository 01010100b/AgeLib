#pragma once
#include <stdint.h>

using namespace System;
using namespace AgeLib::AiModule::Engine;

#pragma managed
static void Passthrough(int version, intptr_t config_ptr)
{
	Receiver::Receive(version, (IntPtr)config_ptr);
}