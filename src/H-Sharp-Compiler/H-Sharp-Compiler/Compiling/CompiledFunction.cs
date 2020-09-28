using System;
using System.Collections.Generic;
using System.Text;

namespace HSharp.Compiling {
    
    public sealed class CompiledFunction {
    
        public List<ByteInstruction> Instructions { get; set; }

        public string Name { get; }

        public CompiledFunction(string name) {
            this.Name = name;
            this.Instructions = new List<ByteInstruction>();
        }

        public override string ToString() => this.Name;

    }

}
