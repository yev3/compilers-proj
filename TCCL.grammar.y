%namespace Proj3Semantics
%using Proj3Semantics.AST;
%partial
%parsertype TCCLParser
%visibility public
%tokentype Token
%YYSTYPE Node

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
    :   DeclarationList     { $$ = new CompilationUnit(@1, $1); }
    ;

DeclarationList
    :   Declaration         { $$ = new DeclarationList($1); }
    |   DeclarationList Declaration
                            { $1.AddChild($2); $$ = $1; }
    ;

Declaration
    :   NamespaceDecl       { $$ = $1; }
    |   ClassDeclaration    { $$ = $1; }
    |   FunctionDecl        { $$ = $1; }    // done
    |   LocalVarDecl        { $$ = $1; }
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


//  CLASS
ClassDeclaration    
    :   Modifiers CLASS Identifier ClassBody 
                            { $$ = new ClassDeclaration($1 as Modifiers, $3 as Identifier, $4 as ClassBody);}
    ;

//  CLASS   MODIFIERS 
Modifiers           
    :   PUBLIC              { $$ = new Modifiers(ModifierType.PUBLIC);}
    |   PRIVATE             { $$ = new Modifiers(ModifierType.PRIVATE);}
    |   STATIC              { $$ = new Modifiers(ModifierType.STATIC);}
    |   Modifiers PUBLIC    { ((Modifiers)$1).AddModType(ModifierType.PUBLIC); $$ = $1;}
    |   Modifiers PRIVATE   { ((Modifiers)$1).AddModType(ModifierType.PRIVATE); $$ = $1;}
    |   Modifiers STATIC    { ((Modifiers)$1).AddModType(ModifierType.STATIC); $$ = $1;}
    ;

//  CLASS   BODY    
ClassBody           
    :   LBRACE ClassBodyItems RBRACE        { $$ = $2;}
    |   LBRACE RBRACE                       { $$ = new ClassBody();}
    ;

// CLASS    BODY    DECLS
ClassBodyItems   
    :   ClassBodyDecl                       { $$ = new ClassBody($1); }
    |   ClassBodyItems ClassBodyDecl        { $1.AddChild($2); $$ = $1;}
    ;

// CLASS    BODY    DECL
ClassBodyDecl    
    :   ClassFieldDecl SEMICOLON            { $$ = $1; }    
    |   ClassMethodDecl                     { $$ = $1; }
    |   ConstructorDeclaration              { $$ = $1; }
    |   StaticInitializer                   { $$ = $1; }
    |   StructDeclaration                   { $$ = $1; }
    ;

// CLASS    BODY    DECL    FIELD
ClassFieldDecl    
    :   Modifiers TypeNode FieldVarDecls    { $$ = new ClassFieldDeclStatement($1, new LocalVarDecl($2 as TypeNode, $3 as VarDeclList)); }
    ;

// CLASS    BODY    DECL    METHOD
ClassMethodDecl           
    :   Modifiers FunctionDecl              { $$ = new ClassMethodDecl($1 as Modifiers, $2 as FunctionDecl); }
    |   FunctionDecl                        { $$ = new ClassMethodDecl($1 as FunctionDecl); }       // private non-static by default
    ;

// CLASS    BODY    DECL    CTOR
ConstructorDeclaration      
    :   Modifiers MethodDeclarator Block    { $$ = new NotImplemented("ConstructorDeclaration"); }
    ;

// CLASS    BODY    DECL    STATICINIT?? .. i dunno what this is
StaticInitializer           
    :   STATIC Block                        { $$ = new StaticInitializer($2); }
    ;

// CLASS    BODY    DECL    STRUCTDECL
StructDeclaration   
    :   Modifiers STRUCT Identifier ClassBody   
                                            { $$ = new NotImplemented("StructDeclaration"); }
    ;


TypeNode               
    :   TypeName                            { $$ = $1; }
    |   ArraySpecifier                      { $$ = $1; }
    ;

TypeName                    
    :   PrimitiveType                       { $$ = $1; }
    |   QualName                       { $$ = $1; }
    ;

ArraySpecifier              
    :   TypeName LBRACKET RBRACKET          { $$ = new NotImplemented("Arrays not supported"); }
    ;
                            
PrimitiveType               
    :   BOOLEAN                             { $$ = TypeNode.TypeNodeBoolean; }
    |   INT                                 { $$ = TypeNode.TypeNodeInt; }
    |   STRING                              { $$ = TypeNode.TypeNodeString; }
    |   VOID                                { $$ = TypeNode.TypeNodeVoid; }
    ;

