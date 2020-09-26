using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public class SeperatorNode : ASTNode {
        public SeperatorNode(string identifier, SourcePosition pos) : base(pos, identifier, LexTokenType.Separator) { }
    }

}
