using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;

namespace Proj3Semantics.AST
{
    using IEnv = ISymbolTable<Symbol>;

    public class FieldVarDecl : Node, IClassFieldTypeDesc
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


    public class ParamList : Node, IEquatable<ParamList>
    {
        public List<ParamDecl> ParamDeclList { get; } = new List<ParamDecl>();
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