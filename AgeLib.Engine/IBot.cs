using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeLib.Engine;

public interface IBot
{
    public void Update(IEngine engine);
}
