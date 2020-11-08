using System;

namespace HSharp.Util.Functional {
    
    public static class IfTrueFunctional {
    
        public static void IfTrue<T>(this T obj, Predicate<T> pred, Action<T> action) {
            if (pred(obj)) {
                action(obj);
            }
        }

    }

}
