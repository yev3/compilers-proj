using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proj3Semantics;

namespace Proj3Semantics
{

    public class TypeHelperMethods
    {
        public static IEnumerable<VariablePrimitiveTypes> WideningsFrom(
            VariablePrimitiveTypes types)
        {
            switch (types)
            {
                case VariablePrimitiveTypes.Boolean:
                    return new VariablePrimitiveTypes[0];
                case VariablePrimitiveTypes.Byte:
                    return new[]
                    {
                        VariablePrimitiveTypes.Short,
                        VariablePrimitiveTypes.Int,
                        VariablePrimitiveTypes.Long,
                        VariablePrimitiveTypes.Float,
                        VariablePrimitiveTypes.Double
                    };
                case VariablePrimitiveTypes.Char:
                    return new[]
                    {
                        VariablePrimitiveTypes.Int,
                        VariablePrimitiveTypes.Long,
                        VariablePrimitiveTypes.Float,
                        VariablePrimitiveTypes.Double
                    };
                case VariablePrimitiveTypes.Short:
                    return new[]
                    {
                        VariablePrimitiveTypes.Short,
                        VariablePrimitiveTypes.Int,
                        VariablePrimitiveTypes.Long,
                        VariablePrimitiveTypes.Float,
                        VariablePrimitiveTypes.Double
                    };
                case VariablePrimitiveTypes.Int:
                    return new[]
                    {
                        VariablePrimitiveTypes.Int,
                        VariablePrimitiveTypes.Long,
                        VariablePrimitiveTypes.Float,
                        VariablePrimitiveTypes.Double
                    };
                case VariablePrimitiveTypes.Long:
                    return new[]
                    {
                        VariablePrimitiveTypes.Long,
                        VariablePrimitiveTypes.Float,
                        VariablePrimitiveTypes.Double
                    };
                case VariablePrimitiveTypes.Float:
                    return new[]
                    {
                        VariablePrimitiveTypes.Float,
                        VariablePrimitiveTypes.Double
                    };
                case VariablePrimitiveTypes.Double:
                    return new[]
                    {
                        VariablePrimitiveTypes.Double
                    };
            }
            return new VariablePrimitiveTypes[0];
        }
    }
}
