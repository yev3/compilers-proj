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

            var modifiers = node as ITypeHasModifiers;
            if (modifiers != null)
            {
                var modStr = "{" + modifiers.AccessorType + (modifiers.IsStatic ? ", static}" : "}");
                nodeProps.Add(modStr);
            }

            // type description
            var typeDescriptor = node as ITypeSpecifier;
            if (typeDescriptor != null)
            {
                var typeStrings = new List<string>();
                typeStrings.Add(typeDescriptor.NodeTypeCategory.ToString());

                var primitiveDescriptor = node as IPrimitiveTypeDescriptor;
                if (primitiveDescriptor != null)
                    typeStrings.Add(primitiveDescriptor.VariableTypePrimitive.ToString());

                ITypeSpecifier typeRef = typeDescriptor?.TypeSpecifierRef;
                typeStrings.Add(typeRef?.GetType().Name ?? "null");

                var typeStr = "{" + string.Join(", ", typeStrings) + "}";
                nodeProps.Add(typeStr);
            }



            bool hasDescriptors = nodeProps.Count > 0;
            bool hasName = node is INamedType;

            INamedType namedNode = node as INamedType;
            string nodeName = namedNode?.Name;

            if (hasDescriptors || hasName)
            {
                Console.Write(node + ": ");
                if (hasName)
                {
                    using (OutColor.Cyan)
                        Console.Write(nodeName);
                    if (hasDescriptors)
                        Console.Write(", ");
                }
                if (hasDescriptors)
                {
                    var descriptorStrings = string.Join(", ", nodeProps);
                    using (OutColor.Magenta)
                        Console.Write(descriptorStrings);
                }
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
