using System;

namespace CompilerILGen.AST
{
    public class SelectionStatement : Statement { }

    public class Then : Node
    {
        public Then(Node node)
        {
            AddChild(node);
        }
    }

    public class Else : Node
    {
        public Else(Node node)
        {
            AddChild(node);
        }
    }

    public class IfStatement : SelectionStatement
    {
        public ExprNode Predicate { get; set; }
        public Statement ThenStatement { get; set; }
        public IfStatement(Node predicate, Node thenExpr)
        {
            Predicate = predicate as ExprNode;

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
        public ExprNode Predicate { get; set; }
        public Statement ThenStatement { get; set; }
        public Statement ElseStatement { get; set; }

        public IfStatementElse(
            Node predicate,
            Node thenExpr,
            Node elseExpr)
        {
            Predicate = predicate as ExprNode;

            ThenStatement = thenExpr as Statement;
            if (ThenStatement == null) throw new NullReferenceException(nameof(ThenStatement));

            ElseStatement = elseExpr as Statement;
            if (ElseStatement == null) throw new NullReferenceException(nameof(ElseStatement));

            AddChild(predicate);
            AddChild(new Then(thenExpr));
            AddChild(new Else(elseExpr));
        }
    }
    public class WhileLoop : Node
    {
        public ExprNode Predicate { get; set; }
        public Statement BodyStatement { get; set; }

        public WhileLoop(Node predicateNode, Node bodyStatement)
        {
            Predicate = predicateNode as ExprNode;
            BodyStatement = bodyStatement as Statement;
            if (BodyStatement == null) throw new NullReferenceException(nameof(BodyStatement));

            AddChild(Predicate);
            AddChild(BodyStatement);
        }
    }

}
