using AgeLib.Scripting.Assembly;
using AgeLib.Scripting.Assembly.Instructions;
using AgeLib.Scripting.Compilation.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Scripting.Compilation.Statements;

public class CallStatement : Statement
{
    public string? Result { get; set; }
    public required string MethodName { get; set; }
    public required List<string> Arguments { get; set; }

    internal override List<Instruction> Compile(State state)
    {
        var instructions = new List<Instruction>();
        var caller = state.Resolver.ResolveMethod(this);
        var callee = state.Resolver.ResolveMethod(MethodName);
        var locals = Scope.GetLocalsInScope().ToList();
        var unwind_count = Memory.EXTRA_REGISTERS + locals.Where(x => x is not Constant)
            .Sum(x => state.Resolver.ResolveType(x.TypeName).Size);

        // push registers to stack and reset registers

        instructions.AddRange(state.Copy(state.Memory.RegisterBase, false, state.Memory.StackPtr, true, unwind_count, false));
        instructions.AddRange(state.Clear(state.Memory.RegisterBase, false, state.Memory.GetRegistersSize(callee, state.Resolver), false));
        instructions.Add(new CommandInstruction()
        {
            Command = new()
            {
                Name = "up-modify-goal",
                Arg0 = state.Memory.UnwindCount.ToString(),
                Arg1 = "c:=",
                Arg2 = unwind_count.ToString()
            }
        });

        // copy arguments

        for (int i = 0; i < Arguments.Count; i++)
        {
            var argument = state.Resolver.ResolveVariable(Arguments[i], Scope);
            var parameter = state.Resolver.ResolveVariable(callee.Parameters[i], callee.Scope);
            var size = state.Resolver.ResolveType(argument.TypeName).Size;
            var from = state.Memory.GetAddress(argument);
            var to = state.Memory.GetAddress(parameter);

            if (locals.Any(x => x.Name == argument.Name))
            {
                // locals are on the stack now

                from -= state.Memory.RegisterBase;
                instructions.Add(new CommandInstruction()
                {
                    Command = new()
                    {
                        Name = "up-modify-goal",
                        Arg0 = state.Memory.Intr0.ToString(),
                        Arg1 = "c:=",
                        Arg2 = from.ToString()
                    }
                });
                instructions.Add(new CommandInstruction()
                {
                    Command = new()
                    {
                        Name = "up-modify-goal",
                        Arg0 = state.Memory.Intr0.ToString(),
                        Arg1 = "g:+",
                        Arg2 = state.Memory.StackPtr.ToString()
                    }
                });
                instructions.AddRange(state.Copy(state.Memory.Intr0, true, to, false, size, false));
            }
            else
            {
                instructions.AddRange(state.Copy(from, false, to, false, size, false));
            }
        }

        // increment stack ptr and set frame ptr

        instructions.Add(new CommandInstruction()
        {
            Command = new()
            {
                Name = "up-modify-goal",
                Arg0 = state.Memory.StackPtr.ToString(),
                Arg1 = "c:+",
                Arg2 = unwind_count.ToString()
            }
        });
        instructions.Add(new CommandInstruction()
        {
            Command = new()
            {
                Name = "up-modify-goal",
                Arg0 = state.Memory.FramePtr.ToString(),
                Arg1 = "g:=",
                Arg2 = state.Memory.StackPtr.ToString()
            }
        });

        // set return address and jump

        instructions.Add(new SetReturnAndJumpInstruction()
        {
            ReturnGoal = state.Memory.ReturnAddress,
            Label = callee.Label
        });

        return instructions;
    }

    internal override Statement Copy(Scope scope, IReadOnlyDictionary<string, string> variables)
    {
        throw new NotImplementedException();
    }

    internal override IEnumerable<string> GetVariables()
    {
        foreach (var argument in Arguments)
        {
            yield return argument;
        }

        if (Result is not null)
        {
            yield return Result;
        }
    }

    protected private override void ValidateStatement(Resolver resolver)
    {
        ThrowIf(!resolver.IsAccessible(MethodName, Scope), $"Method {MethodName} is not accessible");
        var method = resolver.ResolveMethod(MethodName);
        ThrowIf(Result is not null && method.ReturnTypeName == "System.Void",
            $"Method {MethodName} returns System.Void but Result is not null.");

        if (Result is not null)
        {
            var result = resolver.ResolveVariable(Result, Scope);
            ThrowIf(result.TypeName != method.ReturnTypeName,
                $"Method {MethodName} returns {method.ReturnTypeName} but Result has type {result.TypeName}.");
        }

        ThrowIf(Arguments.Count != method.Parameters.Count,
            $"Method {MethodName} has {method.Parameters.Count} parrameters but call statement has {Arguments.Count} arguments.");

        for (int i = 0; i < Arguments.Count; i++)
        {
            var argument = resolver.ResolveVariable(Arguments[i], Scope);
            var parameter = resolver.ResolveVariable(method.Parameters[i], method.Scope);
            ThrowIf(argument.TypeName != parameter.TypeName, $"Argument {i} has wrong type.");
        }
    }
}
