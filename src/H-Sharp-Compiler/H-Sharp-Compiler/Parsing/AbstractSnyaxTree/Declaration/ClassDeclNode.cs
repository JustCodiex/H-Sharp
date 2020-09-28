using System.Collections.Generic;
using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Declaration {
    
    public class ClassDeclNode : ASTNode, IDecl {
    
        public string LocalClassName { get; }

        public List<VarDeclNode> Fields { get; set; }

        public List<FuncDeclNode> Methods { get; set; }

        public List<ClassDeclNode> Classes { get; set; }

        public ClassDeclNode(string className, SourcePosition position) : base(position, className, LexTokenType.None) {
            this.LocalClassName = className;
            this.Fields = new List<VarDeclNode>();
            this.Methods = new List<FuncDeclNode>();
            this.Classes = new List<ClassDeclNode>();
        }

        public override string ToString() => $"class {this.LocalClassName} {{ {string.Join(' ', this.Classes)} {string.Join(";", this.Fields)} {string.Join(' ', this.Methods)} }}";

    }

}
