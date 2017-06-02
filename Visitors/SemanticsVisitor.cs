using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.AST;

namespace Proj3Semantics
{

    using IEnv = ISymbolTable<Symbol>;


    public class SemanticsVisitor
    {
        protected static Logger Log = LogManager.GetCurrentClassLogger();

        public IEnv Env { get; set; }

        public SemanticsVisitor(IEnv env) { Env = env; }

        public void Visit(dynamic node)
        {
            if (node == null) return;
            VisitNode(node);
        }

        // continue checking the tree until we find something interesting
        private void VisitNode(Node node)
        {
            Log.Trace("visiting children of" + node.ToDebugString());
            foreach (Node child in node.Children)
                Visit(child);
        }

        private void VisitNode(ClassDeclaration cdecl)
        {
            Log.Trace("Semantic check class: " + cdecl);
            if (cdecl.ClassBody == null) return;

            var visitor = new SemanticsVisitor(cdecl.Env);
            foreach (Node n in cdecl.ClassBody.Children)
                visitor.Visit(n);
        }

        private void VisitNode(AbstractFuncDecl fdecl)
        {
            Log.Trace("Type checking method decl " + fdecl.ToDebugString());
            string name = fdecl.Identifier.Name;

            Log.Trace("  -- Visit ReturnTypeSpecifier");
            Visit(fdecl.ReturnTypeSpecifier);

            var visitor = new SemanticsVisitor(fdecl.Env);

            // TODO
            Log.Trace("  -- Visit ParamListNode");
            visitor.Visit(fdecl.ParamList);

            Log.Trace("  -- Visit Body");
            visitor.Visit(fdecl.MethodBody);
        }

        private void VisitNode(LiteralExpr builtin)
        {
            Log.Trace("Type checking a literal, no action.");
        }
        private void VisitNode(BuiltinType builtin)
        {
            Log.Trace("Type checking builtin, no action.");
        }

        private void VisitNode(QualifiedNode qnode)
        {
            Log.Trace("Type checking qualified type " + qnode.ToString());
            var curEnv = Env;
            string curScopeName = "";
            Symbol curSymbol = null;

            foreach (string curIdStr in qnode.IdentifierList)
            {
                var results = curEnv.Lookup(curIdStr);
                if (results == null || results.Count != 1)
                {
                    string errMsg = curIdStr;
                    if (curScopeName != "")
                        errMsg += " in " + curScopeName;
                    CompilerErrors.Add(SemanticErrorTypes.UndeclaredIdentifier, errMsg);
                    qnode.NodeTypeCategory = NodeTypeCategory.ErrorType;
                    return;

                }
                curSymbol = results.First();
                curEnv = curSymbol.Env;
                curScopeName = curIdStr;
            }
            if (curSymbol != null)
            {
                qnode.SymbolRef = curSymbol;
                qnode.NodeTypeCategory = curSymbol.DeclNode.DeclTypeNode.NodeTypeCategory;
            }
        }

        private void VisitNode(LocalVarDecl decl)
        {
            Log.Trace("Type checking " + nameof(LocalVarDecl) + ": TypeSpecifier");
            Visit(decl.TypeSpecifier);
        }

        private void VisitNode(AssignExpr assn)
        {
            Log.Trace("Type checking " + nameof(AssignExpr));

            Visit(assn.LhsQual);
            QualifiedNode lval = assn.LhsQual;
            Debug.Assert(lval != null);
            if (lval.NodeTypeCategory == NodeTypeCategory.ErrorType)
            {
                assn.EvalType = TypeNode.TypeNodeError;
                return;
            }
            TypeNode ltype = lval.SymbolRef.DeclNode.DeclTypeNode;

            Visit(assn.RhsExprNode);
            TypeNode rtype = assn.RhsExprNode.EvalType;
            Debug.Assert(rtype != null);
            if (rtype.NodeTypeCategory == NodeTypeCategory.ErrorType)
            {
                assn.EvalType = TypeNode.TypeNodeError;
                return;
            }


            if (!rtype.CanConvertTo(ltype))
            {
                assn.EvalType = TypeNode.TypeNodeError;
                CompilerErrors.Add(SemanticErrorTypes.IncompatibleAssignment, rtype + " to " + ltype);
                return;
            }

        }


        private void VisitNode(ExprNode expr)
        {
            // If this function is run, that means that 
            // we need to impolement a proper VisitNode for its subclass 
            throw new NotImplementedException();
        }

        private void VisitNode(CompExpr compExpr)
        {
            Log.Trace("Type checking " + nameof(CompExpr));
            ExprNode lft = compExpr.LhsExprNode;
            Visit(lft);
            if (lft.EvalType.NodeTypeCategory == NodeTypeCategory.ErrorType)
            {
                compExpr.EvalType = TypeNode.TypeNodeError;
                return;
            }

            ExprNode rgt = compExpr.RhsExprNode;
            Visit(rgt);
            if (rgt.EvalType.NodeTypeCategory == NodeTypeCategory.ErrorType)
            {
                compExpr.EvalType = TypeNode.TypeNodeError;
                return;
            }

            if (IsArithmeticCompatible(lft.EvalType, rgt.EvalType))
            {
                compExpr.EvalType = TypeNode.TypeNodeBoolean;
            }
            else
            {
                CompilerErrors.Add(SemanticErrorTypes.IncompatibleOperands, compExpr.ExprType.ToString());
                compExpr.EvalType = TypeNode.TypeNodeError;
            }
        }

