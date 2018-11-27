using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expression;
using ExprParsing;

namespace ConsoleApp2
{
    using static Fun;   
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain domain = AppDomain.CreateDomain("MyDomain");
            InterfBase cr = (InterfBase)domain.CreateInstanceFromAndUnwrap("ExprParsing.dll", "ExprParsing.Class1");
            cr.SayHello();
            
            
            var a = new Variable("a");
            var b = new Variable("b");
            var c = Pow(2, 4);
            var ss = c.Diff();
            double vv = c.Compute();
            vv = ss.Compute(new Dictionary<string, double>
            {
                ["a"] = 4,
                ["b"] = 30
            });
            vv = c.Compute(new Dictionary<string, double>
            {
                ["a"] = 4,
                ["b"] = 3
            });
        }
    }
}
