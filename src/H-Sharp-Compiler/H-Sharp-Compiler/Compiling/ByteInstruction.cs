using System;

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
        public long GetSize() {
            long sz = sizeof(Bytecode);
            foreach (object o in Args) {
                sz += o switch
                {
                    short => sizeof(short),
                    ushort => sizeof(ushort),
                    int => sizeof(int),
                    uint => sizeof(uint),
                    byte => sizeof(byte),
                    _ => throw new NotSupportedException()
                };
            }
            return sz;
        }

    }

}
