using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    class NodeVisitor : INodeVisitor
    {
        public void PreOrderWalk(AbstractNode node)
        {
            if (node == null) return;
            node.Accept(this);
            PreOrderWalk(node.LeftMostChild);
            PreOrderWalk(node.NextSibling);
        }
        public void Visit(CompilationUnit node)
        {
            Console.WriteLine("Visiting CompilationUnit.");
        }

        public void Visit(ClassDeclaration node)
        {
            Console.WriteLine("Visiting ClassDecl.");
        }

        public void Visit(Modifiers node)
        {
            Console.WriteLine("Visiting Modifier.");
        }

        public void Visit(Identifier node)
        {
            Console.WriteLine("Visiting Identifier: " + node.Name);
        }

        public void Visit(ClassBody node)
        {
            Console.WriteLine("Visiting ClassBody.");
        }

        public void Visit(FieldDeclarations node)
        {
            Console.WriteLine("Visiting FieldDecls.");
        }
    }
}
