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

        PUSH,
        POP,

        // LC = Load Const
        LCSI16,
        LCSI32,
        LCSI64,

        LCUI16,
        LCUI32,
        LCUI64,

    }

}
