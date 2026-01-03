using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine;

public interface IEngine
{
    public int MyPlayer { get; }

    public int GetStrategicNumber(int sn);
    public void SetStrategicNumber(int sn, int value);
    public int GetStrategicNumber(StrategicNumber sn);
    public void SetStrategicNumber(StrategicNumber sn, int value);

    public int GetGoal(int goal);
    public void SetGoal(int goal, int value);
    public Point GetPoint(int goal);
    public void SetPoint(int goal, Point point);
    public Cost GetCost(int goal);
    public void SetCost(int goal, Cost cost);
    public SearchState GetSearchState(int goal);
    public void SetSearchState(int goal, SearchState search_state);

    public int Execute(string name, int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0);
    public bool Check(string name, int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0);

    public IEnumerable<string> GetSymbols();
    public bool IsSymbolDefined(string symbol);

    public void ChatToAll(string str);
    public void ChatToPlayer(int player, string str);
    public void ChatDataToAll(string str, int type_op, int value);
    public void ChatDataToPlayer(int player, string str, int type_op, int value);

    public void Log(string message);
}
