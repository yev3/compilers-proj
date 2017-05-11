using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ASTBuilder
{
    public partial class TCCLParser
    {
        public TCCLParser() : base(null) { }

        public void Parse(string filename)
        {
            this.Scanner = new TCCLScanner(File.OpenRead(filename));
            this.Parse();
            DisplayTree();
        }
        public void Parse(Stream strm)
        {
            this.Scanner = new TCCLScanner(strm);
            this.Parse();
            DisplayTree();
        }

        public void DisplayTree()
        {
            NodeVisitor visitor = new NodeVisitor();
            NodeTraverser traverser = new NodeTraverser(visitor);
            traverser.PreOrderWalk(CurrentSemanticValue.AbstractNode);
            
        }



    }
}
