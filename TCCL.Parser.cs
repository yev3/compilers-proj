using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ASTBuilder
{
    public partial class TCCLParser
    {
        public AbstractNode Root => CurrentSemanticValue;
        public TCCLParser() : base(null) { }

        public void Parse(string filename)
        {
            this.Scanner = new TCCLScanner(File.OpenRead(filename));
            this.Parse();
        }
        public void Parse(Stream strm)
        {
            this.Scanner = new TCCLScanner(strm);
            this.Parse();
        }
    }
}
