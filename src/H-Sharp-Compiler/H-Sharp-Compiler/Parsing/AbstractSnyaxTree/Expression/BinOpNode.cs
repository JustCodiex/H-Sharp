using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {

    public class BinOpNode : ASTNode, IExpr {

        public enum OpSide {
            LHS,
            RHS
        }

        private ASTNode m_left;
        private ASTNode m_right;

        public ASTNode Left => this.m_left;

        public ASTNode Right => this.m_right;

        public string Op { get; }

        public BinOpNode(SourcePosition position, ASTNode left, string op, ASTNode right) : base(position, op, LexTokenType.Operator) {
            this.m_left = left;
            this.m_right = right;
            this.Op = op;
        }

        public void Update(OpSide side, ASTNode node) {
            switch (side) {
                case OpSide.LHS: this.m_left = node; break;
                case OpSide.RHS: this.m_right = node; break;
                default: throw new System.NotImplementedException();
            }
        }

        public override string ToString() => $"{this.Left} {this.Op} {this.Right}";

    }

}
