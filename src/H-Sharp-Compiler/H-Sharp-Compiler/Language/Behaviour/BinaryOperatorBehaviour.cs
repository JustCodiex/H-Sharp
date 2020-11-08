using System.Collections.Generic;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Language.Behaviour {
    
    public class BinaryOperatorBehaviour : IOperatorBehaviour {

        private string m_symbol;

        public BinaryOperatorBehaviour(string symbol) {
            this.m_symbol = symbol;
        }

        public bool IsOperatorSymbol(string symbol) => symbol.CompareTo(this.m_symbol) == 0;

        public bool ApplyBehaviour(List<ASTNode> nodes, int opIndex, OrderMethod caller) {
            BinOpNode binaryOperation = new BinOpNode(nodes[opIndex].Pos, nodes[opIndex - 1], nodes[opIndex].Content, nodes[opIndex + 1]);
            ApplyOrderOfOperationsBinary(binaryOperation, caller);
            nodes[opIndex - 1] = binaryOperation;
            nodes.RemoveAt(opIndex + 1);
            nodes.RemoveAt(opIndex);
            return true;
        }

        public static void ApplyOrderOfOperationsBinary(BinOpNode binOpNode, OrderMethod orderEnforcer) {
            if (binOpNode.Left is IGroupedASTNode leftGroup) {
                orderEnforcer(leftGroup.Nodes);
            }
            if (binOpNode.Right is IGroupedASTNode rightGroup) {
                orderEnforcer(rightGroup.Nodes);
            }
        }

        public bool IsLegalWhen(bool pre, bool post) => pre == true && post == true;

        public bool IsLegalPreAndPostCondition(List<ASTNode> nodes, int opIndex) => nodes.Count >= 3;

    }

}
