using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics.ASTNodes
{
    public class EvalExpr : Expression
    {
        public override ExprType ExprType
        {
            get => ExprType.EVALUATION;
            set => throw new AccessViolationException();
        }

        public AbstractNode Child { get; set; }

        public EvalExpr(AbstractNode node)
        {
            Child = node;
            Debug.Assert(Child != null);
            AddChild(node);

        }

    }
}
