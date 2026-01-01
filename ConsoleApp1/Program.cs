using AgeLib.Scripting.Assembly;
using AgeLib.Scripting.Compilation;
using AgeLib.Scripting.Script;
using AgeLib.Scripting.Script.Expressions;
using BinaryLibs.Utils;
using Newtonsoft.Json;
using Reloaded.Injector;
using System.Diagnostics;

namespace ConsoleApp1;

internal class Program
{
    private const string WK_FOLDER = @"F:\AoE\WK\Age2_x1";
    private const string DIST_FOLDER = @"F:\Repos\01010100b\AgeLib\AgeLib.AiModule.Library\dist\Release";

    static void Main(string[] args)
    {
        FileSystem.CopyDirectory(DIST_FOLDER, WK_FOLDER);
        var wk = Path.Combine(WK_FOLDER, "WK.exe");
        var dll = Path.Combine(WK_FOLDER, "AgeLib.AiModule.Library.dll");
        //dll = @"F:\Repos\01010100b\01010100b\AoE2Lib\AoE2Lib\aimodule-aoc.dll";
        var info = new ProcessStartInfo(wk);
        var process = Process.Start(info)!;
        process.WaitForInputIdle();
        Thread.Sleep(10000);
        Console.WriteLine(process.HasExited);
        using var injector = new Injector(process);
        var addr = injector.Inject(dll);
        Console.WriteLine($"addr {addr}");
        Thread.Sleep(5000);
    }
}
