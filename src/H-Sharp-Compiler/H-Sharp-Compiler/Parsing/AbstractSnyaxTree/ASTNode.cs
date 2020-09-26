using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree {

    public class ASTNode {

        public SourcePosition Pos { get; }

        public string Content { get; }

        public LexTokenType LexicalType { get; }

        public ASTNode(SourcePosition position, string content, LexTokenType tokenType) {
            this.Pos = position;
            this.Content = content;
            this.LexicalType = tokenType;
        }

        public override string ToString() => this.Content;

    }

}
