using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ASTBuilder
{
    /// <summary>
    /// All AST nodes are subclasses of this node.  This node knows how to
    /// link itself with other siblings and adopt children.
    /// Each node gets a node number to help identify it distinctly in an AST.
    /// </summary>
    [DebuggerDisplay("AbstrNodeType: {DebugDisp}")]
    public abstract class AbstractNode : LinkedList<AbstractNode>, IVisitableNode
    {
        public int IntVal { get; set; }

        public string DebugDisp => this.ToString();

        public LinkedListNode<AbstractNode> LinkedListNodeContainer { get; set; }

        public virtual AbstractNode LeftMostChild
        {
            get { return First?.Value; }
        }

        public virtual AbstractNode NextSibling
        {
            get { return LinkedListNodeContainer.Next?.Value; }
        }
        public virtual string Name { get; protected set; }

        public override string ToString()
        {
            return this.GetType().FullName;
        }

        public virtual string ClassName()
        {
            return this.GetType().Name;
        }

        public virtual void Accept(INodeReflectiveVisitor myVisitor)
        {
            myVisitor.VisitDispatch(this);
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

        public virtual AbstractNode Parent
        {
            get { return (LinkedListNodeContainer.List as AbstractNode); }
        }

        // These are not currently used.
        // =============================

        //public virtual AbstractNode LeftMostSibling
        //{
        //    get { return LinkedListNodeContainer.List.First?.Value; }
        //}

    }

}