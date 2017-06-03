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

        private Dictionary<string, int> LocalsPosnMap { get; set; }
        private Dictionary<string, string> LocalsTypeMap { get; set; }

        public CodeGenVisitor(StreamWriter il, String assemblyName)
        {
            IL = il;
            AssemblyName = assemblyName;
        }

        public void Generate(Node root)
        {
            GeneratePrelude();
            Visit(root);
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

        void GenerateLocalsMaps(IEnv env)
        {
            var idPosnDict = new Dictionary<string, int>();
            var typesDict = new Dictionary<string, string>();
            var locals = env.GetLocalDeclarations();

            var stuff = from l in locals
                        let d = l.DeclNode
                        select new { varName = d.Name, typeName = GetTypeName(d.DeclTypeNode) };

            int idxCount = 0;
            var declStrings = new List<string>();

            foreach (var s in stuff)
            {
                int curIdx = idxCount++;
                idPosnDict.Add(s.varName, curIdx);
                typesDict.Add(s.varName, s.typeName);
                declStrings.Add(string.Format("\t[{0}] {1} {2}", curIdx, s.typeName, s.varName));
            }

            IL.Write(string.Join(" , \n", declStrings));

            LocalsPosnMap = idPosnDict;
            LocalsTypeMap = typesDict;
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

            IL.WriteLine(".locals init(");
            GenerateLocalsMaps(mdecl.Env);
            IL.WriteLine("\n)");

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
                    t = "int32";
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

        string GetExprTypes(IEnumerable<ExprNode> exprs)
        {
            return string.Join(", ", exprs.Select(e => GetTypeName(e.EvalType)));
        }

        private void VisitNode(WriteStatement stmt)
        {
            foreach (Node child in stmt.Children)
                Visit(child);

            IL.Write("call\t void [mscorlib]System.Console::Write(");
            if (stmt.Children.Count > 1)
                throw new NotImplementedException("more than 1 argument in writeline is unsupported.");
            IL.Write(GetExprTypes(stmt.Children.Cast<ExprNode>()));
            IL.WriteLine(")");
        }

        private void VisitNode(WriteLineStatement stmt)
        {
            foreach (Node child in stmt.Children)
                Visit(child);

            IL.Write("call\t void [mscorlib]System.Console::WriteLine(");

            if (stmt.Children.Count > 1)
                throw new NotImplementedException("more than 1 argument in writeline is unsupported.");
            IL.Write(GetExprTypes(stmt.Children.Cast<ExprNode>()));
            IL.WriteLine(")");
        }

        private void VisitNode(EvalExpr evalExpr)
        {
            Visit(evalExpr.ChildExpr);
        }

        private void VisitNode(LValueNode lval)
        {
            var localPosn = LocalsPosnMap[lval.Identifier.Name];
            // load a local on the stack
            IL.Write("ldloc.");
            IL.WriteLine(localPosn);
        }

        private void VisitNode(ExprNode expr)
        {
            throw new AccessViolationException("YOU ARE MISSING AN OVERLOAD!!");
        }

        private void VisitNode(StringLiteralExpr str)
        {
            IL.Write("ldstr\t \"");
            IL.Write(str.StringVal);
            IL.WriteLine("\"");
        }

        private void VisitNode(AssignExpr expr)
        {
            Visit(expr.RhsExprNode);

            var varName = expr.LValueNode.Identifier.Name;
            int localsPosn = LocalsPosnMap[varName];
            // store into locals
            IL.Write("stloc.");
            IL.WriteLine(localsPosn);
        }

        private void VisitNode(BinaryExpr bexpr)
        {
            Visit(bexpr.LhsExprNode);
            Visit(bexpr.RhsExprNode);
            switch (bexpr.ExprType)
            {
                case ExprType.PLUSOP:
                    IL.WriteLine("add");
                    break;
                case ExprType.MINUSOP:
                    IL.WriteLine("sub");
                    break;
                case ExprType.ASTERISK:
                    IL.WriteLine("mul");
                    break;
                case ExprType.RSLASH:
                    IL.WriteLine("div");
                    break;
                case ExprType.PERCENT:
                default:
                    throw new NotImplementedException("unsupported binary operator ");
            }

        }
        private void VisitNode(IntLiteralExpr intExpr)
        {
           IL.WriteLine("ldc.i4.s\t" + intExpr.IntegerValue);
        }

        private void VisitNode(LocalVarDecl x) { }  // do nothing

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
