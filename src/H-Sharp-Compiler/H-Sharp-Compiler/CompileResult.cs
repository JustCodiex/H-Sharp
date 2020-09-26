namespace HSharp {
    
    public struct CompileResult {

        public bool Success { get; }

        public CompileResult(bool success) {
            this.Success = success;
        }

        public static implicit operator bool(CompileResult r) => r.Success;

    }

}
