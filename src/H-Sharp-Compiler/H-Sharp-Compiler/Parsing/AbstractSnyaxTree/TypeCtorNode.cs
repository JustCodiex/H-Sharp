using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public class TypeCtorNode : ASTNode, ITypeIdentifier {
    
        public TypeIdentifierNode Type { get; }

        public ArgumentsNode Arguments { get; }

        public TypeCtorNode(TypeIdentifierNode typeIdentifier, ArgumentsNode arguments) : base(typeIdentifier.Pos, typeIdentifier.Content, LexTokenType.None) {
            this.Type = typeIdentifier;
            this.Arguments = arguments;
        }

        public override string ToString() => $"{this.Type}{this.Arguments}";

    }

}
