using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    class NodeVisitor
    {
        private INodeReflectiveVisitor _visitorInstance;
        public NodeVisitor(INodeReflectiveVisitor visitorInstance)
        {
            _visitorInstance = visitorInstance;
        }

        public void Visit(AbstractNode node)
        {
            _visit(node);
        }

        private void _visit(AbstractNode node, string prefix = "")
        {
            if (node == null) return;

            bool isLastChild = (node.NextSibling == null);

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(prefix);
            Console.Write(isLastChild ? "└─ " : "├─ ");
            Console.ResetColor();

            node.Accept(_visitorInstance);

            _visit(node.LeftMostChild, prefix + (isLastChild ? "   " : "│  "));
            if (!isLastChild) _visit(node.NextSibling, prefix);
        }


    }
}
