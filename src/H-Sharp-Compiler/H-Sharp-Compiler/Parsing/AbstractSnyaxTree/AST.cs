using System;
using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public class AST {

        public CompileUnitNode Root { get; }

        public SourceProjectFile? Source { get; private set; }

        public AST(CompileUnitNode root) {
            this.Root = root;
            this.Source = null;
        }

        public void SetSource(SourceProjectFile src) {
            if (this.Source is null) {
                this.Source = src;
            } else {
                throw new InvalidOperationException();
            }
        }

    }

}
