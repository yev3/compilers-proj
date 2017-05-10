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
        void Visit(CompilationUnit node);
        void Visit(ClassDeclaration node);
        void Visit(Modifiers node);
        void Visit(Identifier node);
        void Visit(ClassBody node);
        void Visit(FieldDeclarations node);
        void Visit(PrimitiveType node);
        void Visit(TypeName node);
        void Visit(TypeSpecifier node);
        void Visit(MethodDeclaration node);
        void Visit(LocalVariableDeclarationsAndStatements node);
        void Visit(Block node);
        void Visit(MethodBody node);
        void Visit(MethodDeclaratorName node);
        void Visit(DeclaratorName node);
        void Visit(Parameter node);
        void Visit(ParameterList node);
        void Visit(MethodDeclarator node);
        void Visit(FieldDeclaration node);
    }

}