using System.Collections.Generic;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public interface IGroupedASTNode : IExpr {

        List<ASTNode> Nodes { get; }

        void SetNodes(List<ASTNode> nodes);

        ASTNode this[int index] { get; }

    }

}
