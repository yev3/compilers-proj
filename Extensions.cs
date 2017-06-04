﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Proj3Semantics.AST;

namespace Proj3Semantics
{
    using static ConsoleColor;
    public static class Extensions
    {

        public static string GetIlName(this NodeTypeCategory cat)
        {
            switch (cat)
            {
                case NodeTypeCategory.Int:
                    return "int32";
                case NodeTypeCategory.String:
                    return "string";
                case NodeTypeCategory.Object:
                    return "object";
                case NodeTypeCategory.Boolean:
                    return "bool";
                case NodeTypeCategory.Void:
                    return "void";
                case NodeTypeCategory.This:
                    return "this";
                default:
                    throw new NotImplementedException("Unsupported ToString of NodeTypeCategory: " + cat.GetType().Name.ToString());
            }
        }


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


    public class OutColor : IDisposable
    {
        public static OutColor Black => new OutColor(ConsoleColor.Black);
        public static OutColor DarkBlue => new OutColor(ConsoleColor.DarkBlue);
        public static OutColor DarkGreen => new OutColor(ConsoleColor.DarkGreen);
        public static OutColor DarkCyan => new OutColor(ConsoleColor.DarkCyan);
        public static OutColor DarkRed => new OutColor(ConsoleColor.DarkRed);
        public static OutColor DarkMagenta => new OutColor(ConsoleColor.DarkMagenta);
        public static OutColor DarkYellow => new OutColor(ConsoleColor.DarkYellow);
        public static OutColor Gray => new OutColor(ConsoleColor.Gray);
        public static OutColor DarkGray => new OutColor(ConsoleColor.DarkGray);
        public static OutColor Blue => new OutColor(ConsoleColor.Blue);
        public static OutColor Green => new OutColor(ConsoleColor.Green);
        public static OutColor Cyan => new OutColor(ConsoleColor.Cyan);
        public static OutColor Red => new OutColor(ConsoleColor.Red);
        public static OutColor Magenta => new OutColor(ConsoleColor.Magenta);
        public static OutColor Yellow => new OutColor(ConsoleColor.Yellow);
        public static OutColor White => new OutColor(ConsoleColor.White);

        public static OutColor WithColor(ConsoleColor newFore,
            ConsoleColor? newBack = null)
        {
            return new OutColor(newFore, newBack);
        }
        private ConsoleColor _originalForeColor;
        private ConsoleColor _originalBackColor;

        private OutColor(ConsoleColor newFore, ConsoleColor? newBack = null)
        {
            _originalForeColor = Console.ForegroundColor;
            _originalBackColor = Console.BackgroundColor;
            Console.ForegroundColor = newFore;
            if (newBack.HasValue)
            {
                Console.BackgroundColor = newBack.Value;
            }
        }

        public void Dispose()
        {
            Console.ForegroundColor = _originalForeColor;
            Console.BackgroundColor = _originalBackColor;
        }
    }
}