using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using Proj3Semantics.AST;

// found here:
// http://www.ccs.neu.edu/home/riccardo/courses/csu370-fa07/lect4.pdf

namespace Proj3Semantics
{
	public interface IVisitableNode
	{
	   void Accept(IReflectiveVisitor rv);
	}

    public interface IReflectiveVisitor
    {
        void Visit(dynamic node);
        
    }



    public interface INamedType
    {
        string Name { get; set; }
    }

    public interface IClassMember : INamedType
    {
        AccessorType AccessorType { get; set; }
        bool IsStatic { get; set; }
        ClassDeclaration ParentClass { get; set; }

    }

}
