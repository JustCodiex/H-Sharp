using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Declaration {
    
    public class VarDeclNode : ASTNode, IDecl {

        public ASTNode TypeExpr { get; }

        public string VarName { get; }

        public ASTNode AssignToExpr { get; }

        public bool HasAssignment => this.AssignToExpr is not null;

        public ushort EnterIndex { get; set; }

        public VarDeclNode(SourcePosition pos, ASTNode varTypeNode, BinOpNode assignNode) : base(pos, "=", LexTokenType.None) {
            this.TypeExpr = varTypeNode;
            this.VarName = assignNode.Left.Content;
            this.AssignToExpr = assignNode.Right;
        }

        public VarDeclNode(SourcePosition pos, ASTNode varTypeNode, string identifier) : base(pos, "=", LexTokenType.None) {
            this.TypeExpr = varTypeNode;
            this.VarName = identifier;
            this.AssignToExpr = null;
        }

        public override string ToString() => this.HasAssignment ? $"{this.TypeExpr} {this.VarName} = {this.AssignToExpr};" : $"{this.TypeExpr} {this.VarName};";

    }

}
