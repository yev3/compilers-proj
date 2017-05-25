using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics.AST_Nodes
{
    //public enum EvalExprType { QualifiedName, Builtin, Expression, FieldAccess, MethodCall }
    public enum EvaluationNodeTypes { QualifiedName, QualifiedPrimaryExpr }
    public class EvalExpr : Expression
    {
        public override ExprType ExprType
        {
            get => ExprType.EVALUATION;
            set => throw new AccessViolationException();
        }

        public EvaluationNodeTypes NodeType { get; set; }
        public QualifiedName QualifiedName { get; set; } = null;
        public QualifiedPrimaryExpr QualifiedPrimaryExpr { get; set; } = null;

        public EvalExpr(EvaluationNodeTypes nodeType, AbstractNode node)
        {
            NodeType = nodeType;
            AddChild(node);
            switch (nodeType)
            {
                case EvaluationNodeTypes.QualifiedName:
                    QualifiedName = node as QualifiedName;
                    Debug.Assert(QualifiedName != null);
                    break;
                case EvaluationNodeTypes.QualifiedPrimaryExpr:
                    QualifiedPrimaryExpr = node as QualifiedPrimaryExpr;
                    Debug.Assert(QualifiedPrimaryExpr != null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null);
            }

        }

    }
}
