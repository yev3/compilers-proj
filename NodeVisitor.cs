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
            Console.Write("Visiting Modifiers: ");
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
            Console.WriteLine("Visiting TypeName.");
        }

        public void Visit(TypeSpecifier node)
        {
            Console.WriteLine("Visiting TypeSpecifier.");
        }

        public void Visit(MethodDeclaration node)
        {
            Console.WriteLine("Visiting MethodDeclaration.");
        }

        public void Visit(LocalVariableDeclarationsAndStatements node)
        {
            Console.WriteLine("Visiting LocalVariableDeclarationsAndStatements.");
        }

        public void Visit(Block node)
        {
            Console.WriteLine("Visiting Block.");
        }

        public void Visit(MethodBody node)
        {
            Console.WriteLine("Visiting MethodBody.");
        }

        public void Visit(MethodDeclaratorName node)
        {
            Console.WriteLine("Visiting MethodDeclaratorName.");
        }

        public void Visit(DeclaratorName node)
        {
            Console.WriteLine("Visiting DeclaratorName.");
        }

        public void Visit(Parameter node)
        {
            Console.WriteLine("Visiting Parameter.");
        }

        public void Visit(ParameterList node)
        {
            Console.WriteLine("Visiting ParameterList.");
        }

        public void Visit(MethodDeclarator node)
        {
            Console.WriteLine("Visiting MethodDeclarator.");
        }

        public void Visit(FieldDeclaration node)
        {
            Console.WriteLine("Visiting FieldDeclaration.");
        }
    }
}
