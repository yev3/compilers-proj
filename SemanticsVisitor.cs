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
        private ISymbolTable _symbolTable { get; set; }
        public TopDeclVisitor(ISymbolTable symbolTable)
        {
            _symbolTable = symbolTable;
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
            var typeVisitor = new TypeVisitor(_symbolTable);
            decls.TypeNameDecl.Accept(typeVisitor);

            foreach (AbstractNode node in decls.ItemIdList)
            {
                Identifier id = node as Identifier;
                Debug.Assert(id != null, "DeclaredVars node children should be Identifiers");

                if (_symbolTable.IsDeclaredLocally(id.Name))
                {
                    CompilerErrors.Add(SemanticErrorTypes.VariableAlreadyDeclared, id.Name);
                    id.Type = VariableTypes.ErrorType;
                    id.AttributesRef = null;
                }
                else
                {
                    id.Type = decls.TypeNameDecl.Type;
                    id.AttributesRef = decls.TypeNameDecl.AttributesRef;
                    _symbolTable.EnterInfo(id.Name, new SymbolTableEntry() );

                }
            }



        }
        private void VisitNode(AbstractNode node)
        {
            _log.Trace("Visiting {0}", node);
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


    }

    public class TypeVisitor : SemanticsVisitor
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();

        private ISymbolTable SymbolTable { get; set; }
        public TypeVisitor(ISymbolTable symbolTable)
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
            SymbolTableEntry entry = SymbolTable.Lookup(id.Name);
            if (entry != null && entry.EntryType ==
                AttribRecordTypes.TypeAttrib)
            {
                id.Type = entry.KindVariableCategory;
                id.AttributesRef = entry.AttribRecord;
            }
            else
            {
                CompilerErrors.Add(SemanticErrorTypes.IdentifierNotTypeName, id.Name);
                id.Type = VariableTypes.ErrorType;
                id.AttributesRef = null;
            }
        }


    }
}

