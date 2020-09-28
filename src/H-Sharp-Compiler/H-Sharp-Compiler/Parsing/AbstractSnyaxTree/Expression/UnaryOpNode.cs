using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    public class UnaryOpNode : ASTNode, IExpr {

        public ASTNode Expr { get; }

        public string Op { get; }

        public bool IsPostOp { get; }

        public UnaryOpNode(SourcePosition position, ASTNode expr, string op, bool isPostOp) : base(position, op, LexTokenType.Operator) {
            this.Expr = expr;
            this.IsPostOp = isPostOp;
            this.Op = op;
        }

        public override string ToString() => IsPostOp ? $"{this.Expr}{this.Op}" : $"{this.Op}{this.Expr}";

    }
}
