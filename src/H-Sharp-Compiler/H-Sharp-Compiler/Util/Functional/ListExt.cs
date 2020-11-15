using System;
using System.Collections.Generic;
using System.Text;

namespace HSharp.Util.Functional {
    
    public static class ListExt {

        public static List<T> AddAndThen<T>(this List<T> ls, T e) {
            ls.Add(e);
            return ls;
        }

    }

}
