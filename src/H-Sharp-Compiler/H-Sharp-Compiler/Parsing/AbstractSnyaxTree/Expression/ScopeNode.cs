using System.Collections.Generic;
using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    
    public class ScopeNode : ASTNode, IGroupedASTNode {

        private List<ASTNode> m_nodes;

        public List<ASTNode> Nodes => this.m_nodes;

        public ushort[] VarIndices { get; set; }

        public int Size => this.Nodes.Count;

        public ScopeNode(SourcePosition position) : base(position, "{}", LexTokenType.None) {
            this.VarIndices = new ushort[0];
            this.m_nodes = new List<ASTNode>();
        }

        public void SetNodes(List<ASTNode> nodes) => this.m_nodes = nodes;

        public void InjectNodes(List<ASTNode> nodes, int pos) => this.m_nodes.InsertRange(pos, nodes);

        public ASTNode this[int index] => this.m_nodes[index];

        public override string ToString() => $"{{ {string.Join(' ', this.m_nodes)} }}";

    }

}
