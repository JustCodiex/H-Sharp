using System.Collections.Generic;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Expression;
using HSharp.Parsing.AbstractSnyaxTree.Type;

namespace HSharp.Language.Behaviour {
    
    public class IndexLookupOperatorBehaviour : IOperatorBehaviour {

        private string m_symbol;

        public IndexLookupOperatorBehaviour(string symbol) {
            this.m_symbol = symbol;
        }

        public bool IsOperatorSymbol(string symbol) => symbol.CompareTo(this.m_symbol) == 0;

        public bool ApplyBehaviour(List<ASTNode> nodes, int opIndex, OrderMethod caller) {
            if (nodes[opIndex] is IndexerNode ix) {
                if (ix.Nodes.Count == 0 && nodes[opIndex-1].Is<IdentifierNode>().Or<ITypeIdentifier>(out bool isType)) {
                    ITypeIdentifier arrayedType = isType ? nodes[opIndex - 1] as ITypeIdentifier : new TypeIdentifierNode(nodes[opIndex - 1] as IdentifierNode);
                    TypeArrayIdentifierNode lookup = new TypeArrayIdentifierNode(arrayedType, nodes[opIndex - 1].Pos);
                    nodes[opIndex - 1] = lookup;
                    nodes.RemoveAt(opIndex);
                } else {
                    LookupNode lookup = new LookupNode(nodes[opIndex - 1] as IExpr, nodes[opIndex] as IndexerNode, nodes[opIndex - 1].Pos);
                    caller.Invoke(lookup.Index.Nodes); // Invoke on sub-elements
                    nodes[opIndex - 1] = lookup;
                    nodes.RemoveAt(opIndex);
                }
                return true;
            } else {
                return false;
            }
        }

        public bool IsLegalWhen(bool pre, bool post) => pre == true;

        public bool IsLegalPreAndPostCondition(List<ASTNode> nodes, int opIndex) => nodes.Count >= 2 && nodes[opIndex] is IndexerNode;
        
        public int GetAdvancement() => 0;

    }

}
