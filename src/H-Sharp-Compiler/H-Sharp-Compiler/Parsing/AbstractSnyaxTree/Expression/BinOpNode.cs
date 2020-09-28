using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    
    public class BinOpNode : ASTNode, IExpr {

        public ASTNode Left { get; }

        public ASTNode Right { get; }

        public string Op { get; }

        public BinOpNode(SourcePosition position, ASTNode left, string op, ASTNode right) : base(position, op, LexTokenType.Operator) {
            this.Left = left;
            this.Right = right;
            this.Op = op;
        }
        
        public override string ToString() => $"{this.Left} {this.Op} {this.Right}";

    }

}
