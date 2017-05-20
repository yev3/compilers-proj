using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proj3Semantics
{
    using FrameDict = Dictionary<string, SymInfo>;
    public class SymInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
    class SymbolTable
    {
        private Stack<FrameDict> _frames = new Stack<FrameDict>();
        public int CurrentNestLevel => _frames.Count;
        public virtual void IncrNestLevel() => _frames.Push(new FrameDict());
        public virtual void DecrNestLevel() => _frames.Pop();
        public virtual void EnterInfo(string s, SymInfo info) => _frames.Peek().Add(s, info);
        public bool IsDeclaredLocally(string s) => _frames.Peek().ContainsKey(s);

        /// <summary>
        /// Returns the information associated with the innermost currently valid
        ///     declaration of the given symbol.  If there is no such valid declaration,
        ///     return null.  Do NOT throw any excpetions from this method.
        /// </summary>
        public virtual SymInfo Lookup(string s)
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
