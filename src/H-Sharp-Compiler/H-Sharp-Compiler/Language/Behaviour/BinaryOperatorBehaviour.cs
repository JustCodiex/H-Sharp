using System;
using System.Collections.Generic;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Expression;
using HSharp.Parsing.AbstractSnyaxTree.Statement;

namespace HSharp.Language.Behaviour {
    
    public class BinaryOperatorBehaviour<T> : IOperatorBehaviour where T : ASTNode, ILR {

        private string m_symbol;

        public BinaryOperatorBehaviour(string symbol) {
            this.m_symbol = symbol;
        }

        public bool IsOperatorSymbol(string symbol) => symbol.CompareTo(this.m_symbol) == 0;

        public virtual bool ApplyBehaviour(List<ASTNode> nodes, int opIndex, OrderMethod caller) {
            T op = Activator.CreateInstance(typeof(T), nodes[opIndex].Pos) as T;
            op.Update(LeftRight.LHS, nodes[opIndex - 1]);
            op.Update(LeftRight.RHS, nodes[opIndex + 1]);
            ApplyOrderOfOperationsBinary(op, caller);
            nodes[opIndex - 1] = op;
            nodes.RemoveAt(opIndex + 1);
            nodes.RemoveAt(opIndex);
            return true;
        }

        public static void ApplyOrderOfOperationsBinary(T binOpNode, OrderMethod orderEnforcer) {
            if (binOpNode.Left is IGroupedASTNode leftGroup) {
                orderEnforcer(leftGroup.Nodes);
            }
            if (binOpNode.Right is IGroupedASTNode rightGroup) {
                orderEnforcer(rightGroup.Nodes);
            }
        }

        public bool IsLegalWhen(bool pre, bool post) => pre == true && post == true;

        public bool IsLegalPreAndPostCondition(List<ASTNode> nodes, int opIndex) => nodes.Count >= 3;

        public int GetAdvancement() => 0;

    }

    public class BinOpBehaviour : BinaryOperatorBehaviour<BinOpNode> {
        public BinOpBehaviour(string sym) : base(sym) { }
        public override bool ApplyBehaviour(List<ASTNode> nodes, int opIndex, OrderMethod caller) {
            string op = nodes[opIndex].Content;
            if (base.ApplyBehaviour(nodes, opIndex, caller)) {
                (nodes[opIndex - 1] as BinOpNode).SetOperation(op);
                return true;
            } else {
                return false;
            }
        }
    }

    public class AssignmentBehaviour : BinaryOperatorBehaviour<AssignmentNode> {
        public AssignmentBehaviour(string sym) : base(sym) { }
    }

}
