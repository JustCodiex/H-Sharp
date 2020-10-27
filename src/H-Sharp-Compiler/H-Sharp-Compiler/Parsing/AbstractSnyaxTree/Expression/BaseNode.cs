using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    
    public class BaseNode : ASTNode, IExpr{
    
        public BaseNode(SourcePosition position) : base(position, "base", LexTokenType.Keyword) { }

    }

}
