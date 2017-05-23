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
        public AbstractNode TypeNameDecl { get; set; }
        public DeclaredVars ItemIdList { get; set; }
        public Expression Initialization { get; set; }
        public VariableListDeclaring(
            AbstractNode declType,
            AbstractNode itemIdList,
            AbstractNode init = null)
        {
            // adding children for printing
            AddChild(declType);
            AddChild(itemIdList);
            if (init != null) AddChild(init);

            // check that the parser assigned some type of a node with type info
            var decl = declType as ITypeInfo;
            Debug.Assert(decl != null);
            TypeNameDecl = declType;

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




    public class TypeName : AbstractNode { }
    public class TypeSpecifier : AbstractNode { }


    public class QualifiedName : TypeName, ITypeInfo
    {

        public QualifiedName(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }

        public QualifiedName() { }
        public NodeTypeCategory NodeTypeCategory { get; set; }
        public ITypeInfo TypeInfoRef { get; set; }
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

    public class Literal : AbstractNode, ITypeInfo
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

        public ITypeInfo TypeInfoRef
        {
            get => this;
            set => throw new AccessViolationException("unable to set typeref of a string literal");
        }
    }
    public class BuiltinTypeVoid : AbstractNode, ITypeInfo
    {
        public NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.Void;
            set => throw new NotImplementedException();
        }

        public ITypeInfo TypeInfoRef
        {
            get => this;
            set => throw new AccessViolationException("unable to set typeref of a builtin");
        }
    }

    public class BuiltinTypeInt : AbstractNode, IPrimitiveTypeDescriptor
    {
        public NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.Primitive;
            set => throw new NotImplementedException();
        }

        public ITypeInfo TypeInfoRef
        {
            get => this;
            set => throw new AccessViolationException("unable to set typeref of a builtin");
        }

        public VariablePrimitiveTypes VariableTypePrimitive
        {
            get => VariablePrimitiveTypes.Int;
            set => throw new NotImplementedException();
        }
    }

    public class BuiltinTypeBoolean : AbstractNode, IPrimitiveTypeDescriptor
    {
        public VariablePrimitiveTypes VariableTypePrimitive
        {
            get => VariablePrimitiveTypes.Boolean;
            set { throw new NotImplementedException(); }
        }

        public NodeTypeCategory NodeTypeCategory { get; set; }
        public ITypeInfo TypeInfoRef
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

        public ITypeInfo TypeInfoRef
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
