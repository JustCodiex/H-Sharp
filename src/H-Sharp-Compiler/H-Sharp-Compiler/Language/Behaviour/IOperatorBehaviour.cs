using System.Collections.Generic;
using HSharp.Parsing.AbstractSnyaxTree;

namespace HSharp.Language.Behaviour {

    public delegate void OrderMethod(List<ASTNode> nodes);

    public interface IOperatorBehaviour {

        int GetAdvancement();

        bool IsOperatorSymbol(string symbol);

        bool IsLegalWhen(bool pre, bool post);

        bool IsLegalPreAndPostCondition(List<ASTNode> nodes, int opIndex);

        bool ApplyBehaviour(List<ASTNode> nodes, int opIndex, OrderMethod caller);

    }

}
