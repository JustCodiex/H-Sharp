namespace HSharp.Compiling {
    
    public struct ByteInstruction {
    
        public Bytecode Op { get; }

        public object[] Args { get; }

        public ByteInstruction(Bytecode op) {
            this.Op = op;
            this.Args = new object[0];
        }

        public ByteInstruction(Bytecode op, params object[] args) {
            this.Op = op;
            this.Args = args;
        }

        public override string ToString() => (this.Args?.Length ?? 0) == 0 ? this.Op.ToString() : $"{this.Op} [{string.Join(", ", this.Args)}]";

    }

}
