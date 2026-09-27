using Microsoft.Win32;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace AgeLib.Library;

[SupportedOSPlatform("windows")]
internal static unsafe class CoreClrHost
{
    private const int LoadAssemblyAndGetFunctionPointer = 5;
    private static readonly object SyncRoot = new();

    private static nint HostFxrLibrary;
    private static nint HostContext;
    private static nint ReceiverFunctionPointer;
    private static bool StartupAttempted;

    internal static void Receive(nint config_ptr)
    {
        if (ReceiverFunctionPointer == 0)
        {
            EnsureStarted();
        }

        if (ReceiverFunctionPointer != 0)
        {
            if (!ReceiverCallLogged)
            {
                ReceiverCallLogged = true;
                LibraryLog.Write("Calling AgeLib.Engine.Receiver.Receive for the first time.");
            }

            ((delegate* unmanaged<int, nint, void>)ReceiverFunctionPointer)(15, config_ptr);
        }
    }

    private static void EnsureStarted()
    {
        lock (SyncRoot)
        {
            if (StartupAttempted)
            {
                return;
            }

            StartupAttempted = true;
            LibraryLog.Write("Starting CoreCLR host initialization.");

            try
            {
                StartRuntime();
            }
            catch (Exception exception)
            {
                LibraryLog.Write($"CoreCLR host initialization failed: {exception}");
                NativeMethods.OutputDebugString(exception.ToString());
            }
        }
    }

    private static void StartRuntime()
    {
        var folder = Path.GetDirectoryName(Environment.ProcessPath) ?? throw new InvalidOperationException("Could not locate the process directory.");
        var runtime_config = Path.Combine(folder, "AgeLib.Engine.runtimeconfig.json");
        var engine_assembly = Path.Combine(folder, "AgeLib.Engine.dll");
        LibraryLog.Write($"Process folder: {folder}; runtime config exists={File.Exists(runtime_config)}; Engine assembly exists={File.Exists(engine_assembly)}.");
        if (!File.Exists(runtime_config) || !File.Exists(engine_assembly))
        {
            throw new FileNotFoundException("The Engine assembly and runtime configuration must be beside the game executable.");
        }

        var hostfxr_path = FindHostFxr();
        LibraryLog.Write($"Loading hostfxr: {hostfxr_path}.");
        HostFxrLibrary = NativeLibrary.Load(hostfxr_path);
        var initialize = (delegate* unmanaged[Cdecl]<char*, nint, nint*, int>)NativeLibrary.GetExport(HostFxrLibrary, "hostfxr_initialize_for_runtime_config");
        var get_runtime_delegate = (delegate* unmanaged[Cdecl]<nint, int, nint*, int>)NativeLibrary.GetExport(HostFxrLibrary, "hostfxr_get_runtime_delegate");

        fixed (char* runtime_config_ptr = runtime_config)
        {
            nint host_context = 0;
            var status = initialize(runtime_config_ptr, 0, &host_context);
            if (status < 0)
            {
                throw new InvalidOperationException($"hostfxr_initialize_for_runtime_config failed: 0x{status:X8}.");
            }

            HostContext = host_context;
            LibraryLog.Write($"hostfxr initialized with status 0x{status:X8}.");
        }

        nint load_assembly_ptr = 0;
        var delegate_status = get_runtime_delegate(HostContext, LoadAssemblyAndGetFunctionPointer, &load_assembly_ptr);
        if (delegate_status < 0 || load_assembly_ptr == 0)
        {
            throw new InvalidOperationException($"hostfxr_get_runtime_delegate failed: 0x{delegate_status:X8}.");
        }
        LibraryLog.Write($"Acquired load_assembly_and_get_function_pointer delegate, status 0x{delegate_status:X8}.");

        var load_assembly_and_get_function_pointer = (delegate* unmanaged[Cdecl]<char*, char*, char*, char*, nint, nint*, int>)load_assembly_ptr;
        var type_name = "AgeLib.Engine.Receiver, AgeLib.Engine";
        var method_name = "Receive";
        nint receiver = 0;
        fixed (char* engine_assembly_ptr = engine_assembly)
        fixed (char* type_name_ptr = type_name)
        fixed (char* method_name_ptr = method_name)
        {
            var status = load_assembly_and_get_function_pointer(
                engine_assembly_ptr,
                type_name_ptr,
                method_name_ptr,
                unchecked((char*)(nint)(-1)),
                0,
                &receiver);
            if (status < 0 || receiver == 0)
            {
                throw new InvalidOperationException($"Loading AgeLib.Engine.Receiver.Receive failed: 0x{status:X8}.");
            }
        }

        ReceiverFunctionPointer = receiver;
        LibraryLog.Write("AgeLib.Engine.Receiver.Receive resolved successfully.");
    }

    private static string FindHostFxr()
    {
        var install_root = Environment.GetEnvironmentVariable("DOTNET_ROOT(x86)")
            ?? Environment.GetEnvironmentVariable("DOTNET_ROOT")
            ?? GetRegistryInstallRoot()
            ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "dotnet");
        var fxr_root = Path.Combine(install_root, "host", "fxr");
        if (!Directory.Exists(fxr_root))
        {
            throw new DirectoryNotFoundException($"The x86 .NET hostfxr directory was not found: {fxr_root}");
        }

        string? latest_directory = null;
        var latest_version = new Version(0, 0);
        foreach (var directory in Directory.GetDirectories(fxr_root))
        {
            if (Version.TryParse(Path.GetFileName(directory), out var version) && version > latest_version)
            {
                latest_version = version;
                latest_directory = directory;
            }
        }

        var hostfxr_path = latest_directory is null ? null : Path.Combine(latest_directory, "hostfxr.dll");
        return hostfxr_path is not null && File.Exists(hostfxr_path)
            ? hostfxr_path
            : throw new FileNotFoundException($"No x86 hostfxr.dll was found under {fxr_root}.");
    }

    private static string? GetRegistryInstallRoot()
    {
        using var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)
            .OpenSubKey(@"SOFTWARE\dotnet\Setup\InstalledVersions\x86");
        return key?.GetValue("InstallLocation") as string;
    }

    private static bool ReceiverCallLogged;
}