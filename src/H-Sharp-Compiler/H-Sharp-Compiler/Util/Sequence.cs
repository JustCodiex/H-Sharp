using System.Collections.Generic;

namespace HSharp.Util {
    
    public static class TypeSequence<TBase, T1, T2>
        where T1 : TBase
        where T2 : TBase {
        public static bool Match(List<TBase> ls, int from) {
            if (from + 2 <= ls.Count) {
                return ls[from] is T1 && ls[from + 1] is T2;
            } else {
                return false;
            }
        }
    }

    public static class TypeSequence<TBase, T1, T2, T3>
        where T1 : TBase
        where T2 : TBase
        where T3 : TBase {
        public static bool Match(List<TBase> ls, int from) {
            if (from + 3 <= ls.Count) {
                return ls[from] is T1 && ls[from + 1] is T2 && ls[from + 2] is T3;
            } else {
                return false;
            }
        }
    }

    public static class TypeSequence<TBase, T1, T2, T3, T4>
        where T1 : TBase
        where T2 : TBase
        where T3 : TBase
        where T4 : TBase {
        public static bool Match(List<TBase> ls, int from) {
            if (from + 4 <= ls.Count) {
                return ls[from] is T1 && ls[from + 1] is T2 && ls[from + 2] is T3 && ls[from + 3] is T4;
            } else {
                return false;
            }
        }
    }

    public static class TypeSequence<TBase, T1, T2, T3, T4, T5>
        where T1 : TBase
        where T2 : TBase
        where T3 : TBase
        where T4 : TBase 
        where T5 : TBase {
        public static bool Match(List<TBase> ls, int from) {
            if (from + 5 <= ls.Count) {
                return ls[from] is T1 && ls[from+1] is T2 && ls[from + 2] is T3 && ls[from + 3] is T4 && ls[from + 4] is T5;
            } else {
                return false;
            }
        }
    }

    public static class TypeSequence<TBase, T1, T2, T3, T4, T5, T6>
        where T1 : TBase
        where T2 : TBase
        where T3 : TBase
        where T4 : TBase
        where T5 : TBase
        where T6 : TBase {
        public static bool Match(List<TBase> ls, int from) {
            if (from + 6 <= ls.Count) {
                return ls[from] is T1 && ls[from + 1] is T2 && ls[from + 2] is T3 && ls[from + 3] is T4 && ls[from + 4] is T5 && ls[from + 5] is T6;
            } else {
                return false;
            }
        }
    }

    public static class TypeSequence<TBase, T1, T2, T3, T4, T5, T6, T7>
       where T1 : TBase
       where T2 : TBase
       where T3 : TBase
       where T4 : TBase
       where T5 : TBase
       where T6 : TBase
       where T7 : TBase {
        public static bool Match(List<TBase> ls, int from) {
            if (from + 7 <= ls.Count) {
                return ls[from] is T1 && ls[from + 1] is T2 && ls[from + 2] is T3 && ls[from + 3] is T4 && ls[from + 4] is T5 && ls[from + 5] is T6 && ls[from + 6] is T7;
            } else {
                return false;
            }
        }
    }

    public static class TypeSequence<TBase, T1, T2, T3, T4, T5, T6, T7, T8>
      where T1 : TBase
      where T2 : TBase
      where T3 : TBase
      where T4 : TBase
      where T5 : TBase
      where T6 : TBase
      where T7 : TBase
      where T8 : TBase {
        public static bool Match(List<TBase> ls, int from) {
            if (from + 8 <= ls.Count) {
                return ls[from] is T1 && ls[from + 1] is T2 && ls[from + 2] is T3 && ls[from + 3] is T4 && ls[from + 4] is T5 && ls[from + 5] is T6 && ls[from + 6] is T7 && ls[from + 7] is T8;
            } else {
                return false;
            }
        }
    }

}
