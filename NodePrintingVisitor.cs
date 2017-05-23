using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proj3Semantics;
using Proj3Semantics.Nodes;

namespace Proj3Semantics
{
    class NodePrintingVisitor : IReflectiveVisitor
    {

        public void PreorderTraverseRoot(AbstractNode node, string prefix = "")
        {
            if (node == null) return;

            bool isLastChild = (node.NextSibling == null);

            using (OutColor.DarkYellow)
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
            List<string> nodeProps = new List<string>();
            var typeDescriptor = node as ITypeSpecifier;
            if (typeDescriptor != null)
                nodeProps.Add(
                    typeDescriptor.NodeTypeCategory.ToString());

            var primitiveDescriptor = node as IPrimitiveTypeDescriptor;
            if (primitiveDescriptor != null)
                nodeProps.Add(
                    primitiveDescriptor.VariableTypePrimitive.ToString());

            var modifiers = node as ITypeHasModifiers;
            if (modifiers != null)
            {
                var modStr = "{" + modifiers.AccessorType + (modifiers.IsStatic ? ", static}" : "}");
                nodeProps.Add(modStr);
            }

            var typeRef = typeDescriptor?.TypeSpecifierRef;
            //if (typeRef != null)
            nodeProps.Add("TypeRef=" + (typeRef?.GetType().Name ?? "null"));

            bool hasDescriptors = nodeProps.Count > 0;
            //bool hasIdentifier = node.Identifier != null;

            if (hasDescriptors)
            {
                Console.Write(node + ": ");
                var descriptorStrings = "{" + string.Join(", ", nodeProps) + "}";
                using (OutColor.Magenta)
                    Console.Write(descriptorStrings);
            }
            else
            {
                Console.Write(node);
            }
            Console.WriteLine();
        }

        private void VisitNode(Modifiers node)
        {
            Console.Write(node + ": ");
            var stringEnums = node.ModifierTokens.Select(x => x.ToString());
            using (OutColor.Magenta)
                Console.WriteLine(string.Join(", ", stringEnums));
        }

        private void VisitNode(Identifier node)
        {
            Console.Write(node + ": ");
            using (OutColor.Cyan)
                Console.WriteLine(node.Name);
        }
        private void VisitNode(Expression node)
        {
            Console.Write(node + ": ");
            using (OutColor.Magenta)
                Console.WriteLine(node.ExprType);
        }

        private void VisitNode(SpecialName node)
        {
            Console.Write(node + ": ");
            using (OutColor.Yellow)
                Console.WriteLine(node.SpecialType);
        }

        private void VisitNode(Number node)
        {
            Console.Write(node + ": ");
            using (OutColor.Yellow)
                Console.WriteLine(node.Value);
        }
        private void VisitNode(Literal node)
        {
            Console.Write(node + ": ");
            using (OutColor.Yellow)
                Console.WriteLine("\"" + node.Name + "\"");
        }
        //private void VisitNode(ClassVarDecl node)
        //{
        //    using (new WithColor(ConsoleColor.Red))
        //        Console.WriteLine("<NOT IMPLEMENTED " + node.Msg + ">");
        //}
        private void VisitNode(NotImplemented node)
        {
            using (OutColor.Red)
                Console.WriteLine("<NOT IMPLEMENTED " + node.Msg + ">");
        }


    }
}
