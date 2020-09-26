using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public class IdentifierNode : ASTNode {
        public IdentifierNode(string identifier, SourcePosition pos) : base(pos, identifier, LexTokenType.Identifier) { }
    }

}
