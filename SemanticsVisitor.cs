using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.Nodes;

namespace Proj3Semantics
{
    using IEnv = ISymbolTable<ITypeInfo>;
    public abstract class SemanticsVisitor : IReflectiveVisitor
    {
        protected static Logger _log = LogManager.GetCurrentClassLogger();
        public void VisitChildren(AbstractNode node)
        {
            _log.Trace(this.GetType().Name.ToString() + " is visiting children of " + node);

            foreach (AbstractNode child in node)
            {
                child.Accept(this);
            }
        }

        public abstract void Visit(dynamic node);
    }

    /// <summary>
    /// PAGE 302
    /// </summary>
    public class TopDeclVisitor : SemanticsVisitor
    {

        protected new static Logger _log = LogManager.GetCurrentClassLogger();
        private IEnv NameEnv { get; set; }
        private IEnv TypeEnv { get; set; }

        public TopDeclVisitor(
            IEnv typeEnv,
            IEnv nameEnv)
        {
            NameEnv = nameEnv;
            TypeEnv = typeEnv;
        }

        public override void Visit(dynamic node)
        {
            _log.Trace(this.GetType().Name + " is visiting " + node);
            VisitNode(node);
        }

        // VISIT METHODS
        // ============================================================

        //procedureVISIT(ClassDeclaring cd)
        //typeRef ← new TypeDescriptor(ClassType) typeRef.names ← new SymbolTable( )
        //attr ← new Attributes ( ClassAttributes ) attr.classType ← typeRef
        //call currentSymbolTable.ENTERSYMBOL( name.name, attr ) call SETCURRENTCLASS( attr )
        //if cd.parentclass = null
        //then cd.parentclass ← GETREFTOOBJECT( )
        //else
        //typeVisitor ← new TypeVisitor ( )
        //call cd.parentclass.ACCEPT( typeVisitor )
        //if cd.parentclass.type = errorType then attr.classtype ← errorType else
        //if cd.parentclass.type.kind   classType then
        //attr.classtype ← errorType
        //call ERROR( parentClass.name, ”does not name a class” ) else
        //typeRe f.parent ← cd.parentClass.attributeRe f typeRef.isFinal ← MEMBEROF(cd.modifiers,final) typeRef.isAbstractl ← MEMBEROF(cd.modifiers,abstract) call typeRef.names.INCORPORATE(cd.parentclass.type.names) call OPENSCOPE(typeRef.names)
        //call cd. f ields.ACCEPT( this )
        //call cd.constructors.ACCEPT( this )
        //call cd.methods.ACCEPT( this )
        //call CLOSESCOPE( )
        //call SETCURRENTCLASS( null ) end

        private void CheckEnterClassDef(ClassDeclaration cdecl)
        {
            _log.Trace("Checking class declaration in env: " + cdecl);
            string name = cdecl.Identifier.Name;
            ITypeInfo entry = TypeEnv.Lookup(name);
            if (entry != null)
            {
                CompilerErrors.Add(SemanticErrorTypes.DuplicateClassDecl, name);
                cdecl.TypeInfoRef = null;
            }
            else
            {
                TypeEnv.EnterInfo(name, cdecl);
            }
        }
        private static void CheckEnterClassMemberDef(
            Identifier id,
            IEnv env)
        {
            _log.Trace("Checking class member in env: " + id);
            ITypeInfo entry = env.Lookup(id.Name);
            if (entry != null)
            {
                CompilerErrors.Add(SemanticErrorTypes.DuplicateClassDecl, id.Name);
                id.TypeInfoRef = null;
            }
            else
            {
                env.EnterInfo(id.Name, id);
            }
        }

        private void VisitNode(CompilationUnit unit)
        {
            // put class refs into the current type env
            // no need to get the type of the class unless we change to have inheritance
            //var typeVisitor = new TypeVisitor(TypeEnv);
            foreach (AbstractNode child in unit)
                CheckEnterClassDef(child as ClassDeclaration);

            foreach (AbstractNode child in unit)
            {
                var classTypeEnv = TypeEnv.GetNewLevel();
                var classNameEnv = NameEnv.GetNewLevel();
                var topDeclVisitor = new TopDeclVisitor(classTypeEnv, classNameEnv);
                topDeclVisitor.Visit(child);
            }
        }



        private void ProcessModifierTokens(ITypeHasModifiers classDecl)
        {
            bool accessorSet = false;
            classDecl.IsStatic = false;
            classDecl.AccessorType = AccessorType.Private;
            foreach (ModifierType modType in classDecl.Modifiers.ModifierTokens)
            {
                if (modType == ModifierType.PUBLIC || modType == ModifierType.PRIVATE)
                {
                    if (accessorSet)
                    {
                        var name = (classDecl as AbstractNode)?.Identifier.Name ?? "none";
                        CompilerErrors.Add(SemanticErrorTypes.InconsistentModifiers, name);
                        break;
                    }
                    accessorSet = true;
                    if (modType == ModifierType.PUBLIC)
                    {
                        classDecl.AccessorType = AccessorType.Public;
                    }
                }
                else if (modType == ModifierType.STATIC)
                {
                    classDecl.IsStatic = true;
                }
                else
                {
                    throw new NotImplementedException("Unrecognized modifier type encountered.");
                }
            }
            // remove child modifiers
            //(classDecl as AbstractNode)?.Remove(classDecl.Modifiers);
        }


