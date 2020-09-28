using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public class IdentifierNode : ASTNode {
        public ushort Index { get; set; } = ushort.MaxValue;
        public IdentifierNode(string identifier, SourcePosition pos) : base(pos, identifier, LexTokenType.Identifier) { }
    }

}
