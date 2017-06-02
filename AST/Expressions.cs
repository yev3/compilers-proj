using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics.AST
{
    public enum ExprType
    {
        ASSIGNMENT, LOGICAL_OR, LOGICAL_AND, PIPE, HAT, AND, EQUALS,
        NOT_EQUALS, GREATER_THAN, LESS_THAN, LESS_EQUAL, GREATER_EQUAL, PLUSOP, MINUSOP,
        ASTERISK, RSLASH, PERCENT, UNARY, EVALUATION
    }

    public abstract class ExprNode : ExpressionStatement
    {
        public abstract ExprType ExprType { get; set; }
        public TypeNode EvalType { get; set; }
    }

    public class AssignExpr : ExprNode
    {
        public override ExprType ExprType
        {
            get { return ExprType.ASSIGNMENT; }
            set { throw new AccessViolationException(); }
        }

        public QualifiedNode LhsQual { get; set; }
        public ExprNode RhsExprNode { get; set; }

        public AssignExpr(Node lhs, Node rhs)
        {
            LhsQual = lhs as QualifiedNode;
            RhsExprNode = rhs as ExprNode;
            Debug.Assert(LhsQual != null);
            Debug.Assert(RhsExprNode != null);
            AddChild(LhsQual);
            AddChild(RhsExprNode);
        }
    }

    public abstract class LiteralExpr : ExprNode
    {
        public override ExprType ExprType
        {
            get { return ExprType.EVALUATION; }
            set { throw new AccessViolationException(); }
        }
    }

    public class StringLiteralExpr : LiteralExpr
    {
        public string StringVal { get; set; }
        public StringLiteralExpr(string stringVal)
        {
            StringVal = stringVal;
            EvalType = TypeNode.TypeNodeString;
        }


    }
    public class IntLiteralExpr : LiteralExpr
    {

        public int IntegerValue { get; set; }

        public IntLiteralExpr(int integerValue)
        {
            IntegerValue = integerValue;
            EvalType = TypeNode.TypeNodeInt;
        }
    }

    public class EvalExpr : ExprNode
    {
        public override ExprType ExprType
        {
            get { return ExprType.EVALUATION; }
            set { throw new AccessViolationException(); }
        }

        public Node Child { get; set; }

        public EvalExpr(Node node)
        {
            Child = node;
            Debug.Assert(Child != null);
            AddChild(node);

        }

    }
}