        /// <summary>
        /// MARKER ***  14  ***
        /// </summary>
        private void VisitNode(ClassDeclaration cdecl)
        {
            _log.Trace("Analyzing ClassDeclaring");
            ProcessModifierTokens(cdecl);
            foreach (AbstractNode fieldDecl in cdecl.Fields)
            {
                VariableListDeclaring vld = (fieldDecl as ClassFieldDecl)?.VariableListDeclaring as VariableListDeclaring;
                if (vld == null) throw new NullReferenceException("Declared class variable list is null.");
                ITypeInfo typeNameDecl = vld.TypeNameDecl as ITypeInfo;
                if (typeNameDecl == null) throw new NullReferenceException("Declared class field is not ITypeInfo.");
                if (typeNameDecl.TypeInfoRef == null)
                {
                    TypeVisitor tVisitor = new TypeVisitor(TypeEnv);
                    tVisitor.Visit(vld.TypeNameDecl);
                }
                foreach (AbstractNode decl in vld.ItemIdList)
                {
                    Identifier id = decl as Identifier;
                    if (id == null) throw new ArgumentException("Variable being declared is not an identifier.");
                    CheckEnterClassMemberDef(id, TypeEnv);
                }
            }

            foreach (AbstractNode classField in cdecl.Fields)
            {
                // TODO
                var methodTypeVisitor = new TypeVisitor(cdecl.MethodsEnv);
                methodTypeVisitor.VisitChildren(cdecl.Methods);

                var fieldTypeVisitor = new TypeVisitor(cdecl.FieldsEnv);
                fieldTypeVisitor.VisitChildren(cdecl.Fields);
            }
        }

        /// <summary>
        /// MARKER ***  15  ***
        /// </summary>
        private void VisitNode(MethodDeclaration methodNode)
        {
            _log.Trace("Analyzing MethodDeclaration");
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
            var typeVisitor = new TypeVisitor(TypeEnv);
            decls.TypeNameDecl.Accept(typeVisitor);

            foreach (AbstractNode node in decls.ItemIdList)
            {
                Identifier id = node as Identifier;
                Debug.Assert(id != null, "DeclaredVars node children should be Identifiers");

                if (NameEnv.IsDeclaredLocally(id.Name))
                {
                    CompilerErrors.Add(SemanticErrorTypes.VariableAlreadyDeclared, id.Name);
                    id.TypeCategory = NodeTypeCategory.ErrorType;
                }
                else
                {
                    // attributes found in the symbol table
                    var typeDecl = decls.TypeNameDecl as ITypeInfo;
                    Debug.Assert(typeDecl != null, "The node specifying the type is not of ITypeInfo");
                    var attr = typeDecl.NodeTypeCategory;

                    // are now part of the description for this Identifier
                    id.TypeCategory = attr;

                    // and are saved in the symbol table
                    NameEnv.EnterInfo(id.Name, id);
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





    // Fills in the types of the child nodes to be used later
    // ======================================================


    public class TypeVisitor : SemanticsVisitor
    {
        private new static Logger _log = LogManager.GetCurrentClassLogger();

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

        private void VisitNode(ClassDeclaration classDecl)
        {
            //_log.Trace("Type visitor is visiting: " + classDecl);
            //string name = classDecl.Identifier.Name;
            //ITypeInfo entry = TypeEnv.Lookup(name);
            //if (entry != null)
            //{
            //    CompilerErrors.Add(SemanticErrorTypes.DuplicateClassDecl, name);
            //    classDecl.TypeInfoRef = null;
            //}
            //else
            //{
            //    TypeEnv.EnterInfo(name, classDecl);

            //    var methodTypeVisitor = new TypeVisitor(classDecl.MethodsEnv);
            //    methodTypeVisitor.VisitChildren(classDecl.Methods);

            //    var fieldTypeVisitor = new TypeVisitor(classDecl.FieldsEnv);
            //    fieldTypeVisitor.VisitChildren(classDecl.Fields);
            //}
        }
        private void VisitNode(MethodDeclaration mdecl)
        {
            _log.Trace("Type visitor is visiting: " + mdecl);
            //typeVisitor ← new TypeVisitor ( )


            //call md.returnType.ACCEPT(typeVisitor)
            //attr ← new Attributes ( MethodAttributes )
            //attr.returnType ← md.returnType.type
            //attr.modi f iers ← md.modi f iers
            //attr.isDe f inedIn ← GETCURRENTCLASS( )
            //attr.locals ← new SymbolTable ( )
            //call currentSymbolTable.ENTERSYMBOL( name.name, attr ) md.name.attributeRe f ← attr
            //call OPENSCOPE( attr.locals )
            //oldCurrentMethod ← GETCURRENTMETHOD( )
            //call SETCURRENTMETHOD( attr )
            //call md.parameters.ACCEPT( this )
            //attr.signature ← parameters.signature.ADDRETURN(attr.returntype) call md.body.ACCEPT( this )
            //call SETCURRENTMETHOD( oldCurrentMethod )
            //call CLOSESCOPE( )
        }


        private void VisitNode(Identifier id)
        {
            _log.Info("Type visitor is visiting: " + id);

            ITypeInfo entry = TypeEnv.Lookup(id.Name);
            if (entry != null)
            {
                id.TypeCategory = entry.NodeTypeCategory;
                id.TypeInfoRef = entry.TypeInfoRef;
            }
            else
            {
                CompilerErrors.Add(SemanticErrorTypes.IdentifierNotTypeName, id.Name);
                id.TypeCategory = NodeTypeCategory.ErrorType;
                id.TypeInfoRef = null;
            }
        }

        private void VisitNode(ArraySpecifier arrayDef)
        {
            throw new NotImplementedException("Arrays are not supported in this release");
        }

        /// <summary>
        /// DEFAULT
        /// </summary>
        /// <param name="node"></param>
        private void VisitNode(AbstractNode node)
        {
            _log.Trace("NO ACTION --- " + this.GetType().Name + " --- (abstract).");
        }

    }
}