        private void VisitNode(BinaryExpr binaryExpr)
        {
            Log.Trace("Type checking " + nameof(BinaryExpr));
            ExprNode lft = binaryExpr.LhsExprNode;
            Visit(lft);

            if (lft.EvalType.NodeTypeCategory == NodeTypeCategory.ErrorType)
            {
                binaryExpr.EvalType = TypeNode.TypeNodeError;
                return;
            }

            var rgt = binaryExpr.RhsExprNode;
            Visit(rgt);

            if (rgt.EvalType.NodeTypeCategory == NodeTypeCategory.ErrorType)
            {
                binaryExpr.EvalType = TypeNode.TypeNodeError;
                return;
            }

            if (IsArithmeticCompatible(lft.EvalType, rgt.EvalType))
            {
                // for now, just return the type of one of the operands, since we know they will be same
                binaryExpr.EvalType = lft.EvalType;
            }
            else
            {
                CompilerErrors.Add(SemanticErrorTypes.IncompatibleOperands, binaryExpr.ExprType.ToString());
                binaryExpr.EvalType = TypeNode.TypeNodeError;
            }
        }


        private void VisitNode(EvalExpr expr)
        {
            Log.Trace("Checking Evaluation of " + expr.ToDebugString());
            var child = expr.Child;
            Visit(child);

            // TODO TODO
            QualifiedNode qnode = child as QualifiedNode;
            if (qnode != null)
            {
                expr.EvalType = qnode;
                return;
            }

            ExprNode e = child as ExprNode;
            if (e != null)
            {
                expr.EvalType = e.EvalType;
                return;
            }

            CompilerErrors.Add(SemanticErrorTypes.FeatureNotImplemented, "Eval expression of " + child);
            expr.EvalType = TypeNode.TypeNodeError;
        }

        private List<AbstractFuncDecl> GetMethodOverloads(QualifiedNode qnode)
        {
            if (qnode.IdentifierList.Count != 1)
            {
                throw new NotImplementedException("TODO: only simple function calls supported");
            }
            string fname = qnode.IdentifierList.First();

            Log.Trace("Looking up method overload set for " + fname);

            return Env
                .Lookup(fname)
                .Where(sym => sym.SymbolType == SymbolType.Function)
                .Select(sym => sym.DeclNode)
                .Cast<AbstractFuncDecl>()
                .ToList();
        }

        private bool ArgumentsCompatible(ParamList parameters, List<ExprNode> arguments)
        {
            int num_params = parameters?.ParamDeclList?.Count ?? 0;
            int num_args = arguments.Count;
            if (num_args != num_params) return false;
            if (num_args == 0) return true;

            // case when both are same count and more than 0
            var tuples = Enumerable.Zip(parameters.ParamDeclList, arguments, Tuple.Create);
            foreach (Tuple<ParamDecl, ExprNode> t in tuples)
            {
                if (t.Item1.DeclTypeNode != t.Item2.EvalType)
                    return false;
            }
            return true;
        }

        // MethodReference             
        //       :   ComplexPrimaryNoParenthesis     { $$ = $1;}
        //       |   QualifiedName                   { $$ = $1;}
        //       |   SpecialBuiltinName              { $$ = $1;}
        //       |   BuiltinSystemCall               { $$ = $1;}

        // just checking the qualifiednamenode and SystemCall for now

        private void VisitNode(MethodCall call)
        {
            Log.Trace("Start checking MethodCall " + call.ToDebugString());

            QualifiedNode qualNode = call.MethodReference as QualifiedNode;
            if (qualNode == null)
                CompilerErrors.Add(SemanticErrorTypes.FeatureNotImplemented, "Only support calling user-defined functions.");


            // evaluate the arguments
            List<ExprNode> argExpressions
                = call.ArgumentList?.Children?.Cast<ExprNode>()?.ToList() ?? new List<ExprNode>();
            foreach (ExprNode exprNode in argExpressions)
            {
                Visit(exprNode);
                if (exprNode.EvalType == TypeNode.TypeNodeError)
                {
                    call.EvalType = TypeNode.TypeNodeError;
                    return;
                }
            }


            var candidateSet = GetMethodOverloads(qualNode);
            var matchedSet = candidateSet.Where(f => ArgumentsCompatible(f.ParamList, argExpressions)).ToList();
            var numMatched = matchedSet.Count;

            if (numMatched == 0)
            {
                // TODO: better error message text
                CompilerErrors.Add(SemanticErrorTypes.InvalidFuncArg, "Unable to match a function call signature.");
                call.EvalType = TypeNode.TypeNodeError;
                return;
            }

            if (numMatched > 1)
            {
                // TODO: better error message text
                CompilerErrors.Add(SemanticErrorTypes.InvalidFuncArg, "Ambiguous function call");
                call.EvalType = TypeNode.TypeNodeError;
                return;
            }

            var fdecl = matchedSet.First();
            call.EvalType = fdecl.ReturnTypeSpecifier;
        }


