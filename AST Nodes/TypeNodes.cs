using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Proj3Semantics;

namespace Proj3Semantics.Nodes
{
    // TYPE DECLARATIONS

    public class VariableListDeclaring : AbstractNode
    {
        public TypeSpecifier TypeSpecifier { get; set; }
        public DeclaredVars ItemIdList { get; set; }
        public Expression Initialization { get; set; }
        public VariableListDeclaring(
            AbstractNode typeSpecifier,
            AbstractNode itemIdList,
            AbstractNode init = null)
        {
            // adding children for printing
            AddChild(typeSpecifier);
            AddChild(itemIdList);
            if (init != null) AddChild(init);

            // check that the parser assigned some type of a node with type info
            var decl = typeSpecifier as ITypeSpecifier;
            Debug.Assert(decl != null);
            TypeSpecifier = typeSpecifier as TypeSpecifier;

            ItemIdList = itemIdList as DeclaredVars;
            Debug.Assert(itemIdList != null);

            Initialization = init as Expression;
        }
    }


    public class DeclaredVars : AbstractNode
    {
        public DeclaredVars(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }




    public abstract class TypeSpecifier : AbstractNode, ITypeSpecifier{
        public abstract NodeTypeCategory NodeTypeCategory { get; set; }
        public virtual ITypeSpecifier TypeSpecifierRef { get; set; } = null;
    }

    public abstract class TypeName : TypeSpecifier  { }

    public class ArraySpecifier : TypeSpecifier
    {
        public ArraySpecifier(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }

        public override NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.Array;
            set => throw new InvalidOperationException("unable to set node type cat");
        }
    }

    public class QualifiedName : TypeName
    {
        public QualifiedName(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }

        public QualifiedName() { }
        public override NodeTypeCategory NodeTypeCategory { get; set; }
        public override ITypeSpecifier TypeSpecifierRef { get; set; }
    }


    /// <summary>
    /// (Page 303)
    /// </summary>
    public class Identifier : QualifiedName
    {
        public string Name { get; set; }
        public NodeTypeCategory TypeCategory { get; set; }
        public Identifier(string s)
        {
            Name = s;
        }
    }

    // TODO: fix
    public enum SpecialNameType { THIS, NULL }

    public class SpecialName : NotJustName
    {
        public SpecialNameType SpecialType { get; set; }
        public SpecialName(SpecialNameType specialType)
        {
            SpecialType = specialType;
        }

    }

    public class Literal : AbstractNode, ITypeSpecifier
    {
        public string Name { get; set; }
        public Literal(string s)
        {
            Name = s;
        }

        public NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.Class;
            set => throw new NotImplementedException("You're not supposed to set a literal");
        }


        public virtual ITypeSpecifier TypeSpecifierRef
        {
            get => this;
            set => throw new AccessViolationException("unable to set typeref of a string literal");
        }
    }

    public abstract class PrimitiveType : TypeSpecifier, IPrimitiveTypeDescriptor
    {
        public override NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.Primitive;
            set => throw new InvalidOperationException();
        }

        public abstract VariablePrimitiveTypes VariableTypePrimitive { get; set; }
    }

    public class BuiltinTypeVoid : TypeSpecifier
    {
        public override NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.Void;
            set => throw new InvalidOperationException();
        }

        public override ITypeSpecifier TypeSpecifierRef
        {
            get => this;
            set => throw new AccessViolationException("unable to set typeref of a builtin");
        }
    }

    public class BuiltinTypeInt : PrimitiveType
    {
        public override ITypeSpecifier TypeSpecifierRef
        {
            get => this;
            set => throw new AccessViolationException("unable to set typeref of a builtin");
        }

        public override VariablePrimitiveTypes VariableTypePrimitive
        {
            get => VariablePrimitiveTypes.Int;
            set => throw new NotImplementedException();
        }
    }

    public class BuiltinTypeBoolean : PrimitiveType
    {
        public override VariablePrimitiveTypes VariableTypePrimitive
        {
            get => VariablePrimitiveTypes.Boolean;
            set => throw new NotImplementedException();
        }

        public override ITypeSpecifier TypeSpecifierRef
        {
            get => this;
            set => throw new AccessViolationException("unable to set typeref of a builtin");
        }
    }

    public class Number : ComplexPrimary, IPrimitiveTypeDescriptor
    {
        public int Value { get; }
        public Number(int n) => Value = n;

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
    }

}
