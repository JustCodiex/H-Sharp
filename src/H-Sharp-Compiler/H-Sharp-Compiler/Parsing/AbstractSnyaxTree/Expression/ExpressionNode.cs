using System;
using System.Collections.Generic;
using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    
    public class ExpressionNode : ASTNode, IGroupedASTNode {

        private List<ASTNode> m_nodes;

        public List<ASTNode> Nodes => this.m_nodes;

        public int Size => this.m_nodes.Count;

        public ExpressionNode(SourcePosition position) : base(position, "()", LexTokenType.None) {

        }

        public void SetNodes(List<ASTNode> nodes) => this.m_nodes = nodes;

        public ASTNode this[int index] => this.m_nodes[index];

        public ASTNode this[Index index] => this.m_nodes[index];

        public override string ToString() => $"( {string.Join(',', this.m_nodes)} )";

    }

}
