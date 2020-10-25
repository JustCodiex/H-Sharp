using System;

namespace HSharp.Language {
    
    [Flags]
    public enum StorageModifier {

        None = 0,
        Const = 1,
        ConstExpr = 2,
        Static = 4,
        Abstract = 8,
        Override = 16,
        Virtual = 32,
        Final = 64,
        Lazy = 128,

    }

    public static class StorageModifierMethods {

        public static bool IsLegal(this StorageModifier modifier) {
            if (modifier == (StorageModifier.Virtual | StorageModifier.Override)) {
                return false;
            } else {
                return true;
            }
        }

    }

}
