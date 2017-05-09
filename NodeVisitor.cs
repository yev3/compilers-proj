using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    class NodeVisitor : INodeVisitor
    {
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
            Console.Write("Visiting Modifier: ");
            var stringEnums = node.ModifierTokens.Select(x => x.ToString());
            Console.WriteLine(string.Join(", ", stringEnums));
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

        public void Visit(PrimitiveType node)
        {
            Console.WriteLine("Visiting PrimitiveType.");
        }

        public void Visit(TypeName node)
        {
            Console.WriteLine("Visiting PrimitiveType.");
        }

        public void Visit(TypeSpecifier node)
        {
            Console.WriteLine("Visiting PrimitiveType.");
        }

        public void Visit(MethodDeclaration node)
        {
            Console.WriteLine("Visiting PrimitiveType.");
        }
    }
}
