using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Statement {
    
    public class WhileStatement : LoopNode {

        public WhileStatement(IExpr condition, IExpr body, SourcePosition position) : base(condition, body, "while", position) {}

    }

}
