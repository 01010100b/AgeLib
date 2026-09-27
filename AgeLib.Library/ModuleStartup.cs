using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace AgeLib.Library;

[SupportedOSPlatform("windows")]
internal static unsafe class ModuleStartup
{
    private const int CustomStringId = 89733;
    private const uint GameAddress = 0x7912A0;
    private const uint FuncRunListAddress = 0x5F9C10;
    private const uint FuncGetStringAddress = 0x5F9950;

    private static Config* ConfigPointer;
    private static byte* CustomString;
    private static nint FuncRunList;
    private static nint FuncGetString;

    internal static bool InitializeHooks()
    {
        LibraryLog.Write("Installing game hooks.");
        ConfigPointer = (Config*)NativeMemory.AllocZeroed((nuint)sizeof(Config));
        CustomString = (byte*)NativeMemory.AllocZeroed(256);
        ConfigPointer->GamePtr = Translate(GameAddress);
        ConfigPointer->CustomStringPtr = (nint)CustomString;

        FuncRunList = Translate(FuncRunListAddress);
        FuncGetString = Translate(FuncGetStringAddress);
        LibraryLog.Write($"Hook targets translated. Game=0x{(nuint)ConfigPointer->GamePtr:X}, RunList=0x{(nuint)FuncRunList:X}, GetString=0x{(nuint)FuncGetString:X}.");

        var begin_status = NativeMethods.DetourTransactionBegin();
        var update_status = NativeMethods.DetourUpdateThread(NativeMethods.GetCurrentThread());
        var run_list_status = NativeMethods.DetourAttach(ref FuncRunList, (nint)(delegate* unmanaged[Thiscall]<nint, int, nint, int>)&DetouredRunList);
        var get_string_status = NativeMethods.DetourAttach(ref FuncGetString, (nint)(delegate* unmanaged[Thiscall]<nint, int, byte*>)&DetouredGetString);
        var commit_status = NativeMethods.DetourTransactionCommit();
        LibraryLog.Write($"Detours results: begin={begin_status}, updateThread={update_status}, attachRunList={run_list_status}, attachGetString={get_string_status}, commit={commit_status}.");

        return begin_status == 0
            && update_status == 0
            && run_list_status == 0
            && get_string_status == 0
            && commit_status == 0;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvThiscall)])]
    private static int DetouredRunList(nint expert_ptr, int list_id, nint stats_output)
    {
        if (RunListCallCount == 0)
        {
            LibraryLog.Write($"Run-list hook entered. ListId={list_id}, expert=0x{(nuint)expert_ptr:X}.");
        }

        RunListCallCount++;
        ConfigPointer->ExpertPtr = expert_ptr;
        CoreClrHost.Receive((nint)ConfigPointer);

        return ((delegate* unmanaged[Thiscall]<nint, int, nint, int>)FuncRunList)(expert_ptr, list_id, stats_output);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvThiscall)])]
    private static byte* DetouredGetString(nint expert_engine, int string_id)
    {
        if (string_id == CustomStringId)
        {
            if (!CustomStringHookLogged)
            {
                CustomStringHookLogged = true;
                LibraryLog.Write($"Custom string hook served ID {string_id}.");
            }

            return CustomString;
        }

        return ((delegate* unmanaged[Thiscall]<nint, int, byte*>)FuncGetString)(expert_engine, string_id);
    }

    private static nint Translate(uint address)
    {
        var module_base = unchecked((uint)NativeMethods.GetModuleHandleW(0));
        return unchecked((nint)(module_base + address - 0x400000u));
    }

    private static long RunListCallCount;
    private static bool CustomStringHookLogged;

    [StructLayout(LayoutKind.Sequential)]
    private struct Config
    {
        public nint ExpertPtr;
        public nint GamePtr;
        public nint CustomStringPtr;
    }
}