
using System;
using HSharp.IO;
using ValueType = HSharp.Analysis.TypeData.ValueType;

namespace HSharp.Parsing.AbstractSnyaxTree.Literal {
    
    public class BoolLitNode : ASTNode, ILiteral {

        public bool Boolean { get; }

        public BoolLitNode(string bvalue, SourcePosition pos) : base(pos, bvalue, LexTokenType.IntLiteral) {
            this.Boolean = !(bvalue.CompareTo("0") == 0 || bvalue.ToLower().CompareTo("false") == 0);
        }

        public byte[] AsBytes(ValueType type) => type.Name switch
        {
            "int" => BitConverter.GetBytes(this.Boolean ? 1 : 0),
            "double" => BitConverter.GetBytes(this.Boolean ? 1 : 0),
            "bool" => new byte[] { this.Boolean ? (byte)1 : (byte)0 },
            // add more cases here
            _ => throw new InvalidCastException()
        };


    }

}
