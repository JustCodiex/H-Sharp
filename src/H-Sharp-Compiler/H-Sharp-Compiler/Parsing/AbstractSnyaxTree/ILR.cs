namespace HSharp.Parsing.AbstractSnyaxTree {

    public enum LeftRight {
        LHS,
        RHS
    }

    /// <summary>
    /// Left-Right Interface
    /// </summary>
    public interface ILR {
    
        ASTNode Left { get; }

        ASTNode Right { get; }
        
        void Update(LeftRight side, ASTNode node);

    }

}
