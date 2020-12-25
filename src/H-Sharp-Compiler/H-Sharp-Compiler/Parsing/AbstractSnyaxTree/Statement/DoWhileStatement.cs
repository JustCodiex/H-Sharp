using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Statement {
    
    public class DoWhileStatement : LoopNode {
    
        public DoWhileStatement(IExpr body, IExpr condition, SourcePosition position) : base(condition, body, "do", position) {}

        public override string ToString() => $"do {this.Body} while {this.Condition};";

    }

}
