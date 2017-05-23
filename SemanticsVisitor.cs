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
    using IEnv = ISymbolTable<ITypeSpecifier>;
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

        // PAGE 319
        //procedureVISIT(ClassDeclaring cd)
        //typeRef ← new TypeDescriptor(ClassType) typeRef.names ← new SymbolTable( )
        //attr ← new Attributes ( ClassAttributes ) attr.classType ← typeRef
        //call currentSymbolTable.ENTERSYMBOL( name.name, attr ) 
        //call SETCURRENTCLASS( attr )
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
            string name = cdecl.Name;
            ITypeSpecifier entry = TypeEnv.Lookup(name);
            if (entry != null)
            {
                CompilerErrors.Add(SemanticErrorTypes.DuplicateClassDecl, name);
                cdecl.TypeSpecifierRef = null;
            }
            else
            {
                TypeEnv.EnterInfo(name, cdecl);
            }
        }

        private static void CheckEnterClassMemberDef(
            IClassMember classMember,
            IEnv env)
        {
            _log.Trace("Checking class member in env: " + classMember);
            ITypeSpecifier entry = env.Lookup(classMember.Name);
            if (entry != null)
            {
                CompilerErrors.Add(SemanticErrorTypes.DuplicateClassDecl, classMember.Name);
                classMember.TypeSpecifierRef = null;
            }
            else
            {
                env.EnterInfo(classMember.Name, classMember);
            }
        }

        private void VisitNode(CompilationUnit unit)
        {
            // put class refs into the current type env
            // no need to get the type of the class unless we change to have inheritance
            //var typeVisitor = new TypeVisitor(TypeEnv);


            // First enter all the top class declarations into the env
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


        /// <summary>
        /// MARKER ***  14  ***
        /// </summary>

        private void ProcessClassFields(ClassFields fields, IEnv classNameEnv)
        {
            foreach (AbstractNode node in fields)
            {
                ClassFieldDeclStatement classFieldDeclStatement = node as ClassFieldDeclStatement;
                if (classFieldDeclStatement == null) throw new NullReferenceException();

                VariableListDeclaring vld = classFieldDeclStatement.VariableListDeclaring as VariableListDeclaring;
                if (vld == null) throw new NullReferenceException("Declared class variable list is null.");

                ITypeSpecifier typeNameDecl = vld.FieldTypeSpecifier;
                if (typeNameDecl == null) throw new NullReferenceException("Declared class field is not ITypeInfo.");

                // lookup the types
                if (typeNameDecl.TypeSpecifierRef == null)
                {
                    TypeVisitor tVisitor = new TypeVisitor(TypeEnv);
                    tVisitor.Visit(vld.FieldTypeSpecifier);
                }

                // what are the modifiers
                var modifierTokens = classFieldDeclStatement.Modifiers.ModifierTokens;

                // enter each field
                foreach (AbstractNode decl in vld.ItemIdList)
                {
                    FieldVarDecl fieldVarDecl = decl as FieldVarDecl;
                    if (fieldVarDecl == null) throw new ArgumentException("Variable being declared is not an identifier.");

                    string name = fieldVarDecl.Name;

                    ProcessModifierTokens(fieldVarDecl, modifierTokens, name);

                    // copy the link to the proper type
                    fieldVarDecl.TypeSpecifierRef = typeNameDecl.TypeSpecifierRef;

                    CheckEnterClassMemberDef(fieldVarDecl, classNameEnv);
                }
            }
        }
        private void ProcessClassMethodParams(
            ParameterList parameterList,
            IEnv methodLocalsEnv)
        {
            if (parameterList == null) return;
            foreach (AbstractNode node in parameterList)
            {
                Parameter p = node as Parameter;
                if (p == null) throw new ArgumentNullException(nameof(p));

                if (p.TypeSpecifier.TypeSpecifierRef == null)
                {
                    var tVisitor = new TypeVisitor(TypeEnv);
                    tVisitor.Visit(p.TypeSpecifier);
                }

                p.Identifier.NodeTypeCategory = p.TypeSpecifier.NodeTypeCategory;
                p.Identifier.TypeSpecifierRef = p.TypeSpecifier;
            }
        }

        private void ProcessClassMethods(AbstractNode methods, IEnv classNameEnv)
        {
            foreach (AbstractNode node in methods)
            {
                MethodDeclaration mdecl = (node as MethodDeclaration);
                if (mdecl == null) throw new NullReferenceException();

                string methodName = mdecl.Name;
                var modifierTokens = mdecl.Modifiers.ModifierTokens;
                ProcessModifierTokens(mdecl, modifierTokens, methodName);

                TypeSpecifier retType = mdecl.ReturnType;
                if (retType.TypeSpecifierRef == null)
                {
                    var tVisitor = new TypeVisitor(TypeEnv);
                    tVisitor.Visit(retType);
                }

                mdecl.TypeSpecifierRef = retType.TypeSpecifierRef;

                var methodLocalsEnv = classNameEnv.GetNewLevel();
                ProcessClassMethodParams(mdecl.ParameterList, methodLocalsEnv);

                // TODO: visit the body with the new scope

                CheckEnterClassMemberDef(mdecl, classNameEnv);

            }
        }



        private void VisitNode(ClassDeclaration cdecl)
        {
            _log.Trace("Analyzing ClassDeclaring");

            string name = cdecl.Name;

            var modifiers = cdecl.Modifiers.ModifierTokens;
            ProcessModifierTokens(cdecl, modifiers, name);

            ProcessClassFields(cdecl.Fields, cdecl.NameEnv);
            ProcessClassMethods(cdecl.Methods, cdecl.NameEnv);


        }


        //foreach (AbstractNode classField in cdecl.Fields)
        //{
        //    // TODO
        //    var methodTypeVisitor = new TypeVisitor(cdecl.MethodsEnv);
        //    methodTypeVisitor.VisitChildren(cdecl.Methods);

        //    var fieldTypeVisitor = new TypeVisitor(cdecl.FieldsEnv);
        //    fieldTypeVisitor.VisitChildren(cdecl.Fields);
        //}

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

