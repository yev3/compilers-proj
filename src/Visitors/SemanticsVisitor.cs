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

        private void VisitNode(QualifiedType qnode)
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

        private void VisitNode(LValueNode lval)
        {
            IEnv env;

            if (lval.LeftOfPeriodExpr != null)
            {
                LValueNode lft = lval.LeftOfPeriodExpr as LValueNode;
                if (lft == null)
                {
                    CompilerErrors.Add(SemanticErrorTypes.FeatureNotImplemented, "only support field access on named expressions");
                    lval.EvalType = TypeRefNode.TypeNodeError;
                    return;
                }

                Visit(lft);
                var declNode = lft.SymbolRef?.DeclNode;
                var declSymbol = declNode?.DeclTypeNode as QualifiedType;
                env = declSymbol?.SymbolRef?.Env;
                if (env == null)
                {
                    throw new NullReferenceException();
                }
            }
            else
            {
                env = Env;
            }


            string name = lval.Identifier.Name;
            var symbols = env.Lookup(name);
            if (symbols == null || symbols.Count != 1)
            {
                CompilerErrors.Add(SemanticErrorTypes.UndeclaredIdentifier, "Invalid variable reference " + name);
                lval.EvalType = TypeRefNode.TypeNodeError;
                return;
            }

            lval.SymbolRef = symbols.First();
            lval.EvalType = lval.SymbolRef.DeclNode.DeclTypeNode;
        }


        private void VisitNode(LocalVarDecl decl)
        {
            Log.Trace("Type checking " + nameof(LocalVarDecl) + ": TypeSpecifier");
            Visit(decl.TypeSpecifier);
        }

        private void VisitNode(AssignExpr assn)
        {
            Log.Trace("Type checking " + nameof(AssignExpr));

            Visit(assn.LValueNode);
            LValueNode lval = assn.LValueNode;
            Debug.Assert(lval != null);
            if (lval.EvalType == TypeRefNode.TypeNodeError)
            {
                assn.EvalType = TypeRefNode.TypeNodeError;
                return;
            }

            TypeRefNode ltype = lval.EvalType;

            Visit(assn.RhsExprNode);
            TypeRefNode rtype = assn.RhsExprNode.EvalType;
            Debug.Assert(rtype != null);
            if (rtype.NodeTypeCategory == NodeTypeCategory.ErrorType)
            {
                assn.EvalType = TypeRefNode.TypeNodeError;
                return;
            }


            if (!rtype.CanConvertTo(ltype))
            {
                assn.EvalType = TypeRefNode.TypeNodeError;
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
                compExpr.EvalType = TypeRefNode.TypeNodeError;
                return;
            }

            ExprNode rgt = compExpr.RhsExprNode;
            Visit(rgt);
            if (rgt.EvalType.NodeTypeCategory == NodeTypeCategory.ErrorType)
            {
                compExpr.EvalType = TypeRefNode.TypeNodeError;
                return;
            }

            if (IsArithmeticCompatible(lft.EvalType, rgt.EvalType))
            {
                compExpr.EvalType = TypeRefNode.TypeNodeBoolean;
            }
            else
            {
                CompilerErrors.Add(SemanticErrorTypes.IncompatibleOperands, compExpr.ExprType.ToString());
                compExpr.EvalType = TypeRefNode.TypeNodeError;
            }
        }

        private void VisitNode(BinaryExpr binaryExpr)
        {
            Log.Trace("Type checking " + nameof(BinaryExpr));
            ExprNode lft = binaryExpr.LhsExprNode;
            Visit(lft);

            if (lft.EvalType.NodeTypeCategory == NodeTypeCategory.ErrorType)
            {
                binaryExpr.EvalType = TypeRefNode.TypeNodeError;
                return;
            }

            var rgt = binaryExpr.RhsExprNode;
            Visit(rgt);

            if (rgt.EvalType.NodeTypeCategory == NodeTypeCategory.ErrorType)
            {
                binaryExpr.EvalType = TypeRefNode.TypeNodeError;
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
                binaryExpr.EvalType = TypeRefNode.TypeNodeError;
            }
        }


        private void VisitNode(EvalExpr expr)
        {
            Log.Trace("Checking Evaluation of " + expr.ToDebugString());
            Visit(expr.ChildExpr);
            expr.EvalType = expr.ChildExpr.EvalType;
            Debug.Assert(expr.EvalType != null);
        }

        private List<Symbol> GetMethodOverloads(MethodRef mref)
        {
            Log.Trace("Looking up method overloads for " + mref.ToString());
            IEnv curEnv;

            if (mref.ExprNode != null)
            {
                // figure out the env we are looking up the functions in
                Visit(mref.ExprNode);

                var symRef = (mref.ExprNode as LValueNode)?.SymbolRef;
                curEnv = symRef?.Env;

                if (curEnv == null)
                {
                    // looking up an instance of an object
                    var varDecl = symRef?.DeclNode as VarDecl;
                    if (varDecl != null)
                    {
                        var qType = varDecl.DeclTypeNode as QualifiedType;
                        curEnv = qType?.SymbolRef?.Env;
                    }
                }

                if (curEnv == null)
                {
                    CompilerErrors.Add(SemanticErrorTypes.IdentifierNotTypeName, mref.ExprNode.ToString());
                    return new List<Symbol>();
                }
            }
            else
            {
                curEnv = Env;
            }

            string fname = mref.Identifier.Name;
            Log.Trace("Looking up method overload set for " + fname + " in " + mref);
            return curEnv
                .Lookup(fname)
                .Where(sym => sym.SymbolType == SymbolType.Function)
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


        private void VisitNode(MethodCall call)
        {
            Log.Trace("Start checking MethodCall " + call.ToDebugString());

            // evaluate the arguments
            List<ExprNode> argExpressions
                = call.ArgumentList?.Children?.Cast<ExprNode>()?.ToList() ?? new List<ExprNode>();
            foreach (ExprNode exprNode in argExpressions)
            {
                Visit(exprNode);
                if (exprNode.EvalType == TypeRefNode.TypeNodeError)
                {
                    call.EvalType = TypeRefNode.TypeNodeError;
                    return;
                }
            }


            var candidateSet = GetMethodOverloads(call.MethodRef);
            var matchedSet = candidateSet.Where(f => ArgumentsCompatible((f.DeclNode as AbstractFuncDecl)?.ParamList, argExpressions)).ToList();
            var numMatched = matchedSet.Count;

            if (numMatched == 0)
            {
                // TODO: better error message text
                CompilerErrors.Add(SemanticErrorTypes.InvalidFuncArg, "Unable to match a function call signature.");
                call.EvalType = TypeRefNode.TypeNodeError;
                return;
            }

            if (numMatched > 1)
            {
                // TODO: better error message text
                CompilerErrors.Add(SemanticErrorTypes.InvalidFuncArg, "Ambiguous function call");
                call.EvalType = TypeRefNode.TypeNodeError;
                return;
            }

            var symRef = matchedSet.First();
            var funcDecl = symRef.DeclNode as AbstractFuncDecl;
            if (funcDecl == null) throw new ArgumentNullException(nameof(funcDecl));

            call.MethodRef.AbstractFuncDecl = funcDecl;
            call.EvalType = funcDecl.ReturnTypeSpecifier;
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
            Visit(expr);
            if (expr.EvalType.NodeTypeCategory == NodeTypeCategory.Boolean)
            {
                var tref = expr.EvalType.NodeTypeCategory;
                if (tref == null || tref != NodeTypeCategory.Boolean)
                {
                    CompilerErrors.Add(SemanticErrorTypes.BooleanExpected);
                }
            }
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

        private bool IsArithmeticCompatible(TypeRefNode arg1, TypeRefNode arg2)
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
