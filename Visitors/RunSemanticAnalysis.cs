using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.AST;

namespace Proj3Semantics
{
    using IEnv = ISymbolTable<Symbol>;

    // perform semantic analysis on the compilation unit
    public static class RunSemanticAnalysis
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        public static void Run(CompilationUnit cu)
        {
            IEnv cuEnv = new SymbolTable();

            Log.Info("--- Start Declaration Visits ---");
            var preDeclVisitor = new DeclarationVisitor(cuEnv);
            preDeclVisitor.Visit(cu);

            Log.Info("--- Start Type Checking Visits ---");
            var typeCheckVisitor = new SemanticsVisitor(cuEnv);
            typeCheckVisitor.Visit(cu);
        }
    }
}
