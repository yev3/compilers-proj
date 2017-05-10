using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASTBuilder;

namespace ASTBuilder
{
    using static Token;

    // What we're trying to parse
    // =================================

    //public class good1p {

    //   public static void outInt(int n) {
    //     java.io.PrintStream ps;
    //     ps = java.lang.System.out;
    //     ps.print(n);
    //   }

    //   public static void main431() {
    //     int w,x;
    //     x = 3+4;  TestClasses.good1p.outInt(x);
    //     x = 5*7;  TestClasses.good1p.outInt(x);
    //   }
    //}

    public class CompilationUnit : AbstractNode
    {
        // just for the compilation unit because it's the top node
        public override AbstractNode LeftMostSibling => this;
        public override AbstractNode NextSibling => null;

        public CompilationUnit(AbstractNode classDecl)
        {
            AddChild(classDecl);
        }

        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

    public class ClassDeclaration : AbstractNode
    {
        public ClassDeclaration(
            AbstractNode modifiers,
            AbstractNode className,
            AbstractNode classBody)
        {
            AddChild(modifiers);
            AddChild(className);
            AddChild(classBody);
        }
        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }

    }

    public enum ModifierType { PUBLIC, STATIC, PRIVATE }

    public class Modifiers : AbstractNode
    {
        public List<ModifierType> ModifierTokens { get; set; } = new List<ModifierType>();

        public Modifiers(ModifierType type)
        {
            ModifierTokens.Add(type);
        }
        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }
    public class Identifier : AbstractNode
    {
        public Identifier(string s)
        {
            Name = s;
        }

        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

    public class ClassBody : AbstractNode
    {
        public ClassBody()
        {
            Console.WriteLine("Class body is empty!");
        }

        public ClassBody(AbstractNode c)
        {
            AddChild(c);
        }

        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

    public class FieldDeclarations : AbstractNode
    {
        public FieldDeclarations(AbstractNode fieldDecl)
        {
            AddChild(fieldDecl);
        }

        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }
    public class MethodDeclaration : AbstractNode
    {
        public MethodDeclaration(AbstractNode abstractNode,
            AbstractNode abstractNode1, AbstractNode abstractNode2, AbstractNode abstractNode3)
        {
            AddChild(abstractNode);
        }

        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }


    public class TypeName : AbstractNode
    {
        public TypeName(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

    public class TypeSpecifier : AbstractNode
    {
        public TypeSpecifier(AbstractNode abstractNode, bool isArrType = false)
        {
            AddChild(abstractNode);
        }
        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }
    public enum EnumPrimitiveType { BOOLEAN, INT, VOID }
    public class PrimitiveType : AbstractNode
    {
        public EnumPrimitiveType Type { get; set; }
        public PrimitiveType(EnumPrimitiveType type)
        {
            Type = type;
            Console.WriteLine("Primitive Type Ctor: " + type.ToString());
        }
        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

}
