namespace HSharp.Compiling {

    public enum Bytecode : byte {
    
        NOP = 0,

        ADD,
        SUB,
        DIV,
        MUL,

        INC,
        DEC,

        EQ,
        NEQ,
        LE,
        LQ, // Less or eQual
        GE,
        GQ, // Greater or eQual
        NEG, // Negate

        AND,
        OR,
        XOR,

        ENTER, // Enter scope
        EXIT, // Exit scope

        STORELOC,
        STOREFLD,
        LOADFLD,

        STOREELM,
        LOADELM,

        PUSH,
        PUSHCLOSURE,
        POP,
        RET,

        CALL, // std function call
        VCALL,

        INVOKE, // member call
        VINVOKE, // member virtual call

        NEW,
        NEWARRAY,

        CCPY, // const-copy [start, count]
                // pop array and copy into
                // then push back unto stack

        // LC = Load Const
        LCSI8,
        LCSI16,
        LCSI32,
        LCSI64,

        LCUI8,
        LCUI16,
        LCUI32,
        LCUI64,

    }

}
