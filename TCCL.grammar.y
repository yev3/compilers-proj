%namespace Proj3Semantics
%using Proj3Semantics.ASTNodes
%partial
%parsertype TCCLParser
%visibility public
%tokentype Token
%YYSTYPE AbstractNode

%{
    // user defined functions go here
%}

%start CompilationUnit

/* Terminals */
%token WRITE WRITE_LINE  // builtin calls
%token AND ASTERISK BANG BOOLEAN CLASS NAMESPACE
%token COLON COMMA ELSE EQUALS HAT
%token IF INSTANCEOF INT STRING IDENTIFIER STR_LITERAL INT_NUMBER
%token LBRACE LBRACKET LPAREN MINUSOP
%token NEW NULL OP_EQ OP_GE OP_GT
%token OP_LAND OP_LE OP_LOR OP_LT OP_NE
%token PERCENT PERIOD PIPE PLUSOP PRIVATE
%token PUBLIC QUESTION RBRACE RBRACKET RETURN
%token RPAREN RSLASH SEMICOLON STATIC STRUCT
%token SUPER THIS TILDE VOID WHILE

%nonassoc RP
%nonassoc ELSE

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

CompilationUnit     
    :   NamespaceItems      { $$ = new CompilationUnit($1); }
    ;

NamespaceDecl
    :   NAMESPACE Identifier NamespaceBody
                            { $$ = new NamespaceDecl($2, $3); }
    |   NAMESPACE NamespaceBody
                            { $$ = new NamespaceDecl($2); }
    ;

NamespaceBody           
    :   LBRACE NamespaceItems RBRACE     
                            { $$ = $2;}
    |   LBRACE RBRACE       { $$ = new NamespaceBody();}
    ;

NamespaceItems
    :   NamespaceDeclStmt   { $$ = new NamespaceBody($1); }
    |   NamespaceItems NamespaceDeclStmt
                            { $1.AddChild($2); $$ = $1; }
    ;


NamespaceDeclStmt
    :   NamespaceDecl       { $$ = $1; }
    |   ClassDeclaration    { $$ = $1; }   
    ;




// ClassDeclarations
//     :   ClassDeclaration    { $$ = new CompilationUnit($1); }
//     |   ClassDeclarations ClassDeclaration
//                             { $1.AddChild($2); $$ = $1; }
//     ;

ClassDeclaration    
    :   Modifiers CLASS Identifier ClassBody 
                            { $$ = new ClassDeclaration($1, $3, $4);}
    ;

Modifiers           
    :   PUBLIC              { $$ = new Modifiers(ModifierType.PUBLIC);}
    |   PRIVATE             { $$ = new Modifiers(ModifierType.PRIVATE);}
    |   STATIC              { $$ = new Modifiers(ModifierType.STATIC);}
    |   Modifiers PUBLIC    { ((Modifiers)$1).AddModType(ModifierType.PUBLIC); $$ = $1;}
    |   Modifiers PRIVATE   { ((Modifiers)$1).AddModType(ModifierType.PRIVATE); $$ = $1;}
    |   Modifiers STATIC    { ((Modifiers)$1).AddModType(ModifierType.STATIC); $$ = $1;}
    ;

ClassBody           
    :   LBRACE FieldDeclarations RBRACE     { $$ = $2;}
    |   LBRACE RBRACE                       { $$ = new ClassBody();}
    ;

FieldDeclarations   
    :   FieldDeclaration                    { $$ = new ClassBody($1); }
    |   FieldDeclarations FieldDeclaration  { $1.AddChild($2); $$ = $1;}
    ;

FieldDeclaration    
    :   CLassFieldDecl SEMICOLON  { $$ = $1; }
    |   MethodDeclaration                   { $$ = $1; }
    |   ConstructorDeclaration              { $$ = $1; }
    |   StaticInitializer                   { $$ = $1; }
    |   StructDeclaration                   { $$ = $1; }
    ;

StructDeclaration   
    :   Modifiers STRUCT Identifier ClassBody   { $$ = new NotImplemented("StructDeclaration"); }
    ;

/*
 * This isn't structured so nicely for a bottom up parse.  Recall
 * the example I did in class for Digits, where the "type" of the digits
 * (i.e., the base) is sitting off to the side.  You'll have to do something
 * here to get the information where you want it, so that the declarations can
 * be suitably annotated with their type and modifier information.
 */
CLassFieldDecl    
    :   Modifiers TypeSpecifier FieldVariableDeclarators            
                                            { $$ = new ClassFieldDeclStatement($1, new VariableListDeclaring($2, $3)); }
    ;

TypeSpecifier               
    :   TypeName                            { $$ = $1; }
    |   ArraySpecifier                      { $$ = $1; }
    ;

TypeName                    
    :   PrimitiveType                       { $$ = $1; }
    |   QualifiedName                       { $$ = $1; }
    ;

