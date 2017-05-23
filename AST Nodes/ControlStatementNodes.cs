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
    public class SelectionStatement : AbstractNode { }

    public class Then : AbstractNode
    {
        public Then(AbstractNode node)
        {
            AddChild(node);
        }
    }

    public class Else : AbstractNode
    {
        public Else(AbstractNode node)
        {
            AddChild(node);
        }
    }

    public class IfStatement : SelectionStatement
    {
        public IfStatement(AbstractNode predicate, AbstractNode thenExpr)
        {
            AddChild(predicate);
            AddChild(new Then(thenExpr));
        }
    }

    public class IfStatementElse : SelectionStatement
    {
        public IfStatementElse(AbstractNode predicate, AbstractNode thenExpr, AbstractNode elseExpr)
        {
            AddChild(predicate);
            AddChild(new Then(thenExpr));
            AddChild(new Else(elseExpr));
        }
    }

}
