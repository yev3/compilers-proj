using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics.AST
{
    public abstract class BuiltinStatement : Statement
    {
        public abstract AbstractFuncDecl AbstractFuncDecl { get; set; }

        public BuiltinStatement(ArgumentList args)
        {
            if (args != null) this.Children = args.Children;
        }
    }

    public class WriteStatement : BuiltinStatement
    {
        public override AbstractFuncDecl AbstractFuncDecl { get; set; } = BuiltinMethodRefs.SysWriteRef;
        public WriteStatement(ArgumentList args) : base(args) { }
    }
    public class WriteLineStatement : BuiltinStatement
    {
        public override AbstractFuncDecl AbstractFuncDecl { get; set; } = BuiltinMethodRefs.SysWriteLineRef;
        public WriteLineStatement(ArgumentList args) : base(args) { }
    }

    public class BuiltinMethodRefs
    {
        public static SysWriteRef SysWriteRef { get; } = new SysWriteRef();
        public static SysWriteLineRef SysWriteLineRef { get; } = new SysWriteLineRef();

    }
    public class SysWriteRef : AbstractFuncDecl
    {
        public override ISymbolTable<Symbol> Env { get; set; } = null;
        public override TypeNode ReturnTypeSpecifier { get; } = TypeNode.TypeNodeVoid;

        public override ParamList ParamList { get; set; }
            = new ParamList(new[]{ new ParamDecl(TypeNode.TypeNodeObject, new Identifier("obj"))});
        public override Block MethodBody { get; set; } = null;
        public List<ParamDecl> MethodParameters { get; set; }
        public TypeNode ReturnTypeNode { get; set; } = TypeNode.TypeNodeVoid;

        public SysWriteRef() : base(new Identifier("BuiltinCallWrite")) { }

        protected SysWriteRef(Identifier identifier) : base(identifier) { }
    }
    public class SysWriteLineRef : SysWriteRef
    {
        public SysWriteLineRef() : base(new Identifier("BuiltinCallWriteLine")) { }
    }
}
