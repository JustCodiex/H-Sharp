using HSharp.Util;

namespace HSharp.Analysis.Verifying {

    public class VarScope {

        public LookupStack<string, ushort> stack = new LookupStack<string, ushort>();

        public ushort top = 0;

        public VarScope() { }

        public VarScope(VarScope copy) {
            this.stack = new LookupStack<string, ushort>(copy.stack);
            this.top = copy.top;
        }

        public ushort Enter(string v) {
            this.stack.Enter(v, this.top);
            return this.top++;
        }

        public ushort Lookup(string v) => this.stack.Top(v);

        public bool Lookup(string v, out ushort index) {
            if (this.stack.Exists(v)) {
                index = this.Lookup(v);
                return true;
            } else {
                index = ushort.MaxValue;
                return false;
            }
        }

        public void Exit(string v) => this.stack.Exit(v);

    }

}
