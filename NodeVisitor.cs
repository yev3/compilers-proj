using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    class NodeVisitor : INodeVisitor
    {
        public void Visit(AbstractNode node)
        {
            Console.WriteLine("<" + node.ClassName() + ">");
        }

        public void Visit(Modifiers node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            var stringEnums = node.ModifierTokens.Select(x => x.ToString());
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Join(", ", stringEnums));
            Console.ResetColor();
        }

        public void Visit(Identifier node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.Name);
            Console.ResetColor();
        }
        public void Visit(PrimitiveType node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.Type);
            Console.ResetColor();
        }
        public void Visit(Expression node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.ExprType);
            Console.ResetColor();
        }

        public void Visit(SpecialName node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.SpecialType);
            Console.ResetColor();
        }

        public void Visit(Number node)
        {
            Console.Write("<" + node.ClassName() + ">: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(node.Value);
            Console.ResetColor();
        }

        public void Visit(NotImplemented node)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("<NOT IMPLEMENTED: " + node.Name + ">");
            Console.ResetColor();
        }
    }
}
