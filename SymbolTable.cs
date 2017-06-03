using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Proj3Semantics.AST;

namespace Proj3Semantics
{
    using IEnv = ISymbolTable<Symbol>;
    public enum SymbolType
    {
        Variable, Namespace, Class, Method, Function
    }
    public class Symbol
    {
        public SymbolType SymbolType { get; set; }
        public DeclNode DeclNode { get; set; }
        public IEnv Env { get; set; }
        public bool HasEnv() => Env != null;
        public Symbol(SymbolType symbolType, DeclNode node, IEnv env = null)
        {
            SymbolType = symbolType;
            DeclNode = node;
            Env = env;
        }
    }


    // https://www.dotnetperls.com/multimap 
    public class MultiMap<TEntry>
    {
        Dictionary<string, List<TEntry>> _dictionary =
            new Dictionary<string, List<TEntry>>();

        public void Add(string key, TEntry value)
        {
            List<TEntry> list;
            if (this._dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new List<TEntry>();
                list.Add(value);
                this._dictionary[key] = list;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return this._dictionary.Keys;
            }
        }

        public List<TEntry> this[string key]
        {
            get
            {
                List<TEntry> list;
                if (!this._dictionary.TryGetValue(key, out list))
                {
                    list = new List<TEntry>();
                    this._dictionary[key] = list;
                }
                return list;
            }
        }

        public bool ContainsKey(string key)
        {
            List<TEntry> list;
            if (!this._dictionary.TryGetValue(key, out list))
            {
                return false;
            }
            return list.Count > 0;
        }
    }

    public interface ISymbolTable<TEntry>
    {
        int CurrentNestLevel { get; }
        List<TEntry> GetLocalDeclarations();
        ISymbolTable<TEntry> GetNewLevel(string envName = "");
        ISymbolTable<TEntry> GetPrevLevel();
        void EnterInfo(string s, TEntry info);
        bool IsVarDeclaredLocally(string s);
        /// <summary>
        /// Returns the information associated with the innermost currently valid
        ///     declaration of the given symbol.  If there is no such valid declaration,
        ///     return null.  Do NOT throw any excpetions from this method.
        /// </summary>
        List<TEntry> Lookup(string s);
        List<TEntry> LookupLocal(string s);
        List<TEntry> LookupLocalEntriesByType(string s, SymbolType type);
    }

    [DebuggerDisplay("{ToString()}")]
    public class SymbolTable : ISymbolTable<Symbol>
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        private string EnvName { get; }
        private SymbolTable Parent { get; set; }
        private MultiMap<Symbol> MultiMap { get; } = new MultiMap<Symbol>();
        public int CurrentNestLevel { get; }
        public List<Symbol> GetLocalDeclarations()
        {
            var result = new List<Symbol>();
            foreach (string key in MultiMap.Keys)
            {
                result.AddRange(MultiMap[key].Where(s => s.SymbolType == SymbolType.Variable));
            }
            return result;
        }

        public SymbolTable(IEnv parent = null, string envName = "")
        {
            CurrentNestLevel = (parent?.CurrentNestLevel ?? 0) + 1;
            Parent = parent as SymbolTable;
            EnvName = envName;
        }

        public IEnv GetNewLevel(string envName = "")
        {
            return new SymbolTable(this, envName);
        }

        public IEnv GetPrevLevel() => Parent;
        public void EnterInfo(string s, Symbol info)
        {
            Log.Info("Added {0} to {1}", s, this.ToString());
            MultiMap.Add(s, info);
        }

        public bool IsVarDeclaredLocally(string s)
        {
            return MultiMap[s].Any(sym => sym.SymbolType == SymbolType.Variable);
        }

        /// <summary>
        /// Returns the information associated with the innermost currently valid
        ///     declaration of the given symbol.  If there is no such valid declaration,
        ///     return null.  Do NOT throw any excpetions from this method.
        /// </summary>
        public List<Symbol> Lookup(string s)
        {
            SymbolTable curNode = this;
            while (curNode != null)
            {
                if (curNode.MultiMap.ContainsKey(s))
                {
                    return curNode.MultiMap[s];
                }
                curNode = curNode.Parent;
            }
            return null;
        }

        public List<Symbol> LookupLocal(string s)
        {
            return MultiMap[s];
        }


        public List<Symbol> LookupLocalEntriesByType(string s, SymbolType type)
        {
            return LookupLocal(s).Where(sym => sym.SymbolType == type).ToList();
        }

        public override string ToString()
        {
            var names = new Stack<string>();

            SymbolTable curNode = this;
            while (curNode != null)
            {
                if (string.IsNullOrEmpty(curNode.EnvName))
                {
                    names.Push("global");
                }
                else
                {
                    names.Push(curNode.EnvName);
                }
                curNode = curNode.Parent;
            }
            return string.Join(".", names);
        }
    }

}
