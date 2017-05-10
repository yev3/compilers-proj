%namespace ASTBuilder
%partial
%parsertype TCCLParser
%visibility public
%tokentype Token
%YYSTYPE AbstractNode

/* %union{
    public Token token;
    public AbstractNode abstractNode;
} */

%{
    public string yytext
    {
        get { return ((TCCLScanner)Scanner).yytext; }
    }

%}

%start CompilationUnit

// %token STATIC, STRUCT, QUESTION, RSLASH, MINUSOP, NULL, INT, OP_EQ, OP_LT, COLON, OP_LOR
// %token ELSE, PERCENT, THIS, CLASS, PIPE, PUBLIC, PERIOD, HAT, COMMA, VOID, TILDE
// %token LPAREN, RPAREN, OP_GE, SEMICOLON, IF, NEW, WHILE, PRIVATE, BANG, OP_LE, AND 
// %token LBRACE, RBRACE, LBRACKET, RBRACKET, BOOLEAN, INSTANCEOF, ASTERISK, EQUALS, PLUSOP
// %token RETURN, OP_GT, OP_NE, OP_LAND, INT_NUMBER, IDENTIFIER, LITERAL, SUPER

%token AND ASTERISK BANG BOOLEAN CLASS
%token COLON COMMA ELSE EQUALS HAT
%token IDENTIFIER IF INSTANCEOF INT INT_NUMBER
%token LBRACE LBRACKET LITERAL LPAREN MINUSOP
%token NEW NULL OP_EQ OP_GE OP_GT
%token OP_LAND OP_LE OP_LOR OP_LT OP_NE
%token PERCENT PERIOD PIPE PLUSOP PRIVATE
%token PUBLIC QUESTION RBRACE RBRACKET RETURN
%token RPAREN RSLASH SEMICOLON STATIC STRUCT
%token SUPER THIS TILDE VOID WHILE


%right EQUALS
%left  OP_LOR
%left  OP_LAND
%left  PIPE
%left  HAT
%left  AND
%left  OP_EQ, OP_NE
%left  OP_GT, OP_LT, OP_LE, OP_GE
%left  PLUSOP, MINUSOP
%left  ASTERISK, RSLASH, PERCENT
%left  UNARY 

%%

CompilationUnit     :   ClassDeclaration                    {$$ = new CompilationUnit($1);}
                    ;

ClassDeclaration    :   Modifiers CLASS Identifier ClassBody {$$ = new ClassDeclaration($1, $3, $4);}
                    ;

Modifiers           :   PUBLIC                              { $$ = new Modifiers(ModifierType.PUBLIC);}
                    |   PRIVATE                             { $$ = new Modifiers(ModifierType.PRIVATE);}
                    |   STATIC                              { $$ = new Modifiers(ModifierType.STATIC);}
                    |   Modifiers PUBLIC                    { $1.AddChild($2); $$ = $1;}
                    |   Modifiers PRIVATE                   { $1.AddChild($2); $$ = $1;}
                    |   Modifiers STATIC                    { $1.AddChild($2); $$ = $1;}
                    ;

ClassBody           :   LBRACE FieldDeclarations RBRACE     { $$ = new ClassBody($2);}
                    |   LBRACE RBRACE                       { $$ = new ClassBody();}
                    ;

FieldDeclarations   :   FieldDeclaration                    { $$ = new FieldDeclarations($1); }
                    |   FieldDeclarations FieldDeclaration  { $1.AddChild($2); $$ = $1;}
                    ;

FieldDeclaration    :   FieldVariableDeclaration SEMICOLON  { $$ = new Identifier("fake var decl"); }
                    |   MethodDeclaration                   { $$ = new Identifier("***** DOING RIGHT NOW *****");        }
                    |   ConstructorDeclaration              { $$ = new Identifier("field ctor decl");          }
                    |   StaticInitializer                   { $$ = new Identifier("field static init decl");   }
                    |   StructDeclaration                   { $$ = new Identifier("field struct decl");        }
                    ;

StructDeclaration   :   Modifiers STRUCT Identifier ClassBody   {}
                    ;



/*
 * This isn't structured so nicely for a bottom up parse.  Recall
 * the example I did in class for Digits, where the "type" of the digits
 * (i.e., the base) is sitting off to the side.  You'll have to do something
 * here to get the information where you want it, so that the declarations can
 * be suitably annotated with their type and modifier information.
 */
FieldVariableDeclaration    :   Modifiers TypeSpecifier FieldVariableDeclarators            {}
                            ;

TypeSpecifier               :   TypeName                                                    { $$ = new TypeSpecifier($1); }
                            |   ArraySpecifier                                              { $$ = new TypeSpecifier($1); }
                            ;

TypeName                    :   PrimitiveType                                               { $$ = new TypeName($1); }
                            |   QualifiedName                                               { $$ = new TypeName($1); }
                            ;

ArraySpecifier              :   TypeName LBRACKET RBRACKET                                  {}
                            ;
                            
PrimitiveType               :   BOOLEAN                                                     { $$ = new PrimitiveType(EnumPrimitiveType.BOOLEAN); }
                            |   INT                                                         { $$ = new PrimitiveType(EnumPrimitiveType.INT); }
                            |   VOID                                                        { $$ = new PrimitiveType(EnumPrimitiveType.VOID); }
                            ;

FieldVariableDeclarators    :   FieldVariableDeclaratorName                                 {}
                            |   FieldVariableDeclarators COMMA FieldVariableDeclaratorName  {}
                            ;


