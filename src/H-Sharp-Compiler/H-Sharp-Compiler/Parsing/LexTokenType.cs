namespace HSharp.Parsing {
    
    public enum LexTokenType {

        None,

        Keyword,
    
        Identifier,

        Operator,

        Separator,

        StringLiteral,

        CharLiteral,

        IntLiteral,

        DoubleLiteral,

        FloatLiteral,

        BoolLiteral,

        NullLiteral,

        BlockStart = '{',
        BlockEnd = '}',

        ExpressionStart = '(',
        ExpressionEnd = ')',

        IndexerStart = '[',
        IndexerEnd = ']',

    }

}
