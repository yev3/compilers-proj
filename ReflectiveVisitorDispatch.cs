using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ASTBuilder
{
    class ReflectiveVisitorDispatch<TWithVisitMethods> : INodeReflectiveVisitor where TWithVisitMethods : new()
    {
        private readonly bool _throwOnNoMatch;
        private readonly bool _debugTrace;
        private Dictionary<Type, MethodInfo> MethodTable { get; set; }
        private TWithVisitMethods ObjInstance { get; set; } = new TWithVisitMethods();
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="throwOnNoMatch">Throw an exception if no suitable visit method was found</param>
        /// <param name="debugTrace">Print detailed trace log</param>
        public ReflectiveVisitorDispatch(bool throwOnNoMatch = true, bool debugTrace = false)
        {
            _throwOnNoMatch = throwOnNoMatch;
            _debugTrace = debugTrace;

            var evalMethods = typeof(TWithVisitMethods).GetVisitMethodsOneArg();
            MethodTable = evalMethods.ToDictionary(x => x.GetParameters().First().ParameterType, x => x);

            if (MethodTable.Count == 0)
            {
                throw new ArgumentException("Supplied type " +
                                        typeof(TWithVisitMethods).Name +
                                        " has no methods Visit with a single parameter");
            }

            if (_debugTrace)
            {
                Console.WriteLine(_getLookupTable());
            }
        }

        private string _getLookupTable()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Visitor Lookup Table: ");
            sb.AppendLine("----------------------");
            foreach (KeyValuePair<Type, MethodInfo> pair in MethodTable)
                sb.AppendLine(pair.ToString());
            sb.AppendLine();
            return sb.ToString();
        }

        private void trace(string msg, params Object[] objs)
        {
            if (!_debugTrace) return;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(msg);
            if (objs.Length > 0)
            {
                Console.Write(": ");
                var printObjs =
                    objs.Select(
                        x => x.ToString() + "(" + x.GetType().Name + ")");
                Console.WriteLine(string.Join(", ", printObjs));
            }
            else
            {
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        /// <summary>
        /// Called by the node. Finds the best method to call from the template paramemter
        /// </summary>
        /// <param name="node"></param>
        public void VisitDispatch(Object node)
        {
            var nodeType = node.GetType();
            trace("Starting to visit", nodeType);
            var inheritanceHieararchy = nodeType.GetInheritanceHierarchy();

            MethodInfo bestMethodMatch = null;
            foreach (Type t in inheritanceHieararchy)
            {
                trace("Looking for: " + t + " (walking inheritance tree)");
                if (MethodTable.TryGetValue(t, out bestMethodMatch))
                {
                    trace("found match, calling " + bestMethodMatch);
                    break;
                }
            }
            if (bestMethodMatch == null)
            {
                if (_throwOnNoMatch || _debugTrace)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("No suitable method for ");
                    sb.Append(node.GetType());
                    sb.AppendLine(" was found.");
                    sb.Append(_getLookupTable());
                    string msg = sb.ToString();
                    trace(msg);
                    if (_throwOnNoMatch) throw new ArgumentException(msg);
                }
            }

            var result = bestMethodMatch?.Invoke(ObjInstance, new object[] { node });
        }

    }


}
