using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree;

namespace HSharp {
    
    public struct CompileResult {

        public bool Success { get; }

        public string Message { get; }

        public SourcePosition Origin { get; private set; }

        public CompileResult(bool success, string message = "") {
            this.Success = success;
            this.Message = message;
            this.Origin = new SourcePosition(uint.MaxValue, uint.MaxValue);
        }

        public CompileResult SetOrigin(SourcePosition position) {
            this.Origin = position;
            return this;
        }

        public CompileResult SetOrigin(ASTNode node) => this.SetOrigin(node.Pos);

        public override string ToString() => Success ? "Success" : $"Fatal Compile Error {this.Origin.GetAbsolute()} - \"{this.Message}\"";

        public static implicit operator bool(CompileResult r) => r.Success;

    }

}
