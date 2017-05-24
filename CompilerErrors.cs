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
        VariableAlreadyDeclared,
        IdentifierNotTypeName,
        InconsistentModifiers,
        DuplicateClassDecl,
        InvalidQualifier,
        DuplicateNamespaceDef,
        DuplicateParamName
    }

    public static class SemanticErrorTypeMessages
    {
        public static string Details(this SemanticErrorTypes type)
        {
            switch (type)
            {
                case VariableAlreadyDeclared:
                    return "Variable is already declared.";
                case IdentifierNotTypeName:
                    return "This identifier is not a type name.";
                case InconsistentModifiers:
                    return "Inconsistent class declaration modifiers.";
                case DuplicateClassDecl:
                    return "Duplicate class declaration.";
                case InvalidQualifier:
                    return "Invalid qualifier access.";
                case DuplicateNamespaceDef:
                    return "Duplicate namespace declaration.";
                case DuplicateParamName:
                    return "Duplicate method parameter name declaration.";
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

        public static ErrList Instance { get; } = new ErrList();

        public static void Add(SemanticErrorTypes type, string additionalMessage = null)
        {
            var newError = new SemanticError(type, additionalMessage);
            _log.Error(newError.ToString);
            Instance.Add(newError);
        }
    }
}
