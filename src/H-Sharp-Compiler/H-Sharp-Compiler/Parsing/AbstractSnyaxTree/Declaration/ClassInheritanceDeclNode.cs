using System.Collections.Generic;
using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Declaration {
    
    public class ClassInheritanceDeclNode : ASTNode, IDecl {

        public List<TypeIdentifierNode> InheritanceNodes { get; }

        public ClassInheritanceDeclNode(SourcePosition position) : base(position, ":", LexTokenType.None) {
            this.InheritanceNodes = new List<TypeIdentifierNode>();
        }

        public void AddType(IdentifierNode identifier) => this.InheritanceNodes.Add(new TypeIdentifierNode(identifier));

        public override string ToString() => InheritanceNodes.Count > 0 ? $": {string.Join(", ", InheritanceNodes)}" : string.Empty;

    }

}
