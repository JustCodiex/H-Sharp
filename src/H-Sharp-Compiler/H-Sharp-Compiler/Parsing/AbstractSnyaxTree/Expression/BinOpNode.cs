using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {

    public class BinOpNode : ASTNode, IExpr, ILR {

        protected ASTNode m_left;
        protected ASTNode m_right;
        protected string m_op;

        public ASTNode Left => this.m_left;

        public ASTNode Right => this.m_right;

        public string Op => this.m_op;

        public BinOpNode(SourcePosition position) : base(position, string.Empty, LexTokenType.None) {
            this.m_op = "?";
        }

        public BinOpNode(SourcePosition position, ASTNode left, string op, ASTNode right) : base(position, op, LexTokenType.Operator) {
            this.m_left = left;
            this.m_right = right;
            this.m_op = op;
        }

        public void Update(LeftRight side, ASTNode node) {
            switch (side) {
                case LeftRight.LHS: this.m_left = node; break;
                case LeftRight.RHS: this.m_right = node; break;
                default: throw new System.NotImplementedException();
            }
        }

        public void SetOperation(string operation) => this.m_op = operation;

        public override string ToString() => $"{this.Left} {this.Op} {this.Right}";

    }

}
