using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public class TypeIdentifierNode : ASTNode, ITypeIdentifier {

        private MemberAccessNode m_subExpr;

        public TypeIdentifierNode(string typename, SourcePosition position) : base(position, typename, LexTokenType.Identifier) {
            this.m_subExpr = null;
        }

        public TypeIdentifierNode(IdentifierNode identifierNode) : base(identifierNode.Pos, identifierNode.Content, LexTokenType.Identifier) {
            this.m_subExpr = null;
        }

        public TypeIdentifierNode(MemberAccessNode typeAccessNode, SourcePosition position) : base(position, typeAccessNode.ToString(), LexTokenType.Identifier) {
            this.m_subExpr = typeAccessNode;
        }

        public override string ToString() {
            if (this.m_subExpr is null) {
                return this.Content;
            } else {
                return this.m_subExpr.ToString();
            }
        } 
    }

}
