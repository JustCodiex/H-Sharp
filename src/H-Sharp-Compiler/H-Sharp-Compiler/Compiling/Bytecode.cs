namespace HSharp.Compiling {

    public enum Bytecode : byte {
    
        NOP = 0,

        ADD,
        SUB,
        DIV,
        MUL,

        INC,
        DEC,

        ENTER, // Enter scope
        EXIT, // Exit scope

        STORELOC,
        STOREFLD,
        LOADFLD,

        PUSH,
        PUSHCLOSURE,
        POP,
        RET,

        CALL, // std function call

        INVOKE, // member call
        VINVOKE, // member virtual call

        // LC = Load Const
        LCSI16,
        LCSI32,
        LCSI64,

        LCUI16,
        LCUI32,
        LCUI64,

    }

}
