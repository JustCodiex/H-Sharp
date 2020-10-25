using System;
using HSharp.IO;
using HSharp.Language;

namespace HSharp.Parsing.AbstractSnyaxTree.Directive {
    
    public class StorageModifierNode : ASTNode, IDirective {


        public StorageModifier Modifier { get; }

        public StorageModifierNode(string modifier, SourcePosition position) : base(position, modifier, LexTokenType.Keyword) {
            this.Modifier = modifier switch
            {
                
                _ => throw new ArgumentException()
            };
        }


    }

}
