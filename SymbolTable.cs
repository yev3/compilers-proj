using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics
{

    public interface ISymbolTable<TEntry>
    {
        int CurrentNestLevel { get; }
        ISymbolTable<TEntry> GetNewLevel(string envName = "");
        ISymbolTable<TEntry>  GetPrevLevel();
        void EnterInfo(string s, TEntry info);
        bool IsDeclaredLocally(string s);
        /// <summary>
        /// Returns the information associated with the innermost currently valid
        ///     declaration of the given symbol.  If there is no such valid declaration,
        ///     return null.  Do NOT throw any excpetions from this method.
        /// </summary>
        TEntry Lookup(string s);
    }

    [DebuggerDisplay("{ToString()}")]
    public class SymbolTable<TEntry> : ISymbolTable<TEntry>
    {
        private string EnvName { get; }
        private SymbolTable<TEntry> Parent { get; set; }
        private Dictionary<string, TEntry> Dict { get; } = new Dictionary<string, TEntry>();
        public int CurrentNestLevel { get; }

        public SymbolTable(SymbolTable<TEntry> parent = null, string envName = "")
        {
            CurrentNestLevel = (parent?.CurrentNestLevel ?? 0) + 1;
            Parent = parent;
            EnvName = envName;
        }

        public ISymbolTable<TEntry> GetNewLevel(string envName = "")
        {
            return new SymbolTable<TEntry>(this, envName);
        }

        public ISymbolTable<TEntry> GetPrevLevel() => Parent;
        public void EnterInfo(string s, TEntry info) => Dict.Add(s, info);
        public bool IsDeclaredLocally(string s) => Dict.ContainsKey(s);

        /// <summary>
        /// Returns the information associated with the innermost currently valid
        ///     declaration of the given symbol.  If there is no such valid declaration,
        ///     return null.  Do NOT throw any excpetions from this method.
        /// </summary>
        public TEntry Lookup(string s)
        {
            SymbolTable<TEntry> curNode = this;
            while (curNode != null)
            {
                if (curNode.Dict.ContainsKey(s))
                {
                    return curNode.Dict[s];
                }
                curNode = curNode.Parent;
            }
            return default(TEntry);
        }

        public override string ToString()
        {
            var names = new List<string>();

            SymbolTable<TEntry> curNode = this;
            while (curNode != null)
            {
                names.Add(curNode.EnvName);
                curNode = curNode.Parent;
            }
            return string.Join(".", names);
        }
    }

}
