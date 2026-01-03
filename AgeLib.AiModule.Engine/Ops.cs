using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine;

public static class TypeOp
{
    public static int C { get; private set; } = 6;
    public static int G { get; private set; } = 13;
    public static int S { get; private set; } = 20;
}

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

public static class MathOp
{
    public static int C_SET { get; private set; } = 0;
    public static int C_ADD { get; private set; } = 1;
    public static int C_SUB { get; private set; } = 2;
    public static int C_MUL { get; private set; } = 3;
    public static int C_DIV { get; private set; } = 4;
    public static int C_DIV_ROUND { get; private set; } = 5;
    public static int C_MOD { get; private set; } = 6;
    public static int C_MIN { get; private set; } = 7;
    public static int C_MAX { get; private set; } = 8;
    public static int C_NEG { get; private set; } = 9;
    public static int C_PERC_DIV { get; private set; } = 10;
    public static int C_PERC_MUL { get; private set; } = 11;
    public static int G_SET { get; private set; } = 12;
    public static int G_ADD { get; private set; } = 13;
    public static int G_SUB { get; private set; } = 14;
    public static int G_MUL { get; private set; } = 15;
    public static int G_DIV { get; private set; } = 16;
    public static int G_DIV_ROUND { get; private set; } = 17;
    public static int G_MOD { get; private set; } = 18;
    public static int G_MIN { get; private set; } = 19;
    public static int G_MAX { get; private set; } = 20;
    public static int G_NEG { get; private set; } = 21;
    public static int G_PERC_DIV { get; private set; } = 22;
    public static int G_PERC_MUL { get; private set; } = 23;
    public static int S_SET { get; private set; } = 24;
    public static int S_ADD { get; private set; } = 25;
    public static int S_SUB { get; private set; } = 26;
    public static int S_MUL { get; private set; } = 27;
    public static int S_DIV { get; private set; } = 28;
    public static int S_DIV_ROUND { get; private set; } = 29;
    public static int S_MOD { get; private set; } = 30;
    public static int S_MIN { get; private set; } = 31;
    public static int S_MAX { get; private set; } = 32;
    public static int S_NEG { get; private set; } = 33;
    public static int S_PERC_DIV { get; private set; } = 34;
    public static int S_PERC_MUL { get; private set; } = 35;
}