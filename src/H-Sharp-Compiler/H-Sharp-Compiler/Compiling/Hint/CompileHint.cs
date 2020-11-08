namespace HSharp.Compiling.Hint {
    
    public struct CompileHint {
    
        public CompileHintType Type { get; }

        public object[] Args { get; }

        public CompileHint(CompileHintType type, params object[] hints) {
            this.Type = type;
            this.Args = hints;
        }

        public override string ToString() => $"{this.Type}: {string.Join(',', this.Args)}";

    }

}
