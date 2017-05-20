using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Proj3Semantics.Nodes
{
    /// <summary>
    /// All AST nodes are subclasses of this node.  This node knows how to
    /// link itself with other siblings and adopt children.
    /// Each node gets a node number to help identify it distinctly in an AST.
    /// </summary>
    [DebuggerDisplay("AbstrNodeType: {ToString()}")]
    public abstract class AbstractNode : LinkedList<AbstractNode>, IVisitableNode
    {
        // Identifier ref associated with this node
        public Identifier Identifier { get; protected set; }

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

        public void AddChild(AbstractNode child)
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

        public virtual void Accept(IReflectiveVisitor myVisitor)
        {
            myVisitor.Visit(this);
        }


    }

}