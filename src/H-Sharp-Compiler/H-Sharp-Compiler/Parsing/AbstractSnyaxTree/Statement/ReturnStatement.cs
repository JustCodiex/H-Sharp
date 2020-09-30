using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Statement {
    
    public class ReturnStatement : ASTNode, IStatement{

        public IExpr Expression { get; }

        public ReturnStatement(IExpr expression, SourcePosition position) : base(position, "return", LexTokenType.None) {
            this.Expression = expression;
        }

        public override string ToString() => $"return {this.Expression};";

    }

}
