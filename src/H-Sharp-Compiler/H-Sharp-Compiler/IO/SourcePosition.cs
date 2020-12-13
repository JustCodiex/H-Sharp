namespace HSharp.IO {
    
    public readonly struct SourcePosition {
    
        public uint Line { get; }

        public uint Column { get; }

        public string File { get; }

        public SourcePosition(uint l, uint c) {
            this.Line = l;
            this.Column = c;
            this.File = string.Empty;
        }

        public SourcePosition(uint l, uint c, string s) {
            this.Line = l;
            this.Column = c;
            this.File = s;
        }

        public string GetAbsolute() => $"\"{this.File}\" @ ln {this.Line}:{this.Column}";

        public override string ToString() => $"Ln: {this.Line}:{this.Column}";

    }

}
