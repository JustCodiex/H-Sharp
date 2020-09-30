using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    
    public class NewObjectNode : ASTNode, IExpr {

        public TypeIdentifierNode Type { get; }

        public ArgumentsNode CtorArguments { get; }

        public bool IsValid => CtorArguments.IsValid;

        public NewObjectNode(TypeIdentifierNode typeIdentifier, ExpressionNode ctorArgs, SourcePosition position) : base(position, "new", LexTokenType.None) {
            this.Type = typeIdentifier;
            this.CtorArguments = new ArgumentsNode(ctorArgs);
        }

        public override string ToString() => $"new {this.Type}({this.CtorArguments})";

    }

}
