using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    
    public class MemberAccessNode : ASTNode, IExpr {

        public const string MemberAccess = ".";
        public const string CtorAccess = ".ctor:";

        public IExpr Left { get; }

        public IdentifierNode Right { get; }

        public string AccessMethodType { get; }

        public MemberAccessNode(IExpr left, IdentifierNode right, string accessType, SourcePosition position) : base(position, accessType, LexTokenType.None) {
            this.Left = left;
            this.Right = right;
            this.AccessMethodType = accessType;
        }

        public override string ToString() => $"{this.Left}{this.AccessMethodType}{this.Right}";

    }

}
