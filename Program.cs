using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    class Program
    {



        private static List<string> test_files = new List<string>()
        {
            @"TestFiles\good1p.txt",
            @"TestFiles\iftest.txt",
            @"TestFiles\logictest.txt"
        };


        static void Main(string[] args)
        {


            //// create an instance of my object with evaluate methods
            //var objWithEvals = new VisitMethods1();
            //var reflectiveVisitor = new ReflectiveVisitor<CustomMethods>(debugTrace: true);

            //string str = "hello";
            //Console.WriteLine("string str = \"hello\";");
            //reflectiveVisitor.Visit(str);

            //var number = new Number(13);
            //Console.WriteLine("var number = new Number(13); ");
            //reflectiveVisitor.Visit(number);

            //var literal = new Literal("lit");
            //Console.WriteLine("var literal = new Literal('lit'); ");
            //reflectiveVisitor.Visit(literal);

            //AbstractNode abstr = new Block();
            //Console.WriteLine("AbstractNode abstr = new Block(); ");
            //reflectiveVisitor.Visit(abstr);

            //AbstractNode abstrLiteral = literal;
            //Console.WriteLine(" AbstractNode abstr_literal = literal; ");
            //reflectiveVisitor.Visit(abstrLiteral);

            //return;



            var testNbr = 1;
            foreach (var file in test_files)
            {
                var parser = new TCCLParser();

                Console.WriteLine("Test case {0}:", testNbr++);
                Console.WriteLine("========================================\n");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(File.ReadAllText(file));
                Console.ResetColor();
                Console.WriteLine();
                parser.Parse(file);

                var visitDispatcher = new ReflectiveVisitorDispatch<NodeVisitorMethods>();
                var nodeVisitor = new NodeVisitor(visitDispatcher);
                nodeVisitor.Visit(parser.Root);
            }


            //Console.ReadKey();
        }
    }
}
