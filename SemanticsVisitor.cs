using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proj3Semantics.Nodes;

namespace Proj3Semantics
{
    class SemanticsVisitor : IReflectiveVisitor
    {
        public void Visit(dynamic node)
        {
            this.VisitNode(node);
        }

        // Call this method to begin the semantic checking process
        public void CheckSemantics(AbstractNode node)
        {
            if (node == null) { 
                return;
            }

            ///More here
        }
        public void VisitNode(AbstractNode node)
        {
            
        }

        public void VisitNode(Modifiers node)
        {
            
        }

        
    }
}

