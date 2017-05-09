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

        public void PreOrderWalk(AbstractNode node, int depth = 0)
        {
            if (node == null) return;
            for (int i = 0; i < depth; i++) { Console.Write("  "); }
            node.Accept(_visitor);
            PreOrderWalk(node.LeftMostChild, depth + 1);
            PreOrderWalk(node.NextSibling, depth);
        }
    }
}
