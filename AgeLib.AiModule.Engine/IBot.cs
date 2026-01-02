using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.AiModule.Engine;

public interface IBot
{
    public string Name { get; }

    public void Tick(IEngine engine);
}
