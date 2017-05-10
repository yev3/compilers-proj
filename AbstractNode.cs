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
    public abstract class AbstractNode : LinkedList<AbstractNode>, IReflectiveVisitable
    {
        public string DebugDisp => this.ToString();

        public LinkedListNode<AbstractNode> LinkedListNodeContainer { get; set; }
        public Token Token { get; set; }

        // these are accessible:
        // Count	
        // First	
        // Last	

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

        public virtual AbstractNode LeftMostChild
        {
            get { return First?.Value; }
        }

        public virtual AbstractNode NextSibling
        {
            get { return LinkedListNodeContainer.Next?.Value; }
        }

        public virtual AbstractNode LeftMostSibling
        {
            get { return LinkedListNodeContainer.List.First?.Value; }
        }


        public virtual string Name { get; protected set; }

        public override string ToString()
        {
            return this.GetType().FullName;
        }
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

        /// <summary>
        /// Obsolete, do not use! </summary>
        //public virtual void walkTree(Visitable v)
        //{
        //internWalk(0, v);
        //}
        public abstract void Accept(INodeVisitor myVisitor);
    }

}