using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine;

public interface IEngine
{
    public int MyPlayer { get; }

    public int Execute(string name, int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0);
    public bool Check(string name, int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0);

    public IEnumerable<string> GetSymbols();
    public bool IsSymbolDefined(string symbol);

    public int GetGoal(int goal);
    public void SetGoal(int goal, int value);

    public void ChatToAll(string str);
    public void ChatToPlayer(int player, string str);
    public void ChatDataToAll(string str, int type_op, int value);
    public void ChatDataToPlayer(int player, string str, int type_op, int value);

    public void Log(string message);
}
