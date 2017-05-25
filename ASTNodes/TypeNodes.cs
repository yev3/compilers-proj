﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Proj3Semantics.ASTNodes
{
    // TYPE DECLARATIONS

    public class VariableListDeclaring : AbstractNode
    {
        public TypeSpecifier FieldTypeSpecifier { get; set; }
        public DeclaredVars ItemIdList { get; set; }
        public Expression Initialization { get; set; }
        public VariableListDeclaring(
            AbstractNode typeSpecifier,
            AbstractNode itemIdList,
            AbstractNode init = null)
        {
            // adding children for printing
            AddChild(typeSpecifier);
            AddChild(itemIdList);
            if (init != null) AddChild(init);

            // check that the parser assigned some type of a node with type info
            var decl = typeSpecifier as ITypeSpecifier;
            Debug.Assert(decl != null);
            FieldTypeSpecifier = typeSpecifier as TypeSpecifier;

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




    public abstract class TypeSpecifier : AbstractNode, ITypeSpecifier
    {
        public abstract NodeTypeCategory NodeTypeCategory { get; set; }
        public virtual ITypeSpecifier TypeSpecifierRef { get; set; } = null;
        public ISymbolTable<ITypeSpecifier> TypeEnv { get; set; } = null;
        public ISymbolTable<ITypeSpecifier> NameEnv { get; set; } = null;
    }

    public abstract class TypeName : TypeSpecifier { }

    public class ArraySpecifier : TypeSpecifier
    {
        public ArraySpecifier(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }

        public override NodeTypeCategory NodeTypeCategory
        {
            get { return NodeTypeCategory.Array; }
            set
            {
                throw new InvalidOperationException(
                    "unable to set node type cat");
            }
        }
    }

    public class QualifiedName : TypeName
    {
        public List<string> IdentifierList { get; set; } = new List<string>();

        public QualifiedName(AbstractNode node)
        {
            AppendIdentifier(node);
        }

        private void AppendIdentifier(AbstractNode child)
        {
            Identifier id = child as Identifier;
            if (id == null) throw new ArgumentNullException(nameof(id));
            IdentifierList.Add(id.Name);

        }

        public override void AddChild(AbstractNode child)
        {
            AppendIdentifier(child);
        }

        public override NodeTypeCategory NodeTypeCategory { get; set; } = NodeTypeCategory.NOT_SET;
        public override ITypeSpecifier TypeSpecifierRef { get; set; }
    }


    /// <summary>
    /// (Page 303)
    /// </summary>
    public class Identifier : TypeName
    {
        public string Name { get; set; }
        public Identifier(string s)
        {
            Name = s;
        }

        public override NodeTypeCategory NodeTypeCategory { get; set; } = NodeTypeCategory.NOT_SET;
    }


    public class StringLiteral : AbstractNode, IPrimitiveTypeDescriptor
    {
        public string Name { get; set; }
        public StringLiteral(string s)
        {
            Name = s;
        }

        public NodeTypeCategory NodeTypeCategory
        {
            get { return NodeTypeCategory.Primitive; }
            set
            {
                throw new NotImplementedException(
                    "You're not supposed to set a literal");
            }
        }


        public virtual ITypeSpecifier TypeSpecifierRef
        {
            get { return this; }
            set
            {
                throw new AccessViolationException(
                    "unable to set typeref of a string literal");
            }
        }


        public VariablePrimitiveTypes VariableTypePrimitive
        {
            get { return VariablePrimitiveTypes.String; }
            set
            {
                throw new NotImplementedException(
                    "You're not supposed to set a number literal");
            }
        }
    }



}
