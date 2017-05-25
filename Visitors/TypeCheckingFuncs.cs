using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics.Visitors
{
    public class TypeCheckingFuncs
    {

        public static bool CanAssign(ITypeDescriptor dst, ITypeDescriptor src)
        {
            if (dst == null || src == null) return false;

            switch (dst.NodeTypeCategory)
            {
                case NodeTypeCategory.NOT_SET:
                    break;
                case NodeTypeCategory.Primitive:
                    break;
                case NodeTypeCategory.Null:
                    break;
                case NodeTypeCategory.Array:
                    break;
                case NodeTypeCategory.Class:
                    break;
                case NodeTypeCategory.Void:
                    break;
                case NodeTypeCategory.This:
                    break;
                case NodeTypeCategory.ErrorType:
                    break;
                case NodeTypeCategory.ClassFieldDef:
                    break;
                case NodeTypeCategory.ClassMethodDef:
                    break;
                case NodeTypeCategory.NamespaceDecl:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        /// <summary>
        /// Returns the max of the two types or an error
        /// </summary>
        public static ITypeDescriptor Max(ITypeDescriptor t1, ITypeDescriptor t2)
        {


            return null;
        }
    }
}
