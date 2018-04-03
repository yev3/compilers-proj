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
        private static TCCLParser Parser { get; } = new TCCLParser();


        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "RUN_ALL")
            {
                TestAll();
                return;
            }

            while (true)
            {
                Console.Write("Enter a file name: ");

                string input = Console.ReadLine();
                if (input == null) continue;
                string cmd = input.ToLower().Trim();
                if (cmd == "exit" || cmd == "quit") return;
                string fname = input + ".txt";

                if (System.IO.File.Exists(fname))
                {
                    RunFile(fname);
                    Console.WriteLine("Parsing complete");
                }
                else
                {
                    using (OutColor.Red)
                    {
                        Console.WriteLine("Error: {0} not found.", fname);
                        Console.WriteLine("Current directory: {0}", Directory.GetCurrentDirectory());
                    }
                }
            }

            //Console.WriteLine("Press any key to continue..");
            //Console.ReadKey();
        }

        static void RunFile(string fname)
        {
            using (OutColor.Cyan)
                Console.WriteLine(File.ReadAllText(fname));

            Console.WriteLine();
            Parser.Parse(fname);

            Console.WriteLine("Start semantic analysis");
            Console.WriteLine("========================================\n");


            CompilerErrors.ClearAll();
            SemanticAnalysis.Run(Parser.Root as CompilationUnit);
            if (CompilerErrors.ErrList.Count > 0)
            {
                using (OutColor.Red)
                {
                    foreach (CompilerError error in CompilerErrors.ErrList)
                    {
                        Console.WriteLine("Semantic Error: {0}", error);
                    }
                }
            }
            else
            {
                using (OutColor.Green)
                    Console.WriteLine("No semantic errors.");
            }

            Console.WriteLine("\nPrint Tree After Analyzing:");
            Console.WriteLine("========================================\n");
            var nodeVisitor = new NodePrintingVisitor();
            nodeVisitor.PreorderTraverseRoot(Parser.Root);

        }

        private static void TestAll()
        {

            List<string> testFiles = new List<string>()
            {
                "01hello.txt",
                "02errors1.txt",
                "03compute.txt",
                "04twomethods0.txt",
                "05twomethods1.txt",
                "06writenums.txt",
                "07iftest.txt",
                "08loop.txt",
            };

            var testNbr = 1;
            foreach (var file in testFiles)
            {
                Console.WriteLine("Test case {0}: {1}", testNbr++, file);
                Console.WriteLine("========================================\n");
                RunFile(file);
            }

        }


    }
}
