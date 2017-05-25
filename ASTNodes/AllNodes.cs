using System;
using System.Diagnostics;

namespace Proj3Semantics.ASTNodes
{
    public class CompilationUnit : AbstractNode
    {
        // just for the compilation unit because it's the top node
        //public override AbstractNode LeftMostSibling => this;
        public override AbstractNode NextSibling => null;

        public CompilationUnit(AbstractNode classDecl)
        {
            AddChild(classDecl);
        }

    }



    public class Block : Statement
    {
        public Block() { }
        public Block(AbstractNode child)
        {
            AddChild(child);
        }
    }



    public class Statement : LocalVarDeclOrStatement { }

    public class ExpressionStatement : Statement { }

    public class EmptyStatement : Statement { }

    public enum ExprType
    {
        ASSIGNMENT, LOGICAL_OR, LOGICAL_AND, PIPE, HAT, AND, EQUALS,
        NOT_EQUALS, GREATER_THAN, LESS_THAN, LESS_EQUAL, GREATER_EQUAL, PLUSOP, MINUSOP,
        ASTERISK, RSLASH, PERCENT, UNARY, EVALUATION
    }
    public abstract class Expression : ExpressionStatement, ITypeSpecifier
    {
        public abstract ExprType ExprType { get; set; }

        public NodeTypeCategory NodeTypeCategory { get; set; }
        public ITypeSpecifier TypeSpecifierRef { get; set; }
    }

    public class BinaryExpr : Expression
    {
        public sealed override ExprType ExprType { get; set; }
        public Expression LhsExpression { get; set; }
        public Expression RhsExpression { get; set; }
        public BinaryExpr(AbstractNode lhs, ExprType exprType, AbstractNode rhs)
        {
            LhsExpression = lhs as Expression;
            RhsExpression = rhs as Expression;
            Debug.Assert(LhsExpression != null);
            Debug.Assert(RhsExpression != null);
            ExprType = exprType;
        }
    }

    public class AssignExpr : Expression
    {
        public override ExprType ExprType
        {
            get { return ExprType.ASSIGNMENT; }
            set { throw new AccessViolationException(); }
        }

        public QualifiedName LhsQualName { get; set; }
        public Expression RhsExpression { get; set; }

        public AssignExpr(AbstractNode lhs, AbstractNode rhs)
        {
            LhsQualName = lhs as QualifiedName;
            RhsExpression = rhs as Expression;
            Debug.Assert(LhsQualName != null);
            Debug.Assert(RhsExpression != null);
        }
    }




    public class QualifiedPrimaryExpr : AbstractNode { }

    public class ComplexPrimary : QualifiedPrimaryExpr { }

    public class MethodCall : ComplexPrimary
    {

        public MethodCall(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }

        public MethodCall(AbstractNode methodRef, AbstractNode argList)
        {
            AddChild(methodRef);
            AddChild(argList);
        }
    }



    public class ReturnStatement : Statement
    {
        public ReturnStatement() { }

        public ReturnStatement(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }

    public class StaticInitializer : AbstractNode
    {
        public StaticInitializer(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }


    public class ArgumentList : AbstractNode
    {
        public ArgumentList(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }

    public class NamespaceBody : AbstractNode
    {
        public NamespaceBody() { }

        public NamespaceBody(AbstractNode singleItem)
        {
            AddChild(singleItem);
        }
    }

    public class NamespaceDecl : AbstractNode, ITypeSpecifier, IHasOwnScope, INamedType
    {
        public ISymbolTable<ITypeSpecifier> NameEnv { get; set; } = null;
        public ISymbolTable<ITypeSpecifier> TypeEnv { get; set; } = null;
        public string Name { get; set; }
        public NamespaceBody NamespaceBody { get; set; }

        public NamespaceDecl(AbstractNode identifier, AbstractNode body)
        {
            Identifier id = identifier as Identifier;
            if (id == null) throw new ArgumentNullException(nameof(id));
            Name = id.Name;

            NamespaceBody = body as NamespaceBody;
            if (NamespaceBody == null) throw new ArgumentNullException(nameof(id));

            AddChild(NamespaceBody);
        }

        public NodeTypeCategory NodeTypeCategory { get; set; } = NodeTypeCategory.NamespaceDecl;
        public ITypeSpecifier TypeSpecifierRef
        {
            get { return this; }
            set { throw new AccessViolationException(); }
        }
    }

    public class NotImplemented : AbstractNode
    {
        public string Msg { get; set; }
        public NotImplemented(string msg)
        {
            Msg = msg;
        }
    }

}
