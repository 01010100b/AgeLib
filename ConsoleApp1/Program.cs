using AgeLib.Scripting.Assembly;
using AgeLib.Scripting.Compilation;
using AgeLib.Scripting.Script;
using AgeLib.Scripting.Script.Expressions;
using BinaryLibs.Utils;
using Newtonsoft.Json;
using Reloaded.Injector;
using System.Diagnostics;
using System.Text;

namespace ConsoleApp1;

internal class Program
{
#if DEBUG
    private const string CONFIG = "Debug";
#else
    private const string CONFIG = "Release";
#endif
    private const string WK_FOLDER = @"F:\AoE\WK\Age2_x1";
    private const string LIB_FOLDER = @$"F:\Repos\01010100b\AgeLib\AgeLib.AiModule.Library\dist\{CONFIG}";
    private const string ENGINE_FOLDER = @$"F:\Repos\01010100b\AgeLib\AgeLib.AiModule.Engine\bin\{CONFIG}\net8.0";

    static void Main(string[] args)
    {
        FileSystem.CopyDirectory(LIB_FOLDER, WK_FOLDER);
        FileSystem.CopyDirectory(ENGINE_FOLDER, WK_FOLDER);

        var wk = Path.Combine(WK_FOLDER, "WK.exe");
        var dll = Path.Combine(WK_FOLDER, "AgeLib.AiModule.Library.dll");
        var info = new ProcessStartInfo(wk);
        var process = Process.Start(info)!;
        process.WaitForInputIdle();
        Thread.Sleep(10000);

        using var injector = new Injector(process);
        var addr = injector.Inject(dll);
        Console.WriteLine($"addr {addr}");
        Thread.Sleep(5000);
    }
}
