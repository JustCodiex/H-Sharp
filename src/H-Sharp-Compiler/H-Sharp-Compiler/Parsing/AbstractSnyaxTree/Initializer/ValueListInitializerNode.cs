using System.Collections;
using System.Collections.Generic;

using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Initializer {
    
    public class ValueListInitializerNode : ASTNode, IInitializer, IEnumerable<IExpr> {

        private List<IExpr> m_valueNodes;

        public int Length => this.m_valueNodes.Count;

        public ValueListInitializerNode(SourcePosition position) : base(position, string.Empty, LexTokenType.None) {
            this.m_valueNodes = new List<IExpr>();
        }

        public void Value(IExpr expr) => this.m_valueNodes.Add(expr);

        public override string ToString() => $"[{string.Join(", ", this.m_valueNodes)}]";

        public IEnumerator<IExpr> GetEnumerator() => ((IEnumerable<IExpr>)this.m_valueNodes).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)this.m_valueNodes).GetEnumerator();

    }

}
