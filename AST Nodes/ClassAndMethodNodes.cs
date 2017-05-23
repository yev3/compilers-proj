using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proj3Semantics;

namespace Proj3Semantics.Nodes
{
    public enum ModifierType { PUBLIC, STATIC, PRIVATE }
    public enum AccessorType { Public, Private }

    public class Modifiers : AbstractNode
    {
        public List<ModifierType> ModifierTokens { get; set; } = new List<ModifierType>();

        public void AddModType(ModifierType type)
        {
            ModifierTokens.Add(type);
        }

        public Modifiers(ModifierType type)
        {
            AddModType(type);
        }
    }
    public class ClassDeclaration : AbstractNode, IClassTypeDescriptor
    {

        // Interface Implementations
        // --------------------------------------------------
        #region InterfaceImplementations
        public NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.Class;
            set => throw new AccessViolationException("unable to set the type category.");
        }

        public ITypeInfo TypeInfoRef
        {
            get => this;
            set => throw new AccessViolationException("unable to set the class decl to a diff class decl.");
        }
        public ISymbolTable<ITypeInfo> FieldsEnv { get; set; } = new SymbolTable<ITypeInfo>();
        public ISymbolTable<ITypeInfo> MethodsEnv { get; set; } = new SymbolTable<ITypeInfo>();
        public IClassTypeDescriptor ParentClass { get; set; } = null;
        public AccessorType AccessorType { get; set; }
        public bool IsStatic { get; set; }
        #endregion

        public Modifiers Modifiers { get; set; }
        public ClassFields Fields { get; set; } = new ClassFields();
        public ClassMethods Methods { get; set; } = new ClassMethods();
        public NotImplemented NotImplemented { get; set; } = null;
        public ClassDeclaration(
            AbstractNode modifiers,
            AbstractNode className,
            AbstractNode classBody)
        {
            this.Modifiers = modifiers as Modifiers;
            this.Identifier = className as Identifier;
            foreach (var child in classBody)
            {
                if (child is ClassFieldDecl)
                {
                    Fields.AddChild(child);
                }
                else if (child is MethodDeclaration)
                {
                    Methods.AddChild(child);
                }
                else
                {
                    if (NotImplemented == null) { NotImplemented = new NotImplemented("Class Nodes"); }
                    NotImplemented.AddChild(child);
                }
            }

            // Note: modifiers will be type-checked later.
            AddChild(modifiers);
            AddChild(Fields);
            AddChild(Methods);
            if (NotImplemented != null)
                AddChild(NotImplemented);
        }

    }

    public class ClassFields : AbstractNode { }
    public class ClassMethods : AbstractNode { }


    public class ClassFieldDecl : AbstractNode, IClassFieldTypeDesc
    {
        public Modifiers Modifiers { get; set; }
        public AbstractNode VariableListDeclaring { get; set; }

        public ClassFieldDecl(
            AbstractNode modifiers,
            AbstractNode variableDeclarations)
        {
            Modifiers = modifiers as Modifiers;
            VariableListDeclaring = variableDeclarations;

            AddChild(modifiers);
            AddChild(variableDeclarations);
        }

        public NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.ClassFieldDef;
            set => throw new AccessViolationException("unable to set class field type");
        }
        public ITypeInfo TypeInfoRef { get; set; }
        public AccessorType AccessorType { get; set; }
        public bool IsStatic { get; set; }
    }

    public class ClassBody : AbstractNode
    {
        public ClassBody()
        {
            //Console.WriteLine("Class body is empty!");
        }
        public ClassBody(AbstractNode c)
        {
            AddChild(c);
        }
    }


    public class MethodDeclarator : AbstractNode
    {
        public readonly ParameterList ParameterList;
        public MethodDeclarator(AbstractNode identifier)
        {
            this.Identifier = identifier as Identifier;
        }
        public MethodDeclarator(AbstractNode identifier, AbstractNode paramList)
        {
            this.Identifier = identifier as Identifier;
            this.ParameterList = paramList as ParameterList;
        }
    }
    public class MethodDeclaration : AbstractNode, IClassMethodTypeDesc
    {
        public Modifiers Modifiers { get; set; }
        public ITypeInfo ReturnType { get; set; }
        public ParameterList ParameterList { get; set; }
        public Block MethodBody { get; set; }

        public MethodDeclaration(
            AbstractNode modifiers,
            AbstractNode typeSpecifier,
            AbstractNode methodDeclarator,
            AbstractNode methodBody)
        {
            this.Modifiers = modifiers as Modifiers;
            this.ReturnType = typeSpecifier as ITypeInfo;
            this.Identifier = methodDeclarator.Identifier;
            this.ParameterList = (methodDeclarator as MethodDeclarator)?.ParameterList;
            this.MethodBody = methodBody as Block;

            AddChild(modifiers);
            AddChild(typeSpecifier);
            if (this.ParameterList != null) AddChild(this.ParameterList);
            AddChild(methodBody);
        }

        public NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.ClassMethodDef;
            set => throw new AccessViolationException("unable to set method node to diff type.");
        } 
        public ITypeInfo TypeInfoRef { get; set; }
        public AccessorType AccessorType { get; set; }
        public bool IsStatic { get; set; }
    }

    public class Parameter : AbstractNode
    {
        public Parameter(AbstractNode typeSpec, AbstractNode declName)
        {
            AddChild(typeSpec);
            AddChild(declName);
        }
    }

    public class ParameterList : AbstractNode
    {
        public ParameterList(AbstractNode parameter)
        {
            AddChild(parameter);
        }
    }


    public class LocalVarDeclOrStatement : AbstractNode { }
}