ArraySpecifier              
    :   TypeName LBRACKET RBRACKET          { $$ = new ArraySpecifier($1); }
    ;
                            
PrimitiveType               
    :   BOOLEAN                             { $$ = new BuiltinTypeBoolean(); }
    |   INT                                 { $$ = new BuiltinTypeInt(); }
    |   STRING                              { $$ = new BuiltinTypeString(); }
    |   VOID                                { $$ = new BuiltinTypeVoid(); }
    ;

FieldVariableDeclarators    
    :   FieldVariableDeclaratorName         { $$ = new DeclaredVars($1); }
    |   FieldVariableDeclarators COMMA FieldVariableDeclaratorName  
                                            { $$.AddChild($3); $$ = $1;}
    ;


MethodDeclaration           
    :   Modifiers TypeSpecifier MethodDeclarator MethodBody         
                                            { $$ = new MethodDeclaration($1, $2, $3, $4); }
    ;

MethodDeclarator            
    :   MethodDeclaratorName LPAREN ParameterList RPAREN            
                                            { $$ = new MethodDeclarator($1, $3); }
    |   MethodDeclaratorName LPAREN RPAREN  { $$ = new MethodDeclarator($1); }
    ;

ParameterList               
    :   Parameter                           { $$ = new ParameterList($1); }
    |   ParameterList COMMA Parameter       { $1.AddChild($3); $$ = $1;}  
    ;

Parameter                   
    :   TypeSpecifier DeclaratorName        { $$ = new Parameter($1, $2); }
    ;

QualifiedName               
    :   Identifier                          { $$ = new QualifiedName($1);}
    |   QualifiedName PERIOD Identifier     { $$.AddChild($3); $$ = $1;}
    ;

DeclaratorName              
    :   Identifier                          { $$ = $1; }
    ;

MethodDeclaratorName        
    :   Identifier                          { $$ = $1; }
    ;

FieldVariableDeclaratorName 
    :   Identifier                          { $$ = new FieldVarDecl($1); }
    ;

LocalVariableDeclaratorName 
    :   Identifier                          { $$ = $1; }
    ;

MethodBody                  
    :   Block                               { $$ = $1; }
    ;

ConstructorDeclaration      
    :   Modifiers MethodDeclarator Block    { $$ = new NotImplemented("ConstructorDeclaration"); }
    ;

StaticInitializer           
    :   STATIC Block                        { $$ = new StaticInitializer($2); }
    ;
        
/*
 * These can't be reorganized, because the order matters.
 * For example
    :  int i;  i = 5;  int j = i;
 */

Block                       
    :   LBRACE LocalVariableDeclarationsAndStatements RBRACE        
                                            { $$ = $2; }
    |   LBRACE RBRACE                       { $$ = new Block(); }
    ;

LocalVariableDeclarationsAndStatements  
    :   LocalVarDeclOrStatement             { $$ = new Block($1);}
    |   LocalVariableDeclarationsAndStatements LocalVarDeclOrStatement
                                            { $1.AddChild($2); $$ = $1; }
    ;

LocalVarDeclOrStatement 
    :   LocalVariableDecl                   { $$ = $1;}
    |   Statement                           { $$ = $1;}
    ;

LocalVariableDecl       
    :   TypeSpecifier LocalVariableDeclarators SEMICOLON    
                                            { $$ = new VariableListDeclaring($1, $2);}
    |   StructDeclaration                   { $$ = new NotImplemented("struct decl not supported");}
    ;

LocalVariableDeclarators    
    :   LocalVariableDeclaratorName         { $$ = new DeclaredVars($1); }
    |   LocalVariableDeclarators COMMA LocalVariableDeclaratorName  
                                            { $1.AddChild($3); $$ = $1; }
    ;

Statement                   
    :   EmptyStatement                      { $$ = $1; }
    |   ExpressionStatement SEMICOLON       { $$ = $1; }
    |   SelectionStatement                  { $$ = $1; }
    |   IterationStatement                  { $$ = $1; }
    |   ReturnStatement                     { $$ = $1; }
    |   Block                               { $$ = $1; }
    ;

EmptyStatement              
    :   SEMICOLON                           { $$ = new EmptyStatement();}
    ;

ExpressionStatement         
    :   Expression                          { $$ = $1; }
    ;

/*
 *  You will eventually have to address the shift/reduce error that
 *     occurs when the second IF-rule is uncommented.
 *
 */

SelectionStatement          
    :   IF LPAREN Expression RPAREN Statement ELSE Statement    { $$ = new IfStatementElse($3,$5,$7); }
    |   IF LPAREN Expression RPAREN Statement                   { $$ = new IfStatement($3,$5); }
    ;


IterationStatement          
    :   WHILE LPAREN Expression RPAREN Statement
                                            { $$ = new WhileLoop($3, $5); }
    ;

ReturnStatement         
    :   RETURN Expression SEMICOLON         { $$ = new ReturnStatement($2); }
    |   RETURN            SEMICOLON         { $$ = new ReturnStatement(); }
    ;

