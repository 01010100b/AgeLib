using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine;

public interface IEngine
{
    public int MyPlayer { get; }

    public bool Check(string name, int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0)
        => Execute(name, arg1, arg2, arg3, arg4) != 0;
    public int Execute(string name, int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0);
    public int GetGoal(int goal);
}
