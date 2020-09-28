using System;
using HSharp.IO;

namespace HSharp.Parsing {
    
    public class SyntaxError : Exception {
    
        public int SyntaxViolationCode { get; }

        public SourcePosition Origin { get; }

        public SyntaxError(int code, SourcePosition origin, string description) : base(description) {
            this.SyntaxViolationCode = code;
            this.Origin = origin;
        }

        public override string ToString() => $"HS{this.SyntaxViolationCode:0000} @ {this.Origin} :: {this.Message}";

    }

}
