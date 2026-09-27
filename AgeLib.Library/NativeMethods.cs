using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AgeLib.Library;

internal static partial class NativeMethods
{
    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial nint GetModuleHandleW(nint module_name);

    [LibraryImport("kernel32.dll", EntryPoint = "GetCurrentThread")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial nint GetCurrentThread();

    [LibraryImport("kernel32.dll", EntryPoint = "OutputDebugStringW", StringMarshalling = StringMarshalling.Utf16)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial void OutputDebugString(string message);

    [LibraryImport("detours", EntryPoint = "DetourTransactionBegin")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial int DetourTransactionBegin();

    [LibraryImport("detours", EntryPoint = "DetourUpdateThread")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial int DetourUpdateThread(nint thread);

    [LibraryImport("detours", EntryPoint = "DetourAttach")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial int DetourAttach(ref nint pointer, nint detour);

    [LibraryImport("detours", EntryPoint = "DetourTransactionCommit")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial int DetourTransactionCommit();

    [LibraryImport("detours", EntryPoint = "DetourDetach")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial int DetourDetach(ref nint pointer, nint detour);
}