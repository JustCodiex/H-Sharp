using System;
using System.Collections.Generic;
using System.Text;

namespace HSharp.Parsing {
    
    public struct ParsingResult {
    
        public bool Success { get; }

        public ParsingResult(bool success) {
            this.Success = success;
        }

        public static implicit operator bool(ParsingResult r) => r.Success;

    }

}
