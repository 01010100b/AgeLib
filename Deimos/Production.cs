using AgeLib.Common;
using AgeLib.Common.Enums;
using AgeLib.Common.Types;
using AgeLib.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deimos;

internal class Production
{
    private class Command(int priority, int blocking_permille, int id, Command.CommandType type)
    {
        public enum CommandType { RESEARCH, TRAIN, BUILD_NORMAL }

        public int Priority { get; } = priority;
        public int BlockingPermille { get; } = Math.Clamp(blocking_permille, 0, 1000);
        public int Id { get; } = id;
        public CommandType Type { get; } = type;

        public void Execute(IEngine engine)
        {
            if (Type == CommandType.RESEARCH && engine.Check("can-research", Id))
            {
                engine.Execute("research", Id);
            }
            else if (Type == CommandType.TRAIN && engine.Check("can-train", Id))
            {
                engine.Execute("train", Id);
            }
            else if (Type == CommandType.BUILD_NORMAL && engine.Check("can-build", Id))
            {
                engine.Execute("build", Id);
            }
        }
    }

    private List<Command> Commands { get; } = [];

    public void Research(int id, int priority, int blocking_permille)
        => Commands.Add(new(priority, blocking_permille, id, Command.CommandType.RESEARCH));

    public void Train(int id , int priority, int blocking_permille)
        => Commands.Add(new(priority, blocking_permille, id, Command.CommandType.TRAIN));

    public void BuildNormal(int id, int priority, int blocking_permille)
        => Commands.Add(new(priority, blocking_permille, id, Command.CommandType.BUILD_NORMAL));

    public void Produce(IEngine engine)
    {
        if (Commands.Count == 0)
        {
            return;
        }

        const int GOAL = 100;

        var food = engine.GetFact(engine.MyPlayer, FactId.FOOD_AMOUNT);
        var wood = engine.GetFact(engine.MyPlayer, FactId.WOOD_AMOUNT);
        var stone = engine.GetFact(engine.MyPlayer, FactId.STONE_AMOUNT);
        var gold = engine.GetFact(engine.MyPlayer, FactId.GOLD_AMOUNT);
        var res = new Cost(food, wood, stone, gold);
        var is_researching = false;
        engine.Execute("up-setup-cost-data", 1, GOAL);
        Commands.Sort((a, b) => b.Priority.CompareTo(a.Priority));

        foreach (var command in Commands)
        {
            engine.Execute("up-reset-cost-data", GOAL);

            if (command.Type == Command.CommandType.RESEARCH)
            {
                engine.Execute("up-add-research-cost", TypeOp.C, command.Id, TypeOp.C, 1);
            }
            else
            {
                engine.Execute("up-add-object-cost", TypeOp.C, command.Id, TypeOp.C, 1);
            }

            var cost = engine.GetCost(GOAL);
            var can_afford = cost.Food <= res.Food && cost.Wood <= res.Wood 
                && cost.Stone <= res.Stone && cost.Gold <= res.Gold;

            if (can_afford)
            {
                command.Execute(engine);
                res -= cost;

                if (command.Type == Command.CommandType.RESEARCH)
                {
                    is_researching = true;
                }
            }
            else if (command.BlockingPermille > 0)
            {
                var blocked = cost * command.BlockingPermille / 1000;
                res -= blocked;
            }
        }

        Commands.Clear();

        if (is_researching)
        {
            engine.SetStrategicNumber(StrategicNumber.ENABLE_TRAINING_QUEUE, 0);
        }
        else
        {
            engine.SetStrategicNumber(StrategicNumber.ENABLE_TRAINING_QUEUE, 1);
        }
    }
}
