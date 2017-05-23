﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.Nodes;

namespace Proj3Semantics
{
    using IEnv = ISymbolTable<ITypeSpecifier>;


    /// <summary>
    /// This class RETRIEVES out the information about types in:
    /// (Does not modify the symbol table, that's the job of the other visitors)
    /// Declarations in statements
    /// Return types and args in functions
    /// Types of class fields
    /// </summary>
    public class TypeVisitor : SemanticsVisitor
    {
        private new static Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The current scope that is being searched
        /// </summary>
        private IEnv TypeEnv { get; set; }
        public TypeVisitor(IEnv typeEnv)
        {
            TypeEnv = typeEnv;
        }

        public override void Visit(dynamic node)
        {
            _log.Trace("    -- dispatching " + node);
            VisitNode(node);
        }


        // VISIT METHODS
        // --------------------------------------------------


        private void VisitNode(Identifier id)
        {
            _log.Info("Type visitor is visiting: " + id);

            ITypeSpecifier entry = TypeEnv.Lookup(id.Name);
            if (entry != null)
            {
                id.NodeTypeCategory = entry.NodeTypeCategory;
                id.TypeSpecifierRef = entry.TypeSpecifierRef;
            }
            else
            {
                CompilerErrors.Add(SemanticErrorTypes.IdentifierNotTypeName, id.Name);
                id.NodeTypeCategory = NodeTypeCategory.ErrorType;
                id.TypeSpecifierRef = null;
            }
        }
        private void VisitNode(PrimitiveType arrayDef)
        {
            _log.Trace("Type visitor is ignoring primitive type for now..");
        }

        private void VisitNode(QualifiedName qname)
        {
            var curTypeEnv = TypeEnv;
            string curScopeName = "";
            ITypeSpecifier curResult = null;

            foreach (AbstractNode node in qname)
            {
                Identifier id = node as Identifier;
                if (id == null) throw new ArgumentNullException(nameof(id));

                //  look up the id in the env
                string curLookupName = id.Name;
                curResult = curTypeEnv?.Lookup(curLookupName);

                if (curResult == null)
                {
                    string errMsg = curLookupName;
                    if (curScopeName != "")
                        errMsg += " in " + curScopeName;
                    CompilerErrors.Add(SemanticErrorTypes.InvalidQualifier, errMsg);
                    qname.NodeTypeCategory = NodeTypeCategory.ErrorType;
                    qname.TypeSpecifierRef = null;
                    return;
                }

                curTypeEnv = curResult?.LocalNameEnv;
                curScopeName = curLookupName;
            }
        }

        private void VisitNode(ArraySpecifier arrayDef)
        {
            throw new NotImplementedException("Arrays are not supported in this release");
        }


        private void VisitNode(AbstractNode node)
        {
            _log.Warn("NO ACTION BY TypeVisitor --- " + this.GetType().Name + " --- (abstract).");
        }

    }
}