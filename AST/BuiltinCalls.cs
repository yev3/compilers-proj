using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics.AST
{
    public class BuiltinCallWrite : ExprNode
    {

        public override ExprType ExprType { get; set; }
        public AccessorType AccessorType { get; set; } = AccessorType.Public;
        public bool IsStatic { get; set; } = true;
        public string Name { get; set; } = "Write";
        public List<ParamDecl> MethodParameters { get; set; }
            = new List<ParamDecl>() {
                new ParamDecl(TypeNode.TypeNodeObject, new Identifier("obj") )
            };
        public TypeNode ReturnTypeNode { get; set; } = TypeNode.TypeNodeVoid;
    }
    public class BuiltinCallWriteLine : BuiltinCallWrite
    {
        // TODO: figure out how to do a newline
    }
}
