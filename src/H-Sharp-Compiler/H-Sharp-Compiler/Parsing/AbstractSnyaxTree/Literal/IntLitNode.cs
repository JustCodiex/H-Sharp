using System;
using HSharp.IO;
using ValueType = HSharp.Analysis.TypeData.ValueType;

namespace HSharp.Parsing.AbstractSnyaxTree.Literal {
    
    public class IntLitNode : ASTNode, ILiteral {
        
        public int Integer { get; }
        
        public IntLitNode(string intvalue, SourcePosition pos) : base(pos, intvalue, LexTokenType.IntLiteral) {
            this.Integer = int.Parse(intvalue);
        }

        public byte[] AsBytes(ValueType type) => type.Name switch
        {
            "int" => BitConverter.GetBytes(this.Integer),
            "double" => BitConverter.GetBytes((double)this.Integer),
            // add more cases here
            _ => throw new InvalidCastException()
        };

    }

}
