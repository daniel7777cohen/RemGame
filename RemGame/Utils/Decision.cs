using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemGame
{
    abstract class Decision
    {
        public abstract string Evaluate(Enemy en);

    }
}
