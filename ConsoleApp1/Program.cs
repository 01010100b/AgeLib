using AgeLib.Scripting.Script;
using AgeLib.Scripting.Script.Expressions;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var f1 = new Command("up-compare-goal", "1", "c:=", "2");
            var f2 = new Command("up-compare-goal", "2", "c:=", "3");
            var f3 = new Command("up-compare-goal", "3", "c:=", "4");

            var expr = new OrExpression(
                new AndExpression(new AtomicExpression(f1), new AtomicExpression(f2)),
                new NotExpression(new AtomicExpression(f3))
                );

            var rule = new Rule();
            rule.Facts.Add(expr);

            Console.WriteLine(rule.ToString());
        }
    }
}
