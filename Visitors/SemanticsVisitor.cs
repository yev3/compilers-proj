using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using NLog;
using Proj3Semantics.ASTNodes;
using Parameter = Proj3Semantics.ASTNodes.Parameter;

namespace Proj3Semantics
{
    using IEnv = ISymbolTable<ITypeSpecifier>;
    public abstract class SemanticsVisitor : IReflectiveVisitor
    {
        protected static Logger _log = LogManager.GetCurrentClassLogger();
        public void VisitChildren(AbstractNode node)
        {
            _log.Trace(this.GetType().Name.ToString() + " is visiting children of " + node);

            foreach (AbstractNode child in node)
            {
                child.Accept(this);
            }
        }

        public abstract void Visit(dynamic node);
    }


    //// walks the tree. enters namespaces, classes, fields, functions
    //public class TypeDeclChecker : SemanticsVisitor
    //{
    //    private new static Logger _log = LogManager.GetCurrentClassLogger();
    //    private IEnv NameEnv { get; set; }
    //    private IEnv TypeEnv { get; set; }
    //    public TypeDeclChecker(IEnv nameEnv, IEnv typeEnv)
    //    {
    //        NameEnv = nameEnv;
    //        TypeEnv = typeEnv;
    //    }

    //    public override void Visit(dynamic node)
    //    {
    //        _log.Trace(this.GetType().Name + " is visiting " + node + " in type/name env " + TypeEnv + " / " + NameEnv);
    //        VisitNode(node);
    //    }


    //    private void VisitNode(AbstractNode node)
    //    {
    //        _log.Trace("Visiting {0}, no action.", node);
    //    }
    //}








    // Fills in the types of the child nodes to be used later
    // ======================================================



}

