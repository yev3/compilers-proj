using System;
using System.Collections.Generic;
using System.Diagnostics;
using QUT.Gppg;

namespace Proj3Semantics.AST
{
    public class CompilationUnit : Node
    {
        public LexLocation Location { get; }

        public CompilationUnit(Node classDecl)
        {
            AddChild(classDecl);
        }

        public CompilationUnit(LexLocation lexLocation, Node classDecl)
        {
            this.Location = lexLocation;
            AddChild(classDecl);
        }
    }



    public class Block : Statement
    {
        public Block() { }
        public Block(Node child)
        {
            AddChild(child);
        }
    }

    public class LocalVarDeclOrStatement : Node { }

    public class Statement : LocalVarDeclOrStatement { }

    public class ExpressionStatement : Statement { }

    public class EmptyStatement : Statement { }


    public class BinaryExpr : ExprNode
    {
        public sealed override ExprType ExprType { get; set; }
        public ExprNode LhsExprNode { get; set; }
        public ExprNode RhsExprNode { get; set; }
        public BinaryExpr(Node lhs, ExprType exprType, Node rhs)
        {
            LhsExprNode = lhs as ExprNode;
            RhsExprNode = rhs as ExprNode;
            Debug.Assert(LhsExprNode != null);
            Debug.Assert(RhsExprNode != null);
            ExprType = exprType;

            AddChild(lhs);
            AddChild(rhs);
        }
    }

    public class CompExpr : BinaryExpr {
        public CompExpr(Node lhs, ExprType exprType, Node rhs) : base(lhs, exprType, rhs)
        {
        }
    }




    public class MethodCall : EvalExpr
    {
        public Node MethodReference { get; set; }
        public ArgumentList ArgumentList { get; set; } = null;
        public bool HasArguments => ArgumentList == null;

        public MethodCall(Node methodReference)  
        {
            MethodReference = methodReference;
            AddChild(MethodReference);
        }

        public MethodCall(Node methodRef, Node argList)
        {
            MethodReference = methodRef;
            ArgumentList = argList as ArgumentList;
            if (ArgumentList == null) throw new NullReferenceException(typeof(ArgumentList).ToString());
            AddChild(MethodReference);
            AddChild(ArgumentList);
        }

    }

    public class ReturnStatement : Statement
    {
        public ReturnStatement() { }

        public ReturnStatement(Node node)
        {
            AddChild(node);
        }
    }

    public class StaticInitializer : Node
    {
        public StaticInitializer(Node node)
        {
            AddChild(node);
        }
    }


    public class ArgumentList : Node
    {
        public ArgumentList(ExprNode node)
        {
            AddChild(node);
        }

    }

    public class NamespaceBody : Node
    {
        public NamespaceBody() { }

        public NamespaceBody(Node singleItem)
        {
            AddChild(singleItem);
        }
    }

    public class NamespaceDecl : Node
    {

        public ISymbolTable<Symbol> Env { get; set; } = null;
        public string Name { get; set; }
        public NamespaceBody NamespaceBody { get; set; }

        public NamespaceDecl(Node identifier, Node body)
        {
            Identifier id = identifier as Identifier;
            if (id == null) throw new ArgumentNullException(nameof(id));
            Name = id.Name;

            NamespaceBody = body as NamespaceBody;
            if (NamespaceBody == null) throw new ArgumentNullException(nameof(id));

            AddChild(NamespaceBody);
        }

        public NamespaceDecl(Node body)
        {
            Name = "";
            NamespaceBody = body as NamespaceBody;
            if (NamespaceBody == null) throw new ArgumentNullException();

            AddChild(NamespaceBody);
        }

        public NodeTypeCategory NodeTypeCategory { get; set; } = NodeTypeCategory.NamespaceDecl;
    }

    public class NotImplemented : Node
    {
        public string Msg { get; set; }
        public NotImplemented(string msg)
        {
            Msg = msg;
        }
    }

}
