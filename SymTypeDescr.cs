using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics
{

    //Primitive, Null, Array, Class, Void, ErrorType
    public abstract class TypeDescriptor
    {
        public VariableTypes KindVariableCategory { get; set; }
    }

    public class PrimitiveTypeDescriptor : TypeDescriptor
    {
        public PrimitiveTypeDescriptor()
        {
            KindVariableCategory = VariableTypes.Primitive;
        }
        public VariablePrimitiveTypes VariableTypeOfPrimitive { get; set; }
    }

    public class NullTypeDescriptor : TypeDescriptor
    {
        public NullTypeDescriptor()
        {
            KindVariableCategory = VariableTypes.Null;
        }

    }

    public class ClassTypeDescriptor : TypeDescriptor
    {
        public ClassTypeDescriptor()
        {
            KindVariableCategory = VariableTypes.Class;
        }
    }
    class VoidDescriptorType : TypeDescriptor
    {
        public VoidDescriptorType()
        {
            KindVariableCategory = VariableTypes.Void;
        }
    }

    class ErrorDescriptorType : TypeDescriptor
    {
        public ErrorDescriptorType()
        {
            KindVariableCategory = VariableTypes.ErrorType;
        }
    }


}
