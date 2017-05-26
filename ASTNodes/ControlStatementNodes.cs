using System;

namespace Proj3Semantics.ASTNodes
{
    public class SelectionStatement : Statement { }

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
        public Expression Predicate { get; set; }
        public Statement ThenStatement { get; set; }
        public IfStatement(AbstractNode predicate, AbstractNode thenExpr)
        {
            Predicate = predicate as Expression;

            ThenStatement = thenExpr as Statement;

            AddChild(predicate);
            if (ThenStatement == null)
                throw new NullReferenceException(nameof(ThenStatement));
            else
                AddChild(new Then(thenExpr));
        }
    }

    public class IfStatementElse : SelectionStatement
    {
        public Expression Predicate { get; set; }
        public Statement ThenStatement { get; set; }
        public Statement ElseStatement { get; set; }

        public IfStatementElse(
            AbstractNode predicate,
            AbstractNode thenExpr,
            AbstractNode elseExpr)
        {
            Predicate = predicate as Expression;

            ThenStatement = thenExpr as Statement;
            if (ThenStatement == null) throw new NullReferenceException(nameof(ThenStatement));

            ElseStatement = elseExpr as Statement;
            if (ElseStatement == null) throw new NullReferenceException(nameof(ElseStatement));

            AddChild(predicate);
            AddChild(new Then(thenExpr));
            AddChild(new Else(elseExpr));
        }
    }
    public class WhileLoop : AbstractNode
    {
        public Expression Predicate { get; set; }
        public Statement BodyStatement { get; set; }

        public WhileLoop(AbstractNode predicateNode, AbstractNode bodyStatement)
        {
            Predicate = predicateNode as Expression;
            BodyStatement = bodyStatement as Statement;
            if (BodyStatement == null) throw new NullReferenceException(nameof(BodyStatement));

            AddChild(Predicate);
            AddChild(BodyStatement);
        }
    }

}
