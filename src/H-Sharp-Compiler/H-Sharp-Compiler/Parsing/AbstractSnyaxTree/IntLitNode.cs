using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public class IntLitNode : ASTNode {
        public int Integer { get; }
        public IntLitNode(string intvalue, SourcePosition pos) : base(pos, intvalue, LexTokenType.IntLiteral) {
            this.Integer = int.Parse(intvalue);
        }
    }

}
