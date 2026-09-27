using System.Buffers.Binary;
using System.Runtime.Versioning;
using Iced.Intel;

namespace AgeLib.Library;

[SupportedOSPlatform("windows")]
internal static unsafe class NativeDetour
{
    private const int JumpLength = 5;
    private const int MaximumInstructionLength = 15;
    private const uint MemCommitReserve = 0x3000;
    private const uint MemRelease = 0x8000;
    private const uint PageExecuteRead = 0x20;
    private const uint PageExecuteReadWrite = 0x40;

    internal static bool TryInstall(nint target, nint replacement, out nint trampoline, out int stolen_length)
    {
        trampoline = 0;
        stolen_length = 0;

        var original_bytes = new ReadOnlySpan<byte>((void*)target, MaximumInstructionLength * 2).ToArray();
        var decoder = Decoder.Create(32, original_bytes, unchecked((ulong)target), DecoderOptions.None);
        while (stolen_length < JumpLength)
        {
            var instruction = decoder.Decode();
            if (instruction.IsInvalid || instruction.FlowControl != FlowControl.Next)
            {
                return false;
            }

            stolen_length += instruction.Length;
        }

        var trampoline_size = (nuint)(stolen_length + JumpLength);
        trampoline = NativeMethods.VirtualAlloc(0, trampoline_size, MemCommitReserve, PageExecuteReadWrite);
        if (trampoline == 0)
        {
            return false;
        }

        new ReadOnlySpan<byte>((void*)target, stolen_length)
            .CopyTo(new Span<byte>((void*)trampoline, stolen_length));
        WriteJump(trampoline + stolen_length, target + stolen_length);

        if (NativeMethods.VirtualProtect(trampoline, trampoline_size, PageExecuteRead, out _) == 0
            || NativeMethods.FlushInstructionCache(NativeMethods.GetCurrentProcess(), trampoline, trampoline_size) == 0)
        {
            NativeMethods.VirtualFree(trampoline, 0, MemRelease);
            trampoline = 0;
            stolen_length = 0;
            return false;
        }

        if (NativeMethods.VirtualProtect(target, (nuint)stolen_length, PageExecuteReadWrite, out var old_protection) == 0)
        {
            NativeMethods.VirtualFree(trampoline, 0, MemRelease);
            trampoline = 0;
            stolen_length = 0;
            return false;
        }

        WriteJump(target, replacement);
        new Span<byte>((void*)(target + JumpLength), stolen_length - JumpLength).Fill(0x90);
        var flushed = NativeMethods.FlushInstructionCache(NativeMethods.GetCurrentProcess(), target, (nuint)stolen_length) != 0;
        if (!flushed)
        {
            new ReadOnlySpan<byte>(original_bytes, 0, stolen_length)
                .CopyTo(new Span<byte>((void*)target, stolen_length));
            NativeMethods.FlushInstructionCache(NativeMethods.GetCurrentProcess(), target, (nuint)stolen_length);
        }

        NativeMethods.VirtualProtect(target, (nuint)stolen_length, old_protection, out _);
        if (!flushed)
        {
            NativeMethods.VirtualFree(trampoline, 0, MemRelease);
            trampoline = 0;
            stolen_length = 0;
            return false;
        }

        return true;
    }

    internal static bool TryRemove(nint target, nint trampoline, int stolen_length)
    {
        if (NativeMethods.VirtualProtect(target, (nuint)stolen_length, PageExecuteReadWrite, out var old_protection) == 0)
        {
            return false;
        }

        new ReadOnlySpan<byte>((void*)trampoline, stolen_length)
            .CopyTo(new Span<byte>((void*)target, stolen_length));
        var restored = NativeMethods.FlushInstructionCache(NativeMethods.GetCurrentProcess(), target, (nuint)stolen_length) != 0;
        NativeMethods.VirtualProtect(target, (nuint)stolen_length, old_protection, out _);
        return restored;
    }

    private static void WriteJump(nint source, nint destination)
    {
        var jump = new Span<byte>((void*)source, JumpLength);
        jump[0] = 0xE9;
        var displacement = unchecked((int)(destination.ToInt64() - (source.ToInt64() + JumpLength)));
        BinaryPrimitives.WriteInt32LittleEndian(jump[1..], displacement);
    }
}