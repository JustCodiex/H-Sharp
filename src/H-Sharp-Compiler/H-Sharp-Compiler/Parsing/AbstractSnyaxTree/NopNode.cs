namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public class NopNode : ASTNode {
        public NopNode() : base(new IO.SourcePosition(uint.MaxValue, uint.MaxValue), string.Empty, LexTokenType.None) { }
    }

}
