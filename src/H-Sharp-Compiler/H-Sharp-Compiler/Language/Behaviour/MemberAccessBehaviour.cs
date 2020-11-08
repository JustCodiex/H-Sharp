using System.Collections.Generic;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Language.Behaviour {
    
    public class MemberAccessBehaviour : IOperatorBehaviour {

        private string m_symbol;

        public MemberAccessBehaviour(string symbol) {
            this.m_symbol = symbol;
        }

        public bool IsOperatorSymbol(string symbol) => symbol.CompareTo(this.m_symbol) == 0;

        public bool ApplyBehaviour(List<ASTNode> nodes, int opIndex, OrderMethod caller) {
            if (nodes[opIndex - 1].Is<IdentifierNode>().Is<ThisNode>().Or<MemberAccessNode>() && nodes[opIndex + 1] is IdentifierNode accessId) {
                IExpr accessExpr = nodes[opIndex - 1] as IExpr;
                nodes[opIndex - 1] = new MemberAccessNode(accessExpr, accessId, this.m_symbol, nodes[opIndex].Pos);
                nodes.RemoveAt(opIndex + 1);
                nodes.RemoveAt(opIndex);
                return true;
            } else {
                return false;
            }
        }

        public bool IsLegalWhen(bool pre, bool post) => pre == true && post == true;

        public bool IsLegalPreAndPostCondition(List<ASTNode> nodes, int opIndex) => nodes.Count >= 3;

    }

}
