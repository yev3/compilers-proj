using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.ASTNodes;

namespace Proj3Semantics
{
    using IEnv = ISymbolTable<ITypeDescriptor>;
    /// <summary>
    /// PAGE 302
    /// </summary>
    public class FirstPassDeclVisitor : SemanticsVisitor
    {

        protected new static Logger _log = LogManager.GetCurrentClassLogger();
        private IEnv NameEnv { get; set; }
        private IEnv TypeEnv { get; set; }

        public FirstPassDeclVisitor(
            IEnv typeEnv,
            IEnv nameEnv)
        {
            NameEnv = nameEnv;
            TypeEnv = typeEnv;
        }

        public override void Visit(dynamic node)
        {
            _log.Trace(this.GetType().Name + " is visiting " + node + " in type/name env " + TypeEnv + " / " + NameEnv);
            VisitNode(node);
        }


        private void VisitNode(CompilationUnit unit)
        {
            foreach (AbstractNode child in unit)
                Visit(child);
        }


        private void VisitNode(NamespaceDecl nsdecl)
        {
            _log.Trace("Checking Namespace declaration in env: " + TypeEnv);
            string name = nsdecl.Name;
            ITypeDescriptor entry = TypeEnv.Lookup(name);
            if (entry != null)
            {
                CompilerErrors.Add(SemanticErrorTypes.DuplicateNamespaceDef, name);
                nsdecl.NodeTypeCategory = NodeTypeCategory.ErrorType;
            }
            else
            {
                TypeEnv.EnterInfo(name, nsdecl);
                var nsTypeEnv = TypeEnv.GetNewLevel(name);
                var nsNameEnv = NameEnv.GetNewLevel(name);
                nsdecl.TypeEnv = nsTypeEnv;
                nsdecl.NameEnv = nsNameEnv;
                var visitor = new FirstPassDeclVisitor(nsTypeEnv, nsNameEnv);
                visitor.Visit(nsdecl.NamespaceBody);
            }
        }

        private void VisitNode(NamespaceBody namespaceBody)
        {
            _log.Trace("Checking Namespace declaration in env: " + TypeEnv);
            foreach (AbstractNode node in namespaceBody)
                Visit(node);
        }


        private void VisitNode(ClassDeclaration cdecl)
        {
            _log.Trace("Checking Class declaration in env: " + TypeEnv);
            string name = cdecl.Name;
            ITypeDescriptor entry = TypeEnv.Lookup(name);
            if (entry != null)
            {
                CompilerErrors.Add(SemanticErrorTypes.DuplicateClassDecl, name);
                cdecl.TypeDescriptorRef = null;
                return;
            }
            TypeEnv.EnterInfo(name, cdecl);

            var modifiers = cdecl.Modifiers.ModifierTokens;
            ProcessModifierTokens(cdecl, modifiers, name);
            cdecl.Remove(cdecl.Modifiers);

            // open new scope
            var classTypeEnv = TypeEnv.GetNewLevel(name);
            var classNameEnv = NameEnv.GetNewLevel(name);
            cdecl.TypeEnv = classTypeEnv;
            cdecl.NameEnv = classNameEnv;
            var visitor = new FirstPassDeclVisitor(classTypeEnv, classNameEnv);

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
            DeclaredVars declFields = vld.ItemIdList;

            ITypeDescriptor typeNameDecl = vld.FieldTypeDescriptor;
            if (typeNameDecl == null) throw new NullReferenceException("Declared class field is not ITypeInfo.");

            var modifierTokens = fieldDecl.Modifiers.ModifierTokens;
            fieldDecl.Remove(fieldDecl.Modifiers);

            // lookup the types
            // TODO: done on the second pass?
            //if (typeNameDecl.TypeSpecifierRef == null)
            //{
            //    TypeVisitor tVisitor = new TypeVisitor(TypeEnv);
            //    tVisitor.Visit(vld.FieldTypeSpecifier);
            //}

            foreach (AbstractNode field in declFields)
            {
                FieldVarDecl fdecl = field as FieldVarDecl;
                if (fdecl == null) throw new ArgumentNullException(nameof(fdecl));

                string name = fdecl.Name;
                ProcessModifierTokens(fdecl, modifierTokens, name);

                ITypeDescriptor entry = TypeEnv.Lookup(name);
                if (entry != null)
                {
                    CompilerErrors.Add(SemanticErrorTypes.DuplicateClassDecl, name);
                    fdecl.NodeTypeCategory = NodeTypeCategory.ErrorType;
                    fdecl.TypeDescriptorRef = null;
                }
                else
                {
                    TypeEnv.EnterInfo(name, fdecl);
                }
            }

            // copy the link to the proper type
            // TODO: second pass?
            //fieldVarDecl.TypeSpecifierRef = typeNameDecl.TypeSpecifierRef;

        }



