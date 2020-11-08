using System;
using HSharp.Parsing.AbstractSnyaxTree.Expression;
using HSharp.Parsing.AbstractSnyaxTree.Type;
using HSharp.Util.Functional;

namespace HSharp.Parsing.AbstractSnyaxTree {
    
    public static class ASTExt {

        public static bool Is(this ASTNode node, string isString) => node.Content.CompareTo(isString) == 0;

        public static IsType<ASTNode> Is<T>(this ASTNode node) where T : ASTNode => new IsType<ASTNode>(node).Is<T>();

        public static ITypeIdentifier ToTypeIdentifier(this ASTNode node) => node switch
        {
            IdentifierNode id => new TypeIdentifierNode(id),
            ITypeIdentifier => node as ITypeIdentifier,
            _ => throw new NotImplementedException(),
        };

    }

}
