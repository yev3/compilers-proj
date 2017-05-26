using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics.ASTNodes
{
    // BUILT INS
    // =========================================

    public abstract class BuiltInType : TypeDescriptor
    {
        public abstract override NodeTypeCategory NodeTypeCategory { get; set; }
        public override ITypeDescriptor TypeDescriptorRef
        {
            get { return this; }
            set
            {
                throw new AccessViolationException(
                    "unable to set typeref of a builtin");
            }
        }
    }

    public class BuiltInTypeNull : BuiltInType
    {
        public override NodeTypeCategory NodeTypeCategory
        {
            get { return NodeTypeCategory.Null; }
            set { throw new InvalidOperationException(); }
        }
    }
    public class BuiltInTypeThis : BuiltInType
    {
        public override NodeTypeCategory NodeTypeCategory
        {
            get { return NodeTypeCategory.This; }
            set { throw new InvalidOperationException(); }
        }
    }

    public class BuiltInTypeObject : BuiltInType, IPrimitiveTypeDescriptor
    {
        public override NodeTypeCategory NodeTypeCategory
        {
            get { return NodeTypeCategory.Primitive; }
            set { throw new InvalidOperationException(); }
        }

        public VariablePrimitiveType VariablePrimitiveType
        {
            get { return VariablePrimitiveType.Object; }
            set { throw new AccessViolationException(); }
        }
    }

    public class BuiltinTypeVoid : BuiltInType
    {
        public override NodeTypeCategory NodeTypeCategory
        {
            get { return NodeTypeCategory.Void; }
            set { throw new InvalidOperationException(); }
        }
    }
    public abstract class PrimitiveType : BuiltInType, IPrimitiveTypeDescriptor
    {
        public override NodeTypeCategory NodeTypeCategory
        {
            get { return NodeTypeCategory.Primitive; }
            set { throw new InvalidOperationException(); }
        }

        public abstract VariablePrimitiveType VariablePrimitiveType { get; set; }
    }

    public class BuiltinTypeString : PrimitiveType
    {
        public override VariablePrimitiveType VariablePrimitiveType
        {
            get { return VariablePrimitiveType.String; }
            set { throw new AccessViolationException(); }
        }
    }

    public class BuiltinTypeInt : PrimitiveType
    {
        public override VariablePrimitiveType VariablePrimitiveType
        {
            get { return VariablePrimitiveType.Int; }
            set { throw new AccessViolationException(); }
        }
    }

    public class BuiltinTypeBoolean : PrimitiveType
    {
        public override VariablePrimitiveType VariablePrimitiveType
        {
            get { return VariablePrimitiveType.Boolean; }
            set { throw new NotImplementedException(); }
        }
    }

    public class NumberLiteral : PrimitiveType, IPrimitiveTypeDescriptor
    {
        public int Value { get; }
        public NumberLiteral(int n)
        {
            Value = n;
        }


        public override VariablePrimitiveType VariablePrimitiveType
        {
            get { return VariablePrimitiveType.Int; }
            set
            {
                throw new NotImplementedException(
                    "You're not supposed to set a number literal");
            }
        }

        public override ITypeDescriptor TypeDescriptorRef
        {
            get { return this; }
            set
            {
                throw new AccessViolationException(
                    "unable to set typeref of a builtin");
            }
        }


    }



}
