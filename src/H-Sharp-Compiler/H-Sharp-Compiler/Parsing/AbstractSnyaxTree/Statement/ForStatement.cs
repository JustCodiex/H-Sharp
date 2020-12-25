using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Statement {
    
    public class ForStatement : LoopNode, IVariableScope {

        public ASTNode Init { get; }

        public IExpr After { get; }

        public ushort[] VarIndices { get; set; }

        public ForStatement(SourcePosition pos, ASTNode init, IExpr condition, IExpr after, IExpr body) : base(condition, body, "for", pos) {
            this.Init = init;
            this.After = after;
        }

        public override string ToString() => $"for ({this.Init} {this.Condition}; {this.After}) {this.Body}";

    }

}
