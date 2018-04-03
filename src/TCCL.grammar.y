%namespace CompilerILGen
%using CompilerILGen.AST;
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
%token B_AND ASTERISK BANG BOOLEAN CLASS NAMESPACE
%token COLON COMMA ELSE EQUALS B_XOR
%token IF INSTANCEOF INT STRING IDENTIFIER STR_LITERAL INT_NUMBER
%token LBRACE LBRACKET LPAREN MINUSOP
%token NEW NULL OP_EQ OP_GE OP_GT
%token OP_LAND OP_LE OP_LOR OP_LT OP_NE
%token PERCENT PERIOD B_OR PLUSOP PRIVATE
%token PUBLIC QUESTION RBRACE RBRACKET RETURN
%token RPAREN RSLASH SEMICOLON STATIC STRUCT
%token SUPER THIS TILDE VOID WHILE

%nonassoc RP
%nonassoc ELSE

%right EQUALS
%left  OP_LOR
%left  OP_LAND
%left  B_OR
%left  B_XOR
%left  B_AND
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
    |   ClassDecl           { $$ = $1; }
    |   FuncDecl            { $$ = $1; }    // done
    |   LocalDecl           { $$ = $1; }
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
    |   ClassDecl    { $$ = $1; }   
    ;


//  Used in the class and field declarations
Modifiers           
    :   PUBLIC              { $$ = new Modifiers(ModifierType.PUBLIC);}
    |   PRIVATE             { $$ = new Modifiers(ModifierType.PRIVATE);}
    |   STATIC              { $$ = new Modifiers(ModifierType.STATIC);}
    |   Modifiers PUBLIC    { ((Modifiers)$1).AddModType(ModifierType.PUBLIC); $$ = $1;}
    |   Modifiers PRIVATE   { ((Modifiers)$1).AddModType(ModifierType.PRIVATE); $$ = $1;}
    |   Modifiers STATIC    { ((Modifiers)$1).AddModType(ModifierType.STATIC); $$ = $1;}
    ;


// --------------------------------------------------
// CLASS STUFF
// --------------------------------------------------

//  ClassDecl
ClassDecl    
    :   Modifiers CLASS Identifier ClassBody 
                            { $$ = new ClassDeclaration($1 as Modifiers, $3 as Identifier, $4 as ClassBody);}
    ;

//  ClassDecl -> ClassBody    
ClassBody           
    :   LBRACE ClassBodyItems RBRACE        { $$ = $2;}
    |   LBRACE RBRACE                       { $$ = new ClassBody();}
    ;

// ClassDecl -> ClassBody -> ClassBodyItems
ClassBodyItems   
    :   ClassBodyDecl                       { $$ = new ClassBody($1); }
    |   ClassBodyItems ClassBodyDecl        { $1.AddChild($2); $$ = $1;}
    ;

// ClassDecl -> ClassBody -> ClassBodyDecl
ClassBodyDecl    
    :   ClassFieldDecl SEMICOLON    { $$ = $1; }    
    |   ClassMethodDecl             { $$ = $1; }
    |   ConstructorDeclaration      { $$ = $1; }
    |   StaticInitializer           { $$ = $1; }
    |   StructDecl                  { $$ = $1; }
    ;

// ClassDecl -> ClassBody -> ClassBodyDecl -> ClassFieldDecl
ClassFieldDecl    
    :   Modifiers TypeRef FieldVarDecls    
                                    { $$ = new ClassFieldDeclStatement($1, new LocalVarDecl($2 as TypeRefNode, $3 as VarDeclList)); }
    ;

// ClassDecl -> ClassBody -> ClassBodyDecl -> ClassFieldDecl -> FieldVarDecls
FieldVarDecls    
    :   FieldVarDeclName            { $$ = new VarDeclList($1 as VarDecl); }
    |   FieldVarDecls COMMA FieldVarDeclName    
                                    { $$.AddChild($3); $$ = $1;}
    ;

// ClassDecl -> ClassBody -> ClassBodyDecl -> ClassFieldDecl -> FieldVarDecls -> FieldVarDeclName
FieldVarDeclName 
    :   Identifier                  { $$ = new VarDecl($1 as Identifier); }
    ;

// ClassDecl -> ClassBody -> ClassBodyDecl -> ClassMethodDecl
ClassMethodDecl           
    :   Modifiers FuncDecl          { $$ = new ClassMethodDecl($1 as Modifiers, $2 as FuncDecl); }
    |   FuncDecl                    { $$ = new ClassMethodDecl($1 as FuncDecl); }       // private non-static by default
    ;

// ClassDecl -> ClassBody -> ClassBodyDecl -> ConstructorDeclaration
ConstructorDeclaration      
    :   Modifiers Identifier LPAREN ParamList RPAREN Block    
                                    { $$ = new NotImplemented("ConstructorDeclaration"); }
    ;

// ClassDecl -> ClassBody -> ClassBodyDecl    STATICINIT?? .. i dunno what this is
StaticInitializer           
    :   STATIC Block                { $$ = new StaticInitializer($2); }
    ;

