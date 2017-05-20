using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics
{
    using FrameDict = Dictionary<string, SymbolTableEntry>;

    public enum AttribRecordTypes
    {
        TypeAttrib, MethodAttribs, ClassAttribs
    }
    public class SymbolTableEntry
    {
        public AttribRecordTypes EntryType { get; set; }
        public SymbolAttributes AttribRecord { get; set; }
        public VariableTypes VariableType { get; set; }
    }

    public interface ISymbolTable
    {
        int CurrentNestLevel { get; }
        void IncrNestLevel();
        void DecrNestLevel();
        void EnterInfo(string s, SymbolTableEntry info);
        bool IsDeclaredLocally(string s);
        /// <summary>
        /// Returns the information associated with the innermost currently valid
        ///     declaration of the given symbol.  If there is no such valid declaration,
        ///     return null.  Do NOT throw any excpetions from this method.
        /// </summary>
        SymbolTableEntry Lookup(string s);
    }

    public class SymbolTable : ISymbolTable
    {
        private Stack<FrameDict> _frames = new Stack<FrameDict>();
        public int CurrentNestLevel => _frames.Count;
        public virtual void IncrNestLevel() => _frames.Push(new FrameDict());
        public virtual void DecrNestLevel() => _frames.Pop();
        public virtual void EnterInfo(string s, SymbolTableEntry info) => _frames.Peek().Add(s, info);
        public bool IsDeclaredLocally(string s) => _frames.Peek().ContainsKey(s);

        /// <summary>
        /// Returns the information associated with the innermost currently valid
        ///     declaration of the given symbol.  If there is no such valid declaration,
        ///     return null.  Do NOT throw any excpetions from this method.
        /// </summary>
        public virtual SymbolTableEntry Lookup(string s)
        {
            foreach (FrameDict frame in _frames)
            {
                if (frame.ContainsKey(s))
                {
                    return frame[s];
                }
            }
            return null;
        }

    }

}
