using System;
using System.Collections.Generic;

namespace HSharp.Util.Functional {
    
    public static class PredicateLoop {
    
        public static bool IsTrueForAll<T>(this IList<T> ls, Predicate<int> predicate) {
            for (int i = 0; i < ls.Count; i++) {
                if (!predicate(i)) {
                    return false;
                }
            }
            return true;
        }

        public static bool IsTrueForAll(this Range r, Predicate<int> predicate) {
            if (r.End.IsFromEnd) {
                throw new NotSupportedException();
            }
            int count = r.End.Value - r.Start.Value;
            for (int i = 0; i < count; i++) {
                if (!predicate(i)) {
                    return false;
                }
            }
            return true;
        }

    }

}
