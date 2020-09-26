using System;
using System.Collections.Generic;
using System.Text;

namespace HSharp.Compiling {
    
    public class CompileContext {

        private Dictionary<string, Stack<int>> m_varPtrs;
        private int m_varStackId;
    
        public CompileResult Result { get; set; }

        public CompileContext() {
            this.Result = new CompileResult(true);
            this.m_varPtrs = new Dictionary<string, Stack<int>>();
            this.m_varStackId = 0;
        }

        public void UpdateResultIfErr(CompileResult result) {
            if (!result.Success) {
                this.Result = result;
            }
        }

        public int Enter(string varId) {
            int next = this.m_varStackId++;
            if (this.m_varPtrs.ContainsKey(varId)) {
                this.m_varPtrs[varId].Push(next);
            } else {
                Stack<int> stack = new Stack<int>();
                stack.Push(next);
                this.m_varPtrs.Add(varId, stack);
            }
            return next;
        }

        public int Lookup(string varId) => this.m_varPtrs[varId].Peek();

    }

}