// ClassDecl -> ClassBody -> ClassBodyDecl -> StructDecl    (Can also be local)
StructDecl   
    :   Modifiers STRUCT Identifier ClassBody   
                                    { $$ = new NotImplemented("StructDeclaration"); }
    ;



// TYPE STUFF HERE
// ==========================

// TypeRef
TypeRef               
    :   TypeName                    { $$ = $1; }
    |   ArraySpecifier              { $$ = $1; }
    ;

// TypeRef -> TypeName
TypeName                    
    :   PrimitiveType               { $$ = $1; }
    |   QualifiedType               { $$ = $1; }
    ;

// TypeRef -> TypeName -> PrimitiveType
PrimitiveType               
    :   BOOLEAN                     { $$ = TypeRefNode.TypeNodeBoolean; }
    |   INT                         { $$ = TypeRefNode.TypeNodeInt; }
    |   STRING                      { $$ = TypeRefNode.TypeNodeString; }
    |   VOID                        { $$ = TypeRefNode.TypeNodeVoid; }
    ;

// TypeRef -> TypeName -> QualifiedType
QualifiedType               
    :   Identifier                      { $$ = new QualifiedType($1 as Identifier);}
    |   QualifiedType PERIOD Identifier { ($$ as QualifiedType).AddChild($3 as Identifier); $$ = $1; }
    ;

// TypeRef -> ArraySpecifier
ArraySpecifier              
    :   TypeName LBRACKET RBRACKET  { $$ = new NotImplemented("Arrays not supported"); }
    ;
                            

// FUNCTION DECLARATION
// ------------------------------------------------------------

FuncDecl
    :   TypeRef FuncDeclName LPAREN ParamList RPAREN FuncBody
                                    { $$ = new FuncDecl($1 as TypeRefNode, $2 as Identifier, $4 as ParamList, $6 as Block); } 
    |   TypeRef FuncDeclName LPAREN RPAREN FuncBody
                                    { $$ = new FuncDecl($1 as TypeRefNode, $2 as Identifier, null , $5 as Block); } 
    ;

ParamList               
    :   ParamDecl                   { $$ = new ParamList($1); }
    |   ParamList COMMA ParamDecl   { $1.AddChild($3); $$ = $1;}  
    ;
	 
ParamDecl                   
    :   TypeRef ParamName          { $$ = new ParamDecl($1 as TypeRefNode, $2 as Identifier); }
    ;


FuncDeclName    :   Identifier      { $$ = $1; } ;
ParamName       :   Identifier      { $$ = $1; } ;
FuncBody        :   Block           { $$ = $1; } ;
        
/*
 * These can't be reorganized, because the order matters.
 * For example
    :  int i;  i = 5;  int j = i;
 */

Block                       
    :   LBRACE BlockItems RBRACE    { $$ = $2; }
    |   LBRACE RBRACE               { $$ = new Block(); }
    ;

BlockItems  
    :   LocalDeclOrStmt             { $$ = new Block($1);}
    |   BlockItems LocalDeclOrStmt  { $1.AddChild($2); $$ = $1; }
    ;

LocalDeclOrStmt 
    :   LocalDecl                   { $$ = $1;}
    |   Stmt                        { $$ = $1;}
    ;

// LocalDeclOrStmt -> LocalDecl
LocalDecl       
    :   TypeRef VarDeclList SEMICOLON  { $$ = new LocalVarDecl($1 as TypeRefNode, $2 as VarDeclList);}
    |   StructDecl                      { $$ = new NotImplemented("struct decl not supported");}
    ;

// LocalDeclOrStmt -> LocalDecl -> VarDeclList
VarDeclList    
    :   VarDecl                     { $$ = new VarDeclList($1 as VarDecl); }
    |   VarDeclList COMMA VarDecl   { $1.AddChild($3); $$ = $1; }
    ;

VarDecl         :   Identifier      { $$ = new VarDecl($1 as Identifier); } ;

// LocalDeclOrStmt -> LocalDecl -> Stmt
Stmt                   
    :   EmptyStmt                   { $$ = $1; }
    |   BuiltinStmt                 { $$ = $1; } 
    |   ExpressionStmt SEMICOLON    { $$ = $1; }
    |   IfStmt                      { $$ = $1; }
    |   WhileStmt                   { $$ = $1; }
    |   ReturnStmt                  { $$ = $1; }
    |   Block                       { $$ = $1; }
    ;

// Stmt -> EmptyStmt
EmptyStmt       :   SEMICOLON       { $$ = new EmptyStatement();} ;

// Stmt -> ExpressionStmt
ExpressionStmt  :   Expr            { $$ = $1; } ;

/* You will eventually have to address the shift/reduce error that
 * occurs when the second IF-rule is uncommented.  */

// Stmt -> IfStmt
IfStmt          
    :   IF LPAREN Expr RPAREN Stmt ELSE Stmt    { $$ = new IfStatementElse($3,$5,$7); }
    |   IF LPAREN Expr RPAREN Stmt              { $$ = new IfStatement($3,$5); }
    ;

