using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Expression;
using System.CodeDom.Compiler;
using System.IO;

namespace ConsoleApp2
{
    using static Fun;
    class Program
    {

        static void Main(string[] args)
        {                    
            AppDomain domain = AppDomain.CreateDomain("MyDomain");
            var cr = (Expression.Parsing.InterfBase)domain.CreateInstanceFromAndUnwrap("Expression.dll", "Expression.Parsing.Pars");

            Expr expr = null;
            using (StreamReader sr = new StreamReader("Data.txt", Encoding.Default))
            {
                using (StreamWriter fs = new StreamWriter("Output.txt", true, Encoding.Default))
                {
                    string input = "";
                    while (!sr.EndOfStream)
                    {
                        input = sr.ReadLine();
                        string[] par = input.Split(' ');
                        if (par.Length == 1)
                        {
                            fs.WriteLine("");
                            fs.WriteLine("");
                            expr = cr.GetExpr(input);
                            fs.WriteLine("Выражение: " + expr.ToString());
                            fs.WriteLine("Ответ: " + expr.Compute());
                            fs.WriteLine("Производная: " + expr.Diff().ToString());
                            fs.WriteLine("");
                            fs.WriteLine("");
                        }
                        else
                        {
                            
                            var Var = new Dictionary<string, double>();
                            for (int i = 1; i < par.Length; i = i + 2)
                            {
                                Var.Add(par[i], Convert.ToDouble(par[i + 1]));
                            }
                            try
                            {
                                expr = cr.GetExpr(par[0], Var.Keys.ToArray());
                                fs.WriteLine("");
                                fs.WriteLine("");
                                fs.WriteLine("Выражение: " + expr.ToString());
                                fs.WriteLine("Ответ: " + expr.Compute(Var));
                                fs.WriteLine("Производная: " + expr.Diff().ToString());
                                fs.WriteLine("");
                                fs.WriteLine("");
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                
            }
        }
    }
}