        // TODO: FIX METHOD SIGS
        private void VisitNode(MethodDeclaration mdecl)
        {
            _log.Trace("Checking fields declaration in env: " + TypeEnv);

            string name = mdecl.Name;
            var modifierTokens = mdecl.Modifiers.ModifierTokens;
            ProcessModifierTokens(mdecl, modifierTokens, name);
            mdecl.Remove(mdecl.Modifiers);

            // TODO: second pass?
            //TypeSpecifier retType = mdecl.ReturnType;
            //if (retType.TypeSpecifierRef == null)
            //{
            //    var tVisitor = new TypeVisitor(TypeEnv);
            //    tVisitor.Visit(retType);
            //}

            // the type of this 
            mdecl.TypeDescriptorRef = mdecl;

            ITypeDescriptor entry = TypeEnv.Lookup(name);
            if (entry != null)
            {
                CompilerErrors.Add(SemanticErrorTypes.DuplicateClassDecl, name);
                //mdecl.NodeTypeCategory = NodeTypeCategory.ErrorType;
                mdecl.TypeDescriptorRef = null;
            }
            else
            {
                TypeEnv.EnterInfo(name, mdecl);

                // visit the body with the new scope
                IEnv bodyTypeEnv = TypeEnv.GetNewLevel(name);
                IEnv bodyNameEnv = NameEnv.GetNewLevel(name);
                mdecl.TypeEnv = bodyTypeEnv;
                mdecl.NameEnv = bodyNameEnv;

                // THESE ARE DONE ON THE SECOND PASS!
                //ProcessClassMethodParams(mdecl.ParameterList, bodyNameEnv);
                //var bodyVisitor = new TopDeclVisitor(bodyTypeEnv, bodyNameEnv);
                //bodyVisitor.Visit(mdecl.MethodBody);
            }

        }






        // LOCAL VARIABLES
        // =============================================

        /// <summary>
        /// MARKER ***  12  ***
        /// </summary>
        /// <param name="decls"></param>
        private void VisitNode(VariableListDeclaring decls)
        {
            _log.Trace("Analyzing VariableDecls");

            // TODO: process later
            //var typeVisitor = new TypeVisitor(TypeEnv);
            //decls.FieldTypeSpecifier.Accept(typeVisitor);

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
                    var typeSpecEntry = decls.FieldTypeDescriptor as ITypeDescriptor;
                    Debug.Assert(typeSpecEntry != null, "The node specifying the type is not of ITypeInfo");

                    // copy the found entry to the declared var
                    id.NodeTypeCategory = typeSpecEntry.NodeTypeCategory;
                    id.TypeDescriptorRef = typeSpecEntry.TypeDescriptorRef;

                    // and are saved in the symbol table
                    NameEnv.EnterInfo(id.Name, id);
                }
            }
        }

        // ------------------------------------------------------------
        // HELPER METHODS
        // ------------------------------------------------------------


        /// <summary>
        /// NOTE: THIS IS DONE ON THE SECOND PASS
        /// </summary>
        //private void ProcessClassMethodParams(
        //    ParameterList parameterList,
        //    IEnv methodLocalsEnv)
        //{
        //    if (parameterList == null) return;
        //    foreach (AbstractNode node in parameterList)
        //    {
        //        Parameter p = node as Parameter;
        //        if (p == null) throw new ArgumentNullException(nameof(p));

        //        if (p.TypeSpecifier.TypeSpecifierRef == null)
        //        {
        //            var tVisitor = new TypeVisitor(TypeEnv);
        //            tVisitor.Visit(p.TypeSpecifier);
        //        }

        //        p.Identifier.NodeTypeCategory = p.TypeSpecifier.NodeTypeCategory;
        //        p.Identifier.TypeSpecifierRef = p.TypeSpecifier;
        //    }
        //}



        private void ProcessModifierTokens(
            ITypeHasModifiers nodeModified,
            List<ModifierType> modifiers,
            string nodeName)
        {
            bool accessorWasSet = false;
            nodeModified.IsStatic = false;
            nodeModified.AccessorType = AccessorType.Private;
            foreach (ModifierType modifier in modifiers)
            {
                if (modifier == ModifierType.PUBLIC || modifier == ModifierType.PRIVATE)
                {
                    if (accessorWasSet)
                    {
                        CompilerErrors.Add(SemanticErrorTypes.InconsistentModifiers, nodeName);
                        break;
                    }
                    accessorWasSet = true;
                    switch (modifier)
                    {
                        case ModifierType.PUBLIC:
                            nodeModified.AccessorType = AccessorType.Public;
                            break;
                        case ModifierType.PRIVATE:
                            nodeModified.AccessorType = AccessorType.Private;
                            break;
                        default:
                            throw new ArgumentException();
                    }
                }
                else if (modifier == ModifierType.STATIC)
                {
                    nodeModified.IsStatic = true;
                }
                else
                {
                    throw new NotImplementedException("Unrecognized modifier type encountered.");
                }
            }
        }


        //  WHAT DOES THIS DO?
        //     MARKER ***  13  ***
        //private void VisitNode(TypeDeclaring variableListDeclaring)
        //{
        //    _log.Trace("Analyzing TypeDeclaring");
        //}

        private void VisitNode(AbstractNode node)
        {
            _log.Trace("Visiting {0}, no action.", node);
        }
    }

}
