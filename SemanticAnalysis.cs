using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using Proj3Semantics.Nodes;

namespace Proj3Semantics
{
    using IEnv = ISymbolTable<ITypeSpecifier>;

    // perform semantic analysis on the compilation unit
    public class SemanticAnalysis
    {

        public static void Run(CompilationUnit cu)
        {
            IEnv cuTypeEnv = new SymbolTable<ITypeSpecifier>();
            IEnv cuNameEnv = new SymbolTable<ITypeSpecifier>();
            var visitor = new TopDeclVisitor(cuTypeEnv, cuNameEnv);
            visitor.Visit(cu);
        }

    }
}
