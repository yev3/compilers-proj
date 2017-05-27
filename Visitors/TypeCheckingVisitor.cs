using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.ASTNodes;

namespace Proj3Semantics
{

    using IEnv = ISymbolTable<ITypeDescriptor>;


    public class TypeCheckingVisitor : IHasOwnScope
    {
        protected static Logger _log = LogManager.GetCurrentClassLogger();

        public IEnv NameEnv { get; set; }
        public IEnv TypeEnv { get; set; }

        public TypeCheckingVisitor(
            IEnv typeEnv,
            IEnv nameEnv)
        {
            NameEnv = nameEnv;
            TypeEnv = typeEnv;
        }

        public TypeCheckingVisitor(IHasOwnScope node)
        {
            NameEnv = node.NameEnv;
            TypeEnv = node.TypeEnv;
        }

        public void Visit(dynamic node)
        {
            if (node == null) return;

            VisitNode(node);
        }

        // continue checking the tree until we find something interesting
        private void VisitNode(AbstractNode node)
        {
            _log.Trace("visiting children of" + (node as AbstractNode)?.ToDebugString());
            foreach (AbstractNode child in node)
            {
                Visit(child);
            }
        }

        private void VisitNode(ClassDeclaration cdecl)
        {
            _log.Info("Found a class, visiting Methods of" + (cdecl as AbstractNode)?.ToDebugString());
            Visit(cdecl.Methods);
        }

        private void VisitNode(MethodDeclaration mdecl)
        {
            _log.Info("Found a class method body, visiting block " + (mdecl as AbstractNode)?.ToDebugString());
            var blockVisitor = new TypeCheckingVisitor(mdecl);
            blockVisitor.Visit(mdecl.MethodBody);
        }

        private void VisitNode(Identifier id)
        {
            _log.Trace("Visiting {0}.    **** TODO ****", id);
        }

        private void VisitNode(VariableListDeclaring vld)
        {
            var visitor = new DeclarationVisitor(this);
            visitor.Visit(vld);
        }




