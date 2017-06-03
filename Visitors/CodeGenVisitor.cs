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
        private IEnv Env { get; set; }
        private StreamWriter IL { get; set; }
        private string AssemblyName { get; set; }

        private const string EntryPointFunc = "main";

        public CodeGenVisitor(IEnv env, StreamWriter il, String assemblyName)
        {
            Env = env;
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
            List<string> list = new List<string>()
            {
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
            };
            IL.Write(".method ");

            if (mdecl.IsStatic)
            {
                IL.Write("static ");
            }

            var returnType = mdecl.ReturnTypeSpecifier;

            IL.Write(returnType + " ");

            IL.Write(mdecl.Name + "(");
            foreach (ParamDecl paramListChild in mdecl.ParamList.ParamDeclList)
            {
                Visit(paramListChild);
            }
            IL.WriteLine(")");
            IL.WriteLine("{");
            IL.WriteLine(".entrypoint");
            IL.WriteLine(".maxstack 15");

            foreach (Node bodyChild in mdecl.MethodBody.Children)
            {
                Visit(bodyChild);
            }
            IL.WriteLine("ret");
            IL.WriteLine("}");
        }

        private void VisitNode(ParamDecl paramDecl)
        {
            // TODO
        }

        private void VisitNode(Block block)
        {
            // TODO
            foreach (Node bodyChild in block.Children)
            {
                Visit(bodyChild);
            }
        }

        private void VisitNode(Statement statement)
        {
            // TODO
            foreach (Node bodyChild in block.Children)
            {
                Visit(bodyChild);
            }
        }

    }
}
