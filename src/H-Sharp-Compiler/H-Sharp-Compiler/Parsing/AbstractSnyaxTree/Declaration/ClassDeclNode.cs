using System.Collections.Generic;
using HSharp.IO;
using HSharp.Language;

namespace HSharp.Parsing.AbstractSnyaxTree.Declaration {
    
    public class ClassDeclNode : ASTNode, IDecl, IAccessModifiable, IStorageModifiable {

        private AccessModifier m_accessType = AccessModifier.Default;
        private StorageModifier m_storageType = StorageModifier.None;
        private ClassInheritanceDeclNode m_inheritNodes;

        public string LocalClassName { get; }

        public List<VarDeclNode> Fields { get; set; }

        public List<FuncDeclNode> Methods { get; set; }

        public List<ClassDeclNode> Classes { get; set; }

        public ClassInheritanceDeclNode Inheritance { get => this.m_inheritNodes; set => this.m_inheritNodes = value; }

        public ClassDeclNode(string className, SourcePosition position) : base(position, className, LexTokenType.None) {
            this.LocalClassName = className;
            this.Fields = new List<VarDeclNode>();
            this.Methods = new List<FuncDeclNode>();
            this.Classes = new List<ClassDeclNode>();
            this.m_inheritNodes = null;
        }

        public override string ToString() => $"class {this.LocalClassName} {{ {string.Join(' ', this.Classes)} {string.Join(";", this.Fields)} {string.Join(' ', this.Methods)} }}";

        public void SetAccessModifier(AccessModifier modifier) => this.m_accessType = modifier;

        public AccessModifier GetAccessModifier() => this.m_accessType;

        public void AddStorageModifier(StorageModifier modifier) => this.m_storageType |= modifier;

        public StorageModifier GetStorageModifier() => this.m_storageType;

        public bool IsAllowedStorageModifier() {
            if (this.m_storageType.IsLegal()) {
                return this.m_storageType != StorageModifier.ConstExpr && this.m_storageType != StorageModifier.Lazy && this.m_storageType != StorageModifier.Override
                    && this.m_storageType != StorageModifier.Static && this.m_storageType != StorageModifier.Virtual && this.m_storageType != StorageModifier.Const;
            } else {
                return false;
            }
        }

    }

}
