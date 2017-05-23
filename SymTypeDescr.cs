using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using Proj3Semantics.Nodes;

namespace Proj3Semantics
{

    public enum NodeTypeCategory
    {
        Primitive, Null, Array, Class, Void, This, ErrorType, ClassFieldDef, ClassMethodDef
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

    public interface IPrimitiveTypeDescriptor : ITypeSpecifier
    {
        VariablePrimitiveTypes VariableTypePrimitive { get; set; }
    }

    public interface ITypeHasModifiers
    {
        AccessorType AccessorType { get; set; }
        bool IsStatic { get; set; }

    }

    public interface IClassTypeDescriptor : ITypeSpecifier, ITypeHasModifiers
    {
        ISymbolTable<ITypeSpecifier> NameEnv { get; set; }
        IClassTypeDescriptor ParentClass { get; set; }
    }

    public interface IClassFieldTypeDesc : ITypeSpecifier, ITypeHasModifiers
    {
        Identifier Identifier { get; set; }
    }

    public interface IClassMethodTypeDesc : ITypeSpecifier, ITypeHasModifiers
    {
        TypeSpecifier ReturnType { get; set; }
    }

}
