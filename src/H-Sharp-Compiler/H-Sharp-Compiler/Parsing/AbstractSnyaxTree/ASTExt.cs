using HSharp.Util.Functional;

namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public static class ASTExt {

        public static bool Is(this ASTNode node, string isString) => node.Content.CompareTo(isString) == 0;

        public static IsType<ASTNode> Is<T>(this ASTNode node) where T : ASTNode => new IsType<ASTNode>(node).Is<T>();

    }

}
