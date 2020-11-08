using System.Collections.Generic;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    
    public interface IGroupedASTNode : IExpr {

        List<ASTNode> Nodes { get; }

        void SetNodes(List<ASTNode> nodes);

        ASTNode this[int index] { get; }

    }

}
