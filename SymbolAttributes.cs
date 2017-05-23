using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proj3Semantics;

namespace Proj3Semantics
{

    // found here:
    // http://www.ccs.neu.edu/home/riccardo/courses/csu370-fa07/lect4.pdf

    public enum VariablePrimitiveTypes
    {
        Boolean, Byte, Char, Short, Int, Long, Float, Double, NotPrimitive
        //boolean	true or false	false	1 bit	true, false
        //byte	twos complement integer	0	8 bits	(none)
        //char	Unicode character	\u0000	16 bits	'a', '\u0041', '\101', '\\', '\'', '\n', 'ß'
        //short	twos complement integer	0	16 bits	(none)
        //int	twos complement integer	0	32 bits	-2, -1, 0, 1, 2
        //long	twos complement integer	0	64 bits	-2L, -1L, 0L, 1L, 2L
        //float	IEEE 754 floating point	0.0	32 bits	1.23e100f, -1.23e-100f, .3f, 3.14F
        //double	IEEE 754 floating point	0.0	64 bits	1.23456e300d, -1.23456e-300d, 1e1d 
    }
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
