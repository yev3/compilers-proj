using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    public static class Extensions
    {
        //var methods =  this.GetType().GetMethodsBySig(typeof(void), typeof(int), typeof(string));

        private const string EVAL_METHOD_NAME = "visit";
        public static IEnumerable<MethodInfo> GetVisitMethodsOneArg(this Type type)
        {
            return type
                .GetMethods()
                .Where(m => m.Name.ToLower().Equals(EVAL_METHOD_NAME))
                .Where(m => m.GetParameters().Length == 1);
        }

        // http://stackoverflow.com/a/5152539 
        public static IEnumerable<MethodInfo> GetMethodsBySig(this Type type, Type returnType, params Type[] parameterTypes)
        {
            return type.GetMethods().Where((m) =>
            {
                if (m.ReturnType != returnType) return false;
                var parameters = m.GetParameters();
                if ((parameterTypes == null || parameterTypes.Length == 0))
                    return parameters.Length == 0;
                if (parameters.Length != parameterTypes.Length)
                    return false;
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    if (parameters[i].ParameterType != parameterTypes[i])
                        return false;
                }
                return true;
            });
        }

        // http://stackoverflow.com/a/5716933 
        public static IEnumerable<Type> GetInheritanceHierarchy(this Type type)
        {
            for (var current = type; current != null; current = current.BaseType)
                yield return current;
        }
    }
}
