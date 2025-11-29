using AgeLib.Scripting.Assembly;
using AgeLib.Scripting.Assembly.Instructions;
using AgeLib.Scripting.Compilation.Compilation;
using AgeLib.Scripting.Script;
using AgeLib.Scripting.Script.Expressions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation;

public class Compiler
{
    public int MaxGoals { get; init; } = 16000;

    public List<Instruction> Compile(Module module, string main_method, IEnumerable<Func<string, Module?>> resolvers)
    {
        var resolver = new Resolver(module, [.. resolvers]);
        var state = new State(resolver);
        var instructions = new List<Instruction>();

        instructions.AddRange(GetPrefix(state));
        var main = resolver.ResolveMethod(main_method);
        instructions.AddRange(MethodCompiler.Compile(main, state));

        foreach (var method in resolver.ResolvedModules.SelectMany(x => x.Methods).Except([main]))
        {
            instructions.AddRange(MethodCompiler.Compile(method, state));
        }

        instructions.AddRange(GetPostFix(state));

        Console.WriteLine(JsonConvert.SerializeObject(instructions, Formatting.Indented));

        return instructions;
    }

    private List<Instruction> GetPrefix(State state)
    {
        var instructions = new List<Instruction>();
        var label_postinit = Guid.NewGuid().ToString();

        // initialize memory

        instructions.Add(new RuleInstruction()
        {
            Facts =
            [
                new AtomicExpression()
                {
                    Command = new()
                    {
                        Name = "up-compare-goal",
                        Arg0 = state.Memory.MaxGoals.ToString(),
                        Arg1 = "c:<",
                        Arg2 = "0"
                    }
                }
            ],
            Actions =
            [
                new()
                {
                    Name = "up-modify-goal",
                    Arg0 = state.Memory.MaxGoals.ToString(),
                    Arg1 = "c:=",
                    Arg2 = "0"
                }
            ]
        });

        
        instructions.Add(new JumpIfNotZeroInstruction()
        {
            Goal = state.Memory.MaxGoals,
            Label = label_postinit
        });

        instructions.Add(new CommandInstruction()
        {
            Command = new()
            {
                Name = "up-modify-goal",
                Arg0 = state.Memory.MaxGoals.ToString(),
                Arg1 = "c:=",
                Arg2 = "1"
            }
        });

        instructions.Add(new RuleInstruction()
        {
            Facts =
            [
                new AtomicExpression()
                {
                    Command = new()
                    {
                        Name = "up-compare-goal",
                        Arg0 = state.Memory.MaxGoals.ToString(),
                        Arg1 = "c:<",
                        Arg2 = MaxGoals.ToString()
                    }
                }
            ],
            Actions =
            [
                new()
                {
                    Name = "up-modify-goal",
                    Arg0 = state.Memory.MaxGoals.ToString(),
                    Arg1 = "c:+",
                    Arg2 = "1"
                },
                new()
                {
                    Name = "up-set-indirect-goal",
                    Arg0 = "g:",
                    Arg1 = state.Memory.MaxGoals.ToString(),
                    Arg2 = "c:",
                    Arg3 = "0"
                },
                new()
                {
                    Name = "up-jump-rule",
                    Arg0 = "-1"
                }
            ]
        });

        instructions.Add(new CommandInstruction()
        {
            Command = new()
            {
                Name = "up-modify-goal",
                Arg0 = state.Memory.StackPtr.ToString(),
                Arg1 = "c:=",
                Arg2 = state.Memory.InitialStackBase.ToString()
            }
        });

        // TODO add module initializer methods

        instructions.Add(new CommandInstruction()
        {
            Command = new()
            {
                Name = "up-modify-goal",
                Arg0 = state.Memory.StackBasePtr.ToString(),
                Arg1 = "g:=",
                Arg2 = state.Memory.StackPtr.ToString()
            }
        });

        instructions.Add(new LabelInstruction() { Label = label_postinit });

        instructions.Add(new JumpIfNotZeroInstruction()
        {
            Goal = state.Memory.ExceptionCode,
            Label = state.ExceptionLabel
        });

        instructions.Add(new CommandInstruction()
        {
            Command = new()
            {
                Name = "up-modify-goal",
                Arg0 = state.Memory.StackPtr.ToString(),
                Arg1 = "g:=",
                Arg2 = state.Memory.StackBasePtr.ToString()
            }
        });

        instructions.Add(new CommandInstruction()
        {
            Command = new()
            {
                Name = "up-modify-goal",
                Arg0 = state.Memory.FramePtr.ToString(),
                Arg1 = "g:=",
                Arg2 = state.Memory.StackBasePtr.ToString()
            }
        });

        instructions.Add(new CommandInstruction()
        {
            Command = new()
            {
                Name = "up-modify-goal",
                Arg0 = state.Memory.ReturnAddress.ToString(),
                Arg1 = "c:=",
                Arg2 = "1000000"
            }
        });

        return instructions;
    }

    private List<Instruction> GetPostFix(State state)
    {
        var instructions = new List<Instruction>();

        // unwind stack

        instructions.Add(new LabelInstruction() { Label = state.UnwindStackLabel });


        // exceptions

        instructions.Add(new LabelInstruction() { Label = state.ExceptionLabel });
        
        foreach (var exception in state.Exceptions.OrderBy(x => x.Value))
        {
            instructions.Add(new RuleInstruction()
            {
                Facts =
                [
                    new AtomicExpression()
                    {
                        Command = new()
                        {
                            Name = "up-compare-goal",
                            Arg0 = state.Memory.ExceptionCode.ToString(),
                            Arg1 = "c:==",
                            Arg2 = exception.Value.ToString()
                        }
                    }
                ],
                Actions =
                [
                    new Command()
                    {
                        Name = "chat-to-all",
                        Arg0 = $"\"ERROR: {exception.Key}\""
                    }
                ]
            });
        }

        return instructions;
    }
}
