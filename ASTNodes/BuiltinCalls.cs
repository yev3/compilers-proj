using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics.ASTNodes
{
    public class BuiltinCallWrite : BuiltInType, IClassMethodTypeDesc
    {
        public override NodeTypeCategory NodeTypeCategory { get; set; } = NodeTypeCategory.ClassMethodDef;

        public AccessorType AccessorType { get; set; } = AccessorType.Public;
        public bool IsStatic { get; set; } = true;
        public string Name { get; set; } = "Write";
        public List<Parameter> MethodParameters { get; set; }
            = new List<Parameter>() {
                new Parameter(new BuiltInTypeObject(), new Identifier("obj") )
            };
        public ITypeDescriptor ReturnTypeNode { get; set; } = new BuiltinTypeVoid();
    }
    public class BuiltinCallWriteLine : BuiltinCallWrite
    {
        // TODO: figure out how to do a newline
    }
}
