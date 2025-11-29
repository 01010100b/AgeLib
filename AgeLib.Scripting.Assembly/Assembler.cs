using AgeLib.Scripting.Assembly.Instructions;
using AgeLib.Scripting.Script;
using AgeLib.Scripting.Script.Expressions;
using BinaryLibs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Assembly;

public class Assembler
{
    public int MaxCommandsPerRule { get; set; } = 32;

    public Per Assemble(IEnumerable<Instruction> instructions)
    {
        var per = new Per();
        var rule = new Rule();

        foreach (var instruction in instructions)
        {
            Assert.That(rule.CommandCount <= MaxCommandsPerRule);

            if (rule.CommandCount == MaxCommandsPerRule)
            {
                per.Rules.Add(rule);
                rule = new();
            }

            if (instruction is RuleInstruction ri)
            {
                per.Rules.Add(rule);
                rule = new()
                {
                    Facts = [.. ri.Facts],
                    Actions = [.. ri.Actions]
                };

                per.Rules.Add(rule);
                rule = new();
            }
            else if (instruction is LabelInstruction li)
            {
                if (!rule.IsEmpty)
                {
                    per.Rules.Add(rule);
                    rule = new();
                }

                rule.Labels.Add(li.Label);
            }
            else if (instruction is CommandInstruction ci)
            {
                rule.Actions.Add(ci.Command);
            }
            else if (instruction is JumpInstruction ji)
            {
                rule.Actions.Add(new()
                {
                    Name = "up-jump-direct",
                    Arg0 = "c:",
                    Arg1 = ji.Label
                });

                per.Rules.Add(rule);
                rule = new();
            }
            else if (instruction is JumpIndirectInstruction jii)
            {
                rule.Actions.Add(new()
                {
                    Name = "up-jump-direct",
                    Arg0 = "g:",
                    Arg1 = jii.Goal.ToString()
                });

                per.Rules.Add(rule);
                rule = new();
            }
            else if (instruction is SetReturnAndJumpInstruction sraji)
            {
                if (rule.CommandCount > MaxCommandsPerRule - 3)
                {
                    per.Rules.Add(rule);
                    rule = new();
                }

                rule.Actions.Add(new()
                {
                    Name = "up-get-rule-id",
                    Arg0 = sraji.ReturnGoal.ToString()
                });
                rule.Actions.Add(new()
                {
                    Name = "up-modify-goal",
                    Arg0 = sraji.ReturnGoal.ToString(),
                    Arg1 = "c:+",
                    Arg2 = "1"
                });
                rule.Actions.Add(new()
                {
                    Name = "up-jump-direct",
                    Arg0 = "c:",
                    Arg1 = sraji.Label
                });

                per.Rules.Add(rule);
                rule = new();
            }
            else if (instruction is JumpIfZeroInstruction jizi)
            {
                if (!rule.IsEmpty)
                {
                    per.Rules.Add(rule);
                    rule = new();
                }

                rule.Facts.Add(new AtomicExpression()
                {
                    Command = new()
                    {
                        Name = "up-compare-goal",
                        Arg0 = jizi.Goal.ToString(),
                        Arg1 = "c:==",
                        Arg2 = "0"
                    }
                });

                rule.Actions.Add(new()
                {
                    Name = "up-jump-direct",
                    Arg0 = "c:",
                    Arg1 = jizi.Label
                });

                per.Rules.Add(rule);
                rule = new();
            }
            else if (instruction is JumpIfNotZeroInstruction jinzi)
            {
                if (!rule.IsEmpty)
                {
                    per.Rules.Add(rule);
                    rule = new();
                }

                rule.Facts.Add(new AtomicExpression()
                {
                    Command = new()
                    {
                        Name = "up-compare-goal",
                        Arg0 = jinzi.Goal.ToString(),
                        Arg1 = "c:!=",
                        Arg2 = "0"
                    }
                });

                rule.Actions.Add(new()
                {
                    Name = "up-jump-direct",
                    Arg0 = "c:",
                    Arg1 = jinzi.Label
                });

                per.Rules.Add(rule);
                rule = new();
            }
            else
            {
                throw new NotImplementedException($"Instruction type {instruction.GetType().Name} not implemented");
            }
        }

        per.Rules.Add(rule);

        return per;
    }
}
