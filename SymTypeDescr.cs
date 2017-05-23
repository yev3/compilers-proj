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
    public interface ITypeInfo
    {
        // every type belongs to some kind of a category
        NodeTypeCategory NodeTypeCategory { get; set; }
        ITypeInfo TypeInfoRef { get; set; }
    }

    public interface IPrimitiveTypeDescriptor : ITypeInfo
    {
        VariablePrimitiveTypes VariableTypePrimitive { get; set; }
    }

    public interface ITypeHasModifiers
    {
        Modifiers Modifiers { get; set; }
        AccessorType AccessorType { get; set; }
        bool IsStatic { get; set; }
        
    }

    public interface IClassTypeDescriptor : ITypeInfo, ITypeHasModifiers
    {
        ISymbolTable<ITypeInfo> FieldsEnv { get; set; }
        ISymbolTable<ITypeInfo> MethodsEnv { get; set; }
        IClassTypeDescriptor ParentClass { get; set; }
    }

    public interface IClassFieldTypeDesc : ITypeInfo, ITypeHasModifiers
    {
        
    }

    public interface IClassMethodTypeDesc : ITypeInfo, ITypeHasModifiers
    {
        ITypeInfo ReturnType { get; set; }
    }

}
