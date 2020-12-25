using System;
using System.Collections.Generic;
using System.Linq;
using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    
    public class ExpressionNode : ASTNode, IGroupedASTNode {

        private List<ASTNode> m_nodes;

        public List<ASTNode> Nodes => this.m_nodes;

        public int Size => this.m_nodes.Count;

        public ExpressionNode(SourcePosition position) : base(position, "()", LexTokenType.None) {}

        public ExpressionNode(SourcePosition position, List<ASTNode> nodes) : base(position, "()", LexTokenType.None) {
            this.m_nodes = nodes;
        }

        public int Count(Predicate<ASTNode> condition) => this.m_nodes.Count(x => condition(x));

        public List<List<ASTNode>> Split(Predicate<ASTNode> splitCondition) {
            var ls = new List<List<ASTNode>>();
            var curr = new List<ASTNode>();
            for (int i = 0; i < this.m_nodes.Count; i++) {
                if (splitCondition(this.m_nodes[i])) {
                    ls.Add(curr);
                    curr = new List<ASTNode>();
                } else {
                    curr.Add(this.m_nodes[i]);
                }
            }
            if (curr.Count > 0) {
                ls.Add(curr);
            }
            return ls;
        }

        public void SetNodes(List<ASTNode> nodes) => this.m_nodes = nodes;

        public ASTNode this[int index] => this.m_nodes[index];

        public ASTNode this[Index index] => this.m_nodes[index];

        public override string ToString() => $"( {string.Join(' ', this.m_nodes)} )";

    }

}
