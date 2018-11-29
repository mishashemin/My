using System;
using Expression;
using System.CodeDom.Compiler;
namespace Expression
{
    namespace Parsing
    {

        public interface InterfBase
        {
            Expr GetExpr(string expression, string[] variables = null);
        }
        [Serializable]
        public class Pars : InterfBase
        {
            public readonly string[] fun_s;
            public Expr GetExpr(string expression, string[] variables = null)
            {             
                foreach (var fun in fun_s)
                {
                    expression = expression.Replace(fun, "Fun." + fun);
                }
                var compiler = CodeDomProvider.CreateProvider("CSharp");
                var compilerParams = new CompilerParameters()
                {
                    CompilerOptions = "/t:library",
                    GenerateExecutable = true,
                    GenerateInMemory = true,
                };
                compilerParams.ReferencedAssemblies.Add("Expression.dll");
                string arg = "";
                if (variables != null)
                    foreach (var variable in variables) arg += @"var " + variable + @" = new Variable(""" + variable + @"""); ";
                string source =
    @"using Expression;
namespace A
{
    public static class B
    {
        public static Expr Parsing()
        {
            " + arg + @"
            var expr = " + expression + @";
            return expr;
        }
    }
}";
                CompilerResults res = compiler.CompileAssemblyFromSource(compilerParams, source);
                //if (!res.Errors.HasErrors)
                    return (Expr)res.CompiledAssembly.GetType("A.B").GetMethod("Parsing").Invoke(null, null);
                //else return null;

            }
            public Pars()
            {
                fun_s = new string[] { "Log", "Pow", "Sin", "Cos", "Tan", "Ctan", "Asin", "Acos", "Atan", "Actan", "Sh", "Ch", "Th", "Cth","E","Pi" };
            }

        }
    }
}