using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//        private static string test_input = @"
//public class good1p {

//   public static void outInt(int n) {
//     java.io.PrintStream ps;
//     ps = java.lang.System.out;
//     ps.print(n);
//   }

//   public static void main431() {
//     int w,x;
//     x = 3+4;  TestClasses.good1p.outInt(x);
//     x = 5*7;  TestClasses.good1p.outInt(x);
//   }
//}
//";

namespace ASTBuilder
{
    class Program
    {
        private static List<string> test_cases = new List<string>()
        {
@"public class good1p {
   public static void outInt(int n) {
     java.io.PrintStream ps;
     ps = java.lang.System.out;
     ps.print(n);
   }
}",
@"public class good2p {
   public static void main431() {
     int w,x;
     x = 3+4;  TestClasses.good1p.outInt(x);
     x = 5*7;  TestClasses.good1p.outInt(x);
   }
}",
        };
        static void Main(string[] args)
        {
            var parser = new TCCLParser();
            //var name = "good1p.txt";
            //Console.WriteLine("Parsing file " + name);

            foreach (string test_input in test_cases)
            {
                Console.WriteLine("Test case:");
                Console.WriteLine("========================================\n");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(test_input);
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine();

                Stream strm = new MemoryStream(Encoding.ASCII.GetBytes(test_input));
                parser.Parse(strm);
            }


            Console.ReadKey();
        }
    }
}
