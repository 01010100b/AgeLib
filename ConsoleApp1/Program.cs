using AgeLib.Scripting.Assembly;
using AgeLib.Scripting.Compilation;
using AgeLib.Scripting.Script;
using AgeLib.Scripting.Script.Expressions;
using Newtonsoft.Json;

namespace ConsoleApp1;

internal class Program
{
    static void Main(string[] args)
    {
        var module = TestModule.Create();
        var compiler = new Compiler();
        var instructions = compiler.Compile(module, "Test.Main", []);

        var json = JsonConvert.SerializeObject(instructions, Formatting.Indented);
        //Console.WriteLine(json);

        var assembler = new Assembler();
        var per = assembler.Assemble(instructions);

        Console.WriteLine(per.ToString());
    }
}
