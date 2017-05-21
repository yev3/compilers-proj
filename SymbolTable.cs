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
        public VariableTypes KindVariableCategory { get; set; }
    }

    public interface ISymbolTable<TEntry>
    {
        int CurrentNestLevel { get; }
        void IncrNestLevel();
        void DecrNestLevel();
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
        private Stack<Dictionary<string, TEntry>> _frames = new Stack<Dictionary<string, TEntry>>();
        public int CurrentNestLevel => _frames.Count;
        public void IncrNestLevel() => _frames.Push(new Dictionary<string, TEntry>());
        public void DecrNestLevel() => _frames.Pop();
        public void EnterInfo(string s, TEntry info) => _frames.Peek().Add(s, info);
        public bool IsDeclaredLocally(string s) => _frames.Peek().ContainsKey(s);

        /// <summary>
        /// Returns the information associated with the innermost currently valid
        ///     declaration of the given symbol.  If there is no such valid declaration,
        ///     return null.  Do NOT throw any excpetions from this method.
        /// </summary>
        public TEntry Lookup(string s)
        {
            foreach (var frame in _frames)
            {
                if (frame.ContainsKey(s))
                {
                    return frame[s];
                }
            }
            return default(TEntry);
        }
    }

}
