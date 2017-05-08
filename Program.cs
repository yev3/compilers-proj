using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new TCCLParser();
            var name = "good1p.txt";
            Console.WriteLine("Parsing file " + name);
            parser.Parse(name);
            Console.WriteLine("Parsing complete");

        }
    }
}
