using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Literal {
    
    public class IntLitNode : ASTNode, ILiteral {
        public int Integer { get; }
        public IntLitNode(string intvalue, SourcePosition pos) : base(pos, intvalue, LexTokenType.IntLiteral) {
            this.Integer = int.Parse(intvalue);
        }
    }

}