FieldVarDecls    
    :   FieldVarDeclName                        { $$ = new VarDeclList($1 as VarDecl); }
    |   FieldVarDecls COMMA FieldVarDeclName    { $$.AddChild($3); $$ = $1;}
    ;



FunctionDecl
    :   TypeNode MethodDeclarator MethodBody
                                            { $$ = new FunctionDecl($1 as TypeNode, $2 as MethodDeclarator, $3 as Block); }
    ;

MethodDeclarator            
    :   MethodDeclaratorName LPAREN ParamListNode RPAREN            
                                            { $$ = new MethodDeclarator($1 as Identifier, $3 as ParamListNode); }
    |   MethodDeclaratorName LPAREN RPAREN  { $$ = new MethodDeclarator($1 as Identifier); }
    ;

ParamListNode               
    :   ParamDecl                           { $$ = new ParamListNode($1); }
    |   ParamListNode COMMA ParamDecl       { $1.AddChild($3); $$ = $1;}  
    ;

ParamDecl                   
    :   TypeNode DeclaratorName             { $$ = new ParamDecl($1 as TypeNode, $2 as Identifier); }
    ;

QualName               
    :   Identifier                          { $$ = new QualifiedNode($1 as Identifier);}
    |   QualName PERIOD Identifier     { ($$ as QualifiedNode).AddChild($3 as Identifier); $$ = $1;}
    ;

DeclaratorName              
    :   Identifier                          { $$ = $1; }
    ;

MethodDeclaratorName        
    :   Identifier                          { $$ = $1; }
    ;

FieldVarDeclName 
    :   Identifier                          { $$ = new VarDecl($1 as Identifier); }
    ;


MethodBody                  
    :   Block                               { $$ = $1; }
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
    :   LocalVarDecl                        { $$ = $1;}
    |   Statement                           { $$ = $1;}
    ;

LocalVarDecl       
    :   TypeNode VarDeclList SEMICOLON      { $$ = new LocalVarDecl($1 as TypeNode, $2 as VarDeclList);}
    |   StructDeclaration                   { $$ = new NotImplemented("struct decl not supported");}
    ;

VarDeclList    
    :   VarDecl                             { $$ = new VarDeclList($1 as VarDecl); }
    |   VarDeclList COMMA VarDecl           { $1.AddChild($3); $$ = $1; }
    ;

VarDecl 
    :   Identifier                          { $$ = new VarDecl($1 as Identifier); }
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
    :   Expr                          { $$ = $1; }
    ;

/*
 *  You will eventually have to address the shift/reduce error that
 *     occurs when the second IF-rule is uncommented.
 *
 */

SelectionStatement          
    :   IF LPAREN Expr RPAREN Statement ELSE Statement    
                                            { $$ = new IfStatementElse($3,$5,$7); }
    |   IF LPAREN Expr RPAREN Statement                   
                                            { $$ = new IfStatement($3,$5); }
    ;


IterationStatement          
    :   WHILE LPAREN Expr RPAREN Statement
                                            { $$ = new WhileLoop($3, $5); }
    ;

ReturnStatement         
    :   RETURN Expr SEMICOLON         { $$ = new ReturnStatement($2); }
    |   RETURN            SEMICOLON         { $$ = new ReturnStatement(); }
    ;

ArgumentList            
    :   Expr                          { $$ = new ArgumentList($1); }
    |   ArgumentList COMMA Expr       { $1.AddChild($3); $$ = $1; }
    ;

