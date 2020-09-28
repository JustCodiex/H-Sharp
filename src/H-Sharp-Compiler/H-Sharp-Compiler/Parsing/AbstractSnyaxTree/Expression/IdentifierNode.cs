using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    
    public class IdentifierNode : ASTNode, IExpr {

        public ushort Index { get; set; }
        
        public bool IsFuncIdentifier { get; set; }
        
        public IdentifierNode(string identifier, SourcePosition pos) : base(pos, identifier, LexTokenType.Identifier) {
            this.Index = ushort.MaxValue;
            this.IsFuncIdentifier = false;
        }

    }

}
