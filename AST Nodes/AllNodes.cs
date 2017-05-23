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
    using static Token;

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
        ASTERISK, RSLASH, PERCENT, UNARY, @EVAL
    }
    public class Expression : ExpressionStatement
    {
        public ExprType ExprType { get; set; }
        public Expression(AbstractNode expr, ExprType type)
        {
            AddChild(expr);
            ExprType = type;
        }
        public Expression(AbstractNode lhs, ExprType type, AbstractNode rhs)
        {
            AddChild(lhs);
            AddChild(rhs);
            ExprType = type;
        }

    }
    public class PrimaryExpression : AbstractNode { }

    public class NotJustName : PrimaryExpression { }


    public class ComplexPrimary : NotJustName { }

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


    public class NotImplemented : AbstractNode
    {
        public string Msg { get; set; }
        public NotImplemented(string msg)
        {
            Msg = msg;
        }
    }

}
