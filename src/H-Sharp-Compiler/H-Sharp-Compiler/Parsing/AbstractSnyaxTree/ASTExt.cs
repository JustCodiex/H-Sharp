namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public static class ASTExt {
        public static bool Is(this ASTNode node, string isString) => node.Content.CompareTo(isString) == 0;
    }

}