ArgumentList            
    :   Expression                          { $$ = new ArgumentList($1); }
    |   ArgumentList COMMA Expression       { $1.AddChild($3); $$ = $1; }
    ;

Expression                  
    :   QualifiedName EQUALS Expression     { $$ = new AssignExpr($1, $3); }
    |   Expression OP_LOR Expression        { $$ = new BinaryExpr($1, ExprType.LOGICAL_OR, $3); }   /* short-circuit OR  */  
    |   Expression OP_LAND Expression       { $$ = new BinaryExpr($1, ExprType.LOGICAL_AND, $3); }   /* short-circuit AND */  
    |   Expression PIPE Expression          { $$ = new BinaryExpr($1, ExprType.PIPE, $3); }                
    |   Expression HAT Expression           { $$ = new BinaryExpr($1, ExprType.HAT, $3); }                
    |   Expression AND Expression           { $$ = new BinaryExpr($1, ExprType.AND, $3); }                
    |   Expression OP_EQ Expression         { $$ = new BinaryExpr($1, ExprType.EQUALS, $3); }                
    |   Expression OP_NE Expression         { $$ = new BinaryExpr($1, ExprType.NOT_EQUALS, $3); }                
    |   Expression OP_GT Expression         { $$ = new BinaryExpr($1, ExprType.GREATER_THAN, $3); }                
    |   Expression OP_LT Expression         { $$ = new BinaryExpr($1, ExprType.LESS_THAN, $3); }                
    |   Expression OP_LE Expression         { $$ = new BinaryExpr($1, ExprType.LESS_EQUAL, $3); }                
    |   Expression OP_GE Expression         { $$ = new BinaryExpr($1, ExprType.GREATER_EQUAL, $3); }                
    |   Expression PLUSOP Expression        { $$ = new BinaryExpr($1, ExprType.PLUSOP, $3); }                
    |   Expression MINUSOP Expression       { $$ = new BinaryExpr($1, ExprType.MINUSOP, $3); }                
    |   Expression ASTERISK Expression      { $$ = new BinaryExpr($1, ExprType.ASTERISK, $3); }                
    |   Expression RSLASH Expression        { $$ = new BinaryExpr($1, ExprType.RSLASH, $3); }                
    |   Expression PERCENT Expression       { $$ = new BinaryExpr($1, ExprType.PERCENT, $3); }   /* remainder */
    |   ArithmeticUnaryOperator Expression  %prec UNARY
                                            { $$ = new NotImplemented("ArithmeticUnaryOperator Expression  %prec UNARY"); }
    |   EvalExpression                      { $$ = $1; }
    ;

ArithmeticUnaryOperator     
    :   PLUSOP                          { $$ = new NotImplemented("ArithmeticUnaryOperator"); }
    |   MINUSOP                         { $$ = new NotImplemented("ArithmeticUnaryOperator"); }
    ;
                            
EvalExpression           
    :   QualifiedName                   { $$ = new EvalExpr($1);}   
    |   QualifiedPrimaryExpr            { $$ = new EvalExpr($1);}
    ;

QualifiedPrimaryExpr                 
    :   SpecialBuiltinName              { $$ = $1; }
    |   ComplexPrimary                  { $$ = $1; }
    ;

ComplexPrimary              
    :   LPAREN Expression RPAREN        { $$ = $2; }
    |   ComplexPrimaryNoParenthesis     { $$ = $1;}
    ;

ComplexPrimaryNoParenthesis 
    :   STR_LITERAL                     { $$ = $1; }
    |   Number                          { $$ = $1; }
    |   FieldAccess                     { $$ = $1; }    
    |   MethodCall                      { $$ = $1; }    
    ;

FieldAccess                 
    :   QualifiedPrimaryExpr PERIOD Identifier   
                                        { $$ = new NotImplemented("FieldAccess");}   
    ;       

MethodCall                  
    :   MethodReference LPAREN ArgumentList RPAREN
                                        { $$ = new MethodCall($1, $3);}
    |   MethodReference LPAREN RPAREN   { $$ = new MethodCall($1);}
    ;

MethodReference             
    :   ComplexPrimaryNoParenthesis     { $$ = $1;}
    |   QualifiedName                   { $$ = $1;}
    |   SpecialBuiltinName              { $$ = $1;}
    |   BuiltinSystemCall               { $$ = $1;}
    ;

BuiltinSystemCall                 
    :   WRITE                           { $$ = $1;}
    |   WRITE_LINE                      { $$ = $1;}
    ;

SpecialBuiltinName                 
    :   THIS                            { $$ = new BuiltInTypeThis();}
    |   NULL                            { $$ = new BuiltInTypeNull();}
    ;

Identifier                  
    :   IDENTIFIER                      { $$ = $1;}
    ;

Number                      
    :   INT_NUMBER                      { $$ = $1;}
    ;

%%

