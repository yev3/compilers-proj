using System;
using System.Collections;
using System.Collections.Generic;

namespace ASTBuilder
{
    public class AbstractNodeLinkedList : LinkedList<AbstractNode>
    {
        public AbstractNode Parent { get; set; }
        
    }


    /// <summary>
    /// All AST nodes are subclasses of this node.  This node knows how to
    /// link itself with other siblings and adopt children.
    /// Each node gets a node number to help identify it distinctly in an AST.
    /// </summary>
    public abstract class AbstractNode : ReflectiveVisitable
    {
        private AbstractNodeLinkedList _children = null;
        private LinkedListNode<AbstractNode> _myEnclosedNode;


        private static int nodeNums = 0;
        private int nodeNum;
        private AbstractNode nextSibling;
        private AbstractNode leftmostSibling;

        public AbstractNode(LinkedListNode<AbstractNode> nodeIBelongTo)
        {
            _myEnclosedNode = nodeIBelongTo;
            nextSibling = null;
            leftmostSibling = this;
            nodeNum = ++nodeNums;
        }

        /// <summary>
        /// Join the end of this sibling's list with the supplied sibling's list </summary>
        public virtual AbstractNode AppendSiblings(AbstractNode sib)
        {
            if (sib == null)
            {
                throw new Exception("Call to makeSibling supplied null-valued parameter");
            }
            AbstractNode appendAt = this;

            while (appendAt.nextSibling != null)
            {
                appendAt = appendAt.nextSibling;
            }
            appendAt.nextSibling = sib.leftmostSibling;


            AbstractNode ans = sib.leftmostSibling;
            ans.leftmostSibling = appendAt.leftmostSibling;

            while (ans.nextSibling != null)
            {
                ans = ans.nextSibling;
                ans.leftmostSibling = appendAt.leftmostSibling;
            }
            return (ans);
        }

        /// <summary>
        /// Adopt the linked list of children</summary>
        public virtual void AdoptChildren(AbstractNodeLinkedList children)
        {
            _children = children;
        }

        public virtual AbstractNode orphan()
        {
            leftmostSibling = this;
            return this;
        }

        public virtual AbstractNode AbandonChildren()
        {
            LeftChild = null;
            return this;
        }

        //public virtual AbstractNode Parent {
        //    get
        //    {
        //        _myEnclosedNode.   
        //    }
        //}
        public virtual AbstractNode NextSibling { get; set; }
        public virtual AbstractNode LeftChild { get; set; }
        public virtual AbstractNode LeftMostSibling { get; set; }
        public virtual string Name { get; protected set; }


        //public override string ToString()
        //{
            //Type t = NodeType;
            //string tString = (t != null) ? ("<" + t.ToString() + "> ") : "";

            //return "" + NodeNum + ": " + tString + whatAmI() + "  \"" + ToString() + "\"";
        //}


        private static string trimClass(string cl)
        {
            int dollar = cl.LastIndexOf('$');
            int dot = cl.LastIndexOf('.');
            int trimAt = (dollar > dot) ? dollar : dot;
            if (trimAt >= 0)
            {
                cl = cl.Substring(trimAt + 1);
            }
            return cl;
        }

        //private static Type objectClass = (new object()).GetType();
        private static void debugMsg(string s)
        {
        }
        //  private static System.Collections.IEnumerator interfaces(Type c)
        //  {
        //  Type iClass = c;
        //  ArrayList v = new ArrayList();
        //  while (iClass != objectClass)
        //  {
        //debugMsg("Looking for interface  match in " + iClass.Name);
        //Type[] interfaces = iClass.Interfaces;
        //	 for (int i = 0; i < interfaces.Length; i++)
        //	 {
        //	  debugMsg("   trying interface " + interfaces[i]);
        //		  v.Add(interfaces[i]);
        //		Type[] superInterfaces = interfaces[i].Interfaces;
        //		for (int j = 0; j < superInterfaces.Length; ++j)
        //		{
        //	  debugMsg("   trying super interface " + superInterfaces[j]);
        //			  v.Add(superInterfaces[j]);
        //		}

        //	 }
        // iClass = iClass.BaseType;
        //  }
        //  return v.elements();
        //  }

        /// <summary>
        /// Reflectively indicate the class of "this" node </summary>
        public virtual string whatAmI()
        {
            string ans = trimClass(this.GetType().ToString());
            return ans;  /* temporary until remainder is fixed */
            //ISet s = new HashSet();
            //System.Collections.IEnumerator e = interfaces(this.GetType());
            //while (e.MoveNext())
            //{
            //    Type c = (Type)e.Current;
            //    string str = trimClass(c.ToString());
            //    if (!(str.Equals("DontPrintMe") || str.Equals("ReflectiveVisitable")))
            //    {
            //        s.Add(trimClass(c.ToString()));
            //    }
            //}
            //return ans + s.ToString();
        }

        //private void internWalk(int level, Visitable v)
        //{
        //v.pre(level, this);
        //for (AbstractNode c = child; c != null; c = c.mysib)
        //{
        //c.internWalk(level + 1, v);
        //}
        //v.post(level, this);
        //}

        /// <summary>
        /// Reflective visitor pattern </summary>
        public void accept(ReflectiveVisitable v)
        {
            v.accept(this);
        }

        /// <summary>
        /// Obsolete, do not use! </summary>
        //public virtual void walkTree(Visitable v)
        //{
        //internWalk(0, v);
        //}
    }

}