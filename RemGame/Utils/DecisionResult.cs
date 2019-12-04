using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemGame
{
     class DecisionResult : Decision
    {
        public bool Result { get; set; }
        public String Action { get; set; }

        public override string Evaluate(Enemy en)
        {
            return Action;
        }
    }
}
