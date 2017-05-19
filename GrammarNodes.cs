using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proj3Semantics;

namespace Proj3Semantics.Nodes
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
            this.Identifier = className as Identifier; 
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
    }

    /// <summary>
    /// (Page 303)
    /// </summary>
    public class Identifier : AbstractNode
    {
        public string Type { get; set; }
        public Object Attributes { get; set; }
        public string Name { get; set; }

        public Identifier(string s)
        {
            Name = s;
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

    public class MethodDeclarator : AbstractNode
    {
        public readonly ParameterList ParameterList;
        public MethodDeclarator(AbstractNode identifier)
        {
            this.Identifier = identifier as Identifier;
        }
        public MethodDeclarator(AbstractNode identifier, AbstractNode paramList)
        {
            this.Identifier = identifier as Identifier;
            this.ParameterList = paramList as ParameterList;
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

            this.Identifier = methodDeclarator.Identifier;
            AddChild((methodDeclarator as MethodDeclarator)?.ParameterList);
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
    }

    public class Block : Statement
    {
        public Block() { }
        public Block(AbstractNode child)
        {
            AddChild(child);
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

    public class DeclaredVars : AbstractNode
    {
        public DeclaredVars(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }

    public class Statement : LocalVarDeclOrStatement { }

    public class ExpressionStatement : Statement { }

    public class EmptyStatement : Statement { }

    public class QualifiedName : TypeName
    {
        public QualifiedName(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }
    public enum ExprType
    {
        ASSIGNMENT, LOGICAL_OR, LOGICAL_AND, PIPE, HAT, AND, EQUALS,
        NOT_EQUALS, GREATER_THAN, LESS_THAN, LESS_EQUAL, GREATER_EQUAL, PLUSOP, MINUSOP,
        ASTERISK, RSLASH, PERCENT, UNARY, @EVAL
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

    }

    public class NotJustName : PrimaryExpression { }

    public class Literal : AbstractNode
    {
        public string Name { get; set; }
        public Literal(string s)
        {
            Name = s;
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
        public int Value { get; }
        public Number(int n)
        {
            Value = n;
        }

    }

    public class NotImplemented : AbstractNode
    {
        public string Msg { get; set; } 
        public NotImplemented(string msg)
        {
            Msg = msg;
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

    public class ArgumentList : AbstractNode
    {
        public ArgumentList(AbstractNode abstractNode)
        {
            AddChild(abstractNode);
        }
    }
    public class SelectionStatement : AbstractNode { }

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
        public IfStatement(AbstractNode predicate, AbstractNode thenExpr)
        {
            AddChild(predicate);
            AddChild(new Then(thenExpr));
        }
    }

    public class IfStatementElse : SelectionStatement
    {
        public IfStatementElse(AbstractNode predicate, AbstractNode thenExpr, AbstractNode elseExpr)
        {
            AddChild(predicate);
            AddChild(new Then(thenExpr));
            AddChild(new Else(elseExpr));
        }
    }
}
