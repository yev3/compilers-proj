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

        private int LocalsCount { get; set; }
        private int LabelCount { get; set; }

        //private Dictionary<string, int> LocalsPosnMap { get; set; }
        //private Dictionary<string, string> LocalsTypeMap { get; set; }

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

        private void VisitNode(AccessorType accessorType)
        {
            switch (accessorType)
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
        }

        private void VisitNode(ClassDeclaration cdecl)
        {
            IL.Write(@".class ");
            Visit(cdecl.AccessorType);

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

            Visit(mdecl.AccessorType);
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

            LocalsCount = 0;
            Visit(mdecl.MethodBody);

            IL.WriteLine("ret");
            IL.WriteLine("}");
        }

        private void VisitNode(ClassFieldDeclStatement fdecl)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(".field ");
            Visit(fdecl.AccessorType);

            if (fdecl.IsStatic)
                sb.Append("static ");

            var varDecl = fdecl.LocalVarDecl;
            sb.Append(varDecl.TypeSpecifier.NodeTypeCategory.GetIlName());
            sb.Append(' ');

            var dcls = varDecl.VarDeclList.Children.Cast<VarDecl>();
            foreach (VarDecl dcl in dcls)
            {
                IL.Write(sb);
                IL.WriteLine(dcl.Name);
            }
        }


        private void VisitNode(LocalVarDecl vdecls)
        {
            var entryStrings = new List<string>();
            foreach (VarDecl d in vdecls.VarDeclList.Children.Cast<VarDecl>())
            {
                int posn = LocalsCount++;
                entryStrings.Add(string.Format(
                        "\t[{0}] {1} {2}", posn,
                        d.DeclTypeNode.NodeTypeCategory.GetIlName(),
                        d.Name));
                d.IlLocalsPosn = posn;
            }


            IL.WriteLine("\n// LocalVarDecl");
            IL.WriteLine(".locals init(");
            IL.WriteLine(string.Join(",\n", entryStrings));
            IL.WriteLine("\n)");
        }


        private void VisitNode(TypeRefNode type)
        {
            IL.Write(type.NodeTypeCategory.GetIlName());
            IL.Write(" ");
        }

        // this will print types like so.. string, int, bool...
        private void VisitNode(ParamList plist)
        {
            var paramList = plist?.ParamDeclList;
            if (paramList != null)
            {
                int paramPosn = 0;
                foreach (ParamDecl pdecl in paramList)
                    pdecl.IlLocalsPosn = paramPosn++;

                string insideParen = string.Join(", ", paramList.Select(p => (p.DeclTypeNode.NodeTypeCategory.GetIlName())));
                IL.Write(insideParen);
            }
        }

        string GetExprTypes(IEnumerable<ExprNode> exprs)
        {
            return string.Join(", ", exprs.Select(e => e.EvalType.NodeTypeCategory.GetIlName()));
        }
        private void VisitNode(MethodCall call)
        {
            Console.WriteLine();
            // push the arguments
            var args = call?.ArgumentList?.Children;
            if (args != null)
            {
                foreach (Node n in args)
                    Visit(n);
            }

            var funcDecl = call.MethodRef.AbstractFuncDecl;
            if (funcDecl == null) throw new ArgumentNullException(nameof(funcDecl));

            IL.Write("call\t ");
            IL.Write(funcDecl.ReturnTypeSpecifier.NodeTypeCategory.GetIlName());
            IL.Write(" ");

            var parentClass = (funcDecl as IClassMember)?.ParentClass;
            if (parentClass != null)
            {
                IL.Write(parentClass.Name);
                IL.Write("::");
            }

            IL.Write(funcDecl.Name);
            IL.Write("(");
            var paramList = funcDecl.ParamList?.ParamDeclList;
            if (paramList != null)
            {
                var ilNames = paramList.Select(p => p.DeclTypeNode.NodeTypeCategory.GetIlName());
                IL.Write(string.Join(", ", ilNames));
            }
            IL.WriteLine(")");
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
            var declNode = lval.SymbolRef.DeclNode;
            // load a local on the stack

            if (declNode is ParamDecl)
                IL.Write("ldarg.");
            else
                IL.Write("ldloc.");


            IL.WriteLine(declNode.IlLocalsPosn);
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

            int localsPosn = expr.LValueNode.SymbolRef.DeclNode.IlLocalsPosn;
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
                case ExprType.B_OR:
                    IL.WriteLine("or");
                    break;
                case ExprType.B_XOR:
                    IL.WriteLine("xor");
                    break;
                case ExprType.B_AND:
                    IL.WriteLine("and");
                    break;
                default:
                    throw new NotImplementedException("unsupported binary operator ");
            }
        }

        private void VisitNode(CompExpr cexpr)
        {
            Visit(cexpr.LhsExprNode);
            Visit(cexpr.RhsExprNode);
            switch (cexpr.ExprType)
            {
                case ExprType.EQUALS:
                    IL.WriteLine("ceq");    // done
                    break;
                case ExprType.NOT_EQUALS:   //
                    IL.WriteLine("ceq");
                    IL.WriteLine("ldc.i4.0");
                    IL.WriteLine("ceq");
                    break;
                case ExprType.GREATER_THAN:
                    IL.WriteLine("cgt");    // done
                    break;
                case ExprType.LESS_THAN:
                    IL.WriteLine("clt");    //
                    break;
                case ExprType.LESS_EQUAL:
                    IL.WriteLine("cgt");    // first >, then !
                    IL.WriteLine("ldc.i4.0");
                    IL.WriteLine("ceq");
                    break;
                case ExprType.GREATER_EQUAL:
                    IL.WriteLine("clt");    // first <, then !
                    IL.WriteLine("ldc.i4.0");
                    IL.WriteLine("ceq");
                    break;
                default:
                    throw new NotImplementedException("unsupported compare operator");
            }

        }

        private void VisitNode(IfStatementElse condStmt)
        {
            int num = LabelCount++;
            string lblElse = "lbl_else_" + num;
            string lblEnd = "lbl_endif_" + num;

            // todo for elseif: declare a new local temp bool

            // eval predicate
            Visit(condStmt.Predicate);

            // todo for elseif: store in the local temp

            // if the pred is false, then jump to LBL_FALSE
            IL.WriteLine("brfalse\t" + lblElse);

            Visit(condStmt.ThenStatement);
            IL.WriteLine("br\t" + lblEnd);

            IL.WriteLine("\n" + lblElse + ":");
            Visit(condStmt.ElseStatement);

            IL.WriteLine("\n" + lblEnd + ":");

            // todo for elseif: else if support
        }

        private void VisitNode(WhileLoop lp)
        {
            int num = LabelCount++;
            string lblBefore = "before_while_" + num;
            string lblAfter = "after_while_" + num;

            IL.WriteLine("\n" + lblBefore + ":");
            Visit(lp.Predicate);
            IL.WriteLine("brfalse\t" + lblAfter);

            Visit(lp.BodyStatement);
            IL.WriteLine("br\t" + lblBefore);

            IL.WriteLine("\n" + lblAfter + ":");
        }



        private void VisitNode(IntLiteralExpr intExpr)
        {
            IL.WriteLine("ldc.i4\t" + intExpr.IntegerValue);
        }



    }
}
