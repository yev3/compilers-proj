using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    class NodeVisitorMethods
    {
        public static void Visit(AbstractNode node)
        {
            Console.WriteLine("<" + node.ClassName() + ">");
        }

        public static void Visit(Modifiers node)
        {
            Console.Write("<" + node.ClassName() + "> ");
            var stringEnums = node.ModifierTokens.Select(x => x.ToString());
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(string.Join(", ", stringEnums));
            Console.ResetColor();
        }

        public static void Visit(Identifier node)
        {
            Console.Write("<" + node.ClassName() + "> ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(node.Name);
            Console.ResetColor();
        }
        public static void Visit(PrimitiveType node)
        {
            Console.Write("<" + node.ClassName() + "> ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(node.Type);
            Console.ResetColor();
        }
        public static void Visit(Expression node)
        {
            Console.Write("<" + node.ClassName() + "> ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(node.ExprType);
            Console.ResetColor();
        }

        public static void Visit(SpecialName node)
        {
            Console.Write("<" + node.ClassName() + "> ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.SpecialType);
            Console.ResetColor();
        }

        public static void Visit(Number node)
        {
            Console.Write("<" + node.ClassName() + "> ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.Value);
            Console.ResetColor();
        }
        public static void Visit(Literal node)
        {
            Console.Write("<" + node.ClassName() + "> ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\"" + node.Name + "\"");
            Console.ResetColor();
        }
        public static void Visit(NotImplemented node)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("<NOT IMPLEMENTED " + node.Name + ">");
            Console.ResetColor();
        }
    }
}
