using System;

namespace HSharp.Language {
    
    [Flags]
    public enum StorageModifier {

        None = 0,
        Const = 1,
        ConstExpr = 2,
        Static = 4,
        Override = 8,
        Virtual = 16,
        Final = 32, // sealed
        Lazy = 64,

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
