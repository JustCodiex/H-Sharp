using System.Collections.Generic;

namespace HSharp.Parsing.AbstractSnyaxTree.Expression {
    
    public class ArgumentsNode : ASTNode, IExpr {
    
        public bool IsValid { get; }

        public ASTNode[] Arguments { get; }

        public int Count => this.Arguments.Length;

        public ArgumentsNode(ExpressionNode groupedNode) : base(groupedNode.Pos, groupedNode.Content, groupedNode.LexicalType) {

            List<ASTNode> args = new List<ASTNode>();

            for (int i = 0; i < groupedNode.Size; i++) {
                if (groupedNode[i].LexicalType == LexTokenType.Separator && groupedNode[i].Content.CompareTo(",") == 0) {
                } else if (groupedNode[i].LexicalType == LexTokenType.Separator) {
                    return;
                } else {
                    args.Add(groupedNode[i]);
                }
            }

            this.Arguments = args.ToArray();

            this.IsValid = true;

        }

        public ASTNode this[int index] => this.Arguments[index];

        public override string ToString() => $"({string.Join(",", (object[])this.Arguments)})";

    }

}
