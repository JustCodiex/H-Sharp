using System;
using System.Collections.Generic;
using HSharp.Util.Functional;

namespace HSharp.Util {
    
    internal static class SequenceUntil {        
        public static bool Until<TBase, TUntil>(List<TBase> ls, int from, out List<TBase> contained) {
            if (from >= ls.Count) {
                contained = null;
                return false;
            }
            int i = 0;
            contained = new List<TBase>();
            while (ls[from + i] is not TUntil) {
                contained.Add(ls[from + i]);
                i++;
                if (from + i >= ls.Count) {
                    return false;
                }
            }
            return true;
        }
        public static bool UntilPredicate<TBase, TUntil>(List<TBase> ls, int from, int count, out List<TBase> contained, Predicate<TBase>[] predicates) {
            if (from + count + 1 <= ls.Count) {
                if ((from..(from + count)).IsTrueForAll(i => predicates[i]?.Invoke(ls[from + i]) ?? true)) {
                    return Until<TBase, TUntil>(ls, from + count, out contained);
                }
            }
            contained = null;
            return false;
        }
        public static bool Match<TBase, TUntil>(List<TBase> ls, int from, int count, out List<TBase> contained, Predicate<TBase>[] predicates) {
            if (from + count + 1 <= ls.Count && (from..(from + count)).IsTrueForAll(i => predicates[i]?.Invoke(ls[from + i]) ?? true)) {
                return Until<TBase, TUntil>(ls, from + count, out contained);
            } else {
                contained = null;
                return false;
            }
        }
    }

    public static class TypeSequenceUntil<TBase, T1, TUntil> {
        public static bool Match(List<TBase> ls, int from, out List<TBase> contained)
            => SequenceUntil.Match<TBase, TUntil>(ls, from, 1, out contained, new Predicate<TBase>[]{ x => x is T1 });
        public static bool Match(List<TBase> ls, int from, out List<TBase> contained, Predicate<TBase>[] predicates)
          => SequenceUntil.UntilPredicate<TBase, TUntil>(ls, from, 1, out contained, predicates);
    }

    public static class TypeSequenceUntil<TBase, T1, T2, TUntil> {
        public static bool Match(List<TBase> ls, int from, out List<TBase> contained) 
            => SequenceUntil.Match<TBase, TUntil>(ls, from, 2, out contained, new Predicate<TBase>[] { x => x is T1, x => x is T2 });
        public static bool Match(List<TBase> ls, int from, out List<TBase> contained, Predicate<TBase>[] predicates)
          => SequenceUntil.UntilPredicate<TBase, TUntil>(ls, from, 2, out contained, predicates);
    }

    public static class TypeSequenceUntil<TBase, T1, T2, T3, TUntil> {
        public static bool Match(List<TBase> ls, int from, out List<TBase> contained)
            => SequenceUntil.Match<TBase, TUntil>(ls, from, 3, out contained, new Predicate<TBase>[] { x => x is T1, x => x is T2, x => x is T3 });
        public static bool Match(List<TBase> ls, int from, out List<TBase> contained, Predicate<TBase>[] predicates)
            => SequenceUntil.UntilPredicate<TBase, TUntil>(ls, from, 3, out contained, predicates);
    }

    public static class TypeSequenceUntil<TBase, T1, T2, T3, T4, TUntil> {
        public static bool Match(List<TBase> ls, int from, out List<TBase> contained)
            => SequenceUntil.Match<TBase, TUntil>(ls, from, 4, out contained, new Predicate<TBase>[] { x => x is T1, x => x is T2, x => x is T3, x => x is T4 });
        public static bool Match(List<TBase> ls, int from, out List<TBase> contained, Predicate<TBase>[] predicates)
           => SequenceUntil.UntilPredicate<TBase, TUntil>(ls, from, 4, out contained, predicates);
    }

}
