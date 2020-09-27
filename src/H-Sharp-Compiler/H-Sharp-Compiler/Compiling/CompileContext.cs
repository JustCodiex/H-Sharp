using System.Collections.Generic;

namespace HSharp.Compiling {
    
    public class CompileContext {

        private Dictionary<string, Stack<ushort>> m_varPtrs;
        private ushort m_varStackId;
    
        public CompileResult Result { get; set; }

        public CompileContext() {
            this.Result = new CompileResult(true);
            this.m_varPtrs = new Dictionary<string, Stack<ushort>>();
            this.m_varStackId = 0;
        }

        public void UpdateResultIfErr(CompileResult result) {
            if (!result.Success) {
                this.Result = result;
            }
        }

        public ushort Enter(string varId) {
            ushort next = this.m_varStackId++;
            if (this.m_varPtrs.ContainsKey(varId)) {
                this.m_varPtrs[varId].Push(next);
            } else {
                Stack<ushort> stack = new Stack<ushort>();
                stack.Push(next);
                this.m_varPtrs.Add(varId, stack);
            }
            return next;
        }

        public void Exit(string varId) {
            this.m_varPtrs[varId].Pop();
            if (this.m_varPtrs[varId].Count == 0) {
                this.m_varPtrs.Remove(varId);
            }
        }

        public ushort Lookup(string varId) => this.m_varPtrs[varId].Peek();

    }

}
