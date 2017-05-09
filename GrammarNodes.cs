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
        public ClassDeclaration ClassDeclaration { get; set; }
    }

    public class ClassDeclaration : AbstractNode
    {
        public ClassDeclaration(
            AbstractNode modifiers,
            AbstractNode className,
            AbstractNode classBody)
        {
            
            Modifiers = modifiers as Modifiers;
            ClassName = className as Identifier;
            ClassBody = classBody as ClassBody;
        }

        public Modifiers Modifiers { get; set; }
        public Identifier ClassName { get; set; }
        public ClassBody ClassBody { get; set; }
    }

    public class Modifiers : AbstractNode
    {
        public List<Token> ModifierTokens { get; set; } = new List<Token>();

        public Modifiers(Token t)
        {

            if (t != PUBLIC && t != STATIC && t != PRIVATE)
            {
                throw new Exception("not one of the valid tokens");
            }
            ModifierTokens.Add(t);
        }

    }
    public class Identifier : AbstractNode
    {
        public Identifier(string s)
        {
            Name = s;
        }

    }

    public class ClassBody : AbstractNode
    {

    }

}
