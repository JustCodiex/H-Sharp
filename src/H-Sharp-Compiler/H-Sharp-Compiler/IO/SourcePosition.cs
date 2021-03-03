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

        public override bool Equals(object obj) {
            if (obj is SourcePosition sp) {
                bool sourceCheck = !string.IsNullOrEmpty(this.File) && !string.IsNullOrEmpty(sp.File);
                if (sourceCheck) {
                    sourceCheck = this.File == sp.File;
                }
                return this.Line == sp.Line && this.Column == sp.Column && sourceCheck;
            } else {
                return false;
            }
        }

        public static bool operator ==(SourcePosition left, SourcePosition right) => left.Equals(right);

        public static bool operator !=(SourcePosition left, SourcePosition right) => !(left == right);

        public override int GetHashCode() => base.GetHashCode();

    }

}
