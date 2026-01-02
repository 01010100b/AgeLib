using BinaryLibs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine;

internal class Command
{
    public string Name { get; }
    public IntPtr Function { get; }
    public int Argc { get; }
    public bool IsFact { get; }
    public int Calls { get; private set; } = 0;

    public Command(string name, IntPtr function, int argc, bool returns_value)
    {
        Assert.NotEmpty(name);
        Assert.That(function != IntPtr.Zero);
        Assert.That(argc >= 0 && argc <= 4);

        Name = name;
        Function = function;
        Argc = argc;
        IsFact = returns_value;
    }

    public int Execute(int arg1 = 0, int arg2 = 0, int arg3 = 0, int arg4 = 0)
    {
        Calls++;

        unsafe
        {
            if (IsFact)
            {
                switch (Argc)
                {
                    case 0:
                        return ((delegate* unmanaged<int>)Function)();
                    case 1:
                        return ((delegate* unmanaged<int, int>)Function)(arg1);
                    case 2:
                        return ((delegate* unmanaged<int, int, int>)Function)(arg1, arg2);
                    case 3:
                        return ((delegate* unmanaged<int, int, int, int>)Function)(arg1, arg2, arg3);
                    case 4:
                        return ((delegate* unmanaged<int, int, int, int, int>)Function)(arg1, arg2, arg3, arg4);
                }

                return 0;
            }
            else
            {
                switch (Argc)
                {
                    case 0:
                        ((delegate* unmanaged<void>)Function)(); break;
                    case 1:
                        ((delegate* unmanaged<int, void>)Function)(arg1); break;
                    case 2:
                        ((delegate* unmanaged<int, int, void>)Function)(arg1, arg2); break;
                    case 3:
                        ((delegate* unmanaged<int, int, int, void>)Function)(arg1, arg2, arg3); break;
                    case 4:
                        ((delegate* unmanaged<int, int, int, int, void>)Function)(arg1, arg2, arg3, arg4); break;
                }

                return 1;
            }
        }
    }
}
