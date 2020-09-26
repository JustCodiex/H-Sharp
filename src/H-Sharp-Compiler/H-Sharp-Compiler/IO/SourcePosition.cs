namespace HSharp.IO {
    
    public struct SourcePosition {
    
        public uint Line { get; }

        public uint Column { get; }

        public SourcePosition(uint l, uint c) {
            this.Line = l;
            this.Column = c;
        }

        public override string ToString() => $"Ln: {this.Line}:{this.Column}";

    }

}