        private void VisitNode(Expression expr)
        {
            switch (expr.ExprType)
            {
                case ExprType.ASSIGNMENT:
                    VisitAssignment(expr);
                    return;
                case ExprType.EVALUATION:
                    VisitEvaluation(expr as EvalExpr);
                    return;
                case ExprType.EQUALS:
                case ExprType.LESS_THAN:
                case ExprType.GREATER_THAN:
                    VisitComparison(expr as BinaryExpr);
                    return;
                case ExprType.PLUSOP:
                case ExprType.LOGICAL_OR:
                case ExprType.LOGICAL_AND:
                case ExprType.PIPE:
                case ExprType.HAT:
                case ExprType.AND:
                case ExprType.NOT_EQUALS:
                case ExprType.LESS_EQUAL:
                case ExprType.GREATER_EQUAL:
                case ExprType.MINUSOP:
                case ExprType.ASTERISK:
                case ExprType.RSLASH:
                case ExprType.PERCENT:
                    VisitBinaryExpr(expr as BinaryExpr);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void VisitComparison(BinaryExpr binaryExpr)
        {
            var lft = binaryExpr.LhsExpression;
            Visit(lft);
            var rgt = binaryExpr.RhsExpression;
            Visit(rgt);

            if (IsArithmeticCompatible(lft, rgt))
            {
                binaryExpr.NodeTypeCategory = NodeTypeCategory.Primitive;
                binaryExpr.TypeDescriptorRef = new BuiltinTypeBoolean();
            }
            else
            {
                CompilerErrors.Add(SemanticErrorTypes.IncompatibleOperands, binaryExpr.ExprType.ToString());
                binaryExpr.NodeTypeCategory = NodeTypeCategory.ErrorType;
                binaryExpr.TypeDescriptorRef = null;
            }
        }

        private void VisitBinaryExpr(BinaryExpr binaryExpr)
        {
            switch (binaryExpr.ExprType)
            {
                case ExprType.PLUSOP:
                    VisitPlusOp(binaryExpr);
                    break;
                case ExprType.ASTERISK:
                    VisitPlusOp(binaryExpr);    // TODO: separate in code gen
                    break;
                case ExprType.ASSIGNMENT:
                    break;
                case ExprType.LOGICAL_OR:
                    break;
                case ExprType.LOGICAL_AND:
                    break;
                case ExprType.PIPE:
                    break;
                case ExprType.HAT:
                    break;
                case ExprType.AND:
                    break;
                case ExprType.EQUALS:
                    break;
                case ExprType.NOT_EQUALS:
                    break;
                case ExprType.GREATER_THAN:
                    break;
                case ExprType.LESS_THAN:
                    break;
                case ExprType.LESS_EQUAL:
                    break;
                case ExprType.GREATER_EQUAL:
                    break;
                case ExprType.MINUSOP:
                    VisitPlusOp(binaryExpr);    // TODO: separate in code gen
                    break;
                case ExprType.RSLASH:
                    VisitPlusOp(binaryExpr);    // TODO: separate in code gen
                    break;
                case ExprType.PERCENT:
                    break;
                case ExprType.UNARY:
                    break;
                case ExprType.EVALUATION:
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void NodeSetErrorType(ITypeDescriptor node)
        {
            node.NodeTypeCategory = NodeTypeCategory.ErrorType;
            node.TypeDescriptorRef = null;
        }

        private void VisitPlusOp(BinaryExpr binaryExpr)
        {
            var lft = binaryExpr.LhsExpression;
            Visit(lft);

            if (lft.NodeTypeCategory == NodeTypeCategory.ErrorType)
            {
                NodeSetErrorType(binaryExpr);
                return;
            }

            var rgt = binaryExpr.RhsExpression;
            Visit(rgt);

            if (rgt.NodeTypeCategory == NodeTypeCategory.ErrorType)
            {
                NodeSetErrorType(binaryExpr);
                return;
            }

            if (IsArithmeticCompatible(lft, rgt))
            {
                // for now, just return the type of one of the operands, since we know they will be same
                binaryExpr.TypeDescriptorRef = lft.TypeDescriptorRef;
                binaryExpr.NodeTypeCategory = lft.NodeTypeCategory;
            }
            else
            {
                CompilerErrors.Add(SemanticErrorTypes.IncompatibleOperands, binaryExpr.ExprType.ToString());
                NodeSetErrorType(binaryExpr);
            }
        }

        void VisitNode(BuiltInType node)
        {
            _log.Trace("Checking a primitive node, nothing needs to be done for " + (node as AbstractNode)?.ToDebugString());
        }

        private void VisitAssignment(Expression expr)
        {
            _log.Trace("Checking assignment of " + expr.ToDebugString());

            AssignExpr assnExpr = expr as AssignExpr;
            if (assnExpr == null) throw new ArgumentNullException(nameof(assnExpr));

            QualifiedName lhsQname = assnExpr.LhsQualName;
            var typeVisitor = new TypeVisitor(this);
            typeVisitor.Visit(lhsQname);

            ITypeDescriptor lhs = lhsQname.TypeDescriptorRef;
            if (lhs == null)
            {
                // the type visitor should have declared an error for us already
                NodeSetErrorType(expr);
                return;
            }


            Expression rhs = assnExpr.RhsExpression;
            Visit(rhs);

            if (rhs.NodeTypeCategory == NodeTypeCategory.ErrorType)
            {
                NodeSetErrorType(expr);
                return;
            }

            if (!IsAssignable(lhs, rhs))
            {
            string lhsName = string.Join(".", lhsQname.IdentifierList);
                CompilerErrors.Add(SemanticErrorTypes.IncompatibleAssignment, lhsName);
                //lhs.NodeTypeCategory = NodeTypeCategory.ErrorType;
                expr.NodeTypeCategory = NodeTypeCategory.ErrorType;
            }
            else
            {
                expr.NodeTypeCategory = lhs.NodeTypeCategory;
                expr.TypeDescriptorRef = lhs;
            }

        }
        private void VisitEvaluation(EvalExpr expr)
        {
            _log.Trace("Checking Evaluation of " + expr.ToDebugString());

            Visit(expr.Child);

            var childNode = expr.Child as ITypeDescriptor;
            if (childNode == null) throw new ArgumentNullException(nameof(childNode) + " should have a type");

            expr.NodeTypeCategory = childNode.NodeTypeCategory;
            expr.TypeDescriptorRef = childNode.TypeDescriptorRef;
        }

        private void VisitNode(MethodCall call)
        {
            _log.Trace("Start checking MethodCall " + call.ToDebugString());

            // MethodReference             
            //       :   ComplexPrimaryNoParenthesis     { $$ = $1;}
            //       |   QualifiedName                   { $$ = $1;}
            //       |   SpecialBuiltinName              { $$ = $1;}
            //       |   BuiltinSystemCall               { $$ = $1;}

            ITypeDescriptor methodRefDescriptor = call.MethodReference as ITypeDescriptor;
            if (methodRefDescriptor == null) throw new ArgumentNullException(nameof(methodRefDescriptor) + " is not a valid type descriptor");
            _log.Trace("    checking MethodReference " + call.MethodReference.ToDebugString());
            var typeVisitor = new TypeVisitor(this);
            typeVisitor.Visit(call.MethodReference);
            if (methodRefDescriptor.NodeTypeCategory == NodeTypeCategory.NOT_SET ||
                methodRefDescriptor.NodeTypeCategory == NodeTypeCategory.ErrorType ||
                methodRefDescriptor.TypeDescriptorRef == null)
            {
                _log.Trace("    Quitting method call type checking, should have emitted an error..");
                Debug.Assert(methodRefDescriptor.NodeTypeCategory == NodeTypeCategory.ErrorType);
                goto ErrorOccured;
            }

            IClassMethodTypeDesc methodDescriptor = methodRefDescriptor.TypeDescriptorRef as IClassMethodTypeDesc;
            if (methodDescriptor == null) throw new ArgumentNullException(nameof(methodDescriptor));

            List<Parameter> definedParams = methodDescriptor.MethodParameters;
            ArgumentList calledArgs = call.ArgumentList;

            // CHECKING # of args matches
            int numParams = definedParams.Count;
            int numArgs = calledArgs?.Count ?? 0;

            if (numParams != numArgs)
            {
                CompilerErrors.Add(SemanticErrorTypes.NoMethodWithNumArgs, methodDescriptor.Name);
                goto ErrorOccured;
            }

            if (numParams > 0)
            {
                _log.Trace("    checking ArgumentList " + calledArgs?.ToDebugString());
                Debug.Assert(calledArgs != null);

                IEnumerable<Tuple<Parameter, Expression>> pairs =
                    Enumerable.Zip(definedParams, calledArgs.Cast<Expression>(), Tuple.Create);

                foreach (Tuple<Parameter, Expression> pair in pairs)
                {
                    // type check the type of this argument expression
                    VisitNode(pair.Item2);

                    if (!IsAssignable(pair.Item1.TypeDescriptor, pair.Item2))
                    {
                        CompilerErrors.Add(SemanticErrorTypes.InvalidFuncArg, pair.Item1.Name);
                        goto ErrorOccured;
                    }
                }
            }
            else
            {
                _log.Trace("    No arguments");
            }

            call.NodeTypeCategory = methodDescriptor.ReturnTypeNode.NodeTypeCategory;
            call.TypeDescriptorRef = methodDescriptor.ReturnTypeNode.TypeDescriptorRef;
            _log.Trace("Finish checking MethodCall " + call.ToDebugString());

            return;

            // set the return to error type
            ErrorOccured:
            call.NodeTypeCategory = NodeTypeCategory.ErrorType;
            call.TypeDescriptorRef = null;
        }


        private void VisitNode(QualifiedName qn)
        {
            var tvisitor = new TypeVisitor(this);
            tvisitor.Visit(qn);
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


        // ------------------------------------------------------------
        // HELPERS
        // ------------------------------------------------------------

        private void CheckBoolean(Expression expr)
        {
            Visit(expr);
            if (expr.NodeTypeCategory == NodeTypeCategory.Primitive)
            {
                var tref = expr.TypeDescriptorRef as IPrimitiveTypeDescriptor;
                if (tref == null || tref.VariablePrimitiveType != VariablePrimitiveType.Boolean)
                {
                    CompilerErrors.Add(SemanticErrorTypes.BooleanExpected);
                }
            }

        }

        public bool IsAssignable(ITypeDescriptor dst, ITypeDescriptor src)
        {
            if (dst == null || src == null) return false;

            switch (src.NodeTypeCategory)
            {
                case NodeTypeCategory.Primitive:
                    // for now, only can assign things from primitives
                    break;
                case NodeTypeCategory.NOT_SET:
                    return false;
                case NodeTypeCategory.Void:
                    return false;
                default:
                    _log.Warn("RHS in IsAssignable is not implemented " + src.NodeTypeCategory.ToString());
                    return false;
            }


            switch (dst.NodeTypeCategory)
            {
                case NodeTypeCategory.NOT_SET:
                    throw new NotImplementedException("???");
                case NodeTypeCategory.Primitive:
                    return IsPrimitiveAssignable(dst.TypeDescriptorRef as IPrimitiveTypeDescriptor, src.TypeDescriptorRef);
                case NodeTypeCategory.Null:
                    CompilerErrors.Add(SemanticErrorTypes.BuiltinNotAssignable, "null");
                    return false;
                case NodeTypeCategory.Void:
                    CompilerErrors.Add(SemanticErrorTypes.BuiltinNotAssignable, "void");
                    return false;
                case NodeTypeCategory.This:
                    CompilerErrors.Add(SemanticErrorTypes.BuiltinNotAssignable, "this");
                    return false;
                case NodeTypeCategory.ErrorType:
                    return true;    // can always assign to an error type
                default:
                    CompilerErrors.Add(SemanticErrorTypes.FeatureNotImplemented, dst.NodeTypeCategory.ToString());
                    return false;
            }
        }

        private bool IsPrimitiveAssignable(IPrimitiveTypeDescriptor dst, ITypeDescriptor src)
        {
            if (dst == null || src == null) return false;

            IPrimitiveTypeDescriptor srcPrimitive = src.TypeDescriptorRef as IPrimitiveTypeDescriptor;
            if (srcPrimitive == null) throw new NotImplementedException("Assignment from non-primitives is not implemented");

            var tdst = dst.VariablePrimitiveType;
            var tsrc = srcPrimitive.VariablePrimitiveType;

            switch (tdst)
            {
                case VariablePrimitiveType.Object:
                    return true;    // object is the lowest
                default:
                    return tdst == tsrc;
            }
        }

        private bool IsArithmeticCompatible(ITypeDescriptor arg1, ITypeDescriptor arg2)
        {
            IPrimitiveTypeDescriptor primitive1 = arg1.TypeDescriptorRef as IPrimitiveTypeDescriptor;
            IPrimitiveTypeDescriptor primitive2 = arg2.TypeDescriptorRef as IPrimitiveTypeDescriptor;

            if (primitive1 == null || primitive2 == null) return false;

            return primitive1.VariablePrimitiveType == VariablePrimitiveType.Int &&
                   primitive2.VariablePrimitiveType == VariablePrimitiveType.Int;
        }
    }


    /// <summary>
    /// P. 326
    /// </summary>
    public class TypeCheckingLhsVisitor
    {
        protected static Logger _log = LogManager.GetCurrentClassLogger();

        private IEnv NameEnv { get; set; }
        private IEnv TypeEnv { get; set; }

        public TypeCheckingLhsVisitor(
            IEnv typeEnv,
            IEnv nameEnv)
        {
            NameEnv = nameEnv;
            TypeEnv = typeEnv;
        }

        public TypeCheckingLhsVisitor(IHasOwnScope node)
        {
            NameEnv = node.NameEnv;
            TypeEnv = node.TypeEnv;
        }

        public void Visit(dynamic node)
        {
            if (node == null) return;

            _log.Trace("visiting " + node);
            VisitNode(node);
        }

        private void VisitNode(Identifier id)
        {
            _log.Trace("LHS Visiting {0}.", id);
            // TODO
        }

        private void VisitNode(AbstractNode node)
        {
            _log.Info("Visiting {0}, no action.", node);
        }
    }
}
