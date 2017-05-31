using System;
using System.Collections.Generic;
using System.Diagnostics;
using NLog;

namespace Proj3Semantics.AST
{
    /// <summary>
    /// All AST nodes are subclasses of this node.  This node knows how to
    /// link itself with other siblings and adopt children.
    /// Each node gets a node number to help identify it distinctly in an AST.
    /// </summary>
    [DebuggerDisplay("{ToDebugString()}")]
    public abstract class Node : IVisitableNode
    {
        protected static Logger Log = LogManager.GetCurrentClassLogger();


        #region Linked List Funcs

        public List<Node> Children { get; set; } = new List<Node>();

        public virtual void AddChild(Node child)
        {
            if (child == null)
            {
                Log.Error("INFO: Not implemented - tried to add a null child.");
                return;
            }
            Children.Add(child);
        }

        #endregion

        public override string ToString()
        {
            return "<" + this.GetType().Name + ">";
        }

        public string ToDebugString()
        {
            return NodePrintingVisitor.AbstactNodeDebugString(this);
        }

        public virtual void Accept(IReflectiveVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }
    }

}