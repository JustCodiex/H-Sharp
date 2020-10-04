using System;

namespace HSharp.Util.Functional {
    
    public static class Tuple {

        public static (T1Out, T2Out) Select<T1In, T1Out, T2In, T2Out>(this (T1In, T2In) t, Func<T1In, T1Out> first, Func<T2In, T2Out> second)
            => (first(t.Item1), second(t.Item2));

    }

}
