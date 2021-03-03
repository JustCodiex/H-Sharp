using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Directive {

    public class NameIdentifierNode : ASTNode, IDirective {

        private string m_fullpath;

        public string FullName => this.m_fullpath;

        public string[] Elements => this.m_fullpath.Split('.');

        public NameIdentifierNode(IdentifierNode identifier) : base(identifier.Pos, identifier.Content, LexTokenType.None) {
            this.m_fullpath = identifier.Content;
        }
        
        public NameIdentifierNode(MemberAccessNode memberAccess) : base(memberAccess.Pos, memberAccess.Content, LexTokenType.None) {
            this.m_fullpath = memberAccess.ToString();
        }

        public override string ToString() => this.FullName;

    }

}
