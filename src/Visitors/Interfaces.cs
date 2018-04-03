// Node interfaces

using CompilerILGen.AST;

// Ideas for the type system implementation from here:
// http://www.ccs.neu.edu/home/riccardo/courses/csu370-fa07/lect4.pdf

namespace CompilerILGen
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