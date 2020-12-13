using System;
using System.Collections.Generic;
using System.Text;
using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Statement {
    
    public class WhileStatement : LoopNode {

        public IExpr Condition { get; }

        public IExpr Body { get; }

        public WhileStatement(IExpr condition, IExpr body, SourcePosition position) : base(position) {
            this.Condition = condition;
            this.Body = body;
        }

        public override string ToString() => $"while {this.Condition} {this.Body}";

    }

}
