using System;
using System.Collections.Generic;
using System.Text;
using QUT.GplexBuffers;

namespace Proj3Semantics
{
    public partial class TCCLScanner
    {

        public override void yyerror(string format, params object[] args)
        {
            base.yyerror(format, args);

            int curLine = this.tokLin;
            int curCol = this.tokCol;
            int tokFm = tokPos;
            int lineBegin = tokFm - curCol;
            int tokTo = this.tokEPos;

            string line = buffer.GetString(lineBegin, tokTo);
            string highlight = new string(' ', curCol) + new string('^', tokTo - tokFm);
            StringBuilder sb = new StringBuilder(line);

            char nextChar;
            while ((nextChar = (char)buffer.Read()) != ScanBuff.EndOfFile)
            {
                if (nextChar == '\n') break;
                sb.Append(nextChar);
            }

            Console.WriteLine("Error on line " + curLine + ": " + format, args);
            Console.WriteLine(sb);
            using (OutColor.Red)
                Console.WriteLine(highlight);


            Console.WriteLine();
        }

    }
}
