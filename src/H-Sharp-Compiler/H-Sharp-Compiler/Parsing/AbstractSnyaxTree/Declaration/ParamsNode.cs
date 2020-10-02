using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Declaration {
    
    public class ParamsNode : ASTNode, IDecl {

        public class ParameterNode : ASTNode, IDecl {
            public ASTNode Type { get; }
            public IdentifierNode Identifier { get; }
            public ParameterNode(ASTNode typeNode, IdentifierNode identifier, SourcePosition position) : base(position, identifier.Content, LexTokenType.None) {
                this.Type = typeNode;
                this.Identifier = identifier;
            }
            public override string ToString() => $"{Type} {Identifier}";
        }

        ParameterNode[] m_params;

        public bool IsValid { get; }

        public ParameterNode[] Parameters => this.m_params;

        public int Count => this.Parameters.Length;

        public ParamsNode(ExpressionNode source) : base(source.Pos, source.Content, source.LexicalType) {

            // Count the parameters
            int paramCount = 0;

            // If more than one parameter - verify syntax
            if (source.Size > 2) {
                for (int i = 2; i < source.Size; i += 3) {
                    bool isParamRule = source[i - 2] is IdentifierNode && source[i - 1] is IdentifierNode;
                    if (isParamRule && source[i].LexicalType == LexTokenType.Separator && source[i].Content.CompareTo(",") != 0) {
                        return;
                    } else {
                        paramCount++;
                    }
                }
            }

            // If more than one parameter and it doesn't follow parameter grammar rule => return
            if (source.Size > 0 && !(source[^2] is IdentifierNode && source[^1] is IdentifierNode)) {
                return;
            } else if (source.Size > 0) {
                paramCount++;
            }

            // Alloc paremeter array
            this.m_params = new ParameterNode[paramCount];

            // Run though all params
            for (int i = 0, j = 0; i < source.Size; i += 3, j++) {
                this.m_params[j] = new ParameterNode(source[i], source[i + 1] as IdentifierNode, source[i].Pos);
            }

            this.IsValid = true;

        }

        public IdentifierNode GetParameterId(int index) => this.Parameters[index].Identifier;

        public void InsertParam(int pos, ParameterNode node) {
            ParameterNode[] copy = new ParameterNode[this.Parameters.Length + 1];
            for (int i = 0, j = 0; i < copy.Length; i++) {
                if (i == pos) {
                    copy[i] = node;
                } else {
                    copy[i] = this.m_params[j];
                    j++;
                }
            }
            this.m_params = copy;
        }

        public override string ToString() => string.Join(",", (object[])this.Parameters);

    }

}