        private void VisitNode(IfStatementElse ifStatementElse)
        {
            CheckBoolean(ifStatementElse.Predicate);
            Visit(ifStatementElse.ThenStatement);
            Visit(ifStatementElse.ElseStatement);
        }

        private void VisitNode(IfStatement ifStatement)
        {
            CheckBoolean(ifStatement.Predicate);
            Visit(ifStatement.ThenStatement);
        }

        private void VisitNode(WhileLoop loop)
        {
            CheckBoolean(loop.Predicate);
            Visit(loop.BodyStatement);
        }

        // HELPERS
        // ------------------------------------------------------------
        // ------------------------------------------------------------

        public bool IsAssignable(Object dst, Object src)
        {
            //if (dst == null || src == null) return false;

            //switch (src.NodeTypeCategory)

            //{
            //    //case NodeTypeCategory.Primitive:
            //    //    // for now, only can assign things from primitives
            //    //    break;
            //    //case NodeTypeCategory.NOT_SET:
            //    //    return false;
            //    //case NodeTypeCategory.Void:
            //    //    return false;
            //    //default:
            //    //    _log.Warn("RHS in IsAssignable is not implemented " + src.NodeTypeCategory.ToString());
            //    //    return false;
            //}


            //switch (dst.NodeTypeCategory)
            //{
            //    //case NodeTypeCategory.NOT_SET:
            //    //    throw new NotImplementedException("???");
            //    //case NodeTypeCategory.Primitive:
            //    //    return IsPrimitiveAssignable(dst.TypeDescriptorRef as IPrimitiveTypeDescriptor, src.TypeDescriptorRef);
            //    //case NodeTypeCategory.Null:
            //    //    CompilerErrors.Add(SemanticErrorTypes.BuiltinNotAssignable, "null");
            //    //    return false;
            //    //case NodeTypeCategory.Void:
            //    //    CompilerErrors.Add(SemanticErrorTypes.BuiltinNotAssignable, "void");
            //    //    return false;
            //    //case NodeTypeCategory.This:
            //    //    CompilerErrors.Add(SemanticErrorTypes.BuiltinNotAssignable, "this");
            //    //    return false;
            //    //case NodeTypeCategory.Error:
            //    //    return true;    // can always assign to an error type
            //    //default:
            //    //    CompilerErrors.Add(SemanticErrorTypes.FeatureNotImplemented, dst.NodeTypeCategory.ToString());
            //    //    return false;
            //}

            //// TODO: fix
            return false;
        }

        private void CheckBoolean(ExprNode expr)
        {
            //Visit(expr);
            //if (expr.NodeTypeCategory == NodeTypeCategory.Primitive)
            //{
            //    var tref = expr.TypeDescriptorRef as IPrimitiveTypeDescriptor;
            //    if (tref == null || tref.VariablePrimitiveType != VariablePrimitiveType.Boolean)
            //    {
            //        CompilerErrors.Add(SemanticErrorTypes.BooleanExpected);
            //    }
            //}

        }

        private bool IsPrimitiveAssignable(Object dst, Object src)
        {
            //if (dst == null || src == null) return false;

            //IPrimitiveTypeDescriptor srcPrimitive = src.TypeDescriptorRef as IPrimitiveTypeDescriptor;
            //if (srcPrimitive == null) throw new NotImplementedException("Assignment from non-primitives is not implemented");

            //var tdst = dst.VariablePrimitiveType;
            //var tsrc = srcPrimitive.VariablePrimitiveType;

            //switch (tdst)
            //{
            //    case VariablePrimitiveType.Object:
            //        return true;    // object is the lowest
            //    default:
            //        return tdst == tsrc;
            //}


            // TODO
            return false;
        }

        private bool IsArithmeticCompatible(TypeNode arg1, TypeNode arg2)
        {
            if (arg1 == null || arg2 == null) return false;
            return arg1.NodeTypeCategory == arg2.NodeTypeCategory;
        }


        // Not doing classes anymore
        // =========================

        //private void VisitNode(ClassDeclaration cdecl)
        //{
        //    Log.Info("Found a class, visiting Methods of" + (cdecl as Node)?.ToDebugString());
        //    Visit(cdecl.Methods);
        //}

        //private void VisitNode(MethodDeclaration mdecl)
        //{
        //    //_log.Info("Found a class method body, visiting block " + (mdecl as Node)?.ToDebugString());
        //    //var blockVisitor = new TypeCheckingVisitor(mdecl);
        //    //blockVisitor.Visit(mdecl.MethodBody);
        //}




    }
}
