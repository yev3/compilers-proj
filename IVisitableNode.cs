using System;

namespace ASTBuilder
{
	/// <summary> Reflective visitor pattern -- a node must accept a visitor </summary>
	public interface IVisitableNode
	{
	   void Accept(INodeReflectiveVisitor rv);
	}

    public interface INodeReflectiveVisitor
    {
        void VisitDispatch(Object node);
        
    }

}