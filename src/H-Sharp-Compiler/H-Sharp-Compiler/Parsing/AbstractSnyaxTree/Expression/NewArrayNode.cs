using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    
    public class NewArrayNode : ASTNode, IExpr {

        public ITypeIdentifier Type { get; }

        public IndexerNode Indexer { get; }

        public NewArrayNode(ITypeIdentifier type, IndexerNode indexer, SourcePosition position) : base(position, string.Empty, LexTokenType.None) {
            this.Type = type;
            this.Indexer = indexer;
        }

        public override string ToString() => $"new {this.Type}{this.Indexer}";

    }

}
