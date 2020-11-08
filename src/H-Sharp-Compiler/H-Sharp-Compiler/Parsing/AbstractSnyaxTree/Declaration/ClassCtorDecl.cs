using HSharp.IO;
using HSharp.Parsing.AbstractSnyaxTree.Type;

namespace HSharp.Parsing.AbstractSnyaxTree.Declaration {

    public class ClassCtorDecl : FuncDeclNode {

        public ClassCtorDecl(string className, SourcePosition position) : base(className, position) {
            this.Return = new TypeIdentifierNode(className, position);
        }

    }

}
