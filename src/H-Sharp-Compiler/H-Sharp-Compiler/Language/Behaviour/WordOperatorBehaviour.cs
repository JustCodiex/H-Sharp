using System;
using System.Collections.Generic;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Language.Behaviour {

    public class WordOperatorBehaviour : IOperatorBehaviour {

        string m_matchWord;
        bool m_matchReqParenthesis;
        bool m_isObj;

        public WordOperatorBehaviour(string op, bool requireParenthesis) {
            this.m_matchReqParenthesis = requireParenthesis;
            this.m_matchWord = op;
        }

        public bool ApplyBehaviour(List<ASTNode> nodes, int opIndex, OrderMethod caller) { 
            if (this.m_isObj) {
                if (this.m_matchReqParenthesis) { // sizeof operator or something
                    throw new NotImplementedException();
                } else {
                    nodes[opIndex] = new NewObjectNode((nodes[opIndex+1] as IdentifierNode).ToTypeIdentifier(), nodes[opIndex+2] as ExpressionNode, nodes[opIndex].Pos);
                    nodes.RemoveRange(opIndex + 1, 2);
                    return true;
                }
            } else {
                var look = nodes[opIndex + 1] as LookupNode;
                nodes[opIndex] = new NewArrayNode(look.ToTypeIdentifier(), look.Index, nodes[opIndex].Pos);
                nodes.RemoveAt(opIndex + 1);
            }
            return true;
        }

        public bool IsLegalPreAndPostCondition(List<ASTNode> nodes, int opIndex) {
            bool a = opIndex + 1 < nodes.Count && nodes[opIndex + 1] is LookupNode;
            bool b = opIndex + 2 < nodes.Count && nodes[opIndex + 1] is IdentifierNode && nodes[opIndex + 2] is ExpressionNode;
            this.m_isObj = !a && b;
            return a || b;
        }

        public bool IsLegalWhen(bool pre, bool post) => post is true;

        public bool IsOperatorSymbol(string symbol) => symbol.CompareTo(this.m_matchWord) == 0;
        
        public int GetAdvancement() => 0;

    }

}
