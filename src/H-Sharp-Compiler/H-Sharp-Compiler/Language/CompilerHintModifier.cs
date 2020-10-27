using System;

namespace HSharp.Language {

    [Flags]
    public enum CompilerHintModifier {

        None = 0,
        ConstExpr = 2,
        Inline = 4,
        Final = 8, // sealed

    }

}
