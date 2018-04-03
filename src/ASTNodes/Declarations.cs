// Declaration nodes

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CompilerILGen.AST
{
    using IEnv = ISymbolTable<Symbol>;

    public abstract class DeclNode : Node, INamedType
    {
        public string Name { get; set; }
        public Identifier Identifier { get; set; }
        public int IlLocalsPosn { get; set; }

        /// <summary>
        /// Each declaration should have a type associated with it
        /// </summary>
        public virtual TypeRefNode DeclTypeNode
        {
            get { return TypeRefNode.TypeNodeDeclaration; }
            set { throw new InvalidOperationException(); }
        }

        public DeclNode(Identifier identifier)
        {
            Identifier = identifier;
            Name = identifier.Name;
        }

    }

    public class DeclarationList : Node
    {
        public DeclarationList(Node node)
        {
            DeclNode decl = node as DeclNode;
            if (decl == null) throw new ArgumentNullException(nameof(decl));
            AddChild(decl);
        }
    }

    public abstract class AbstractFuncDecl : DeclNode
    {
        public abstract IEnv Env { get; set; }
        public abstract TypeRefNode ReturnTypeSpecifier { get; }
        public abstract ParamList ParamList { get; set; }
        public abstract Block MethodBody { get; set; }
        protected AbstractFuncDecl(Identifier identifier) : base(identifier) { }
    }

    public class FuncDecl : AbstractFuncDecl
    {
        public override IEnv Env { get; set; }
        public override TypeRefNode ReturnTypeSpecifier { get; }
        public override ParamList ParamList { get; set; }
        public override Block MethodBody { get; set; }

        public FuncDecl(FuncDecl fdecl) : base(fdecl.Identifier)
        {
            this.ReturnTypeSpecifier = fdecl.ReturnTypeSpecifier;
            this.ParamList = fdecl.ParamList;
            this.MethodBody = fdecl.MethodBody;
            PopulateChildren();
        }
        public FuncDecl(
            TypeRefNode returnTypeSpecifier,
            Identifier name,
            ParamList paramList,
            Block methodBody)
            : base(name)
        {
            ReturnTypeSpecifier = returnTypeSpecifier;
            ParamList = paramList;
            MethodBody = methodBody;
            PopulateChildren();
        }

        public void PopulateChildren()
        {
            AddChild(ReturnTypeSpecifier);
            if (ParamList != null) AddChild(ParamList);
            AddChild(MethodBody);
        }
    }


    #region ClassDecl
    public enum ModifierType { PUBLIC, STATIC, PRIVATE }
    public enum AccessorType { Public, Private }


    public class Modifiers : Node
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

        public void ProcessModifierTokensFor(IClassMember node)
        {
            bool accessorWasSet = false;
            node.IsStatic = false;
            node.AccessorType = AccessorType.Private;
            foreach (ModifierType modifier in ModifierTokens)
            {
                if (modifier == ModifierType.PUBLIC || modifier == ModifierType.PRIVATE)
                {
                    if (accessorWasSet)
                    {
                        CompilerErrors.Add(SemanticErrorTypes.InconsistentModifiers, node.Name);
                        break;
                    }
                    accessorWasSet = true;
                    switch (modifier)
                    {
                        case ModifierType.PUBLIC:
                            node.AccessorType = AccessorType.Public;
                            break;
                        case ModifierType.PRIVATE:
                            node.AccessorType = AccessorType.Private;
                            break;
                        default:
                            throw new ArgumentException();
                    }
                }
                else if (modifier == ModifierType.STATIC)
                {
                    node.IsStatic = true;
                }
                else
                {
                    throw new NotImplementedException("Unrecognized modifier type encountered.");
                }
            }
        }
    }

    public class ClassDeclaration : DeclNode, IClassMember
    {
        public IEnv Env { get; set; } = null;
        public AccessorType AccessorType { get; set; }
        public bool IsStatic { get; set; }
        public ClassDeclaration ParentClass { get; set; }
        public ClassBody ClassBody { get; set; }
        public ClassDeclaration(
            Modifiers modifiers,
            Identifier identifier,
            ClassBody classBody) : base(identifier)
        {
            // fill in modifier fields for the interface ITypeHasModifiers
            modifiers.ProcessModifierTokensFor(this);
            ClassBody = classBody;
            foreach (Node n in classBody.Children)
            {
                IClassMember cm = n as IClassMember;
                if (cm != null) cm.ParentClass = this;
            }

            AddChild(modifiers);
            AddChild(ClassBody);
        }

    }

    public class ClassMethodDecl : FuncDecl, IClassMember
    {
        public AccessorType AccessorType { get; set; }
        public bool IsStatic { get; set; }
        public ClassDeclaration ParentClass { get; set; }

        public ClassMethodDecl(Modifiers modifiers, FuncDecl funcDecl) : base(funcDecl)
        {
            modifiers.ProcessModifierTokensFor(this);
        }

        // Method decl is private non-static by default
        public ClassMethodDecl(FuncDecl funcDecl) : base(funcDecl)
        {
            AccessorType = AccessorType.Private;
            IsStatic = false;
        }
    }




    public class FieldVarDecl : Node, IClassMember
    {
        public FieldVarDecl(Node identifier)
        {
            Name = (identifier as Identifier)?.Name;
            if (Name == null) throw new ArgumentException("Field variable decl without an identifier.");
        }

        public NodeTypeCategory NodeTypeCategory
        {
            get { return NodeTypeCategory.ClassFieldDef; }
            set
            {
                throw new AccessViolationException(
                    "unable to set class field type");
            }
        }

        public AccessorType AccessorType { get; set; }
        public bool IsStatic { get; set; }
        public ClassDeclaration ParentClass { get; set; }
        public string Name { get; set; }
    }

    public class ClassFieldDeclStatement : Node
    {
        public Modifiers Modifiers { get; set; }
        public LocalVarDecl VarDeclList { get; set; }

        public ClassFieldDeclStatement(
            Node modifiers,
            LocalVarDecl varDeclList)
        {
            Modifiers = modifiers as Modifiers;
            VarDeclList = varDeclList;

            AddChild(modifiers);
            AddChild(VarDeclList);
        }

        public AccessorType AccessorType { get; set; }
        public bool IsStatic { get; set; }
    }

    public class ClassBody : Node
    {
        public ClassBody()
        {
            //Console.WriteLine("Class body is empty!");
        }
        public ClassBody(Node c)
        {
            AddChild(c);
        }
    }

    #endregion

    public class ParamList : Node, IEquatable<ParamList>
    {
        public List<ParamDecl> ParamDeclList { get; } = new List<ParamDecl>();
        public ParamList(IEnumerable<ParamDecl> declList)
        {
            ParamDeclList.AddRange(declList);
        }
        public ParamList(Node parameter)
        {
            AddParameter(parameter);
            base.AddChild(parameter);
        }

        private void AddParameter(Node node)
        {
            ParamDecl p = node as ParamDecl;
            if (p == null) throw new ArgumentNullException(nameof(p));
            ParamDeclList.Add(p);
        }

        public override void AddChild(Node child)
        {
            AddParameter(child);
            base.AddChild(child);
        }

        public bool Equals(ParamList other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ParamDeclList.SequenceEqual(other.ParamDeclList);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ParamList)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19;
                foreach (var item in ParamDeclList)
                {
                    hash = hash * 31 + item.GetHashCode();
                }
                return hash;
            }
        }

        public static bool operator ==(ParamList left, ParamList right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ParamList left, ParamList right)
        {
            return !Equals(left, right);
        }
    }



    public class ParamDecl : DeclNode, IEquatable<ParamDecl>
    {
        public override TypeRefNode DeclTypeNode { get; set; }
        public ParamDecl(TypeRefNode boundType, Identifier name) : base(name)
        {
            DeclTypeNode = boundType;
            AddChild(boundType);
        }

        public bool Equals(ParamDecl other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            // only comparing the types for equality
            return Equals(DeclTypeNode, other.DeclTypeNode);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ParamDecl)obj);
        }

        public override int GetHashCode()
        {
            return DeclTypeNode.GetHashCode();
        }

        public static bool operator ==(ParamDecl left, ParamDecl right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ParamDecl left, ParamDecl right)
        {
            return !Equals(left, right);
        }
    }


    public class LocalVarDecl : Node
    {
        public TypeRefNode TypeSpecifier { get; set; }
        public VarDeclList VarDeclList { get; set; }
        public ExprNode Initialization { get; set; }
        public LocalVarDecl(
            TypeRefNode typeSpecifier,
            VarDeclList varDeclList,
            Node init = null)
        {

            Debug.Assert(typeSpecifier != null);
            TypeSpecifier = typeSpecifier;

            VarDeclList = varDeclList;
            Debug.Assert(varDeclList != null);

            Initialization = init as ExprNode;

            // adding children for printing
            AddChild(typeSpecifier);
            AddChild(varDeclList);
            if (init != null) AddChild(init);
        }
    }

    public sealed class VarDeclList : Node
    {
        public VarDeclList(VarDecl node)
        {
            AddChild(node);
        }
    }

    public class VarDecl : DeclNode
    {
        public VarDecl(Identifier id) : base(id) { }
        public override TypeRefNode DeclTypeNode { get; set; } = null;
        public override string ToString()
        {
            return this.DeclTypeNode.ToString();
        }
    }

}