Expr                  
    :   QualName EQUALS Expr    { $$ = new AssignExpr($1, $3); }
    |   Expr OP_LOR Expr        { $$ = new CompExpr($1, ExprType.LOGICAL_OR, $3); }   /* short-circuit OR  */  
    |   Expr OP_LAND Expr       { $$ = new CompExpr($1, ExprType.LOGICAL_AND, $3); }   /* short-circuit AND */  
    |   Expr PIPE Expr          { $$ = new BinaryExpr($1, ExprType.PIPE, $3); }                
    |   Expr HAT Expr           { $$ = new BinaryExpr($1, ExprType.HAT, $3); }                
    |   Expr AND Expr           { $$ = new CompExpr($1, ExprType.AND, $3); }                
    |   Expr OP_EQ Expr         { $$ = new CompExpr($1, ExprType.EQUALS, $3); }                
    |   Expr OP_NE Expr         { $$ = new CompExpr($1, ExprType.NOT_EQUALS, $3); }                
    |   Expr OP_GT Expr         { $$ = new CompExpr($1, ExprType.GREATER_THAN, $3); }                
    |   Expr OP_LT Expr         { $$ = new CompExpr($1, ExprType.LESS_THAN, $3); }                
    |   Expr OP_LE Expr         { $$ = new CompExpr($1, ExprType.LESS_EQUAL, $3); }                
    |   Expr OP_GE Expr         { $$ = new CompExpr($1, ExprType.GREATER_EQUAL, $3); }                
    |   Expr PLUSOP Expr        { $$ = new BinaryExpr($1, ExprType.PLUSOP, $3); }                
    |   Expr MINUSOP Expr       { $$ = new BinaryExpr($1, ExprType.MINUSOP, $3); }                
    |   Expr ASTERISK Expr      { $$ = new BinaryExpr($1, ExprType.ASTERISK, $3); }                
    |   Expr RSLASH Expr        { $$ = new BinaryExpr($1, ExprType.RSLASH, $3); }                
    |   Expr PERCENT Expr       { $$ = new BinaryExpr($1, ExprType.PERCENT, $3); }   /* remainder */
    |   ArithmeticUnaryOperator Expr  %prec UNARY
                                { $$ = new NotImplemented("ArithmeticUnaryOperator Expr  %prec UNARY"); }
    |   PriExpr          { $$ = $1; }
    ;

ArithmeticUnaryOperator     
    :   PLUSOP                          { $$ = new NotImplemented("ArithmeticUnaryOperator"); }
    |   MINUSOP                         { $$ = new NotImplemented("ArithmeticUnaryOperator"); }
    ;
                            
// Expr -> PriExpr
PriExpr           
    :   QualName                   { $$ = new EvalExpr($1);}   
    |   QualPriExpr            { $$ = new EvalExpr($1);}
    ;

// Expr -> PriExpr -> QualPriExpr
QualPriExpr                 
    :   SpecialBuiltin              { $$ = $1; }
    |   CxPriTypes             { $$ = $1; }
    ;

// Expr -> PriExpr -> QualPriExpr -> SpecialBuiltin
SpecialBuiltin                 
    :   THIS                            { $$ = TypeNode.TypeNodeThis;}
    |   NULL                            { $$ = TypeNode.TypeNodeNull;}
    ;

// Expr -> PriExpr -> QualPriExpr -> CxPriTypes
CxPriTypes              
    :   LPAREN Expr RPAREN        { $$ = $2; }
    |   CxPriExpr                  { $$ = $1;}
    ;

// Expr -> PriExpr -> QualPriExpr -> CxPriTypes -> CxPriExpr
CxPriExpr 
    :   STR_LITERAL                     { $$ = $1; }
    |   Number                          { $$ = $1; }
    |   FieldAccess                     { $$ = $1; }    
    |   MethodCall                      { $$ = $1; }    
    ;

// Expr -> PriExpr -> QualPriExpr -> CxPriTypes -> CxPriExpr -> FieldAccess
FieldAccess                 
    :   QualPriExpr PERIOD Identifier   
                                        { $$ = new NotImplemented("FieldAccess");}   
    ;       

// Expr -> PriExpr -> QualPriExpr -> CxPriTypes -> CxPriExpr -> MethodCall
MethodCall                  
    :   MethodReference LPAREN ArgumentList RPAREN
                                        { $$ = new MethodCall($1, $3); }
    |   MethodReference LPAREN RPAREN   { $$ = new MethodCall($1); }
    ;

// Expr -> PriExpr -> QualPriExpr -> CxPriTypes -> CxPriExpr -> MethodCall -> MethodReference
MethodReference             
    :   CxPriExpr                  { $$ = $1;}
    |   QualName                   { $$ = $1;}
    |   SpecialBuiltin              { $$ = $1;} // Expr -> EvalExpr -> QualPriExpr -> SpecialBuiltin
    |   BuiltinSystemCall               { $$ = $1;}
    ;

BuiltinSystemCall                 
    :   WRITE                           { $$ = $1;}
    |   WRITE_LINE                      { $$ = $1;}
    ;


Identifier                  
    :   IDENTIFIER                      { $$ = $1; }
    ;

Number                      
    :   INT_NUMBER                      { $$ = $1;}
    ;

%%

