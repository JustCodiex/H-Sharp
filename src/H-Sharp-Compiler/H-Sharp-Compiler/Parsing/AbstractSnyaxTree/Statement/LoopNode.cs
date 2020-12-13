using System;
using System.Collections.Generic;
using System.Text;
using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Statement {
    
    public class LoopNode : ASTNode, IStatement {

        public LoopNode(SourcePosition position) : base(position, "Loop", LexTokenType.None) {

        }

    }

}
