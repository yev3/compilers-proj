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
        //public override AbstractNode LeftMostSibling => this;
        public override AbstractNode NextSibling => null;

        public CompilationUnit(AbstractNode classDecl)
        {
            AddChild(classDecl);
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
            //Console.WriteLine("Class body is empty!");
        }
        public ClassBody(AbstractNode c)
        {
            AddChild(c);
        }
    }

    public class FieldDeclarations : AbstractNode
    {
        public FieldDeclarations(AbstractNode fieldDecl)
        {
            AddChild(fieldDecl);
        }

    }
    public class MethodDeclaration : FieldDeclaration
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

    }


    public class TypeName : AbstractNode { }
    public class TypeSpecifier : AbstractNode { }
    public enum EnumPrimitiveType { BOOLEAN, INT, VOID }
    public class PrimitiveType : TypeName
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
    }

    public class Block : Statement
    {
        public Block() { }

        public Block(AbstractNode declsAndStaments)
        {
            AddChild(declsAndStaments);
        }
    }


    public class DeclaratorName : AbstractNode
    {
        public DeclaratorName(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }

    public class Parameter : AbstractNode
    {
        public Parameter(AbstractNode typeSpec, AbstractNode declName)
        {
            AddChild(typeSpec);
            AddChild(declName);
        }
    }

    public class ParameterList : AbstractNode
    {
        public ParameterList(AbstractNode parameter)
        {
            AddChild(parameter);
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
    }
    public class FieldDeclaration : AbstractNode { }
    public class LocalVarDeclOrStatement : AbstractNode { }

    public class LocalVariableDecl : LocalVarDeclOrStatement
    {
        public LocalVariableDecl(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }

        public LocalVariableDecl(AbstractNode typeSpecifier, AbstractNode localVarDecls)
        {
            AddChild(typeSpecifier);
            AddChild(localVarDecls);
        }
    }

    public class LocalVariableDeclarators : AbstractNode
    {
        public LocalVariableDeclarators(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }

    public class Statement : LocalVarDeclOrStatement { }

    public class ExpressionStatement : Statement { }

    public class EmptyStatement : Statement { }

    public class LocalVariableDeclaratorName : AbstractNode
    {
        public LocalVariableDeclaratorName(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }

    public class QualifiedName : TypeName
    {
        public QualifiedName(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }
    public enum ExprType
    {
        EQUALS, OP_LOR, OP_LAND, PIPE, HAT, AND, OP_EQ,
        OP_NE, OP_GT, OP_LT, OP_LE, OP_GE, PLUSOP, MINUSOP,
        ASTERISK, RSLASH, PERCENT, UNARY, PRIMARY
    }
    public class Expression : ExpressionStatement
    {
        public ExprType ExprType { get; set; }
        public Expression(AbstractNode expr, ExprType type)
        {
            AddChild(expr);
            ExprType = type;
        }
        public Expression(AbstractNode lhs, ExprType type, AbstractNode rhs)
        {
            AddChild(lhs);
            AddChild(rhs);
            ExprType = type;
        }

        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }
    public class PrimaryExpression : AbstractNode { }
    public enum SpecialNameType { THIS, NULL }

    public class SpecialName : NotJustName
    {
        public SpecialNameType SpecialType { get; set; }
        public SpecialName(SpecialNameType specialType)
        {
            SpecialType = specialType;
        }

        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

    public class NotJustName : PrimaryExpression { }

    public class ComplexPrimaryNoParenthesis : AbstractNode
    {
        public ComplexPrimaryNoParenthesis(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }

    public class MethodReference : AbstractNode
    {
        public MethodReference(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }

    public class ComplexPrimary : NotJustName { }

    public class MethodCall : ComplexPrimary
    {

        public MethodCall(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }

        public MethodCall(AbstractNode methodRef, AbstractNode argList)
        {
            AddChild(methodRef);
            AddChild(argList);
        }
    }

    public class Number : ComplexPrimary
    {
        public int Value { get; set; }
        public Number(int n)
        {
            Value = n;
        }

        public override void Accept(INodeVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

    public class NotImplemented : AbstractNode
    {
        public NotImplemented(string msg)
        {
            Name = msg;
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class ReturnStatement : Statement
    {
        public ReturnStatement() { }

        public ReturnStatement(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }

    public class StaticInitializer : FieldDeclaration
    {
        public StaticInitializer(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }

    public class FieldVariableDeclaratorName : AbstractNode
    {
        public FieldVariableDeclaratorName(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }

    public class ArraySpecifier : TypeSpecifier
    {
        public ArraySpecifier(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }

}
