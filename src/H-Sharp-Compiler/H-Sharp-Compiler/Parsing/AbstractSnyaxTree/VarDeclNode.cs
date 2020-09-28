using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public class VarDeclNode : ASTNode {

        public ASTNode TypeExpr { get; }

        public string VarName { get; }

        public ASTNode AssignToExpr { get; }

        public ushort EnterIndex { get; set; }

        public VarDeclNode(SourcePosition pos, ASTNode varTypeNode, BinOpNode assignNode) : base(pos, "=", LexTokenType.None) {
            this.TypeExpr = varTypeNode;
            this.VarName = assignNode.Left.Content;
            this.AssignToExpr = assignNode.Right;
        }

        public override string ToString() => $"{this.TypeExpr} {this.VarName} = {this.AssignToExpr};";

    }

}
