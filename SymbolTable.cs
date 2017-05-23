using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics
{

    public class SymbolTableEntry
    {
        //public AttribRecordTypes EntryType { get; set; }
        //public SymbolAttributes AttribRecord { get; set; }
        public NodeTypeCategory KindNodeCategory { get; set; }
    }

    public interface ISymbolTable<TEntry>
    {
        int CurrentNestLevel { get; }
        ISymbolTable<TEntry> GetNewLevel();
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

    public class SymbolTable<TEntry> : ISymbolTable<TEntry>
    {
        private SymbolTable<TEntry> Parent { get; set; }
        private Dictionary<string, TEntry> Dict { get; } = new Dictionary<string, TEntry>();
        public int CurrentNestLevel { get; }

        public SymbolTable(SymbolTable<TEntry> parent = null)
        {
            CurrentNestLevel = (parent?.CurrentNestLevel ?? 0) + 1;
            Parent = parent;
        }

        public ISymbolTable<TEntry> GetNewLevel()
        {
            return new SymbolTable<TEntry>(this);
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
    }

}
