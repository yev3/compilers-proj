﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.Nodes;

namespace Proj3Semantics
{
    abstract class SemanticsVisitor : IReflectiveVisitor
    {
        public void VisitChildren(AbstractNode node)
        {
            foreach (AbstractNode child in node)
            {
                child.Accept(this);
            }
        }

        public abstract void Visit(dynamic node);
    }

    class TopDeclVisitor : SemanticsVisitor
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();
        public override void Visit(dynamic node)
        {
            VisitNode(node);
        }
        private void VisitNode(AbstractNode node)
        {
            _log.Trace("Visiting {0}", node);
        }

        private void VisitNode(ClassDeclaration variableListDeclaring)
        {
            _log.Error("Pretend error occured in LocalVarDecl");
        }

    }

    class TypeVisitor : SemanticsVisitor    
    {
        public override void Visit(dynamic node)
        {
            VisitNode(node);
        }

        public void VisitNode(AbstractNode node)
        {
            Console.WriteLine("TypeVisitor is visiting: " + node); 
        }
    }
}
