using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    
    public class ThisNode : ASTNode, IExpr {
    
        public ThisNode(SourcePosition position) : base(position, "this", LexTokenType.Keyword) { }

    }

}
