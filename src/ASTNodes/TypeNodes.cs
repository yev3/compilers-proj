// Nodes that carry type information

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CompilerILGen.AST
{
    public enum NodeTypeCategory
    {
        Int,
        String,
        Object,
        Null,
        Boolean,
        Class,
        Void,
        This,
        ErrorType,
        ClassFieldDef,
        ClassMethodDef,
        NamespaceDecl,
        Declaration,
        Unknown
    }
    public static class Extensions
    {
        public static string GetIlName(this NodeTypeCategory cat)
        {
            switch (cat)
            {
                case NodeTypeCategory.Int:
                    return "int32";
                case NodeTypeCategory.String:
                    return "string";
                case NodeTypeCategory.Object:
                    return "object";
                case NodeTypeCategory.Boolean:
                    return "bool";
                case NodeTypeCategory.Void:
                    return "void";
                case NodeTypeCategory.This:
                    return "this";
                default:
                    throw new NotImplementedException("Unsupported ToString of NodeTypeCategory: " + cat.GetType().Name.ToString());
            }
        }
    }

    public abstract class TypeRefNode : Node, IEquatable<TypeRefNode>
    {
        public static TypeRefNode TypeNodeInt { get; } = new BuiltinType(NodeTypeCategory.Int);
        public static TypeRefNode TypeNodeString { get; } = new BuiltinType(NodeTypeCategory.String);
        public static TypeRefNode TypeNodeObject { get; } = new BuiltinType(NodeTypeCategory.Object);
        public static TypeRefNode TypeNodeNull { get; } = new BuiltinType(NodeTypeCategory.Null);
        public static TypeRefNode TypeNodeBoolean { get; } = new BuiltinType(NodeTypeCategory.Boolean);
        public static TypeRefNode TypeNodeVoid { get; } = new BuiltinType(NodeTypeCategory.Void);

        public static TypeRefNode TypeNodeThis { get; } = new BuiltinType(NodeTypeCategory.This);

        // Generic declaration node
        public static TypeRefNode TypeNodeDeclaration { get; } = new BuiltinType(NodeTypeCategory.Declaration);
        public static TypeRefNode TypeNodeError { get; } = new BuiltinType(NodeTypeCategory.ErrorType);
        public NodeTypeCategory NodeTypeCategory { get; set; } = NodeTypeCategory.Unknown;

        public abstract bool Equals(TypeRefNode other);


        public abstract override bool Equals(object obj);

        public abstract override int GetHashCode();

        public static bool operator ==(TypeRefNode left, TypeRefNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TypeRefNode left, TypeRefNode right)
        {
            return !Equals(left, right);
        }

        public bool CanConvertTo(TypeRefNode other)
        {
            return other.Equals(this);
        }
    }

    public class BuiltinType : TypeRefNode
    {
        public BuiltinType(NodeTypeCategory type)
        {
            NodeTypeCategory = type;
        }

        public override bool Equals(TypeRefNode other)
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
            return Equals((TypeRefNode) obj);
        }

        public override int GetHashCode()
        {
            return (int) NodeTypeCategory;
        }

        public override string ToString()
        {
            return NodeTypeCategory.ToString() + "/builtin";
        }
    }

    public class QualifiedType : TypeRefNode
    {
        public List<string> IdentifierList { get; set; } = new List<string>();
        public Symbol SymbolRef { get; set; } = null;

        public QualifiedType(Identifier id)
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

        public override bool Equals(TypeRefNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            QualifiedType otherQ = other as QualifiedType;
            if (otherQ == null) return false;
            return SymbolRef == otherQ.SymbolRef;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((QualifiedType) obj);
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