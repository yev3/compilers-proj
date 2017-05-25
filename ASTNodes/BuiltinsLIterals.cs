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

        public VariablePrimitiveTypes VariableTypePrimitive
        {
            get { return VariablePrimitiveTypes.Object; }
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

        public abstract VariablePrimitiveTypes VariableTypePrimitive { get; set; }
    }

    public class BuiltinTypeString : PrimitiveType
    {
        public override VariablePrimitiveTypes VariableTypePrimitive
        {
            get { return VariablePrimitiveTypes.String; }
            set { throw new AccessViolationException(); }
        }
    }

    public class BuiltinTypeInt : PrimitiveType
    {
        public override VariablePrimitiveTypes VariableTypePrimitive
        {
            get { return VariablePrimitiveTypes.Int; }
            set { throw new AccessViolationException(); }
        }
    }

    public class BuiltinTypeBoolean : PrimitiveType
    {
        public override VariablePrimitiveTypes VariableTypePrimitive
        {
            get { return VariablePrimitiveTypes.Boolean; }
            set { throw new NotImplementedException(); }
        }
    }

    public class NumberLiteral : ComplexPrimary, IPrimitiveTypeDescriptor
    {
        public int Value { get; }
        public NumberLiteral(int n)
        {
            Value = n;
        }

        public NodeTypeCategory NodeTypeCategory
        {
            get { return NodeTypeCategory.Primitive; }
            set
            {
                throw new NotImplementedException(
                    "You're not supposed to set a number literal");
            }
        }


        public VariablePrimitiveTypes VariableTypePrimitive
        {
            get { return VariablePrimitiveTypes.Int; }
            set
            {
                throw new NotImplementedException(
                    "You're not supposed to set a number literal");
            }
        }

        public ISymbolTable<ITypeDescriptor> NameEnv
        {
            get { return null; }
            set { throw new AccessViolationException(); }
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
