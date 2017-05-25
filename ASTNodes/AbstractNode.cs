﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Proj3Semantics.ASTNodes
{
    /// <summary>
    /// All AST nodes are subclasses of this node.  This node knows how to
    /// link itself with other siblings and adopt children.
    /// Each node gets a node number to help identify it distinctly in an AST.
    /// </summary>
    [DebuggerDisplay("{ToDebugString()}")]
    public abstract class AbstractNode : LinkedList<AbstractNode>, IVisitableNode
    {
        #region Linked List Funcs

        public LinkedListNode<AbstractNode> LinkedListNodeContainer { get; set; }

        public virtual AbstractNode LeftMostChild
        {
            get { return First?.Value; }
        }

        public virtual AbstractNode NextSibling
        {
            get { return LinkedListNodeContainer.Next?.Value; }
        }

        public virtual void AddChild(AbstractNode child)
        {
            if (child == null)
            {
                Console.WriteLine("INFO: Not implemented - tried to add a null child.");
                return;
                //throw new Exception("Error: tried to add a null child to the linked list.");
            }
            LinkedListNode<AbstractNode> newNode = new LinkedListNode<AbstractNode>(child);
            child.LinkedListNodeContainer = newNode;
            this.AddLast(newNode);
        }

        // These are not currently used.
        // =============================

        //public virtual AbstractNode LeftMostSibling
        //{
        //    get { return LinkedListNodeContainer.List.First?.Value; }
        //}


        public virtual AbstractNode Parent
        {
            get { return (LinkedListNodeContainer.List as AbstractNode); }
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