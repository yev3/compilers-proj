// Built-in language features such as WriteLine

using System.Collections.Generic;

namespace CompilerILGen.AST
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

        public WriteStatement(ArgumentList args) : base(args)
        {
        }
    }

    public class WriteLineStatement : BuiltinStatement
    {
        public override AbstractFuncDecl AbstractFuncDecl { get; set; } = BuiltinMethodRefs.SysWriteLineRef;

        public WriteLineStatement(ArgumentList args) : base(args)
        {
        }
    }

    public class BuiltinMethodRefs
    {
        public static SysWriteRef SysWriteRef { get; } = new SysWriteRef();
        public static SysWriteLineRef SysWriteLineRef { get; } = new SysWriteLineRef();
    }

    public class SysWriteRef : AbstractFuncDecl
    {
        public override ISymbolTable<Symbol> Env { get; set; } = null;
        public override TypeRefNode ReturnTypeSpecifier { get; } = TypeRefNode.TypeNodeVoid;

        public override ParamList ParamList { get; set; }
            = new ParamList(new[] {new ParamDecl(TypeRefNode.TypeNodeObject, new Identifier("obj"))});

        public override Block MethodBody { get; set; } = null;
        public List<ParamDecl> MethodParameters { get; set; }
        public TypeRefNode ReturnTypeNode { get; set; } = TypeRefNode.TypeNodeVoid;

        public SysWriteRef() : base(new Identifier("BuiltinCallWrite"))
        {
        }

        protected SysWriteRef(Identifier identifier) : base(identifier)
        {
        }
    }

    public class SysWriteLineRef : SysWriteRef
    {
        public SysWriteLineRef() : base(new Identifier("BuiltinCallWriteLine"))
        {
        }
    }
}