using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proj3Semantics;
using Proj3Semantics.ASTNodes;

namespace Proj3Semantics
{
    public class NodePrintingVisitor : IReflectiveVisitor
    {
        public static string AbstactNodeDebugString(AbstractNode node)
        {
            TextWriter tw = new StringWriter();
            
            bool hasName = node is INamedType;
            INamedType namedNode = node as INamedType;
            string nodeName = namedNode?.Name;
            tw.Write("<" + node.GetType().Name + "> ");
            PrintName(node, tw);
            tw.Write(" ");
            PrintModifiers(node, tw);
            tw.Write(" ");
            PrintTypeDescr(node, tw);
            return tw.ToString();
        }

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


        private static void PrintModifiers(AbstractNode node, TextWriter cout = null)
        {
            if (cout == null) cout = Console.Out;

            var modifiers = node as ITypeHasModifiers;
            if (modifiers == null) return;
            using (OutColor.Magenta)
            {
                cout.Write("{" + modifiers.AccessorType + (modifiers.IsStatic ? ", Static}" : "}"));
            }
        }

        private static void PrintTypeDescr(AbstractNode node, TextWriter cout = null)
        {
            if (cout == null) cout = Console.Out;

            ITypeDescriptor typeDescriptor = node as ITypeDescriptor;
            if (typeDescriptor == null) return;

            var typeStrings = new List<string>();
            typeStrings.Add(typeDescriptor.NodeTypeCategory.ToString());

            var primitiveDescriptor = node as IPrimitiveTypeDescriptor;
            if (primitiveDescriptor != null)
                typeStrings.Add(primitiveDescriptor.VariablePrimitiveType.ToString());

            ITypeDescriptor typeRef = typeDescriptor?.TypeDescriptorRef;
            typeStrings.Add("tref=" + (typeRef?.ToString() ?? "**NULL**"));

            var typeStr = "{" + string.Join(", ", typeStrings) + "}";
            using (OutColor.Magenta)
                cout.Write(typeStr);

        }

        private static void PrintName(AbstractNode node, TextWriter cout = null)
        {
            if (cout == null) cout = Console.Out;

            bool hasName = node is INamedType;
            INamedType namedNode = node as INamedType;
            string nodeName = namedNode?.Name;
            if (hasName)
            {
                using (OutColor.Cyan)
                    cout.Write(nodeName);
            }

        }



        // ============================================================
        //                  VISIT METHODS BELOW
        // ============================================================
        public void VisitNode(AbstractNode node)
        {

            bool hasName = node is INamedType;
            INamedType namedNode = node as INamedType;
            string nodeName = namedNode?.Name;
            Console.Write("<" + node.GetType().Name + "> ");
            PrintName(node);
            Console.Write(" ");
            PrintModifiers(node);
            Console.Write(" ");
            PrintTypeDescr(node);
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
            {
                Console.Write(node.ExprType);
                Console.Write(" ");
                PrintTypeDescr(node);
                Console.WriteLine();
            }
        }


        private void VisitNode(NumberLiteral node)
        {
            Console.Write(node + ": ");
            using (OutColor.Yellow)
                Console.WriteLine(node.Value);
        }
        private void VisitNode(StringLiteral node)
        {
            Console.Write(node + ": ");
            using (OutColor.Yellow)
                Console.WriteLine("\"" + node.Name + "\"");
        }

        private void VisitNode(QualifiedName node)
        {
            Console.Write(node + ": ");
            var idStr = string.Join(".", node.IdentifierList);
            using (OutColor.Cyan)
                Console.Write(idStr);

            Console.Write(", ");
            PrintTypeDescr(node);
            Console.WriteLine();
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
