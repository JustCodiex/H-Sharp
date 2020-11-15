using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Statement {
    
    public class ElseIfStatement : IfStatement { // NOTE: Because we're inheriting from IfStatement, we will type match on it - so ElseIf must always be matched before If

        private IBranch m_head;

        public IBranch Head => this.m_head;

        public ElseIfStatement(IBranch head, IExpr condition, IExpr body, SourcePosition position) : base(condition, body, position) {
            this.m_head = head;
        }

        public override string ToString() => $"else {base.ToString()}";

    }

}
