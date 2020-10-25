using HSharp.IO;
using HSharp.Language;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Declaration {

    public class FuncDeclNode : ASTNode, IDecl, IAccessModifiable, IStorageModifiable {

        private AccessModifier m_accessType = AccessModifier.Default;
        private StorageModifier m_storageType = StorageModifier.None;

        public string Name { get; }

        public ScopeNode Body { get; set; }

        public ParamsNode Params { get; set; }

        public ASTNode Return { get; set; }

        public FuncDeclNode(string id, SourcePosition pos) : base(pos, id, LexTokenType.None) {
            this.Name = id;
        }

        public override string ToString() => $"{this.Name} ({this.Params}): {this.Return} {this.Body}";

        public void SetAccessModifier(AccessModifier modifier) => this.m_accessType = modifier;

        public AccessModifier GetAccessModifier() => this.m_accessType;

        public void AddStorageModifier(StorageModifier modifier) => this.m_storageType |= modifier;

        public StorageModifier GetStorageModifier() => this.m_storageType;

        public bool IsAllowedStorageModifier() {
            if (this.m_storageType.IsLegal()) {
                return this.m_storageType != StorageModifier.Lazy && this.m_storageType != StorageModifier.Const;
            } else {
                return false;
            }
        }

    }

}
