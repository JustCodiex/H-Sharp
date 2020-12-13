using System;
using System.Collections.Generic;
using System.Text;
using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Statement {
    
    public class LoopNode : ASTNode, IStatement {

        public IExpr Condition { get; }

        public IExpr Body { get; }

        public LoopNode(IExpr condition, IExpr body, string keyword, SourcePosition position) : base(position, keyword, LexTokenType.None) {
            this.Condition = condition;
            this.Body = body;
        }

        public override string ToString() => $"{this.Content} {this.Condition} {this.Body}";

    }

}
