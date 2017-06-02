using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace Proj3Semantics.AST
{
    public enum NodeTypeCategory
    {
        Int, String, Object, Null,
        Boolean, Class, Void,
        This, ErrorType, ClassFieldDef,
        ClassMethodDef, NamespaceDecl,
        Declaration, Unknown
    }

    public abstract class TypeNode : Node, IEquatable<TypeNode>
    {
        public static TypeNode TypeNodeInt { get; } = new BuiltinType(NodeTypeCategory.Int);
        public static TypeNode TypeNodeString { get; } = new BuiltinType(NodeTypeCategory.String);
        public static TypeNode TypeNodeObject { get; } = new BuiltinType(NodeTypeCategory.Object);
        public static TypeNode TypeNodeNull { get; } = new BuiltinType(NodeTypeCategory.Null);
        public static TypeNode TypeNodeBoolean { get; } = new BuiltinType(NodeTypeCategory.Boolean);
        public static TypeNode TypeNodeVoid { get; } = new BuiltinType(NodeTypeCategory.Void);
        public static TypeNode TypeNodeThis { get; } = new BuiltinType(NodeTypeCategory.This);
        // Generic declaration node
        public static TypeNode TypeNodeDeclaration { get; } = new BuiltinType(NodeTypeCategory.Declaration);
        public static TypeNode TypeNodeError { get; } = new BuiltinType(NodeTypeCategory.ErrorType);
        public NodeTypeCategory NodeTypeCategory { get; set; }

        public abstract bool Equals(TypeNode other);


        public abstract override bool Equals(object obj);

        public abstract override int GetHashCode();

        public static bool operator ==(TypeNode left, TypeNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TypeNode left, TypeNode right)
        {
            return !Equals(left, right);
        }

        public bool CanConvertTo(TypeNode other)
        {
            return other.Equals(this);
        }
    }

    public class BuiltinType : TypeNode
    {
        public BuiltinType(NodeTypeCategory type)
        {
            NodeTypeCategory = type;
        }

        public override bool Equals(TypeNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return NodeTypeCategory == other.NodeTypeCategory;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeNode)obj);
        }

        public override int GetHashCode()
        {
            return (int)NodeTypeCategory;
        }

        public override string ToString()
        {
            return NodeTypeCategory.ToString() + "/builtin";
        }
    }

    public class QualifiedNameNode : TypeNode
    {
        public List<string> IdentifierList { get; set; } = new List<string>();
        public Symbol SymbolRef { get; set; } = null;
        public QualifiedNameNode(Identifier id)
        {
            NodeTypeCategory = NodeTypeCategory.Unknown;
            AddChild(id);
        }

        public void AddChild(Identifier child)
        {
            Identifier id = child as Identifier;
            if (id == null) throw new ArgumentNullException(nameof(id));
            IdentifierList.Add(id.Name);
        }

        public override bool Equals(TypeNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            QualifiedNameNode otherQ = other as QualifiedNameNode;
            if (otherQ == null) return false;
            return IdentifierList.SequenceEqual(otherQ.IdentifierList);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeNode)obj);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            string details = SymbolRef?.DeclNode?.ToString() ?? "null";
            return string.Join(".", IdentifierList) + "/" + details;
        }
    }


    [DebuggerDisplay("Identifier: '{Name}'")]
    public class Identifier : Node
    {
        public string Name { get; set; }
        public Identifier(string s)
        {
            Name = s;
        }
    }
}
