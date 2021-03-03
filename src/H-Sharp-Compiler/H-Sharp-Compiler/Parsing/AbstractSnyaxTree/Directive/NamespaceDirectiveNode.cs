using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Directive {

    public class NamespaceDirectiveNode : ASTNode, IDirective {
        
        public ScopeNode Body { get; }
        
        public NameIdentifierNode Name { get; }

        public NamespaceDirectiveNode(NameIdentifierNode name, ScopeNode namespaceBody, SourcePosition position) : base(position, name.FullName, LexTokenType.None) {
            this.Body = namespaceBody;
            this.Name = name;
        }

        public override string ToString() => $"namespace {this.Name} {this.Body}";

    }

}
