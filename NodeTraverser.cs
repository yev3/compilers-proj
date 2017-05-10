using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    class NodeTraverser
    {
        private INodeVisitor _visitor;

        public NodeTraverser(INodeVisitor visitor)
        {
            this._visitor = visitor;
        }

        public void PreOrderWalk(AbstractNode node, string prefix = "")
        {
            //string s = @"├│└─";
            if (node == null) return;

            bool isLastChild = (node.NextSibling == null);

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(prefix);
            Console.Write(isLastChild ? "└─ " : "├─ ");
            Console.ResetColor();
            node.Accept(_visitor);

            PreOrderWalk(node.LeftMostChild, prefix + (isLastChild ? "   " : "│  "));

            if (!isLastChild)
                PreOrderWalk(node.NextSibling, prefix);
        }
    }
}
