using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics.ASTNodes
{
    public class BuiltinCallWrite : BuiltInType
    {
        // TODO
        public override NodeTypeCategory NodeTypeCategory { get; set; } = NodeTypeCategory.ClassMethodDef;
    }
    public class BuiltinCallWriteLine : BuiltInType
    {
        // TODO
        public override NodeTypeCategory NodeTypeCategory { get; set; } = NodeTypeCategory.ClassMethodDef;
    }
}
