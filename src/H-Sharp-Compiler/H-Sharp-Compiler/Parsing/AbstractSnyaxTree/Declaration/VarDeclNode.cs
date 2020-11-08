using HSharp.IO;
using HSharp.Language;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Declaration {
    
    public class VarDeclNode : ASTNode, IDecl, IAccessModifiable, IStorageModifiable {

        private AccessModifier m_accessType = AccessModifier.Default;
        private StorageModifier m_storageType = StorageModifier.None;

        public ITypeIdentifier TypeExpr { get; }

        public string VarName { get; }

        public ASTNode AssignToExpr { get; }

        public bool HasAssignment => this.AssignToExpr is not null;

        public ushort EnterIndex { get; set; }

        public VarDeclNode(SourcePosition pos, ITypeIdentifier varTypeNode, BinOpNode assignNode) : base(pos, "=", LexTokenType.None) {
            this.TypeExpr = varTypeNode;
            this.VarName = assignNode.Left.Content;
            this.AssignToExpr = assignNode.Right;
        }

        public VarDeclNode(SourcePosition pos, ITypeIdentifier varTypeNode, string identifier) : base(pos, "=", LexTokenType.None) {
            this.TypeExpr = varTypeNode;
            this.VarName = identifier;
            this.AssignToExpr = null;
        }

        public override string ToString() => this.HasAssignment ? $"{this.TypeExpr} {this.VarName} = {this.AssignToExpr};" : $"{this.TypeExpr} {this.VarName};";

        public void SetAccessModifier(AccessModifier modifier) => this.m_accessType = modifier;

        public AccessModifier GetAccessModifier() => this.m_accessType;

        public void AddStorageModifier(StorageModifier modifier) => this.m_storageType |= modifier;

        public StorageModifier GetStorageModifier() => this.m_storageType;

        public bool IsAllowedStorageModifier() {
            if (this.m_storageType.IsLegal()) {
                return true;
            } else {
                return false;
            }
        }

    }

}
