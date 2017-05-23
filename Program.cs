using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Proj3Semantics
{
    class Program
    {

        private static List<string> test_files = new List<string>()
        {
            @"TestFiles\good1p.txt",
            //@"TestFiles\iftest.txt",
            //@"TestFiles\logictest.txt"
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
                nodeVisitor.PreorderTraverseRoot(parser.Root);

                Console.WriteLine("Start semantic analysis");
                Console.WriteLine("========================================\n");

                var typeEnv = new SymbolTable<ITypeInfo>();
                var nameEnv = new SymbolTable<ITypeInfo>();
                var topDeclVisitor = new TopDeclVisitor(typeEnv,nameEnv);
                topDeclVisitor.Visit(parser.Root);

                Console.WriteLine("Print Tree After Analyzing:");
                Console.WriteLine("========================================\n");
                nodeVisitor.PreorderTraverseRoot(parser.Root);

            }


            //Console.ReadKey();
        }
    }
}
