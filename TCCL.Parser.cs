using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ASTBuilder
{
    internal partial class TCCLParser
    {
        public TCCLParser() : base(null) { }

        public void Parse(string filename)
        {
            this.Scanner = new TCCLScanner(File.OpenRead(filename));
            this.Parse();
        }
    }
}
