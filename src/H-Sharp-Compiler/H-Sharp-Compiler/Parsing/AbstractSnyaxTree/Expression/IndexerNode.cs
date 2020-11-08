using System.Collections.Generic;
using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    
    public class IndexerNode : ASTNode, IGroupedASTNode {

        private List<ASTNode> m_nodes;

        public List<ASTNode> Nodes => this.m_nodes;

        public IndexerNode(SourcePosition position) : base(position, "[]", LexTokenType.IndexerStart) {}

        public void SetNodes(List<ASTNode> nodes) => this.m_nodes = nodes;

        public ASTNode this[int index] => this.m_nodes[index];

        public override string ToString() => $"[ {string.Join(',', this.m_nodes)} ]";

    }

}
