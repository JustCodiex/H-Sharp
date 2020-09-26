using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public class CompileUnitNode : ASTNode, IEnumerable<ASTNode> {

        private List<ASTNode> m_sequence;

        public ImmutableList<ASTNode> Sequence => this.m_sequence.ToImmutableList();

        public CompileUnitNode(List<ASTNode> sequence) : base(new SourcePosition(1, 1), string.Empty, LexTokenType.None) {
            this.m_sequence = sequence;
        }

        public override string ToString() => $"{{ {string.Join(" ", m_sequence)} }}";

        public IEnumerator<ASTNode> GetEnumerator() => ((IEnumerable<ASTNode>)this.m_sequence).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.m_sequence).GetEnumerator();

    }

}
