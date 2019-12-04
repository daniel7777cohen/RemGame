using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemGame
{
    class DecisionQuery:Decision
    {
        public string Title { get; set; }
        public Decision Positive { get; set; }
        public Decision Negative { get; set; }
        public Func<Enemy, bool> Test { get; set; }

        public override string Evaluate(Enemy en)
        {
            bool result = this.Test(en);
            string resultAsString = result ? "yes" : "no";

            //Console.WriteLine($"\t- {this.Title}? {resultAsString} ");

            if (result) return this.Positive.Evaluate(en);
            else return this.Negative.Evaluate(en);

        }

      
    }
}
