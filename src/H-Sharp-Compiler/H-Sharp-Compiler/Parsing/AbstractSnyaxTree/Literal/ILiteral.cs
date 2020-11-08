using HSharp.Analysis.TypeData;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Literal {
    public interface ILiteral : IExpr {
        byte[] AsBytes(ValueType type);
    }
}
