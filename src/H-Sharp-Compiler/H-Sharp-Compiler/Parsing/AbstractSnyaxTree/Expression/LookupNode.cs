using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    
    public class LookupNode : ASTNode, IExpr {

        public IExpr Left { get; }

        public IndexerNode Index { get; }

        public LookupNode(IExpr left, IndexerNode indexer, SourcePosition position) : base(position, string.Empty, LexTokenType.None) {
            this.Left = left;
            this.Index = indexer;
        }

        public override string ToString() => $"{this.Left}{this.Index}";

    }

}
