using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Type {
    
    public class TypeArrayIdentifierNode : ASTNode, ITypeIdentifier {

        public ITypeIdentifier EncapsulatedType { get; }

        public TypeArrayIdentifierNode(ITypeIdentifier encapsulatedType, SourcePosition position) : base (position, string.Empty, LexTokenType.None) {
            this.EncapsulatedType = encapsulatedType;
        }

        public override string ToString() => $"{this.EncapsulatedType}[]";

    }

}
