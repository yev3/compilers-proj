using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CompilerILGen.AST;

namespace CompilerILGen
{
    public partial class TCCLParser
    {
        public Node Root => CurrentSemanticValue;
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

        public void ParseString(string str)
        {
            Stream strm = new MemoryStream(Encoding.UTF8.GetBytes(str ?? ""));
            Parse(strm);
        }
    }
}
