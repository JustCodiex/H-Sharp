using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Expression;

namespace HSharp.Parsing.AbstractSnyaxTree.Declaration {

    public class FuncDeclNode : ASTNode, IDecl {

        public string Name { get; }

        public ScopeNode Body { get; set; }

        public ParamsNode Params { get; set; }

        public ASTNode Return { get; set; }

        public FuncDeclNode(string id, SourcePosition pos) : base(pos, id, LexTokenType.None) {
            this.Name = id;
        }

        public override string ToString() => $"{this.Name} ({this.Params}): {this.Return} {this.Body}";

    }

}
