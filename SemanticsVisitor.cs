using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.Nodes;

namespace Proj3Semantics
{
    public abstract class SemanticsVisitor : IReflectiveVisitor
    {
        public void VisitChildren(AbstractNode node)
        {
            foreach (AbstractNode child in node)
            {
                child.Accept(this);
            }
        }

        public abstract void Visit(dynamic node);
    }

    public class TopDeclVisitor : SemanticsVisitor
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();

        private ISymbolTable<LocalVarDescriptor> _localVarSymbolTable { get; set; }
        private ISymbolTable<TypeDescriptor> _typeSymbolTable { get; set; }

        public TopDeclVisitor(
            ISymbolTable<LocalVarDescriptor> localVarSymbolTable,
            ISymbolTable<TypeDescriptor> typeSymbolTable)
        {
            _localVarSymbolTable = localVarSymbolTable;
            _typeSymbolTable = typeSymbolTable;
        }

        public override void Visit(dynamic node)
        {
            VisitNode(node);
        }

        // VISIT METHODS
        // ============================================================

        public void VisitNode(VariableListDeclaring decls)
        {
            _log.Trace("Analyzing VariableDecls");
            var typeVisitor = new TypeVisitor(_typeSymbolTable);
            decls.TypeNameDecl.Accept(typeVisitor);

            foreach (AbstractNode node in decls.ItemIdList)
            {
                Identifier id = node as Identifier;
                Debug.Assert(id != null, "DeclaredVars node children should be Identifiers");

                if (_localVarSymbolTable.IsDeclaredLocally(id.Name))
                {
                    CompilerErrors.Add(SemanticErrorTypes.VariableAlreadyDeclared, id.Name);
                    id.Type = VariableTypes.ErrorType;
                    id.TypeDescriptor = null;
                }
                else
                {
                    var attr = new LocalVarDescriptor();
                    attr.Kind = decls.TypeNameDecl.VariableType;
                    attr.TypeDescriptor = decls.TypeNameDecl.TypeDescriptor;

                    id.Type = attr.Kind;
                    id.TypeDescriptor = attr.TypeDescriptor;

                    _localVarSymbolTable.EnterInfo(id.Name, attr);
                }
            }
        }

        private void VisitNode(ClassVarDecl variableListDeclaring)
        {
            _log.Trace("Analyzing ClassVarDecl");
        }
        //private void VisitNode(TypeDeclaring variableListDeclaring)
        //{
        //    _log.Trace("Analyzing TypeDeclaring");
        //}
        private void VisitNode(ClassDeclaration variableListDeclaring)
        {
            _log.Trace("Analyzing ClassDeclaring");
        }
        private void VisitNode(MethodDeclaration variableListDeclaring)
        {
            _log.Trace("Analyzing MethodDeclaration");
        }

        private void VisitNode(AbstractNode node)
        {
            _log.Trace("Visiting {0}", node);
        }


    }

    // Fills in the types of the child nodes to be used later
    public class TypeVisitor : SemanticsVisitor
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();

        private ISymbolTable<TypeDescriptor> SymbolTable { get; set; }
        public TypeVisitor(ISymbolTable<TypeDescriptor> symbolTable)
        {
            SymbolTable = symbolTable;
        }

        public override void Visit(dynamic node)
        {
            VisitNode(node);
        }

        public void VisitNode(AbstractNode node)
        {
            Console.WriteLine("TypeVisitor is visiting: " + node);
        }

        public void VisitNode(Identifier id)
        {
            TypeDescriptor entry = SymbolTable.Lookup(id.Name);
            if (entry != null)
            {
                id.Type = entry.KindVariableCategory;
                id.TypeDescriptor = entry;
            }
            else
            {
                CompilerErrors.Add(SemanticErrorTypes.IdentifierNotTypeName, id.Name);
                id.Type = VariableTypes.ErrorType;
                id.TypeDescriptor = null;
            }
        }


    }
}

