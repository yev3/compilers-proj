using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics.ASTNodes
{
    // BUILT INS
    // =========================================

    public abstract class BuiltInType : TypeSpecifier
    {
        public abstract override NodeTypeCategory NodeTypeCategory { get; set; }
        public sealed override ITypeSpecifier TypeSpecifierRef
        {
            get => this;
            set => throw new AccessViolationException("unable to set typeref of a builtin");
        }
    }

    public class BuiltInTypeNull : BuiltInType
    {
        public override NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.Null;
            set => throw new InvalidOperationException();
        }
    }
    public class BuiltInTypeThis : BuiltInType
    {
        public override NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.This;
            set => throw new InvalidOperationException();
        }
    }

    public class BuiltinTypeVoid : BuiltInType
    {
        public override NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.Void;
            set => throw new InvalidOperationException();
        }
    }
    public abstract class PrimitiveType : BuiltInType, IPrimitiveTypeDescriptor
    {
        public override NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.Primitive;
            set => throw new InvalidOperationException();
        }

        public abstract VariablePrimitiveTypes VariableTypePrimitive { get; set; }
    }

    public class BuiltinTypeString : PrimitiveType
    {
        public override VariablePrimitiveTypes VariableTypePrimitive
        {
            get => VariablePrimitiveTypes.String;
            set => throw new AccessViolationException();
        }
    }

    public class BuiltinTypeInt : PrimitiveType
    {
        public override VariablePrimitiveTypes VariableTypePrimitive
        {
            get => VariablePrimitiveTypes.Int;
            set => throw new AccessViolationException();
        }
    }

    public class BuiltinTypeBoolean : PrimitiveType
    {
        public override VariablePrimitiveTypes VariableTypePrimitive
        {
            get => VariablePrimitiveTypes.Boolean;
            set => throw new NotImplementedException();
        }
    }

    public class NumberLiteral : ComplexPrimary, IPrimitiveTypeDescriptor
    {
        public int Value { get; }
        public NumberLiteral(int n) => Value = n;

        public NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.Primitive;
            set => throw new NotImplementedException("You're not supposed to set a number literal");
        }

        public ITypeSpecifier TypeSpecifierRef
        {
            get => this;
            set => throw new AccessViolationException("unable to set typeref of a builtin");
        }


        public VariablePrimitiveTypes VariableTypePrimitive
        {
            get => VariablePrimitiveTypes.Int;
            set => throw new NotImplementedException("You're not supposed to set a number literal");
        }
        public ISymbolTable<ITypeSpecifier> NameEnv
        {
            get => null;
            set => throw new AccessViolationException();
        }
    }
}
