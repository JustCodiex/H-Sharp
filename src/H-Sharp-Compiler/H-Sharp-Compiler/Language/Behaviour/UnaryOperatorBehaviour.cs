﻿using System.Collections.Generic;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Language.Behaviour {

    public class UnaryOperatorBehaviour : IOperatorBehaviour {

        private bool m_isPostOp;
        private string m_symbol;
        private int m_advanceBy;

        public UnaryOperatorBehaviour(bool isPost, string symbol) {
            this.m_isPostOp = isPost;
            this.m_symbol = symbol;
            this.m_advanceBy = 0;
        }

        public bool ApplyBehaviour(List<ASTNode> nodes, int opIndex, OrderMethod caller) {
            this.m_advanceBy = 0;
            if (this.m_isPostOp) {
                nodes[opIndex - 1] = new UnaryOpNode(nodes[opIndex].Pos, nodes[opIndex - 1], this.m_symbol, true);
                nodes.RemoveAt(opIndex);
                this.m_advanceBy = 0;
            } else {
                nodes[opIndex] = new UnaryOpNode(nodes[opIndex].Pos, nodes[opIndex + 1], this.m_symbol, false);
                nodes.RemoveAt(opIndex + 1);
                this.m_advanceBy = 1;
            }
            return true;
        }

        public bool IsLegalPreAndPostCondition(List<ASTNode> nodes, int opIndex) { 
            if (this.m_isPostOp) {
                return opIndex >= 1;
            } else {
                return opIndex + 1 < nodes.Count;
            }
        }

        public bool IsLegalWhen(bool pre, bool post) 
            => (this.m_isPostOp && pre) || (!this.m_isPostOp && post);
        // Is post operator and has something preceding it
        // Is pre operator and has something following it

        public bool IsOperatorSymbol(string symbol) => this.m_symbol.CompareTo(symbol) == 0;

        public int GetAdvancement() => this.m_advanceBy;

    }

    public class UnaryPreOperatorBehaviour : UnaryOperatorBehaviour {
        public UnaryPreOperatorBehaviour(string symbol) : base(false, symbol) {}
    }

    public class UnaryPostOperatorBehaviour : UnaryOperatorBehaviour {
        public UnaryPostOperatorBehaviour(string symbol) : base(true, symbol) {}
    }

}
