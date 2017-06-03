using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.AST;

namespace Proj3Semantics.Visitors
{
    using IEnv = ISymbolTable<Symbol>;

    public class CodeGenVisitor
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        private StreamWriter IL { get; set; }
        private string AssemblyName { get; set; }

        private const string EntryPointFunc = "main";

        public CodeGenVisitor(StreamWriter il, String assemblyName)
        {
            IL = il;
            AssemblyName = assemblyName;
        }

        public void Generate(Node root)
        {
            GeneratePrelude();
            Visit(root);



            List<string> list = new List<string>()
            {
                @".class public hello",
                @"{",
                @"  .method static void main() ",
                @"  {",
                @"    .entrypoint",
                @"    .maxstack 1",
                @"",
                @"    ldstr     ""Hello, World""",
                @"    call      void [mscorlib]System.Console::WriteLine(string)",
                @"",
                @"    ret          ",
                @"  } ",
                @"} ",
            };

        }

        private void GeneratePrelude()
        {
            IL.WriteLine(@".assembly extern mscorlib {}");
            IL.Write(@".assembly ");
            IL.Write(AssemblyName);
            IL.WriteLine(@" {}");
        }

        private void Visit(dynamic node)
        {
            if (node == null) return;
            VisitNode(node);
        }

        private void VisitNode(Node node)
        {
            foreach (Node child in node.Children)
            {
                Visit(child);
            }
        }

        private void VisitNode(ClassDeclaration cdecl)
        {
            IL.Write(@".class ");

            switch (cdecl.AccessorType)
            {
                case AccessorType.Public:
                    IL.Write(@"public ");
                    break;
                case AccessorType.Private:
                    IL.Write(@"private ");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (cdecl.IsStatic)
            {
                IL.Write("static ");
            }

            var cname = cdecl.Name;
            IL.WriteLine(cname);
            IL.WriteLine("{");

            foreach (Node bodyChild in cdecl.ClassBody.Children)
            {
                Visit(bodyChild);
            }

            IL.Write("} // end of class ");
            IL.WriteLine(cname);
        }

        private void VisitNode(ClassMethodDecl mdecl)
        {
            bool isEntryPoint = mdecl.Name.ToLower() == "main";


            IL.Write(".method ");

            if (mdecl.IsStatic || isEntryPoint)
            {
                IL.Write("static ");
            }

            Visit(mdecl.ReturnTypeSpecifier);

            IL.Write(mdecl.Name + "(");

            Visit(mdecl.ParamList);

            IL.WriteLine(")");
            IL.WriteLine("{");
            if (isEntryPoint)
                IL.WriteLine(".entrypoint");
            IL.WriteLine(".maxstack 15");

            foreach (Node bodyChild in mdecl.MethodBody.Children)
            {
                Visit(bodyChild);
            }
            IL.WriteLine("ret");
            IL.WriteLine("}");
        }



        private void VisitNode(Block block)
        {
            // TODO
            foreach (Node bodyChild in block.Children)
            {
                Visit(bodyChild);
            }
        }

        private string GetTypeName(TypeRefNode type)
        {
            string t;
            switch (type.NodeTypeCategory)
            {
                case NodeTypeCategory.Int:
                    t = "int";
                    break;
                case NodeTypeCategory.String:
                    t = "string";
                    break;
                case NodeTypeCategory.Object:
                    t = "object";
                    break;
                case NodeTypeCategory.Boolean:
                    t = "bool";
                    break;
                case NodeTypeCategory.Void:
                    t = "void";
                    break;
                case NodeTypeCategory.This:
                    t = "this";
                    break;
                default:
                    throw new NotImplementedException("unsupported generation of " + type.NodeTypeCategory);
            }
            return t;
        }

        private void VisitNode(TypeRefNode type)
        {
            IL.Write(GetTypeName(type));
            IL.Write(" ");
        }

        // this will print types like so.. string, int, bool...
        private void VisitNode(ParamList plist)
        {
            var paramList = plist?.ParamDeclList;
            if (paramList != null)
            {
                string insideParen = string.Join(", ", paramList.Select(p => GetTypeName(p.DeclTypeNode)));
                IL.Write(insideParen);
            }

        }

        private void VisitNode(WriteLineStatement stmt)
        {
            foreach (Node child in stmt.Children)
                Visit(child);

            IL.Write("\t\tcall\t\t void [mscorlib]System.Console::WriteLine(");
            Visit(stmt.AbstractFuncDecl.ParamList);
            IL.WriteLine(")");
        }

        private void VisitNode(EvalExpr evalExpr)
        {
            Visit(evalExpr.ChildExpr);
        }

        private void VisitNode(ExprNode expr)
        {
            throw new AccessViolationException("YOU ARE MISSING AN OVERLOAD!!");
        }

        private void VisitNode(StringLiteralExpr str)
        {
            IL.Write("\t\tldstr\t\t \"");
            IL.Write(str.StringVal);
            IL.WriteLine("\"");
        }



        private void VisitNode(Statement statement)
        {
            // TODO
            foreach (Node bodyChild in statement.Children)
            {
                Visit(bodyChild);
            }
        }

    }
}
