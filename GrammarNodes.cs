using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

        public void AddModType(ModifierType type)
        {
            ModifierTokens.Add(type);
        }

        public Modifiers(ModifierType type)
        {
            AddModType(type);
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
        public MethodDeclaration(
            AbstractNode modifiers,
            AbstractNode typeSpecifier, 
            AbstractNode methodDeclarator, 
            AbstractNode methodBody)
        {
            AddChild(modifiers);
            AddChild(typeSpecifier);
            AddChild(methodDeclarator);
            AddChild(methodBody);
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
        public TypeSpecifier(AbstractNode abstractNode)
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
        }
        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }
    public class LocalVariableDeclarationsAndStatements : AbstractNode
    {
        public LocalVariableDeclarationsAndStatements(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

    public class Block : AbstractNode
    {
        public Block() { }

        public Block(AbstractNode declsAndStaments)
        {
            AddChild(declsAndStaments);
        }
        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

    public class MethodBody : AbstractNode
    {
        public MethodBody(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

    public class MethodDeclaratorName : AbstractNode
    {
        public MethodDeclaratorName(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

    public class DeclaratorName : AbstractNode
    {
        public DeclaratorName(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

    public class Parameter : AbstractNode
    {
        public Parameter(AbstractNode typeSpec, AbstractNode declName)
        {
            AddChild(typeSpec);
            AddChild(declName);
        }
        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

    public class ParameterList : AbstractNode
    {
        public ParameterList(AbstractNode parameter)
        {
            AddChild(parameter);
        }
        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

    public class MethodDeclarator : AbstractNode
    {

        public MethodDeclarator(AbstractNode name)
        {
            AddChild(name);
        }

        public MethodDeclarator(AbstractNode name, AbstractNode paramList)
        {
            AddChild(name);
            AddChild(paramList);
        }
        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }
    public class FieldDeclaration : AbstractNode
    {
        public FieldDeclaration(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }

        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

}
