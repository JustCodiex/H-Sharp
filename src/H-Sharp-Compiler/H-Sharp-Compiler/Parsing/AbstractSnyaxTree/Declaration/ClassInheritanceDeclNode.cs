using System.Collections.Generic;
using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Declaration {
    
    public class ClassInheritanceDeclNode : ASTNode, IDecl {

        public List<ITypeIdentifier> InheritanceNodes { get; }

        public ClassInheritanceDeclNode(SourcePosition position) : base(position, ":", LexTokenType.None) {
            this.InheritanceNodes = new List<ITypeIdentifier>();
        }

        public void AddType(IdentifierNode identifier) => this.InheritanceNodes.Add(new TypeIdentifierNode(identifier));

        public void AddType(TypeCtorNode ctor) => this.InheritanceNodes.Add(ctor);

        public override string ToString() => InheritanceNodes.Count > 0 ? $":{string.Join(",", InheritanceNodes)}" : string.Empty;

    }

}
