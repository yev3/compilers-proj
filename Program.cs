using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.AST;

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
                if (string.IsNullOrEmpty(input)) continue;

                Console.Write("Enter an assembly name: ");
                string ass = Console.ReadLine();
                if (string.IsNullOrEmpty(ass)) continue;

                string cmd = input.ToLower().Trim();
                if (cmd == "exit" || cmd == "quit") return;
                string fname = input + ".txt";

                if (System.IO.File.Exists(fname))
                {
                    RunFile(ass, fname);
                    Console.WriteLine("Finished with " + ass);
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

        static void RunFile(string ass, string fname)
        {
            using (OutColor.Cyan)
                Console.WriteLine(File.ReadAllText(fname));

            Console.WriteLine();
            Parser.Parse(fname);

            Console.WriteLine("Start semantic analysis");
            Console.WriteLine("========================================\n");


            CompilerErrors.ClearAll();
            RunSemanticAnalysis.Run(Parser.Root as CompilationUnit);
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

            Console.WriteLine("\nStart Code Generation Phase:");
            Console.WriteLine("==============================\n");
            ILCodeGeneration codeGen = new ILCodeGeneration(Parser.Root, ass);
            codeGen.GenerateCompileAndRun();

        }

        private static void TestAll()
        {
            var testCases = new[] {
                //new {assembly = "scratch", file = "00scratch.cs"},
                //new {assembly = "writenums", file = "00writenums.cs"},
                //new {assembly = "hello", file = "01hello.cs"},
                //new {assembly = "compute", file = "02compute.cs"},
                //new {assembly = "iftest", file = "03iftest.cs"},
                //new {assembly = "loop", file = "04loop.cs"},
                //new {assembly = "twomethods0", file = "05twomethods0.cs"},
                //new {assembly = "twomethods1", file = "06twomethods1.cs"},
                //new {assembly = "fact2", file = "07fact2.cs"},
                //new {assembly = "logictest", file = "08logictest.cs"},
                new {assembly = "struct1", file = "09struct1.cs"},
                //new {assembly = "twoparams", file = "10twoparams.cs"},
                //new {assembly = "errors1", file = "11errors1.cs"},
            };

            foreach (var entry in testCases)
            {
                Console.WriteLine("Test assembly {0}: {1}", entry.assembly, entry.file);
                Console.WriteLine("========================================\n");
                RunFile(entry.assembly, entry.file);
            }

        }


    }
}
