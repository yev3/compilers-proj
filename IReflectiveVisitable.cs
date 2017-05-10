namespace ASTBuilder
{
	/// <summary>
	/// Reflective visitor pattern -- a node must accept a visitor </summary>
	public interface IReflectiveVisitable
	{
	   void Accept(INodeVisitor rv);
	}

    public interface INodeVisitor
    {
        //void Visit(CompilationUnit node);
        void Visit(AbstractNode node);
        void Visit(Modifiers node);
        void Visit(Identifier node);
        void Visit(PrimitiveType node);
        void Visit(Expression node);
    }

}