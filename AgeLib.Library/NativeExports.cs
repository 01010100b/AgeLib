using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace AgeLib.Library;

[SupportedOSPlatform("windows")]
public static class NativeExports
{
    /// <summary>
    /// Initializes the game hooks after the library is loaded.
    /// </summary>
    /// <param name="unused">Reserved for the remote-thread startup signature.</param>
    /// <returns>One when hook installation succeeds; otherwise zero.</returns>
    [UnmanagedCallersOnly(EntryPoint = "Initialize", CallConvs = [typeof(CallConvStdcall)])]
    public static int Initialize(nint unused)
    {
        LibraryLog.Write("Initialize export entered.");

        try
        {
            return ModuleStartup.InitializeHooks() ? 1 : 0;
        }
        catch (Exception exception)
        {
            LibraryLog.Write($"Hook initialization failed: {exception}");
            return 0;
        }
    }
}