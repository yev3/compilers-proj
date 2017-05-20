using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.Nodes;

namespace Proj3Semantics
{
    abstract class SemanticsVisitor : IReflectiveVisitor
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

    class TopDeclVisitor : SemanticsVisitor
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();
        public override void Visit(dynamic node)
        {
            VisitNode(node);
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

        public void VisitNode(VariableListDeclaring decls)
        {
            _log.Trace("Analyzing VariableDecls");
            
        }

    }

    class TypeVisitor : SemanticsVisitor
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
                id.Type = entry.VariableType;
                id.RefAttribRecord = entry.AttribRecord;
            }
            else
            {
                _log.Error("This identifier is not a type name: {0}", id.Name);
                id.Type = VariableTypes.ErrorType;
                id.RefAttribRecord = null;
            }
        }


    }
}

