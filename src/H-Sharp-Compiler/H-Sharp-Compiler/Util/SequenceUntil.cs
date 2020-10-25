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
    }

    public static class TypeSequenceUntil<TBase, T1, TUntil> {
        public static bool Match(List<TBase> ls, int from, out List<TBase> contained) {
            if (from + 2 <= ls.Count && ls[from] is T1) {
                return SequenceUntil.Until<TBase, TUntil>(ls, from + 1, out contained);
            } else {
                contained = null;
                return false;
            }
        }
    }

    public static class TypeSequenceUntil<TBase, T1, T2, TUntil> {
        public static bool Match(List<TBase> ls, int from, out List<TBase> contained) {
            if (from + 3 <= ls.Count && ls[from] is T1 && ls[from+1] is T2) {
                return SequenceUntil.Until<TBase, TUntil>(ls, from + 2, out contained);
            } else {
                contained = null;
                return false;
            }
        }
    }

    public static class TypeSequenceUntil<TBase, T1, T2, T3, TUntil> {
        public static bool Match(List<TBase> ls, int from, out List<TBase> contained) {
            if (from + 4 <= ls.Count && ls[from] is T1 && ls[from + 1] is T2 && ls[from + 2] is T3) {
                return SequenceUntil.Until<TBase, TUntil>(ls, from + 3, out contained);
            } else {
                contained = null;
                return false;
            }
        }
        public static bool Match(List<TBase> ls, int from, out List<TBase> contained, Predicate<TBase>[] predicates) {
            if (from + 4 <= ls.Count) {
                if ((from..(from + 3)).IsTrueForAll(i => predicates[i]?.Invoke(ls[from + i]) ?? true)) {
                    return SequenceUntil.Until<TBase, TUntil>(ls, from + 3, out contained);
                }
            }
            contained = null;
            return false;
        }
    }

    public static class TypeSequenceUntil<TBase, T1, T2, T3, T4, TUntil> {
        public static bool Match(List<TBase> ls, int from, out List<TBase> contained) {
            if (from + 5 <= ls.Count && ls[from] is T1 && ls[from + 1] is T2 && ls[from + 2] is T3 && ls[from + 3] is T4) {
                return SequenceUntil.Until<TBase, TUntil>(ls, from + 3, out contained);
            } else {
                contained = null;
                return false;
            }
        }
    }

}
