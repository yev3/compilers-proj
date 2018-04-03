// 2-pass semantic analysis

using CompilerILGen.AST;
using NLog;

namespace CompilerILGen
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
