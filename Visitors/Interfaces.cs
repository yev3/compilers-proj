using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using Proj3Semantics.ASTNodes;

// found here:
// http://www.ccs.neu.edu/home/riccardo/courses/csu370-fa07/lect4.pdf

namespace Proj3Semantics
{
    using IEnv = ISymbolTable<ITypeSpecifier>;

    public enum NodeTypeCategory
    {
        NOT_SET, Primitive, Null, Array, Class, Void, This, ErrorType, ClassFieldDef, ClassMethodDef, NamespaceDecl
    }

    public enum VariablePrimitiveTypes
    {
        Boolean, Byte, Char, Short, Int, Long, Float, Double, NotPrimitive
    }

	public interface IVisitableNode
	{
	   void Accept(IReflectiveVisitor rv);
	}

    public interface IReflectiveVisitor
    {
        void Visit(dynamic node);
        
    }

    public interface IHasOwnScope
    {
        IEnv NameEnv { get; set; }
        IEnv TypeEnv { get; set; }
    }

    /// <summary>
    /// Specifies if the node has type information attached to it
    /// </summary>
    public interface ITypeSpecifier 
    {
        // every type belongs to some kind of a category
        NodeTypeCategory NodeTypeCategory { get; set; }
        ITypeSpecifier TypeSpecifierRef { get; set; }
    }


    public interface INamedType
    {
        string Name { get; set; }
    }

    public interface IPrimitiveTypeDescriptor : ITypeSpecifier
    {
        VariablePrimitiveTypes VariableTypePrimitive { get; set; }
    }

    public interface ITypeHasModifiers
    {
        AccessorType AccessorType { get; set; }
        bool IsStatic { get; set; }

    }

    public interface IClassTypeDescriptor : ITypeSpecifier, ITypeHasModifiers, INamedType
    {
        IClassTypeDescriptor ParentClass { get; set; }
    }

    public interface IClassMember : ITypeSpecifier, ITypeHasModifiers, INamedType
    {

    }

    public interface IClassFieldTypeDesc : IClassMember
    {
    }

    public interface IClassMethodTypeDesc : IClassMember
    {
        ITypeSpecifier ReturnTypeNode { get; set; }
    }

}
