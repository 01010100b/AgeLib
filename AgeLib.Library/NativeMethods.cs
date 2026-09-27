using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AgeLib.Library;

internal static partial class NativeMethods
{
    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial nint GetModuleHandleW(nint module_name);

    [LibraryImport("kernel32.dll", EntryPoint = "GetCurrentProcess")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial nint GetCurrentProcess();

    [LibraryImport("kernel32.dll", EntryPoint = "VirtualAlloc")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial nint VirtualAlloc(nint address, nuint size, uint allocation_type, uint protection);

    [LibraryImport("kernel32.dll", EntryPoint = "VirtualFree")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial int VirtualFree(nint address, nuint size, uint free_type);

    [LibraryImport("kernel32.dll", EntryPoint = "VirtualProtect")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial int VirtualProtect(nint address, nuint size, uint new_protection, out uint old_protection);

    [LibraryImport("kernel32.dll", EntryPoint = "FlushInstructionCache")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial int FlushInstructionCache(nint process, nint address, nuint size);

    [LibraryImport("kernel32.dll", EntryPoint = "OutputDebugStringW", StringMarshalling = StringMarshalling.Utf16)]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvStdcall)])]
    internal static partial void OutputDebugString(string message);

}