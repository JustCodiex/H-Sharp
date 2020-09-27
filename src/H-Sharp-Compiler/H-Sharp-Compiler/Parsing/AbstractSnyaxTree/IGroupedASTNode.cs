using System.Collections.Generic;

namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public interface IGroupedASTNode {

        List<ASTNode> Nodes { get; }

        void SetNodes(List<ASTNode> nodes);

        ASTNode this[int index] { get; }

    }

}
