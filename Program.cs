using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.ASTNodes;

namespace Proj3Semantics
{
    class Program
    {

        private static List<string> test_files = new List<string>()
        {
            "01hello.txt",
            //"02errors1.txt",
            //"03writenums.txt",
            //"04twomethods0.txt",
            //"05twomethods1.txt",
            //"06compute.txt",
            //"07iftest.txt",
            //"08loop.txt",
            //"ms3_good1p.txt",
            //"ms3_iftest.txt",
            //"ms3_logictest.txt",
            //@"TestFiles\good1pms3.txt",
            //@"TestFiles\iftestms3.txt",
            //@"TestFiles\logictestms3.txt"
            
        };


        static void Main(string[] args)
        {

            var testNbr = 1;
            foreach (var file in test_files)
            {
                var parser = new TCCLParser();

                Console.WriteLine("Test case {0}:", testNbr++);
                Console.WriteLine("========================================\n");

                using (OutColor.Cyan)
                    Console.WriteLine(File.ReadAllText(file));

                Console.WriteLine();
                parser.Parse(file);

                var nodeVisitor = new NodePrintingVisitor();
                // show parse result
                //nodeVisitor.PreorderTraverseRoot(parser.Root);

                Console.WriteLine("Start semantic analysis");
                Console.WriteLine("========================================\n");

                // TODO: Primitive types?
                SemanticAnalysis.Run(parser.Root as CompilationUnit);

                Console.WriteLine("\nPrint Tree After Analyzing:");
                Console.WriteLine("========================================\n");
                nodeVisitor.PreorderTraverseRoot(parser.Root);

            }


            //Console.WriteLine("Press any key to continue..");
            //Console.ReadKey();
        }
    }
}
