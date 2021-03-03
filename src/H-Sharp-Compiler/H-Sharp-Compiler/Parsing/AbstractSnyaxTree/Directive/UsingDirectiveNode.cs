using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSharp.IO;

namespace HSharp.Parsing.AbstractSnyaxTree.Directive {

    public class UsingDirectiveNode : ASTNode, IDirective {
        public UsingDirectiveNode(SourcePosition position, string content, LexTokenType tokenType) : base(position, content, tokenType) {
        }
    }

}
