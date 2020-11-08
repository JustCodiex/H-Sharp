using System.Collections.Generic;

using HSharp.Parsing.AbstractSnyaxTree.Expression;
using HSharp.Parsing.AbstractSnyaxTree.Initializer;
using HSharp.Parsing.AbstractSnyaxTree.Literal;
using ValueType = HSharp.Analysis.TypeData.ValueType;

namespace HSharp.Compiling {
    
    public static class CompileHelper {
    
        public static byte[] ToByteArray(ValueType type, ValueListInitializerNode exprs) {

            List<byte> content = new List<byte>();

            foreach (IExpr expr in exprs) {
                if (expr is ILiteral lit) {
                    content.AddRange(lit.AsBytes(type));
                }
            }

            return content.ToArray();

        }

    }

}