MethodDeclaration           :   Modifiers TypeSpecifier MethodDeclarator MethodBody         {$$ = new MethodDeclaration($1, $2, $3, $4); }
                            ;

MethodDeclarator            :   MethodDeclaratorName LPAREN ParameterList RPAREN            { $$ = new MethodDeclarator($1, $3); }
                            |   MethodDeclaratorName LPAREN RPAREN                          { $$ = new MethodDeclarator($1); }
                            ;

ParameterList               :   Parameter                                                   { $$ = new ParameterList($1); }
                            |   ParameterList COMMA Parameter                               { $1.AddChild($2); $$ = $1;}  
                            ;

Parameter                   :   TypeSpecifier DeclaratorName                                { $$ = new Parameter($1, $2); }
                            ;

QualifiedName               :   Identifier                                                  {}
                            |   QualifiedName PERIOD Identifier                             {}
                            ;

DeclaratorName              :   Identifier                                                  { $$ = new DeclaratorName($1); }
                            ;

MethodDeclaratorName        :   Identifier                                                  { $$ = new MethodDeclaratorName($1); }
                            ;

FieldVariableDeclaratorName :   Identifier                                                  {}
                            ;

LocalVariableDeclaratorName :   Identifier                                                  {}
                            ;

MethodBody                  :   Block                                                       { $$ = new MethodBody($1); }
                            ;

ConstructorDeclaration      :   Modifiers MethodDeclarator Block                            {}
                            ;

StaticInitializer           :   STATIC Block                                                {}
                            ;
        
/*
 * These can't be reorganized, because the order matters.
 * For example:  int i;  i = 5;  int j = i;
 */

Block                       :   LBRACE LocalVariableDeclarationsAndStatements RBRACE        { $$ = new Block($1); }
                            |   LBRACE RBRACE                                               { $$ = new Block(); }
                            ;

LocalVariableDeclarationsAndStatements  :   LocalVariableDeclarationOrStatement             { $$ = new LocalVariableDeclarationsAndStatements($1);}
                                        |   LocalVariableDeclarationsAndStatements LocalVariableDeclarationOrStatement
                                                                                            { $1.AddChild($2); $$ = $1; }
                                        ;

// FIXME
LocalVariableDeclarationOrStatement :   LocalVariableDeclarationStatement                   { $$ = new Identifier("FIXME: local var decl statement");}
                                    |   Statement                                           { $$ = new Identifier("FIXME: statement");}
                                    ;

LocalVariableDeclarationStatement   :   TypeSpecifier LocalVariableDeclarators SEMICOLON
                                    |   StructDeclaration                                           
                                    ;

LocalVariableDeclarators    :   LocalVariableDeclaratorName
                            |   LocalVariableDeclarators COMMA LocalVariableDeclaratorName
                            ;

                            

Statement                   :   EmptyStatement
                            |   ExpressionStatement SEMICOLON
                            |   SelectionStatement
                            |   IterationStatement
                            |   ReturnStatement
                            |   Block
                            ;

EmptyStatement              :   SEMICOLON
                            ;

ExpressionStatement         :   Expression
                            ;

/*
 *  You will eventually have to address the shift/reduce error that
 *     occurs when the second IF-rule is uncommented.
 *
 */

SelectionStatement          :   IF LPAREN Expression RPAREN Statement ELSE Statement
//                          |   IF LPAREN Expression RPAREN Statement
                            ;


IterationStatement          :   WHILE LPAREN Expression RPAREN Statement
                            ;

ReturnStatement             :   RETURN Expression SEMICOLON
                            |   RETURN            SEMICOLON
                            ;

ArgumentList                :   Expression
                            |   ArgumentList COMMA Expression
                            ;


Expression                  :   QualifiedName EQUALS Expression
                            |   Expression OP_LOR Expression   /* short-circuit OR */
                            |   Expression OP_LAND Expression   /* short-circuit AND */
                            |   Expression PIPE Expression
                            |   Expression HAT Expression
                            |   Expression AND Expression
                            |   Expression OP_EQ Expression
                            |   Expression OP_NE Expression
                            |   Expression OP_GT Expression
                            |   Expression OP_LT Expression
                            |   Expression OP_LE Expression
                            |   Expression OP_GE Expression
                            |   Expression PLUSOP Expression
                            |   Expression MINUSOP Expression
                            |   Expression ASTERISK Expression
                            |   Expression RSLASH Expression
                            |   Expression PERCENT Expression   /* remainder */
                            |   ArithmeticUnaryOperator Expression  %prec UNARY
                            |   PrimaryExpression
                            ;

ArithmeticUnaryOperator     :   PLUSOP
                            |   MINUSOP
                            ;
                            
PrimaryExpression           :   QualifiedName
                            |   NotJustName
                            ;

NotJustName                 :   SpecialName
                            |   ComplexPrimary
                            ;

ComplexPrimary              :   LPAREN Expression RPAREN
                            |   ComplexPrimaryNoParenthesis
                            ;

ComplexPrimaryNoParenthesis :   LITERAL
                            |   Number
                            |   FieldAccess
                            |   MethodCall
                            ;

FieldAccess                 :   NotJustName PERIOD Identifier
                            ;       

MethodCall                  :   MethodReference LPAREN ArgumentList RPAREN
                            |   MethodReference LPAREN RPAREN
                            ;

MethodReference             :   ComplexPrimaryNoParenthesis
                            |   QualifiedName
                            |   SpecialName
                            ;

SpecialName                 :   THIS
                            |   NULL
                            ;

Identifier                  :   IDENTIFIER      {$$ = new Identifier(yytext);}
                            ;

Number                      :   INT_NUMBER
                            ;

%%

