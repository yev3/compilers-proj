using System;
using System.Collections.Generic;

namespace Proj3Semantics.AST_Nodes
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
    public class ClassDeclaration : AbstractNode, IClassTypeDescriptor, INamedType, IHasOwnScope
    {

        // Interface Implementations
        // --------------------------------------------------
        #region InterfaceImplementations
        public NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.Class;
            set => throw new AccessViolationException("unable to set the type category.");
        }

        public ITypeSpecifier TypeSpecifierRef
        {
            get => this;
            set => throw new AccessViolationException("unable to set the class decl to a diff class decl.");
        }

        public ISymbolTable<ITypeSpecifier> NameEnv { get; set; } = null;
        public ISymbolTable<ITypeSpecifier> TypeEnv { get; set; } = null;
        public IClassTypeDescriptor ParentClass { get; set; } = null;
        public AccessorType AccessorType { get; set; }
        public bool IsStatic { get; set; }
        public string Name { get; set; }
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
            this.Name = (className as Identifier)?.Name;
            foreach (var child in classBody)
            {
                if (child is ClassFieldDeclStatement)
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

        public override string ToString()
        {
            return "<" + this.GetType().Name + ":" + Name + ">";
        }
    }

    public class ClassFields : AbstractNode { }
    public class ClassMethods : AbstractNode { }

    public class FieldVarDecl : AbstractNode, IClassFieldTypeDesc, INamedType
    {
        public FieldVarDecl(AbstractNode identifier)
        {
            Name = (identifier as Identifier)?.Name;
            if (Name == null) throw new ArgumentException("Field variable decl without an identifier.");
        }

        public NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.ClassFieldDef;
            set => throw new AccessViolationException("unable to set class field type");
        }
        public ITypeSpecifier TypeSpecifierRef { get; set; } = null;
        public AccessorType AccessorType { get; set; }
        public bool IsStatic { get; set; }
        public string Name { get; set; }
        public ISymbolTable<ITypeSpecifier> NameEnv
        {
            get => null;
            set => throw new AccessViolationException();
        }
    }

    public class ClassFieldDeclStatement : AbstractNode
    {
        public Modifiers Modifiers { get; set; }
        public VariableListDeclaring VariableListDeclaring { get; set; }

        public ClassFieldDeclStatement(
            AbstractNode modifiers,
            AbstractNode variableDeclarations)
        {
            Modifiers = modifiers as Modifiers;
            VariableListDeclaring = variableDeclarations as VariableListDeclaring;

            AddChild(modifiers);
            AddChild(variableDeclarations);
        }

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
        public readonly Identifier Identifier;
        public readonly ParameterList ParameterList;
        public MethodDeclarator(AbstractNode methodDeclaratorName)
        {
            this.Identifier = methodDeclaratorName as Identifier;
        }
        public MethodDeclarator(AbstractNode methodDeclaratorName, AbstractNode paramList)
        {
            this.Identifier = methodDeclaratorName as Identifier;
            this.ParameterList = paramList as ParameterList;
        }
    }
    public class MethodDeclaration : AbstractNode, IClassMethodTypeDesc, IHasOwnScope, INamedType
    {
        public Modifiers Modifiers { get; set; }
        public ParameterList ParameterList { get; set; }
        public Block MethodBody { get; set; }

        public MethodDeclaration(
            AbstractNode modifiers,
            AbstractNode returnTypeSpecifier,
            AbstractNode methodDeclarator,
            AbstractNode methodBody)
        {
            this.Modifiers = modifiers as Modifiers;
            this.ReturnTypeNode = returnTypeSpecifier as TypeSpecifier;
            this.Name = (methodDeclarator as MethodDeclarator)?.Identifier?.Name;
            this.ParameterList = (methodDeclarator as MethodDeclarator)?.ParameterList;
            this.MethodBody = methodBody as Block;

            AddChild(modifiers);
            AddChild(returnTypeSpecifier);
            if (this.ParameterList != null) AddChild(this.ParameterList);
            AddChild(methodBody);
        }

        // INTERFACE IMPLEMENTATIONS
        // ------------------------------------------------------------
        public NodeTypeCategory NodeTypeCategory
        {
            get => NodeTypeCategory.ClassMethodDef;
            set => throw new AccessViolationException("unable to set method node to diff type.");
        }
        public ITypeSpecifier TypeSpecifierRef { get; set; }
        public AccessorType AccessorType { get; set; }
        public bool IsStatic { get; set; }
        public ITypeSpecifier ReturnTypeNode { get; set; }
        public string Name { get; set; }
        public ISymbolTable<ITypeSpecifier> NameEnv { get; set; } = null;
        public ISymbolTable<ITypeSpecifier> TypeEnv { get; set; } = null;
    }

    public class Parameter : AbstractNode, INamedType
    {
        public TypeSpecifier TypeSpecifier { get; set; }
        private Identifier Identifier { get; set; }
        public Parameter(AbstractNode typeSpec, AbstractNode declName)
        {
            TypeSpecifier = typeSpec as TypeSpecifier;
            Identifier = declName as Identifier;
            AddChild(typeSpec);
            Name = Identifier.Name;
        }

        public string Name { get; set; }
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







// DELETE

//ITypeSpecifier typeNameDecl = vld.FieldTypeSpecifier;
//if (typeNameDecl == null) throw new NullReferenceException("Declared class field is not ITypeInfo.");

//// lookup the types
//if (typeNameDecl.TypeSpecifierRef == null)
//{
//    TypeVisitor tVisitor = new TypeVisitor(TypeEnv);
//    tVisitor.Visit(vld.FieldTypeSpecifier);
//}


//    FieldVarDecl fieldVarDecl = decl as FieldVarDecl;
//    if (fieldVarDecl == null) throw new ArgumentException("Variable being declared is not an identifier.");

//    string name = fieldVarDecl.Name;

//    ProcessModifierTokens(fieldVarDecl, modifierTokens, name);

//    // copy the link to the proper type
//    fieldVarDecl.TypeSpecifierRef = typeNameDecl.TypeSpecifierRef;

//    CheckEnterClassMemberDef(fieldVarDecl, classNameEnv);