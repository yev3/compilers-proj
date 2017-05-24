using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.Nodes;

namespace Proj3Semantics
{
    using IEnv = ISymbolTable<ITypeSpecifier>;
    /// <summary>
    /// PAGE 302
    /// </summary>
    public class DeclarationVisitor : SemanticsVisitor
    {

        protected new static Logger _log = LogManager.GetCurrentClassLogger();

        private IEnv NameEnv { get; set; }
        private IEnv TypeEnv { get; set; }

        public DeclarationVisitor(
            IEnv typeEnv,
            IEnv nameEnv)
        {
            NameEnv = nameEnv;
            TypeEnv = typeEnv;
        }

        public DeclarationVisitor(IHasOwnScope node)
        {
            NameEnv = node.NameEnv;
            TypeEnv = node.TypeEnv;
        }

        public override void Visit(dynamic node)
        {
            if (node == null) return;

            _log.Trace(this.GetType().Name + " is visiting " + node);
            VisitNode(node);
        }

        private void VisitNode(CompilationUnit unit)
        {
            foreach (AbstractNode child in unit)
                Visit(child);
        }


        private void VisitNode(NamespaceDecl nsdecl)
        {
            _log.Trace("Checking Namespace declaration in env: " + nsdecl.TypeEnv + "/" + nsdecl.NameEnv);
            var visitor = new DeclarationVisitor(nsdecl);
            foreach (AbstractNode child in nsdecl)
                visitor.Visit(child);
        }

        private void VisitNode(NamespaceBody namespaceBody)
        {
            _log.Trace("Checking Namespace body in env: " + TypeEnv + "/" + NameEnv);
            foreach (AbstractNode node in namespaceBody)
            {
                // each namespace body node should have their own env
                (new DeclarationVisitor(node as IHasOwnScope)).Visit(node);
            }
        }


        private void VisitNode(ClassDeclaration cdecl)
        {
            _log.Trace("Checking Class declaration in env: " + TypeEnv);
            var visitor = new DeclarationVisitor(cdecl);
            visitor.Visit(cdecl.Fields);
            visitor.Visit(cdecl.Methods);

        }

        private void VisitNode(ClassFields fields)
        {
            foreach (AbstractNode field in fields)
                Visit(field);
        }
        private void VisitNode(ClassMethods methods)
        {
            foreach (AbstractNode method in methods)
                Visit(method);
        }

        private void VisitNode(ClassFieldDeclStatement fieldDecl)
        {
            _log.Trace("Checking fields declaration in env: " + TypeEnv);

            VariableListDeclaring vld = fieldDecl.VariableListDeclaring;

            ITypeSpecifier typeNameDecl = vld.FieldTypeSpecifier;
            if (typeNameDecl == null) throw new NullReferenceException("Declared class field is not ITypeInfo.");

            if (typeNameDecl.TypeSpecifierRef == null)
            {
                TypeVisitor tVisitor = new TypeVisitor(TypeEnv);
                tVisitor.Visit(vld.FieldTypeSpecifier);
            }

            DeclaredVars declFields = vld.ItemIdList;

            foreach (AbstractNode field in declFields)
            {
                FieldVarDecl fdecl = field as FieldVarDecl;
                if (fdecl == null) throw new ArgumentNullException(nameof(fdecl));

                // copy over from the decl
                fdecl.TypeSpecifierRef = typeNameDecl.TypeSpecifierRef;
            }
        }



        // TODO: FIX METHOD SIGS
        private void VisitNode(MethodDeclaration mdecl)
        {
            _log.Trace("Checking method declaration in env: " + TypeEnv);

            string name = mdecl.Name;

            ITypeSpecifier retType = mdecl.ReturnTypeNode;
            if (retType.TypeSpecifierRef == null)
            {
                var tVisitor = new TypeVisitor(TypeEnv);
                tVisitor.Visit(retType);
            }

            mdecl.ReturnTypeNode = retType;

            var methodVisitor = new DeclarationVisitor(mdecl);
            methodVisitor.Visit(mdecl.ParameterList);
            methodVisitor.Visit(mdecl.MethodBody);
        }


        /// <summary>
        /// Do all the parameter processing on the second pass b/c can't declare things in the method
        /// </summary>
        private void VisitNode(ParameterList paramList)
        {

            if (paramList == null) return;
            foreach (AbstractNode node in paramList)
                Visit(node);
        }

        private void VisitNode(Parameter p)
        {
            if (p.TypeSpecifier.TypeSpecifierRef == null)
            {
                var tVisitor = new TypeVisitor(TypeEnv);
                tVisitor.Visit(p.TypeSpecifier);
            }

            string pname = p.Name;

            if (NameEnv.IsDeclaredLocally(pname))
            {
                CompilerErrors.Add(SemanticErrorTypes.DuplicateParamName, pname);
            }
            else
            {
                NameEnv.EnterInfo(pname, p.TypeSpecifier);
            }
        }

        private void VisitNode(Block body)
        {
            foreach (AbstractNode node in body)
                Visit(node);
        }

        private void VisitNode(Expression expr)
        {
            _log.Trace("Visiting Expr, not yet implemented..");
        }


        private void VisitNode(VariableListDeclaring decls)
        {
            _log.Trace("Analyzing VariableDecls");

            var typeVisitor = new TypeVisitor(TypeEnv);
            decls.FieldTypeSpecifier.Accept(typeVisitor);

            foreach (AbstractNode node in decls.ItemIdList)
            {
                Identifier id = node as Identifier;
                Debug.Assert(id != null, "DeclaredVars node children should be Identifiers");

                if (NameEnv.IsDeclaredLocally(id.Name))
                {
                    CompilerErrors.Add(SemanticErrorTypes.VariableAlreadyDeclared, id.Name);
                    id.NodeTypeCategory = NodeTypeCategory.ErrorType;
                }
                else
                {
                    // this attrib was found in the symbol table by typevisitor
                    var typeSpecEntry = decls.FieldTypeSpecifier as ITypeSpecifier;
                    Debug.Assert(typeSpecEntry != null, "The node specifying the type is not of ITypeInfo");

                    // copy the found entry to the declared var
                    id.NodeTypeCategory = typeSpecEntry.NodeTypeCategory;
                    id.TypeSpecifierRef = typeSpecEntry.TypeSpecifierRef;

                    // and are saved in the symbol table
                    NameEnv.EnterInfo(id.Name, id);
                }
            }
        }

        private void VisitNode(AbstractNode node)
        {
            _log.Trace("Visiting {0}, no action.", node);
        }
    }

}
