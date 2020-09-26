using HSharp.IO;

namespace HSharp.Parsing {
    
    public struct LexToken {
    
        public SourcePosition Position { get; }

        public string Content { get; }

        public object Tag { get; set; }

        public LexTokenType Type { get; }

        public LexToken(LexTokenType type, string content, SourcePosition pos) {
            this.Tag = null;
            this.Position = pos;
            this.Type = type;
            this.Content = content;
        }

        public override string ToString() => $"{this.Content} ({this.Type}) @ {this.Position}";

    }

}
