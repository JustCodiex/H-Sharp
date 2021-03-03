using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Directive {

    public class TypeDirectiveNode : ASTNode, IDirective {
        public TypeDirectiveNode(SourcePosition position, string content, LexTokenType tokenType) : base(position, content, tokenType) {
        }
    }

}
