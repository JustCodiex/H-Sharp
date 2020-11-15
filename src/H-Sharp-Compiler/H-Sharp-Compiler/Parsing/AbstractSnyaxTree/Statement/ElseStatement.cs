using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Statement {
    
    public class ElseStatement : ASTNode, IStatement, IBranch {

        private IBranch m_head;

        public IExpr Body { get; }

        public IBranch Head => this.m_head;

        public ElseStatement(IBranch head, IExpr body, SourcePosition position) : base (position, "else", LexTokenType.None) {
            this.Body = body;
            this.m_head = head;
        }

        public void SetTrail(IBranch err) => throw new System.InvalidOperationException();

        public override string ToString() => $"else {this.Body}";

    }

}