// Stmt -> WhileStmt
WhileStmt          
    :   WHILE LPAREN Expr RPAREN Stmt   { $$ = new WhileLoop($3, $5); }
    ;

// Stmt -> ReturnStmt
ReturnStmt         
    :   RETURN Expr SEMICOLON           { $$ = new ReturnStatement($2); }
    |   RETURN SEMICOLON                { $$ = new ReturnStatement(); }
    ;

BuiltinStmt
    :   WRITE BuiltinStmtArgs SEMICOLON         { $$ = new WriteStatement($2 as ArgumentList);}
    |   WRITE_LINE BuiltinStmtArgs SEMICOLON    { $$ = new WriteLineStatement($2 as ArgumentList);}
    ;

BuiltinStmtArgs
    :   LPAREN CallArgList RPAREN       { $$ = $2; }
    |   LPAREN RPAREN                   { $$ = null; }
    ;


// ------------------------------------------------------------
// Expressions
// ------------------------------------------------------------

Expr                  
    :   LValue EQUALS Expr      { $$ = new AssignExpr($1, $3); }
    |   Expr OP_LOR Expr        { $$ = new CompExpr($1, ExprType.LOGICAL_OR, $3); }   /* short-circuit OR  */  
    |   Expr OP_LAND Expr       { $$ = new CompExpr($1, ExprType.LOGICAL_AND, $3); }   /* short-circuit AND */  
    |   Expr B_OR Expr          { $$ = new BinaryExpr($1, ExprType.B_OR, $3); }                
    |   Expr B_XOR Expr         { $$ = new BinaryExpr($1, ExprType.B_XOR, $3); }                
    |   Expr B_AND Expr         { $$ = new BinaryExpr($1, ExprType.B_AND, $3); }                
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
    |   ArithmeticUnaryOperator Expr  %prec UNARY { $$ = new NotImplemented("ArithmeticUnaryOperator Expr  %prec UNARY"); }
    |   LValue                  { $$ = new EvalExpr($1 as ExprNode);}   
    |   SpecialBuiltin          { $$ = new TypeExpr($1 as TypeRefNode);}
    |   LPAREN Expr RPAREN      { $$ = new EvalExpr($2 as ExprNode);}
    |   CxEvalExpr              { $$ = new EvalExpr($1 as ExprNode);}
    ;

LValue               
    :   Identifier                  { $$ = new LValueNode($1 as Identifier);}
    |   Expr PERIOD Identifier      { $$ = new LValueNode($1 as ExprNode, $3 as Identifier); }
    ;

ArithmeticUnaryOperator     
    :   PLUSOP                  { $$ = new NotImplemented("ArithmeticUnaryOperator"); }
    |   MINUSOP                 { $$ = new NotImplemented("ArithmeticUnaryOperator"); }
    ;
                            
// Expr -> QualPriExpr
QualPriExpr                 
    :   SpecialBuiltin          { $$ = $1; }
    |   LPAREN Expr RPAREN      { $$ = $2; }
    |   CxEvalExpr              { $$ = $1;}
    ;

// Expr -> QualPriExpr -> SpecialBuiltin
SpecialBuiltin                 
    :   THIS                    { $$ = TypeRefNode.TypeNodeThis;}
    |   NULL                    { $$ = TypeRefNode.TypeNodeNull;}
    ;

// Expr -> QualPriExpr -> CxEvalExpr
CxEvalExpr 
    :   STR_LITERAL             { $$ = $1; }
    |   Number                  { $$ = $1; }
    |   FieldAccess             { $$ = $1; }    
    |   MethodCall              { $$ = $1; }    
    ;

// Expr -> QualPriExpr -> CxEvalExpr -> FieldAccess
FieldAccess                 
    :   QualPriExpr PERIOD Identifier   
                                { $$ = new NotImplemented("FieldAccess");}   
    ;       

// Expr -> QualPriExpr -> CxEvalExpr -> MethodCall
MethodCall                  
    :   MethodRef LPAREN CallArgList RPAREN     { $$ = new MethodCall($1 as QualifiedType, $3 as ArgumentList); }
    |   MethodRef LPAREN RPAREN                 { $$ = new MethodCall($1 as QualifiedType); }
    ;

CallArgList            
    :   Expr                    { $$ = new ArgumentList($1 as ExprNode); }
    |   CallArgList COMMA Expr  { $1.AddChild($3); $$ = $1; }
    ;

// Expr -> QualPriExpr -> CxEvalExpr -> MethodCall -> MethodRef
MethodRef             
    :   CxEvalExpr              { $$ = $1;}
    |   QualifiedType           { $$ = $1;} // This can only be a type!
    |   SpecialBuiltin          { $$ = $1;} 
    ;


Identifier  :   IDENTIFIER      { $$ = $1; } ;
Number      :   INT_NUMBER      { $$ = $1;} ;

%%

