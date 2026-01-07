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
    private class Command
    {
        public enum CommandType { RESEARCH, TRAIN, BUILD_NORMAL }

        public required int Priority { get; set; }
        public required int BlockingPermille { get; set; }
        public required int Id { get; set; }
        public required CommandType Type { get; set; }

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
    {
        Commands.Add(new()
        {
            Priority = priority,
            BlockingPermille = blocking_permille,
            Id = id,
            Type = Command.CommandType.RESEARCH
        });
    }

    public void Train(int id , int priority, int blocking_permille)
    {
        Commands.Add(new()
        {
            Priority = priority,
            BlockingPermille = blocking_permille,
            Id = id,
            Type = Command.CommandType.TRAIN
        });
    }

    public void BuildNormal(int id, int priority, int blocking_permille)
    {
        Commands.Add(new()
        {
            Priority = priority,
            BlockingPermille = blocking_permille,
            Id = id,
            Type = Command.CommandType.BUILD_NORMAL
        });
    }

    public void Produce(IEngine engine)
    {
        const int GOAL = 100;

        if (Commands.Count == 0)
        {
            return;
        }

        var food = engine.GetFact(engine.MyPlayer, FactId.FOOD_AMOUNT);
        var wood = engine.GetFact(engine.MyPlayer, FactId.WOOD_AMOUNT);
        var stone = engine.GetFact(engine.MyPlayer, FactId.STONE_AMOUNT);
        var gold = engine.GetFact(engine.MyPlayer, FactId.GOLD_AMOUNT);
        var res = new Cost(food, wood, stone, gold);
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
            }
            else if (command.BlockingPermille > 0)
            {
                var blocked = cost * command.BlockingPermille / 1000;
                res -= blocked;
            }
        }

        Commands.Clear();
    }
}
