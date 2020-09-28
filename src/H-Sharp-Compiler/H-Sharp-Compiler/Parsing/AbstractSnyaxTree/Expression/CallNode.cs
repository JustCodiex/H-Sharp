using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    
    public class CallNode : ASTNode, IExpr {

        public ASTNode IdentifierNode { get; }

        public ArgumentsNode Arguments { get; set; }

        public CallNode(ASTNode expr, SourcePosition position) : base(position, expr.Content, LexTokenType.None) {
            this.IdentifierNode = expr;
        }

        public override string ToString() => $"{this.IdentifierNode}{this.Arguments}";

    }

}
