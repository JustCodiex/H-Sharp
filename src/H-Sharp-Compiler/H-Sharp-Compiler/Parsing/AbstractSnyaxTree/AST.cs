using System;

namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public class AST {

        public CompileUnitNode Root { get; }

        public string Source { get; private set; }

        public AST(CompileUnitNode root) {
            this.Root = root;
            this.Source = null;
        }

        public void SetSource(string src) {
            if (string.IsNullOrEmpty(this.Source)) {
                this.Source = src;
            } else {
                throw new InvalidOperationException();
            }
        }

    }

}
