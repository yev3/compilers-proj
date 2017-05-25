using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.AST_Nodes;

namespace Proj3Semantics
{
    
    using IEnv = ISymbolTable<ITypeSpecifier>;


    public class TypeCheckingVisitor
    {
        protected static Logger _log = LogManager.GetCurrentClassLogger();

        private IEnv NameEnv { get; set; }
        private IEnv TypeEnv { get; set; }

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

            _log.Trace(this.GetType().Name + " is visiting " + node);
            VisitNode(node);
        }



        private void VisitNode(Identifier id)
        {
            _log.Trace("Visiting {0}.", id);
            // TODO
        }

        private void VisitNode(Expression expr)
        {
            switch (expr.ExprType)
            {
                case ExprType.ASSIGNMENT:
                    VisitAssignment(expr);
                    return;
                case ExprType.EVALUATION:
                    VisitAssignment(expr);
                    return;
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
                case ExprType.PLUSOP:
                    break;
                case ExprType.MINUSOP:
                    break;
                case ExprType.ASTERISK:
                    break;
                case ExprType.RSLASH:
                    break;
                case ExprType.PERCENT:
                    break;
                case ExprType.UNARY:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _log.Trace("Unsupported expression type: " + expr.ExprType.ToString());
            
        }

        private void VisitAssignment(Expression expr)
        {
            _log.Trace("Checking assignment..", expr);
            
        }
        private void VisitEvaluation(Expression expr)
        {
            _log.Trace("Checking Evaluation..", expr);
            
        }
        private void VisitNode(AbstractNode node)
        {
            _log.Trace("Visiting {0}, no action.", node);
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

            _log.Trace(this.GetType().Name + " is visiting " + node);
            VisitNode(node);
        }

        private void VisitNode(Identifier id)
        {
            _log.Trace("LHS Visiting {0}.", id);
            // TODO
        }

        private void VisitNode(AbstractNode node)
        {
            _log.Trace("Visiting {0}, no action.", node);
        }
    }
}
