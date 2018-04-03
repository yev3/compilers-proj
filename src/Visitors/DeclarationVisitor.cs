using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.AST;

namespace Proj3Semantics
{
    using IEnv = ISymbolTable<Symbol>;

    public class DeclarationVisitor : IReflectiveVisitor
    {

        protected static Logger Log = LogManager.GetCurrentClassLogger();
        private IEnv Env { get; set; }

        public DeclarationVisitor(IEnv env)
        {
            Env = env;
        }

        public void Visit(dynamic node)
        {
            Log.Trace(this.GetType().Name + " is visiting " + node + " in env " + Env);
            if (node == null) return;
            VisitNode(node);
        }


        private void VisitNode(Node node)
        {
            Log.Trace("Abstract -- visiting all children");
            foreach (Node child in node.Children)
                Visit(child);
        }


        private void VisitNode(ClassDeclaration cdecl)
        {
            Log.Trace("Checking Class declaration in env: " + Env);
            string cname = cdecl.Name;

            var localClassDecls = Env.LookupLocalEntriesByType(cname, SymbolType.Class);
            if (localClassDecls.Count > 0)
            {
                CompilerErrors.Add(SemanticErrorTypes.DuplicateClassDecl, cname);
                return;
            }

            IEnv newScope = Env.GetNewLevel(cname);
            cdecl.Env = newScope;
            Env.EnterInfo(cname, new Symbol(SymbolType.Class, cdecl, newScope));

            var newScopeDeclVisitor = new DeclarationVisitor(newScope);
            newScopeDeclVisitor.Visit(cdecl.ClassBody);
        }

        private void VisitNode(ClassMethodDecl cmdecl)
        {
            VisitNode(cmdecl as AbstractFuncDecl);
        }

        private void VisitNode(AbstractFuncDecl fdecl)
        {
            Log.Trace("Visiting function declaration");
            string fname = fdecl.Identifier.Name;

            var localFunctions = from s in Env.LookupLocalEntriesByType(fname, SymbolType.Function)
                                 let f = s.DeclNode as AbstractFuncDecl
                                 where f != null
                                 select f;

            // check if any other functions have the same signature as me
            foreach (AbstractFuncDecl declared in localFunctions)
            {
                if (declared.ParamList == fdecl.ParamList)
                {
                    CompilerErrors.Add(SemanticErrorTypes.DuplicateFunctionDecl, fname);
                    return;
                }
            }

            IEnv newScope = Env.GetNewLevel(fname);
            fdecl.Env = newScope;
            Env.EnterInfo(fname, new Symbol(SymbolType.Function, fdecl, newScope));

            var newScopeDeclVisitor = new DeclarationVisitor(newScope);
            newScopeDeclVisitor.Visit(fdecl.ParamList);
            newScopeDeclVisitor.Visit(fdecl.MethodBody);
        }

        private void VisitNode(ParamList @params)
        {
            Log.Trace("Visiting parameters");
            foreach (ParamDecl decl in @params.ParamDeclList)
                Visit(decl);
        }

        private void VisitNode(ParamDecl decl)
        {
            Log.Trace("Visiting parameter declaration");
            var name = decl.Identifier.Name;
            var locals = Env.LookupLocalEntriesByType(name, SymbolType.Variable);

            if (locals.Count > 0)
            {
                CompilerErrors.Add(SemanticErrorTypes.DuplicateParamName, name);
            }
            else
            {
                var symbol = new Symbol(SymbolType.Variable, decl);
                Env.EnterInfo(name, symbol);
            }
        }

        private void VisitNode(Block blk)
        {
            Log.Trace("Visiting block");
            foreach (Node child in blk.Children)
                Visit(child);
        }

        private void VisitNode(LocalVarDecl declVars)
        {
            Log.Trace("Visiting VarDeclList");

            var typeNode = declVars.TypeSpecifier;
            foreach (VarDecl decl in declVars.VarDeclList.Children.Cast<VarDecl>())
            {
                decl.DeclTypeNode = typeNode;
                string name = decl.Identifier.Name;
                if (!Env.IsVarDeclaredLocally(name))
                {
                    var sym = new Symbol(SymbolType.Variable, decl);
                    Env.EnterInfo(name, sym);
                }
                else
                {
                    CompilerErrors.Add(SemanticErrorTypes.VariableAlreadyDeclared, name);
                }
            }
        }
        private void VisitNode(ClassFieldDeclStatement fieldDecl)
        {
            Log.Trace("Checking fields declaration in env: " + Env);
            Visit(fieldDecl.LocalVarDecl);
        }

        private void VisitNode(MethodCall call)
        {
            // do nothing
        }



        // TODO BELOW
        // ------------------------------------------------------------

        private void VisitNode(NamespaceDecl nsdecl)
        {
            //_log.Trace("Checking Namespace declaration in env: " + TypeEnv);
            //string name = nsdecl.Name;
            //ITypeDescriptor entry = TypeEnv.Lookup(name);
            //if (entry != null)
            //{
            //    CompilerErrors.Add(SemanticErrorTypes.DuplicateNamespaceDef, name);
            //    nsdecl.NodeTypeCategory = NodeTypeCategory.Error;
            //}
            //else
            //{
            //    TypeEnv.EnterInfo(name, nsdecl);
            //    var nsTypeEnv = TypeEnv.GetNewLevel(name);
            //    var nsNameEnv = NameEnv.GetNewLevel(name);
            //    nsdecl.TypeEnv = nsTypeEnv;
            //    nsdecl.NameEnv = nsNameEnv;
            //    var visitor = new FirstPassDeclVisitor(nsTypeEnv, nsNameEnv);
            //    visitor.Visit(nsdecl.NamespaceBody);
            //}
        }

        private void VisitNode(NamespaceBody namespaceBody)
        {
            Log.Trace("Checking Namespace declaration in env: " + Env);
            foreach (Node node in namespaceBody.Children)
                Visit(node);
        }

    }

}
