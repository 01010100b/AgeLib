using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Common;

public static class CompareOp
{
    public static int C_LESS_THAN { get; private set; } = 0;
    public static int C_LESS_OR_EQUAL { get; private set; } = 1;
    public static int C_GREATER_THAN { get; private set; } = 2;
    public static int C_GREATER_OR_EQUAL { get; private set; } = 3;
    public static int C_EQUAL { get; private set; } = 4;
    public static int C_NOT_EQUAL { get; private set; } = 5;
    public static int G_LESS_THAN { get; private set; } = 7;
    public static int G_LESS_OR_EQUAL { get; private set; } = 8;
    public static int G_GREATER_THAN { get; private set; } = 9;
    public static int G_GREATER_OR_EQUAL { get; private set; } = 10;
    public static int G_EQUAL { get; private set; } = 11;
    public static int G_NOT_EQUAL { get; private set; } = 12;
    public static int S_LESS_THAN { get; private set; } = 14;
    public static int S_LESS_OR_EQUAL { get; private set; } = 15;
    public static int S_GREATER_THAN { get; private set; } = 16;
    public static int S_GREATER_OR_EQUAL { get; private set; } = 17;
    public static int S_EQUAL { get; private set; } = 18;
    public static int S_NOT_EQUAL { get; private set; } = 19;
}