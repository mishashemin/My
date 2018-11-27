using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using Expression;

namespace CodeDownload
{
    class Program
    {
        static Expr Main(string[] args)
        {
            var compiler = CodeDomProvider.CreateProvider("CSharp");
            var compilerParams = new CompilerParameters()
            {
                CompilerOptions = "/t:library",
                GenerateExecutable = true,
                GenerateInMemory = false,
            };
            compilerParams.ReferencedAssemblies.Add("System.dll");
            string source =
            @"             
             using System;
             namespace Foo
             {
                public class Bar
                {                  
                   public static void SayHello()
                   {
                      System.Console.WriteLine(""Hello World"");
                   }
               }
            }
            ";
            CompilerResults res = compiler.CompileAssemblyFromSource(compilerParams, source);
            res.CompiledAssembly.GetType("Foo.Bar").GetMethod("SayHello").Invoke(null, null);
            Console.ReadKey();
            return new Constant(0);
        }
    }
}
