using System;
using HSharp.IO;
using HSharp.Language;

namespace HSharp.Parsing.AbstractSnyaxTree.Directive {
    
    public class AccessModifierNode : ASTNode, IDirective {
    
        public AccessModifier Modifier { get; }

        public AccessModifierNode(string modifier, SourcePosition position) : base(position, modifier, LexTokenType.Keyword) {
            this.Modifier = modifier switch {
                "public external" => AccessModifier.Public | AccessModifier.External,
                "private external" => AccessModifier.Private | AccessModifier.External,
                "public" => AccessModifier.Public,
                "private" => AccessModifier.Private,
                "protected" => AccessModifier.Protected,
                "external" => AccessModifier.External,
                "internal" => AccessModifier.Internal,
                "default" => AccessModifier.Default,
                _ => throw new ArgumentException(nameof(modifier), $"Invalid modifier '{modifier}'.")
            };
        }

    }

}
