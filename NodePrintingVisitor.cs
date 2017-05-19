using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASTBuilder;
using Proj3Semantics.Nodes;

namespace Proj3Semantics
{
    class NodePrintingVisitor : IReflectiveVisitor
    {

        public void PreorderTraverseRoot(AbstractNode node, string prefix = "")
        {
            if (node == null) return;

            bool isLastChild = (node.NextSibling == null);

            using (new WithColor(ConsoleColor.DarkYellow))
            {
                Console.Write(prefix);
                Console.Write(isLastChild ? "└─ " : "├─ ");
            }

            node.Accept(this);

            PreorderTraverseRoot(node.LeftMostChild, prefix + (isLastChild ? "   " : "│  "));
            if (!isLastChild) PreorderTraverseRoot(node.NextSibling, prefix);
        }

        // ============================================================
        //                  DYNAMIC DISPATCH
        // ============================================================
        public void Visit(dynamic node)
        {
            VisitNode(node);
        }

        // ============================================================
        //                  VISIT METHODS BELOW
        // ============================================================
        public void VisitNode(AbstractNode node)
        {
            if (node.Identifier == null)
            {
                Console.WriteLine(node);
            }
            else
            {
                Console.Write(node + ": ");
                using (new WithColor(ConsoleColor.Cyan))
                    Console.WriteLine(node.Identifier?.Name);
            }
        }

        private void VisitNode(Modifiers node)
        {
            Console.Write(node + ": ");
            var stringEnums = node.ModifierTokens.Select(x => x.ToString());
            using (new WithColor(ConsoleColor.Magenta))
                Console.WriteLine(string.Join(", ", stringEnums));
        }

        private void VisitNode(Identifier node)
        {
            Console.Write(node + ": ");
            using (new WithColor(ConsoleColor.Cyan))
                Console.WriteLine(node.Name);
        }
        private void VisitNode(PrimitiveType node)
        {
            Console.Write(node + ": ");
            using (new WithColor(ConsoleColor.Magenta))
                Console.WriteLine(node.Type);
        }
        private void VisitNode(Expression node)
        {
            Console.Write(node + ": ");
            using (new WithColor(ConsoleColor.Magenta))
                Console.WriteLine(node.ExprType);
        }

        private void VisitNode(SpecialName node)
        {
            Console.Write(node + ": ");
            using (new WithColor(ConsoleColor.Yellow))
                Console.WriteLine(node.SpecialType);
        }

        private void VisitNode(Number node)
        {
            Console.Write(node + ": ");
            using (new WithColor(ConsoleColor.Yellow))
                Console.WriteLine(node.Value);
        }
        private void VisitNode(Literal node)
        {
            Console.Write(node + ": ");
            using (new WithColor(ConsoleColor.Yellow))
                Console.WriteLine("\"" + node.Name + "\"");
        }
        //private void VisitNode(ClassVarDecl node)
        //{
        //    using (new WithColor(ConsoleColor.Red))
        //        Console.WriteLine("<NOT IMPLEMENTED " + node.Msg + ">");
        //}
        private void VisitNode(NotImplemented node)
        {
            using (new WithColor(ConsoleColor.Red))
                Console.WriteLine("<NOT IMPLEMENTED " + node.Msg + ">");
        }

    }
}
