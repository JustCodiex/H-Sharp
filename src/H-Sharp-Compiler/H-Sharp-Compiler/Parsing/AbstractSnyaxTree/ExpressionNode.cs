using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree {
    public abstract class ExpressionNode : ASTNode {
        public ExpressionNode(SourcePosition position) : base(position, string.Empty, LexTokenType.None) { }
        public ExpressionNode(SourcePosition position, string content) : base(position, content, LexTokenType.None) { }
    }
}
