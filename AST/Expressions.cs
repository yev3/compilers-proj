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
        ASSIGNMENT, LOGICAL_OR, LOGICAL_AND, B_OR, B_XOR, B_AND, EQUALS,
        NOT_EQUALS, GREATER_THAN, LESS_THAN, LESS_EQUAL, GREATER_EQUAL, PLUSOP, MINUSOP,
        ASTERISK, RSLASH, PERCENT, UNARY, EVALUATION
    }

    public abstract class ExprNode : ExpressionStatement
    {
        public abstract ExprType ExprType { get; set; }
        public TypeRefNode EvalType { get; set; }
    }

    public class AssignExpr : ExprNode
    {
        public override ExprType ExprType
        {
            get { return ExprType.ASSIGNMENT; }
            set { throw new AccessViolationException(); }
        }

        public LValueNode LValueNode { get; set; }
        public ExprNode RhsExprNode { get; set; }

        public AssignExpr(Node lhs, Node rhs)
        {
            LValueNode = lhs as LValueNode;
            RhsExprNode = rhs as ExprNode;
            Debug.Assert(LValueNode != null);
            Debug.Assert(RhsExprNode != null);
            AddChild(LValueNode);
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
            EvalType = TypeRefNode.TypeNodeString;
        }


    }
    public class IntLiteralExpr : LiteralExpr
    {

        public int IntegerValue { get; set; }

        public IntLiteralExpr(int integerValue)
        {
            IntegerValue = integerValue;
            EvalType = TypeRefNode.TypeNodeInt;
        }
    }

    public class EvalExpr : ExprNode
    {
        public override ExprType ExprType
        {
            get { return ExprType.EVALUATION; }
            set { throw new AccessViolationException(); }
        }

        public ExprNode ChildExpr { get; set; }

        public EvalExpr(ExprNode node)
        {
            ChildExpr = node;
            Debug.Assert(ChildExpr != null);
            AddChild(node);

        }
        protected EvalExpr() { }
    }

    public class TypeExpr : ExprNode
    {
        public override ExprType ExprType { get; set; }
        public TypeRefNode TypeRefNode { get; set; }
        public TypeExpr(TypeRefNode typeRef)
        {
            TypeRefNode = typeRef;
        }
    }
    public class LValueNode : EvalExpr
    {
        //public Symbol SymbolRef { get; set; } = null;
        public ExprNode LeftOfPeriodExpr { get; set; }
        public Identifier Identifier { get; set; }
        public Symbol SymbolRef { get; set; }

        public LValueNode(Identifier id)
        {
            Identifier = id;
            AddChild(id);
        }

        public LValueNode(ExprNode lhs, Identifier id)
        {
            LeftOfPeriodExpr = lhs;
            Identifier = id;
            AddChild(lhs);
            AddChild(id);
        }

    }

}
