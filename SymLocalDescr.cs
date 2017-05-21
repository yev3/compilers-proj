using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.Design;

namespace Proj3Semantics
{
    public class LocalVarDescriptor
    {
        public VariableTypes Kind { get; set; }
        public TypeDescriptor TypeDescriptor { get; set; }
    }
}
