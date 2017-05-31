using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proj3Semantics;
using Proj3Semantics.AST;

namespace Proj3Semantics
{
    public class NodePrintingVisitor : IReflectiveVisitor
    {
        public static string AbstactNodeDebugString(Node node)
        {
            TextWriter tw = new StringWriter();

            DeclNode decl = node as DeclNode;
            string nodeName = decl?.Identifier?.Name;
            tw.Write("<" + node.GetType().Name + "> ");
            PrintName(node, tw);
            tw.Write(" ");
            PrintModifiers(node, tw);
            tw.Write(" ");
            PrintTypeDescr(node, tw);
            return tw.ToString();
        }

        public void PreorderTraverseRoot(Node node, string prefix = "", bool isLastChild = true)
        {
            if (node == null) return;

            using (OutColor.DarkYellow)
            {
                Console.Write(prefix);
                Console.Write(isLastChild ? "└─ " : "├─ ");
            }

            node.Accept(this);
            if (node.Children.Count == 0) return;
            Node last = node.Children[node.Children.Count - 1];
            foreach (Node child in node.Children)
            {
                PreorderTraverseRoot(child, prefix + (isLastChild ? "   " : "│  "), child == last);
            }
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



        private static void PrintModifiers(Node node, TextWriter cout = null)
        {
            if (cout == null) cout = Console.Out;

            var modifiers = node as ITypeHasModifiers;
            if (modifiers == null) return;
            using (OutColor.Magenta)
            {
                cout.Write("{" + modifiers.AccessorType + (modifiers.IsStatic ? ", Static}" : "}"));
            }
        }

        private static void PrintTypeDescr(Node node, TextWriter cout = null)
        {
            //if (cout == null) cout = Console.Out;

            //ITypeDescriptor typeDescriptor = node as ITypeDescriptor;
            //if (typeDescriptor == null) return;

            //var typeStrings = new List<string>();
            //typeStrings.Add(typeDescriptor.NodeTypeCategory.ToString());

            //var primitiveDescriptor = node as IPrimitiveTypeDescriptor;
            //if (primitiveDescriptor != null)
            //    typeStrings.Add(primitiveDescriptor.VariablePrimitiveType.ToString());

            //ITypeDescriptor typeRef = typeDescriptor?.TypeDescriptorRef;
            //typeStrings.Add("tref=" + (typeRef?.ToString() ?? "**NULL**"));

            //var typeStr = "{" + string.Join(", ", typeStrings) + "}";
            //using (OutColor.Magenta)
            //    cout.Write(typeStr);

        }

        private static void PrintName(Node node, TextWriter cout = null)
        {
            if (cout == null) cout = Console.Out;
            DeclNode decl = node as DeclNode;
            if (decl != null)
            {
                string nodeName = decl?.Identifier?.Name;
                using (OutColor.Cyan)
                    cout.Write(nodeName);
            }

        }



        // ============================================================
        //                  VISIT METHODS BELOW
        // ============================================================
        public void VisitNode(Node node)
        {

            Console.Write("<" + node.GetType().Name + "> ");
            PrintName(node);
            Console.Write(" ");
            PrintModifiers(node);
            Console.Write(" ");
            PrintTypeDescr(node);
            Console.WriteLine();
        }

        private void VisitNode(DeclNode decl)
        {
            Console.Write(decl + ": ");
            using (OutColor.Cyan)
            {
                Console.Write(decl.Name ?? "null_name");
            }
            Console.Write(" ");
            using (OutColor.Magenta)
            {
                Console.Write("t:");
                Console.Write(decl.DeclTypeNode?.ToString() ?? "null_tnode");


            }
            var mods = decl as ITypeHasModifiers;
            if (mods != null)
            {
                using (OutColor.DarkMagenta)
                {
                    if (mods.IsStatic) Console.Write(" static");
                    Console.Write(" " + mods.AccessorType);
                }
            }
            Console.WriteLine();

        }

        private void VisitNode(Identifier id)
        {
            Console.Write(id + ": ");
            using (OutColor.Cyan)
                Console.WriteLine(id.Name);
        }

        private void VisitNode(Modifiers node)
        {
            Console.Write(node + ": ");
            var stringEnums = node.ModifierTokens.Select(x => x.ToString());
            using (OutColor.Magenta)
                Console.WriteLine(string.Join(", ", stringEnums));
        }

        private void VisitNode(ExprNode node)
        {
            Console.Write(node + ": ");
            using (OutColor.Magenta)
            {
                Console.Write(node.ExprType);
                Console.Write(" t:");
                Console.Write(node.EvalType?.ToString() ?? "null");
                Console.Write(" ");
                PrintTypeDescr(node);
                Console.WriteLine();
            }
        }


        private void VisitNode(IntLiteralExpr node)
        {
            Console.Write(node + ": ");
            using (OutColor.Yellow)
                Console.WriteLine(node.IntegerValue);
        }
        private void VisitNode(StringLiteralExpr node)
        {
            Console.Write(node + ": ");
            using (OutColor.Yellow)
                Console.WriteLine("\"" + node.StringVal + "\"");
        }

        private void VisitNode(TypeNode node)
        {
            Console.Write("<" + node.GetType().Name + ">: ");
            using (OutColor.Magenta)
                Console.Write(node);

            Console.Write(" ");
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
