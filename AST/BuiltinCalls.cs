using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics.AST
{
    public class BuiltinCalls
    {
        public static BuiltinCallWrite BuiltinCallWrite { get; } = new BuiltinCallWrite();
        public static BuiltinCallWriteLine BuiltinCallWriteLine { get; } = new BuiltinCallWriteLine();

    }
    public class BuiltinCallWrite : AbstractFuncDecl
    {
        public override ISymbolTable<Symbol> Env { get; set; } = null;
        public override TypeNode ReturnTypeSpecifier { get; } = TypeNode.TypeNodeVoid;

        public override ParamList ParamList { get; set; }
            = new ParamList(new[]{ new ParamDecl(TypeNode.TypeNodeObject, new Identifier("obj"))});
        public override Block MethodBody { get; set; } = null;
        public List<ParamDecl> MethodParameters { get; set; }
        public TypeNode ReturnTypeNode { get; set; } = TypeNode.TypeNodeVoid;

        public BuiltinCallWrite() : base(new Identifier("BuiltinCallWrite")) { }

        protected BuiltinCallWrite(Identifier identifier) : base(identifier) { }
    }
    public class BuiltinCallWriteLine : BuiltinCallWrite
    {
        public BuiltinCallWriteLine() : base(new Identifier("BuiltinCallWriteLine")) { }
    }
}
