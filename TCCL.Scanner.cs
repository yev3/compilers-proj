using System;
using System.Collections.Generic;
using System.Text;

namespace Proj3Semantics
{
    public partial class TCCLScanner
    {

		public override void yyerror(string format, params object[] args)
		{
			base.yyerror(format, args);
			Console.WriteLine(format, args);
			Console.WriteLine();
		}

    }
}
