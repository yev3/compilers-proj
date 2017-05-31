using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Proj3Semantics
{
    using static SemanticErrorTypes;
    using ErrList = List<CompilerError>;
    public enum SemanticErrorTypes
    {
        FeatureNotImplemented,
        VariableAlreadyDeclared,
        IdentifierNotTypeName,
        InconsistentModifiers,
        DuplicateClassDecl,
        UndeclaredIdentifier,
        DuplicateNamespaceDef,
        DuplicateParamName,
        NoMethodWithNumArgs,
        BuiltinNotAssignable,
        InvalidFuncArg,
        IncompatibleAssignment,
        IncompatibleOperands,
        BooleanExpected,
        DuplicateFunctionDecl
    }

    public static class SemanticErrorTypeMessages
    {
        public static string Details(this SemanticErrorTypes type)
        {
            switch (type)
            {
                case FeatureNotImplemented:
                    return "This feature is not implemented";
                case VariableAlreadyDeclared:
                    return "Variable is already declared";
                case IdentifierNotTypeName:
                    return "This identifier is not a type name";
                case InconsistentModifiers:
                    return "Inconsistent class declaration modifiers";
                case DuplicateClassDecl:
                    return "Duplicate class declaration";
                case UndeclaredIdentifier:
                    return "Undeclared identifier";
                case DuplicateNamespaceDef:
                    return "Duplicate namespace declaration";
                case DuplicateParamName:
                    return "Duplicate method parameter name declaration";
                case NoMethodWithNumArgs:
                    return "No method found with the required number of args";
                case BuiltinNotAssignable:
                    return "Built in type is not assignable";
                case InvalidFuncArg:
                    return "Invalid function argument";
                case IncompatibleAssignment:
                    return "Incompatible assignment";
                case IncompatibleOperands:
                    return "Incompatible operands";
                case BooleanExpected:
                    return "Boolean expected";
                case DuplicateFunctionDecl:
                    return "Duplicate function declaration";
                default:
                    throw new NotImplementedException("TODO: add a text message for the error type");
            }
        }
    }

    public class CompilerError
    {

    }
    public class SemanticError : CompilerError
    {
        public SemanticErrorTypes Type { get; set; }
        public string AdditionalMessage { get; set; }

        public SemanticError(SemanticErrorTypes type, string additionalMessage = null)
        {
            Type = type;
            AdditionalMessage = additionalMessage;
        }

        public override string ToString()
        {
            if (AdditionalMessage == null) return Type.Details();
            return Type.Details() + ": " + AdditionalMessage;
        }
    }

    // collection of errors
    public static class CompilerErrors
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();

        public static void ClearAll() => ErrList.Clear();
        public static ErrList ErrList { get; } = new ErrList();

        public static void Add(SemanticErrorTypes type, string additionalMessage = null)
        {
            var newError = new SemanticError(type, additionalMessage);
            _log.Error(newError.ToString);
            ErrList.Add(newError);
        }
    }
}
